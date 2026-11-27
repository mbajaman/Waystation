using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
///     Draws a passport onto a mesh. Spawns the face layout off-screen, renders it once into a texture,
///     then puts that texture on the target renderer. A new passport means a fresh bake, nothing updates live.
/// </summary>
public class PassportView : MonoBehaviour
{
    private static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");

    // far from everything else so the bake camera only ever sees the layout
    private static readonly Vector3 BakeOrigin = new(0f, -10000f, 0f);

    [Tooltip("Where the passport data comes from. Bakes whenever it generates.")] [SerializeField]
    private PassportObject source;

    [Tooltip("UI prefab with a Canvas and PassportFaceUI on the root. Lay it out at the reference size.")]
    [SerializeField]
    private PassportFaceUI layoutPrefab;

    [Tooltip("Renderer that gets the baked texture as its _BaseMap.")] [SerializeField]
    private Renderer target;

    [Tooltip("Size of the square texture the layout gets baked into.")] [SerializeField]
    private int resolution = 1024;

    [Tooltip("The size the layout prefab was authored at, so it scales with resolution.")] [SerializeField]
    private float referenceSize = 1024f;

    [SerializeField] private Color clearColor = Color.white;

    private RenderTexture _texture;
    private MaterialPropertyBlock _block;
    private Passport _baked;

    private void OnEnable()
    {
        if (source != null) source.Generated += Bake;
    }

    private void OnDisable()
    {
        if (source != null) source.Generated -= Bake;
    }

    // covers the case where the source generated in Awake before we subscribed
    private void Start()
    {
        if (source != null && source.Passport != null && source.Passport != _baked) Bake(source.Passport);
    }

    private void OnDestroy()
    {
        ReleaseTexture();
    }

    /// <summary>
    ///     Editor preview, bakes whatever the source has (or rolls one if it hasn't yet).
    /// </summary>
    [ContextMenu("Bake")]
    private void BakeFromSource()
    {
        if (source == null)
        {
            Debug.LogWarning($"{name} has no PassportObject source to bake from.", this);
            return;
        }

        if (source.Passport == null)
            source.Generate(); // Generate fires Generated, but we aren't subscribed in edit mode
        Bake(source.Passport);
    }

    /// <summary>
    ///     Renders the layout for this passport into a new texture and puts it on the target.
    /// </summary>
    /// <param name="passport">The passport to draw</param>
    public void Bake(Passport passport)
    {
        if (passport == null || layoutPrefab == null || target == null)
        {
            Debug.LogWarning($"{name} can't bake, missing passport, layout prefab or target.", this);
            return;
        }

        ReleaseTexture();
        _texture = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.ARGB32)
        {
            name = $"{name}_PassportFace",
            useMipMap = true,
            autoGenerateMips = true
        };
        _texture.Create();

        // temporary rig: a disabled camera with the layout bound to it as a Screen Space - Camera canvas
        var rig = new GameObject("PassportBakeRig") { hideFlags = HideFlags.HideAndDontSave };
        rig.transform.position = BakeOrigin;
        var cam = rig.AddComponent<Camera>();
        cam.enabled = false;
        cam.orthographic = true;
        cam.orthographicSize = 1f;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 10f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = clearColor;
        cam.targetTexture = _texture;

        var face = Instantiate(layoutPrefab, rig.transform);
        cam.cullingMask = 1 << face.gameObject.layer;
        var canvas = face.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
        canvas.planeDistance = 1f;
        canvas.scaleFactor = resolution / referenceSize;

        face.SetPassport(passport);

        // layout groups and TMP normally update at the end of the frame, we need them now
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)face.transform);
        Canvas.ForceUpdateCanvases();

        var request = new RenderPipeline.StandardRequest { destination = _texture };
        if (RenderPipeline.SupportsRenderRequest(cam, request))
            RenderPipeline.SubmitRenderRequest(cam, request);
        else
            cam.Render();

        // unhook first, destroying a camera that still targets the texture wipes it
        cam.targetTexture = null;
        DestroyImmediate(rig);

        _block ??= new MaterialPropertyBlock();
        target.GetPropertyBlock(_block);
        _block.SetTexture(BaseMapId, _texture);
        target.SetPropertyBlock(_block);
        _baked = passport;
    }

    private void ReleaseTexture()
    {
        if (_texture == null) return;
        if (target != null) target.SetPropertyBlock(null);
        _texture.Release();
        DestroyImmediate(_texture);
        _texture = null;
    }
}
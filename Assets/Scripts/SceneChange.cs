using UnityEngine;

public class SceneChange : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Scene name")]
    private string sceneName;

    /// <summary>Replaces all loaded scenes with this scene</summary>
    public void ChangeScene() => SceneGod.SInstance.ChangeScene(sceneName);

    /// <summary>Adds this scene on top of the current ones</summary>
    public void LoadScene() => SceneGod.SInstance.LoadScene(sceneName);

    /// <summary>Removes this scene</summary>
    public void UnloadScene() => SceneGod.SInstance.UnloadScene(sceneName);
}

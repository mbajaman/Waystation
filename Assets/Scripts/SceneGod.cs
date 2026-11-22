using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persistent game manager. Created automatically before the first scene loads,
/// no matter which scene Play is pressed in. Never place this in a scene.
/// </summary>
[AddComponentMenu("")] // hidden from the Add Component menu
[DisallowMultipleComponent]
public class SceneGod : MonoBehaviour
{
	public static SceneGod SInstance { get; private set; }

	public enum GameState { Playing, Paused }
	public GameState CurrentState { get; private set; }

	// handle to the additively loaded scene, so we unload exactly what we loaded
	private Scene _overlayScene;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	// This allows for the singleton to be prefab-less
	private static void Bootstrap()
	{
		var go = new GameObject(nameof(SceneGod));
		DontDestroyOnLoad(go);
		SInstance = go.AddComponent<SceneGod>();
	}

	private void Awake()
	{
		// Only Bootstrap may create SceneGod. Anything else is a bug, so fail loudly.
		if (SInstance != null && SInstance != this)
		{
			Debug.LogError($"A second SceneGod was created on '{gameObject.name}'. Only one may exist; destroying it.", this);
			Destroy(this);
		}
	}

	/// <summary>
	/// Loads a scene on top of the current one and keeps a direct handle to it.
	/// </summary>
	public void LoadOverlay(string sceneName)
	{
		if (_overlayScene.IsValid())
		{
			Debug.LogWarning($"Overlay {_overlayScene.name} already loaded!");
			return;
		}
		_overlayScene = SceneManager.LoadScene(sceneName, new LoadSceneParameters(LoadSceneMode.Additive));
	}

	/// <summary>
	/// Unloads the overlay scene loaded by LoadOverlay.
	/// </summary>
	public void UnloadOverlay()
	{
		if (!_overlayScene.IsValid())
		{
			Debug.LogWarning("No overlay loaded!");
			return;
		}
		SceneManager.UnloadSceneAsync(_overlayScene);
		_overlayScene = default;
	}

	// private enum Scene
	// {
	// 	MainMenu,
	// 	LevelScene1,
	// 	End,
	// }
}

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persistent scene service. Created automatically. Do not put game logic here
/// </summary>
[AddComponentMenu("")] // hidden from the Add Component menu
[DisallowMultipleComponent]
public class SceneGod : MonoBehaviour
{
	public static SceneGod SInstance { get; private set; }

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
	/// Replaces all loaded scenes with the given scene.
	/// </summary>
	public void ChangeScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}

	/// <summary>
	/// Adds a scene on top of the currently loaded ones (e.g. a pause menu).
	/// Remove it again with UnloadScene.
	/// </summary>
	public void LoadScene(string sceneName)
	{
		// Duplicate guard
		if (SceneManager.GetSceneByName(sceneName).IsValid())
		{
			Debug.LogWarning($"Scene '{sceneName}' is already loaded!");
			return;
		}
		SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
	}

	/// <summary>
	/// Removes a scene added with LoadScene. To replace the level, use ChangeScene.
	/// </summary>
	public void UnloadScene(string sceneName)
	{ 
		SceneManager.UnloadSceneAsync(sceneName);
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGod : MonoBehaviour
{
	public static SceneGod SInstance { get; private set; }

	public enum GameState { Playing, Paused }
	public GameState CurrentState { get; private set; }

	// handle to the additively loaded scene, so we unload exactly what we loaded
	private Scene _overlayScene;

	private void Awake()
	{
		// kill itself if there is another
		if (SInstance != null && SInstance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			SInstance = this;
			if (transform.parent != null)
			{
				Debug.LogWarning("SceneGod was parented, detaching to root so it persists.", this);
				transform.SetParent(null);
			}
			DontDestroyOnLoad(gameObject);
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
}

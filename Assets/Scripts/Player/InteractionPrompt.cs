using TMPro;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// World-space label that floats at a point in the scene and faces the camera.
    /// The Interactor decides the text and the position. This only shows it.
    /// </summary>
    public class InteractionPrompt : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private GameObject visual; // the background and text, hidden until Show

        private Camera _camera;
        private Vector3 _worldPosition;
        private bool _visible;

        private void Awake()
        {
            Hide();
        }

        /// <summary>
        /// Shows the label at a world position. Safe to call every frame while focused.
        /// </summary>
        public void Show(string text, Vector3 worldPosition)
        {
            label.text = text;
            _worldPosition = worldPosition;
            if (_visible)
                return;

            _visible = true;
            visual.SetActive(true);
        }

        /// <summary>
        /// Hides the label.
        /// </summary>
        public void Hide()
        {
            _visible = false;
            if (visual != null)
                visual.SetActive(false);
        }

        private void LateUpdate()
        {
            if (!_visible)
                return;

            if (_camera == null)
                _camera = Camera.main;
            if (_camera == null)
                return;

            transform.position = _worldPosition;
            // Match the camera so the canvas stays flat to the view.
            transform.rotation = _camera.transform.rotation;
        }
    }
}

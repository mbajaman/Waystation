using Interaction;
using Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// Looks out from the camera, and outlines whatever IInteractable is in reach.
    /// Press Interact to use it. The prompt shows the binding and the target's own text.
    /// </summary>
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private Transform view; // BobPivot, the camera follow target
        [SerializeField] private float reach = 2.5f; // metres
        [SerializeField] private float castRadius = 0.1f; // metres
        [SerializeField] private LayerMask mask = Physics.DefaultRaycastLayers;
        [SerializeField] private InteractionPrompt prompt;
        [SerializeField] private float promptOffset = 0.15f; // metres above the target's bounds

        private InputAction _interact;
        private IInteractable _focus;
        private Component _focusComponent; // Unity's null check only works on the Component
        private Renderer[] _outlined;

        private void Awake()
        {
            _interact = InputSystem.actions.FindAction("Interact");
            if (_interact == null)
                Debug.LogError("No Interact action in the input actions.", this);
        }

        private void Update()
        {
            IInteractable next = FindTarget();
            if (!IsFocused(next))
                SetFocus(next);

            if (_focusComponent == null)
            {
                if (prompt != null)
                    prompt.Hide();
                return;
            }

            if (prompt != null)
                prompt.Show(PromptText(), PromptPosition());

            if (_interact != null && _interact.WasPressedThisFrame())
                _focus.Interact(this);
        }

        private void OnDisable()
        {
            SetFocus(null);
            if (prompt != null)
                prompt.Hide();
        }

        private IInteractable FindTarget()
        {
            if (view == null)
                return null;

            if (!Physics.SphereCast(view.position, castRadius, view.forward, out RaycastHit hit, reach, mask, QueryTriggerInteraction.Ignore))
                return null;

            IInteractable candidate = hit.collider.GetComponentInParent<IInteractable>();
            if (candidate == null || !candidate.CanInteract(this))
                return null;

            return candidate;
        }

        private bool IsFocused(IInteractable candidate)
        {
            return (candidate as Component) == _focusComponent;
        }

        private void SetFocus(IInteractable next)
        {
            Outline.Set(_outlined, false);
            _outlined = null;
            _focus = next;
            _focusComponent = next as Component;

            if (_focusComponent == null)
                return;

            _outlined = _focusComponent.GetComponentsInChildren<Renderer>();
            Outline.Set(_outlined, true);
        }

        private string PromptText()
        {
            string binding = _interact != null ? _interact.GetBindingDisplayString() : "";
            if (string.IsNullOrEmpty(binding))
                return _focus.Prompt;

            return $"[{binding}] {_focus.Prompt}";
        }

        private Vector3 PromptPosition()
        {
            if (_outlined == null || _outlined.Length == 0)
                return _focusComponent.transform.position + Vector3.up * promptOffset;

            bool hasBounds = false;
            Bounds bounds = default;
            for (int i = 0; i < _outlined.Length; i++)
            {
                Renderer renderer = _outlined[i];
                if (renderer == null)
                    continue;

                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            if (!hasBounds)
                return _focusComponent.transform.position + Vector3.up * promptOffset;

            return bounds.center + Vector3.up * (bounds.extents.y + promptOffset);
        }
    }
}

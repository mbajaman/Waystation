using Player;
using UnityEngine;

namespace Interaction
{
    /// <summary>
    /// A physics prop the player can pick up. The Interactor draws the prompt and the outline.
    /// Holding, dropping and throwing belong to PlayerHands. This only changes the body while it is held.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Pickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private string displayName = "Item"; // shown in the prompt
        [SerializeField] private float heldLinearDamping = 5f; // while held
        [SerializeField] private float heldAngularDamping = 5f; // while held

        private Rigidbody _body;
        private bool _held;

        private bool _savedGravity;
        private float _savedLinearDamping;
        private float _savedAngularDamping;

        private Transform[] _layerTargets;
        private int[] _layers;
        private Collider[] _ignored;
        private Collider _holder;

        /// <summary>
        /// Text the Interactor shows, for example "Pick up Crate".
        /// </summary>
        public string Prompt => $"Pick up {displayName}";

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// True when this prop is free and the player is not already holding something.
        /// </summary>
        public bool CanInteract(Interactor interactor)
        {
            if (_held)
                return false;

            PlayerHands hands = interactor.GetComponentInParent<PlayerHands>();
            return hands != null && !hands.IsHolding;
        }

        /// <summary>
        /// Hands this prop to the player's hands.
        /// </summary>
        public void Interact(Interactor interactor)
        {
            PlayerHands hands = interactor.GetComponentInParent<PlayerHands>();
            if (hands != null)
                hands.Hold(this);
        }

        /// <summary>
        /// Turns off gravity, ignores the player, and drops this prop out of the interact ray.
        /// </summary>
        public void BeginHold(Collider holder)
        {
            if (_held)
                return;

            _held = true;
            _savedGravity = _body.useGravity;
            _savedLinearDamping = _body.linearDamping;
            _savedAngularDamping = _body.angularDamping;
            _body.useGravity = false;
            _body.linearDamping = heldLinearDamping;
            _body.angularDamping = heldAngularDamping;

            int ignoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
            _layerTargets = GetComponentsInChildren<Transform>(true);
            _layers = new int[_layerTargets.Length];
            for (int i = 0; i < _layerTargets.Length; i++)
            {
                _layers[i] = _layerTargets[i].gameObject.layer;
                _layerTargets[i].gameObject.layer = ignoreRaycast;
            }

            _holder = holder;
            if (holder == null)
                return;

            _ignored = GetComponentsInChildren<Collider>();
            for (int i = 0; i < _ignored.Length; i++)
                Physics.IgnoreCollision(_ignored[i], holder, true);
        }

        /// <summary>
        /// Puts gravity, damping, layers and player collision back the way they were.
        /// </summary>
        public void EndHold()
        {
            if (!_held)
                return;

            _held = false;
            _body.useGravity = _savedGravity;
            _body.linearDamping = _savedLinearDamping;
            _body.angularDamping = _savedAngularDamping;

            if (_layerTargets != null)
            {
                for (int i = 0; i < _layerTargets.Length; i++)
                {
                    if (_layerTargets[i] != null)
                        _layerTargets[i].gameObject.layer = _layers[i];
                }
            }

            if (_holder != null && _ignored != null)
            {
                for (int i = 0; i < _ignored.Length; i++)
                {
                    if (_ignored[i] != null)
                        Physics.IgnoreCollision(_ignored[i], _holder, false);
                }
            }

            _layerTargets = null;
            _layers = null;
            _ignored = null;
            _holder = null;
        }
    }
}

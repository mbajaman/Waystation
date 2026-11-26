using Interaction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// Holds one pickup in front of the player and pulls it along with physics.
    /// A short press of Drop lets it fall. Holding Drop throws it, harder the longer it is held.
    /// </summary>
    public class PlayerHands : MonoBehaviour
    {
        [SerializeField] private Transform holdPoint; // child of Head, in front of the camera
        [SerializeField] private float followStrength = 15f; // 1/s
        [SerializeField] private float maxFollowSpeed = 12f; // m/s
        [SerializeField] private float rotateStrength = 10f;
        [SerializeField] private float breakDistance = 1.5f; // metres, snag on a wall and it drops
        [SerializeField] private float tapTime = 0.2f; // seconds, shorter than this drops instead of throwing
        [SerializeField] private float maxChargeTime = 1f; // seconds
        [SerializeField] private float minThrowImpulse = 2f;
        [SerializeField] private float maxThrowImpulse = 12f;

        private InputAction _drop;
        private Pickup _pickup;
        private Rigidbody _body;
        private float _dropPressedAt = -1f;
        private bool _missingHoldPoint;

        /// <summary>
        /// True while a prop is in the player's hands.
        /// </summary>
        public bool IsHolding => _pickup != null;

        private void Awake()
        {
            _drop = InputSystem.actions.FindAction("Drop");
            if (_drop == null)
                Debug.LogError("No Drop action in the input actions.", this);
        }

        /// <summary>
        /// Takes a prop and keeps the player's collider from fighting it.
        /// </summary>
        public void Hold(Pickup pickup)
        {
            if (pickup == null || IsHolding)
                return;

            pickup.BeginHold(GetComponent<CharacterController>());
            _pickup = pickup;
            _body = pickup.GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Lets go. A zero impulse drops the prop with no speed. Any other vector is an impulse.
        /// </summary>
        public void Release(Vector3 impulse)
        {
            Pickup pickup = _pickup;
            Rigidbody body = _body;
            _pickup = null;
            _body = null;
            _dropPressedAt = -1f;

            if (pickup != null)
                pickup.EndHold();
            if (body == null)
                return;

            if (impulse.sqrMagnitude < 0.0001f)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                return;
            }

            body.AddForce(impulse, ForceMode.Impulse);
        }

        private void OnDisable()
        {
            if (IsHolding)
                Release(Vector3.zero);
        }

        private void Update()
        {
            if (_pickup == null || _drop == null)
                return;

            if (_drop.WasPressedThisFrame())
                _dropPressedAt = Time.time;

            if (!_drop.WasReleasedThisFrame() || _dropPressedAt < 0f)
                return;

            float heldTime = Time.time - _dropPressedAt;
            _dropPressedAt = -1f;

            if (heldTime < tapTime || holdPoint == null)
            {
                Release(Vector3.zero);
                return;
            }

            float charge = Mathf.InverseLerp(tapTime, maxChargeTime, heldTime);
            float impulse = Mathf.Lerp(minThrowImpulse, maxThrowImpulse, charge);
            Release(holdPoint.forward * impulse);
        }

        private void FixedUpdate()
        {
            if (_pickup == null || _body == null)
                return;

            if (holdPoint == null)
            {
                if (!_missingHoldPoint)
                {
                    Debug.LogError("PlayerHands has no hold point.", this);
                    _missingHoldPoint = true;
                }
                return;
            }

            Vector3 toHold = holdPoint.position - _body.position;
            if (toHold.magnitude > breakDistance)
            {
                Release(Vector3.zero);
                return;
            }

            _body.linearVelocity = Vector3.ClampMagnitude(toHold * followStrength, maxFollowSpeed);
            RotateTowardHold();
        }

        private void RotateTowardHold()
        {
            Quaternion delta = holdPoint.rotation * Quaternion.Inverse(_body.rotation);
            delta.ToAngleAxis(out float degrees, out Vector3 axis);
            if (degrees > 180f)
                degrees -= 360f;

            // ToAngleAxis leaves the axis undefined when the rotation is already matched.
            if (degrees * degrees < 0.01f || float.IsInfinity(axis.sqrMagnitude))
            {
                _body.angularVelocity = Vector3.zero;
                return;
            }

            _body.angularVelocity = axis.normalized * (degrees * Mathf.Deg2Rad * rotateStrength);
        }
    }
}

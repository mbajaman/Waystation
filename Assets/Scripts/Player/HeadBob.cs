using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class HeadBob : MonoBehaviour
    {
        [SerializeField] private Transform bobPivot;
        [SerializeField] private float walkSpeed = 5f; // match PlayerMovement.moveSpeed
        [SerializeField] private float bobHeight = 0.014f; // metres, one bob per footfall
        [SerializeField] private float bobSway = 0.006f; // metres, one sway per two steps
        [SerializeField] private float rollDegrees = 0.2f;
        [SerializeField] private float stepRate = 1.8f; // footfalls per second at walkSpeed
        [SerializeField] private float settleTime = 0.09f;

        private InputAction _move;
        private Vector3 _baseLocal;
        private float _phase;
        private float _amount;

        private void Awake()
        {
            _baseLocal = bobPivot.localPosition;
            _move = InputSystem.actions.FindAction("Move");
        }

        private void LateUpdate()
        {
            float speed01 = Mathf.Clamp01(_move.ReadValue<Vector2>().magnitude);
            _amount = Mathf.Lerp(_amount, speed01, 1f - Mathf.Exp(-Time.deltaTime / settleTime));

            if (_amount > 0.001f)
                _phase += Time.deltaTime * stepRate * Mathf.Lerp(0.7f, 1f, speed01);

            float y = Mathf.Sin(_phase * Mathf.PI * 2f) * bobHeight * _amount;
            float x = Mathf.Cos(_phase * Mathf.PI) * bobSway * _amount;
            float roll = Mathf.Cos(_phase * Mathf.PI) * rollDegrees * _amount;

            bobPivot.localPosition = _baseLocal + new Vector3(x, y, 0f);
            bobPivot.localRotation = Quaternion.Euler(0f, 0f, roll);
        }
    }
}

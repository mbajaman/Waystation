using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSensitivity = 0.1f;
    [SerializeField] private Transform head;
    [SerializeField] private float gravity = -9.81f;
    
    private CharacterController _ctrl;
    private InputAction _moveAction, _lookAction, _jumpAction;
    private float _pitch;

    void Awake()
    {
        //Later this needs to go into the state machine or ideally one location to make sure we dont have to trace it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //setup
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _jumpAction = InputSystem.actions.FindAction("Jump"); //imma do it later
        _ctrl = GetComponent<CharacterController>();
    }

    void Update()
    {
        // convinence will do cleaner setup later
        // #if UNITY_EDITOR
        //         if (Keyboard.current.escapeKey.wasPressedThisFrame)
        //             UnityEditor.EditorApplication.isPlaying = false;
        // #endif
        
        //movement
        Vector2 input = _moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        _ctrl.Move(move * (moveSpeed * Time.deltaTime));

        //gravity
        _ctrl.Move(Vector3.up * (gravity * Time.deltaTime));

        // Head
        Vector2 look = _lookAction.ReadValue<Vector2>();
        transform.Rotate(0f, look.x * lookSensitivity, 0f);
        _pitch = Mathf.Clamp(_pitch - look.y * lookSensitivity, -80f, 80f);
        head.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }
}

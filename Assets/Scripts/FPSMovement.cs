using UnityEngine;
using UnityEngine.InputSystem;

public class FPSMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2f;

    [Header("Jump and  Fall Settings")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float gravity = -12f;
    [SerializeField] private float initialFallSpeed = -2f;

    [Header("Crouch Settings")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchingHeight = 1f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private float cameraOffset = 0.4f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;

    private CharacterController _characterController;
    private Vector2 _moveInput;
    private bool _isGrounded;
    private bool _isSprinting;
    private bool _isCrouching;
    private float _verticalVelocity;
    private float _targetHeight;
 

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _targetHeight = standingHeight;

        if (_characterController == null)
        {
            Debug.LogError("CharacterController component is missing on this GameObject.");
        }
    }

    private void OnEnable()
    {
        moveAction.action.performed += StoreMovementInput;
        moveAction.action.canceled += StoreMovementInput;
        jumpAction.action.performed += Jump;
        sprintAction.action.performed += Sprint;
        sprintAction.action.canceled += Sprint;
        crouchAction.action.performed += Crouch;


    }

    private void OnDisable()
    {
        moveAction.action.performed -= StoreMovementInput;
        moveAction.action.canceled -= StoreMovementInput;
        jumpAction.action.performed -= Jump;
        sprintAction.action.performed -= Sprint;
        sprintAction.action.canceled -= Sprint;
        crouchAction.action.performed -= Crouch;

    }


    private void Update()
    {
        _isGrounded = _characterController.isGrounded;
        HandleGravity();
        HandleMovement();
        HandleCrouchTransition();
    }

    private void StoreMovementInput(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_isGrounded)
        {
            _verticalVelocity = jumpForce;
        }
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        if (_isCrouching)
        {
            if (!CanStandUp())
            {
                return; 
            }
            _targetHeight = standingHeight;
        }
        else
        {
            _targetHeight = crouchingHeight;
        }
        _isCrouching = !_isCrouching;
    }

    private bool CanStandUp()
    {
        return !Physics.CapsuleCast(
            transform.position + _characterController.center, 
            transform.position + (Vector3.up * _characterController.height / 2f), 
            _characterController.radius, 
            Vector3.up
            );
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        _isSprinting = context.performed;
    }
    
    private void HandleGravity()
    {
        if (_isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = initialFallSpeed; // Small negative value to keep the character grounded
        }
        _verticalVelocity += gravity * Time.deltaTime; // Apply gravity over time

    }

    private void HandleMovement()
    {
        var move = cameraTransform.TransformDirection(new Vector3(_moveInput.x, 0, _moveInput.y)).normalized;
        var currentSpeed = _isCrouching ? crouchSpeed : _isSprinting ? sprintSpeed : walkSpeed;
        var finalMove = move * currentSpeed;
        finalMove.y = _verticalVelocity; 

        var collisionFlags = _characterController.Move(finalMove * Time.deltaTime);
        if ((collisionFlags & CollisionFlags.Above) != 0)
        {
            _verticalVelocity = initialFallSpeed; 
        }
    }

    private void HandleCrouchTransition()
    {
        float currentHeight = _characterController.height;
        if (Mathf.Abs(currentHeight - _targetHeight) < 0.01f)
        {
            _characterController.height = _targetHeight; // Snap to target height if close enough
            return;
        }


        float newHeight = Mathf.Lerp(currentHeight, _targetHeight, Time.deltaTime * crouchTransitionSpeed);
        _characterController.height = newHeight;
        _characterController.center = Vector3.up * (newHeight / 2f); 

        var cameraLocalPos = cameraTransform.localPosition;
        cameraLocalPos.y = _targetHeight - cameraOffset;
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition, 
            cameraLocalPos, 
            Time.deltaTime * crouchTransitionSpeed);
    }
}

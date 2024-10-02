using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Sensibility")] 
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;
    
    [Header("Camera")]
    [SerializeField] Transform cameraTransform;
    
    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpsCooldown;
    [SerializeField] private float airMultiplier;
    
    [Header("Crouching")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float startCrouchYScale;
    
    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    
    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;

    private RaycastHit _slopeHit;
    private Transform _selfTransform;
    private Rigidbody _selfRigidbody;
    
    private bool _isGrounded;
    private bool _readyToJump = true;
    private bool _isJumpingHeld = false;

    private float _xRotation;
    private float _yRotation;
    private float _movementSpeed;
    private float _crouchYScale;

    private Vector2 _movementInput;
    private Vector3 _movementDirection;

    private MovementState _movementState;
    
    public enum MovementState
    {
        Walking,
        Sprinting,
        Crouching,
        Air
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _selfTransform = transform;
        _selfRigidbody = GetComponent<Rigidbody>();
        _selfRigidbody.freezeRotation = true;
        
        _movementSpeed = walkSpeed;
        startCrouchYScale = _selfTransform.localScale.y;
    }

    private void FixedUpdate()
    {
        _movementDirection = _selfTransform.right * _movementInput.x + _selfTransform.forward * _movementInput.y;

        if (IsGrounded())
        {
            if (OnSlope())
            {
                Vector3 slopeDirection = GetSlopeMoveDirection();
                _selfRigidbody.AddForce(slopeDirection * _movementSpeed * 1000f * Time.deltaTime, ForceMode.Force);
            }
            else
            {
                _selfRigidbody.AddForce(_movementDirection.normalized * _movementSpeed * 1000f * Time.deltaTime, ForceMode.Force);
            }
        }
        else
        {
            // Air movement with air multiplier
            _selfRigidbody.AddForce(_movementDirection.normalized * _movementSpeed * 1000f * airMultiplier * Time.deltaTime, ForceMode.Force);
        }
    }

    private void Update()
    {
        _selfRigidbody.linearDamping = IsGrounded() ? groundDrag : 0.0f;
        
        // Check velocity limits
        SpeedControl();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Started)
        {
            _movementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _movementInput = Vector2.zero;  
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        float mouseX = context.ReadValue<Vector2>().x  * sensX;
        float mouseY = context.ReadValue<Vector2>().y  * sensY;

        _yRotation += mouseX;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        
        _selfTransform.localRotation = Quaternion.Euler(0f, _yRotation, 0f);
        cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Started)
        {
            _isJumpingHeld = true;
            if (_readyToJump)
            {
                StartCoroutine(JumpRoutine());
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _isJumpingHeld = false;
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Started)
        {
            if (_movementState != MovementState.Sprinting && IsGrounded())
            {
                _movementSpeed = sprintSpeed;
                _movementState = MovementState.Sprinting;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _movementSpeed = walkSpeed;
            _movementState = MovementState.Walking;
        }
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _selfTransform.localScale = new Vector3(_selfTransform.localScale.x, _crouchYScale, _selfTransform.localScale.z);
            _movementSpeed = crouchSpeed;
            _selfRigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            _movementState = MovementState.Crouching;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _selfTransform.localScale = new Vector3(_selfTransform.localScale.x, startCrouchYScale, _selfTransform.localScale.z);
            _movementSpeed = walkSpeed;
            _movementState = MovementState.Walking;
        }
    }

    private IEnumerator JumpRoutine()
    {
        while (_isJumpingHeld)
        {
            if (IsGrounded() && _readyToJump)
            {
                _selfRigidbody.linearVelocity = new Vector3(_selfRigidbody.linearVelocity.x, 0, _selfRigidbody.linearVelocity.z);
                _selfRigidbody.AddForce(_selfTransform.up * jumpForce, ForceMode.Impulse);

                _readyToJump = false;
                yield return new WaitForSeconds(jumpsCooldown);
                _readyToJump = true;
            }
            else
            {
                yield return null;
            }
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(_selfRigidbody.linearVelocity.x, 0, _selfRigidbody.linearVelocity.z);

        if (flatVelocity.magnitude > _movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * _movementSpeed;
            _selfRigidbody.linearVelocity = new Vector3(limitedVelocity.x, _selfRigidbody.linearVelocity.y, limitedVelocity.z);
        }
    }

    private bool IsGrounded()
    {
        float castRadius = 0.3f;
        float castDistance = playerHeight * 0.5f + 0.3f;
        return Physics.SphereCast(_selfTransform.position, castRadius, Vector3.down, out _slopeHit, castDistance, whatIsGround);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(_selfTransform.position, Vector3.down, out _slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < maxSlopeAngle && angle > 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_movementDirection, _slopeHit.normal).normalized;
    }
}

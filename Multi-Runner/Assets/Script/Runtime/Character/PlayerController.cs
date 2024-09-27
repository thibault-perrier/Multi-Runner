using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Sensibility")] 
    [SerializeField] private float sensX = 2.0f;
    [SerializeField] private float sensY = 2.0f;
    
    [Header("Camera")]
    [SerializeField] Transform cameraTransform;
    
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 7.0f;
    [SerializeField] private float groundDrag = 0.5f;
    [SerializeField] private float jumpForce = 2.0f;
    [SerializeField] private float jumpsCooldown = 2.0f;
    [SerializeField] private float airMultiplier = 1.0f;
    
    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    
    private Transform _selfTransform;
    private Rigidbody _selfRigidbody;
    
    private bool _isGrounded;
    private bool _readyToJump = true;
    private bool _isJumpingHeld = false;
    
    private float _xRotation;
    private float _yRotation;
    
    private Vector2 _movementInput;
    private Vector3 _movementDirection;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _selfTransform = transform;
        _selfRigidbody = GetComponent<Rigidbody>();
        _selfRigidbody.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        _movementDirection = _selfTransform.right * _movementInput.x + _selfTransform.forward * _movementInput.y;
        
        if (_isGrounded)
            _selfRigidbody.AddForce(_movementDirection.normalized * movementSpeed * 1000f * Time.deltaTime, ForceMode.Force);
        else
            _selfRigidbody.AddForce(
                _movementDirection.normalized * movementSpeed * 1000f * airMultiplier * Time.deltaTime,
                ForceMode.Force);
    }

    private void Update()
    {
        // Check if the player is grounded
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        
        _selfRigidbody.linearDamping = _isGrounded ? groundDrag : 0.0f;
        
        // Check velocity limits
        SpeedControle();
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
        float mouseX = context.ReadValue<Vector2>().x * Time.deltaTime * sensX;
        float mouseY = context.ReadValue<Vector2>().y * Time.deltaTime * sensY;

        _yRotation += mouseX;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        
        _selfTransform.localRotation = Quaternion.Euler(0f, _yRotation, 0f);
        cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }

    // Jump logic
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Started)
        {
            _isJumpingHeld = true;  // Mark that the jump button is held down
            if (_readyToJump)
            {
                StartCoroutine(JumpRoutine());  // Start the jump routine
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _isJumpingHeld = false;  // Stop jumping when button is released
        }
    }

    private IEnumerator JumpRoutine()
    {
        while (_isJumpingHeld)  // Keep jumping as long as the button is held down
        {
            if (_isGrounded && _readyToJump)
            {
                // Reset y velocity to zero before jumping
                _selfRigidbody.linearVelocity = new Vector3(_selfRigidbody.linearVelocity.x, 0, _selfRigidbody.linearVelocity.z);
                
                // Apply the jump force
                _selfRigidbody.AddForce(_selfTransform.up * jumpForce, ForceMode.Impulse);
                
                _readyToJump = false;
                
                // Set jump cooldown
                yield return new WaitForSeconds(jumpsCooldown);  // Wait for the cooldown before the next jump
                
                _readyToJump = true;  // Allow jumping again after cooldown
            }
            else
            {
                yield return null;  // Wait until the player is grounded
            }
        }
    }
    
    private void SpeedControle()
    {
        // Limit the player's velocity on the x and z axes to avoid exceeding movementSpeed
        Vector3 flatVelocity = new Vector3(_selfRigidbody.linearVelocity.x, 0, _selfRigidbody.linearVelocity.z);

        if (flatVelocity.magnitude > movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed;
            _selfRigidbody.linearVelocity = new Vector3(limitedVelocity.x, _selfRigidbody.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void ReadyToJump()
    {
        _readyToJump = true;
    }
}

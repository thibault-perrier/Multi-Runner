using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Sensibility")] 
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;
    
    [Header("Camera")]
    [SerializeField] Transform cameraTransform;
    
    [Header("Movement")]
    [SerializeField] private float walkSpeed;
     public float sprintSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpsCooldown;
    [SerializeField] private float airMultiplier;
    
    [Header("Crouching")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    
    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    
    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;

    [Header("Debug")] 
    [SerializeField] private TextMeshProUGUI debugSpeed;
    
    [HideInInspector] public bool sliding = false;
    
    
    
    private bool _sprinting = false;
    
    
    
    private RaycastHit _slopeHit;
    private Transform _selfTransform;
    private Rigidbody _selfRigidbody;
    
    private bool _readyToJump = true;

    private float _xRotation;
    private float _yRotation; 
    [HideInInspector] public float currentMovementSpeed;
    [HideInInspector] public float newMovementSpeed;
    private float _lastMovementSpeed;

    private Vector2 _movementInput;
    [HideInInspector] public Vector3 MovementDirection {private set; get;}

    [HideInInspector] public MovementState movementState;
    
    public enum MovementState
    {
        Walking,
        Sprinting,
        Crouching,
        Sliding,
        Air
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _selfTransform = transform;
        _selfRigidbody = GetComponent<Rigidbody>();
        _selfRigidbody.freezeRotation = true;
        
        currentMovementSpeed = walkSpeed;
    }

    private void FixedUpdate()
    {
        MovementDirection = _selfTransform.right * _movementInput.x + _selfTransform.forward * _movementInput.y;
        

        if (IsGrounded())
        {
            if (OnSlope())
            {
                Vector3 slopeDirection = GetSlopeMoveDirection(MovementDirection);
                _selfRigidbody.AddForce(slopeDirection * currentMovementSpeed * 1000f * Time.deltaTime, ForceMode.Force);
            }
            else
            {
                _selfRigidbody.AddForce(MovementDirection.normalized * currentMovementSpeed * 1000f * Time.deltaTime, ForceMode.Force);
            }
        }
        else
        {
            _selfRigidbody.AddForce(MovementDirection.normalized * currentMovementSpeed * 1000f * airMultiplier * Time.deltaTime, ForceMode.Force);
        }
    }

    private void Update()
    {

        if (Mathf.Abs(_lastMovementSpeed - newMovementSpeed) > 4f)
        {
            // call coroutine to smoothly change velocity 
            StopCoroutine(SmoothSpeedMovementChange());
            StartCoroutine(SmoothSpeedMovementChange());
            
             _lastMovementSpeed = newMovementSpeed;
        }
        else if (Mathf.Abs(newMovementSpeed - currentMovementSpeed) < 3f)
        {
            currentMovementSpeed = newMovementSpeed;
        }
        else
        {
            _lastMovementSpeed = newMovementSpeed;
        }
        
        _selfRigidbody.linearDamping = IsGrounded() ? groundDrag : 0.0f;
        SpeedControl();
        
        debugSpeed.text = _selfRigidbody.linearVelocity.magnitude.ToString("0.00");
    }
    

    private IEnumerator SmoothSpeedMovementChange()
    {

        float time = 0;
        float startspeed = currentMovementSpeed;
        float duration = Mathf.Abs(startspeed - newMovementSpeed);
        float newspeed = newMovementSpeed;

        while (time < duration)
        {
            currentMovementSpeed = Mathf.Lerp(startspeed, newspeed, time / duration);
            time += Time.deltaTime;

            if (newspeed != newMovementSpeed)
            {
                yield break;
            }
            
            yield return null;
            
            
        }


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
        float mouseX = context.ReadValue<Vector2>().x * sensX;
        float mouseY = context.ReadValue<Vector2>().y * sensY;

        
        _yRotation += mouseX;
        _selfTransform.localRotation = Quaternion.Euler(0f, _yRotation, 0f); 
    
        
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f); 
        cameraTransform.localRotation = Quaternion.Euler(_xRotation, cameraTransform.localRotation.eulerAngles.y, 0f); 
    }


    public void Jump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && _readyToJump && IsGrounded())
        {
            _selfRigidbody.linearVelocity = new Vector3(_selfRigidbody.linearVelocity.x, 0, _selfRigidbody.linearVelocity.z);
            _selfRigidbody.AddForce(_selfTransform.up * jumpForce, ForceMode.Impulse);

            _readyToJump = false;
            StartCoroutine(JumpCooldown());
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed  && IsGrounded())
        {
            currentMovementSpeed = sprintSpeed;
            movementState = MovementState.Sprinting;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            currentMovementSpeed = walkSpeed;
            movementState = MovementState.Walking;
        }
    }

    
    private IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpsCooldown);
        _readyToJump = true;
    }

    private void SpeedControl()
    {
        if (OnSlope())
        {
            if (_selfRigidbody.linearVelocity.magnitude > currentMovementSpeed)
            {
                _selfRigidbody.linearVelocity = _selfRigidbody.linearVelocity.normalized * currentMovementSpeed;
            }
        }
        else
        {
            Vector3 flatVelocity = new Vector3(_selfRigidbody.linearVelocity.x, 0, _selfRigidbody.linearVelocity.z);
            if (flatVelocity.magnitude > currentMovementSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * currentMovementSpeed;
                _selfRigidbody.linearVelocity = new Vector3(limitedVelocity.x, _selfRigidbody.linearVelocity.y, limitedVelocity.z);
            }
        }
    }

    private bool IsGrounded()
    {
        float castRadius = 0.3f;
        float castDistance = playerHeight * 0.5f + 0.3f;
        bool isGrounded = Physics.Raycast(_selfTransform.position, Vector3.down, out _slopeHit, castDistance, whatIsGround);

        if (OnSlope() && isGrounded)
        {
            return true;
        }
        return isGrounded;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(_selfTransform.position, Vector3.down, out _slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(transform.up, _slopeHit.normal);
            
            return angle < maxSlopeAngle && angle > 0;
        }
        
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }
    
    
}

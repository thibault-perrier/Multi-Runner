using System;
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
    
    [Header("Ground Check")]
    [SerializeField] private float playerHeigth;
    [SerializeField] private LayerMask whatIsGround;
    
    private Transform _selfTransform;
    private Rigidbody _selfRigidbody;
    
    private bool _isGrounded;
    
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
        _movementDirection = cameraTransform.right * _movementInput.x + cameraTransform.forward * _movementInput.y;
        _selfRigidbody.AddForce(_movementDirection.normalized * movementSpeed * 10f, ForceMode.Force);
    }

    private void Update()
    {
        _isGrounded = Physics.Raycast(transform.position,Vector3.down, playerHeigth * 0.5f + 0.2f, whatIsGround);
        Debug.Log(_isGrounded);
        
        if(_isGrounded)
            _selfRigidbody.linearDamping = groundDrag;
        else
            _selfRigidbody.linearDamping = 0.0f;
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

   
    
}

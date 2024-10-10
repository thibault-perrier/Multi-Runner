using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrouchnSlideComponent : MonoBehaviour
{
    
    [Header("References")]
    private Transform _selfTransform;
    private Rigidbody _selfRigidBody;
    private PlayerController _playerController;
    
    [Header("Slinding Options")]
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce;
    [SerializeField] private float slideYScale;
    private float _slideTimer;
    private bool _sliding;
    
    
    [Header("Crouching Options")]
    [SerializeField] private float crouchYScale;
    [SerializeField] private float crouchSpeed;
    private float _startYScale;

    private Vector3 _inputDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _selfTransform = transform;
        _selfRigidBody = GetComponent<Rigidbody>();
        _playerController = GetComponent<PlayerController>();
        _startYScale = transform.localScale.y;
    }

    private void Update()
    {
        _inputDirection = _playerController.movementDirection;
    }

    private void FixedUpdate()
    {
        if (_sliding)
            SlidingMovement();
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (_selfRigidBody.linearVelocity.magnitude > 7.0f)
            {
                StartSlide();
            }
            else
            {
                StopSlide();
                _selfTransform.localScale = new Vector3(_selfTransform.localScale.x, crouchYScale, _selfTransform.localScale.z);
                _playerController.movementSpeed = crouchSpeed;
                _selfRigidBody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
                _playerController.movementState = PlayerController.MovementState.Crouching;
            }
        }
        else if (context.phase == InputActionPhase.Performed)
        {
            if (_selfRigidBody.linearVelocity.magnitude > 7.0f)
            {
                StartSlide();
            }
            else
            {
                StopSlide();
                _selfTransform.localScale = new Vector3(_selfTransform.localScale.x, crouchYScale, _selfTransform.localScale.z);
                _playerController.movementSpeed = crouchSpeed;
                _selfRigidBody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
                _playerController.movementState = PlayerController.MovementState.Crouching;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            StopSlide();
            _selfTransform.localScale = new Vector3(_selfTransform.localScale.x, _startYScale, _selfTransform.localScale.z);
            _playerController.movementSpeed = _playerController.walkSpeed;
            _playerController.movementState = PlayerController.MovementState.Walking;
        }
    }

    private void StartSlide()
    {
        _sliding = true;
        
        _selfTransform.localScale = new Vector3(_selfTransform.localScale.x, slideYScale, _selfTransform.localScale.z);
        
        _selfRigidBody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        
        _slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        _selfRigidBody.AddForce(_inputDirection.normalized * slideForce, ForceMode.Force);
        
        _slideTimer -= Time.deltaTime;

        if (_slideTimer <= 0)
            StopSlide();
        
    }
    
    private void StopSlide()
    {
        _sliding = false;
        _selfTransform.localScale = new Vector3(_selfTransform.localScale.x, _startYScale, _selfTransform.localScale.z);
    }
}

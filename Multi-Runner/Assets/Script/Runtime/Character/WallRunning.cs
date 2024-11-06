using System;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
     [Header("Wall running settings")]
     [SerializeField] private LayerMask whatIsWall;
     [SerializeField] private LayerMask whatIsGround;
     [SerializeField] private float wallRunForce;
     [SerializeField] private float maxWallRunTime;
     private float _wallRunTimer;
     
     [Header("Detection settings")]
     [SerializeField] private float wallCheckDistance;
     [SerializeField] private float minJumpHeight;
     private RaycastHit _rightWallHit;
     private RaycastHit _leftWallHit;
     private bool _wallLeft;
     private bool _wallRight;

     [Header("References")] 
     private Rigidbody _selfRigidbody;
     private PlayerController _playerController;
     private Transform _selfTransform;

     private void Start()
     {
          _selfRigidbody = GetComponent<Rigidbody>();
          _selfTransform = transform;
          _playerController = GetComponent<PlayerController>();
     }

     private void Update()
     {
          CheckForWall();
     }


     private void CheckForWall()
     {
          _wallRight = Physics.Raycast(_selfTransform.position, _selfTransform.right, out _rightWallHit, whatIsWall);
          _wallRight = Physics.Raycast(_selfTransform.position, -_selfTransform.right, out _leftWallHit, whatIsWall);
     }

     private bool AboveGround()
     {
          return !Physics.Raycast(_selfTransform.position, Vector3.down , minJumpHeight, whatIsGround);
     }
}

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
     private RaycastHit2D _rightWallHit;
     private RaycastHit2D _leftWallHit;
     private bool _wallLeft;
     private bool _wallRight;
     


}

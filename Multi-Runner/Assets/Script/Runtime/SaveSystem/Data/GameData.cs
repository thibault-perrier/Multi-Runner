using UnityEngine;
using UnityEngine.UI;

public class GameData
{
    public float CurrentMovementSpeed;
    public float NewMovementSpeed;
    public PlayerController.MovementState CurrentMovementState;
    
    public GameData()
    {
        this.CurrentMovementSpeed = 0;
        this.NewMovementSpeed = 0;
        this.CurrentMovementState = PlayerController.MovementState.Walking;

    }
}

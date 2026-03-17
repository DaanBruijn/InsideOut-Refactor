using UnityEngine;

// - Script for the jump object in the game
// - Gives data to the controller for how far/high the jump is
// - Daniel Bruijn

public class JumpData : MonoBehaviour
{
    [Header("Jump Info")] 
    public float jumpHeightScale; // - Float for how long the car stays in the air
    public float jumpPushScale; // - Float for how far the car jumps
}

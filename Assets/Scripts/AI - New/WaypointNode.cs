using UnityEngine;

// - Script for Using the WaypointNodes in the Level
// - Used for the AI cars to follow
// - Daniel Bruijn

public class WaypointNode : MonoBehaviour
{
    // - Variables
    // - Public
    [Header("Speed set once Waypoint reached")]
    public float maxSpeed = 0;
    
    [Header("Waypoint AI is going Towards - Not yet reached")]
    public float minDistanceToWayPoint = 1.5f;

    public WaypointNode[] nextWaypointNode;
}

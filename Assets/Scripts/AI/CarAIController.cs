using System.Collections;
using UnityEngine;
using System.Linq;

// - Script for handling the AI cars in the Game
// - Daniel Bruijn

public class CarAIController : MonoBehaviour
{
    // - Variables
    public enum AIMode
    {
        followPlayer,
        followWaypoints,
        followMouse
    };
    
    // - Public
    [Header("AI Settings")]
    public AIMode aiMode;
    public float maxSpeed = 6.5f;
    public bool isAvoidingCars = true;
    
    // - Private
    private Vector3 targetPosition = Vector3.zero;
    private Transform targetTransform = null;
    
    // - Waypoints
    WaypointNode currentWaypointNode = null;
    WaypointNode[] allWaypoints;
    
    // - Avoidance
    Vector2 avoidanceVectorLerp = Vector3.zero;
    
    // - Colliders
    BoxCollider2D boxCollider2D;
    
    // - Components
    CarController carController;

    void Awake()
    {
        // - References - Sets all the components 
        carController = GetComponent<CarController>();
        allWaypoints = FindObjectsOfType<WaypointNode>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        Vector2 inputVector = Vector2.zero;

        switch (aiMode)
        {
            case AIMode.followPlayer:
                FollowPlayer();
                break;
            case  AIMode.followWaypoints:
                FollowWayPoint();
                break;
            case AIMode.followMouse:
                FollowMousePosition();
                break;
        }

        inputVector.x = turnTowardsTarget();
        inputVector.y = applyDrivingForce(inputVector.x);

        // - Send Input to CarController
        carController.SetInputVector(inputVector);
        
        Debug.DrawLine(transform.position, targetPosition, Color.red);
    }

    void FollowPlayer()
    {
        // - Handles the Tranform to Player Transform
        if (targetTransform == null)
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (targetTransform != null)
            targetPosition = targetTransform.position;
    }

    void FollowWayPoint()
    {
        // - Pick the Closest waypoint if none are selected yet
        if (currentWaypointNode == null)
            currentWaypointNode = FindClosestWaypoint();

        // - Set Target to next waypoint position
        if (currentWaypointNode != null)
        {
            targetPosition = currentWaypointNode.transform.position;
            
            // - Store distnace to target position
            float distanceToWayPoint = (transform.position - targetPosition).magnitude;

            if (distanceToWayPoint <= currentWaypointNode.minDistanceToWayPoint)
            {
                if (currentWaypointNode.maxSpeed > 0)
                    maxSpeed = currentWaypointNode.maxSpeed;
                else maxSpeed = 100; // - Set high number to make car travel max speed
                
                currentWaypointNode = currentWaypointNode.nextWaypointNode[Random.Range(0, currentWaypointNode.nextWaypointNode.Length)];
            }
        }
    }

    void FollowMousePosition()
    {
        // - Converts Mouse Position to World Position
        Vector3 worldPosition  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // - Set target location
        targetPosition = worldPosition;
    }

    WaypointNode FindClosestWaypoint()
    {
        // - Find the next closest Waypoint
        return allWaypoints.OrderBy(t => Vector3.Distance(transform.position, t.transform.position))
            .FirstOrDefault();
    }

    float turnTowardsTarget()
    {
        Vector2 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.Normalize();
        
        // - Apply Steering to go around target
        if (isAvoidingCars)
            AvoidCars(vectorToTarget, out vectorToTarget);
        
        // - Calculate angle towards target
        float angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleToTarget *= -1;

        // - Makes car turn more and less depending on corner/angle
        float steerAmount = angleToTarget / 45.0f;
        
        // - Clamp Steering
        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);
        
        return steerAmount;
    }

    float applyDrivingForce(float SteeringAngle)
    {
        // - If car is going to fast do not go faster
        if (carController.GetVelocityMagnitude() > maxSpeed)
            return 0;
        
        // - Apply throttle based on steering angle
        return 1.05f - Mathf.Abs(SteeringAngle) / 1.0f;
    }

    bool isCarInfrontOfLine(out Vector3 position, out Vector3 otherCarRightVector)
    {
        // - Disable Car collider to avoid car detect self
        boxCollider2D.enabled = false;
        
        // - CircleCast to see if there is car infront
        RaycastHit2D raycastHit = Physics2D.CircleCast(transform.position + transform.up * 0.5f, 0.4f, 
            transform.up, 2, 1 << LayerMask.NameToLayer("ObjectUnderBridge"));
        
        // - Enable Car collider so the car can collide with objects
        boxCollider2D.enabled = true;

        if (raycastHit.collider != null)
        {
            Debug.DrawRay(transform.position, transform.up * 2, Color.red);

            position = raycastHit.collider.transform.position;
            otherCarRightVector = raycastHit.collider.transform.right;

            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.up * 2, Color.black);
        }
           
        // - No car was detected
        position = Vector3.zero;
        otherCarRightVector = Vector3.zero;

        return false;
    }

    void AvoidCars(Vector2 vectorToTarget, out Vector2 newVectorToTarget)
    {
        if (isCarInfrontOfLine(out Vector3 otherCarPosition, out Vector3 otherCarRightVector))
        {
            Vector2 avoidanceVector = Vector2.zero;

            // - Calculate the reflecting vector if car were t ohit other car
            avoidanceVector = Vector2.Reflect((otherCarPosition - transform.position).normalized, otherCarRightVector);
            
            float distanceToTarget = (targetPosition - transform.position).magnitude;
            
            // - When AI car approuches the waypoint the Influence to go towards the Waypoint instead of avoiding the other car increases
            float driveTargetInfluence = 6.0f / distanceToTarget;

            // - Limit the influence between 25 and 100%
            driveTargetInfluence = Mathf.Clamp(driveTargetInfluence, 0.25f, 1.0f);
            
            float avoindanceInfluence = 1.0f - driveTargetInfluence;

            avoidanceVectorLerp = Vector2.Lerp(avoidanceVectorLerp, avoidanceVector, Time.fixedDeltaTime * 3);
            
            // - AvoidanceVector
            newVectorToTarget = vectorToTarget * driveTargetInfluence + avoidanceVector * avoindanceInfluence;
            newVectorToTarget.Normalize();
            
            // - Avoidance Vector
            Debug.DrawRay(transform.position, avoidanceVector * 5, Color.green);
            
            // - New path
            Debug.DrawRay(transform.position, newVectorToTarget * 5, Color.yellow);

            return;
        }
        
        // - No car was detected so out is Default
        newVectorToTarget = vectorToTarget;
    }
}

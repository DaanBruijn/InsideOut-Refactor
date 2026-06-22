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
    
    [Header("Personality")]
    public AIPersonalitySO personality;
    
    // - Private
    private Vector3 _targetPosition = Vector3.zero;
    private Transform _targetTransform = null;
    
    // - Waypoints
    WaypointNode _currentWaypointNode = null;
    WaypointNode[] _allWaypoints;
    
    // - Avoidance
    Vector2 _avoidanceVectorLerp = Vector3.zero;
    
    // - Colliders
    BoxCollider2D _boxCollider2D;
    
    // - Components
    CarController _carController;

    void Awake()
    {
        // - References - Sets all the components 
        _carController = GetComponent<CarController>();
        _allWaypoints = FindObjectsOfType<WaypointNode>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
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
        }

        inputVector.x = TurnTowardsTarget();
        inputVector.y = ApplyDrivingForce(inputVector.x);
        
        _carController.SetInputVector(inputVector);
        
        Debug.DrawLine(transform.position, _targetPosition, Color.red);
    }
    
    void RandomizePersonality()
    {
        personality.speedMultiplier = Random.Range(0.5f, 2f);
        personality.aggression = Random.Range(0.5f, 2f);
        personality.corneringSkill = Random.Range(0.5f, 2f);
        personality.avoidanceStrength = Random.Range(0f, 1f);
    }

    void FollowPlayer()
    {
        if (_targetTransform == null)
            _targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (_targetTransform != null)
            _targetPosition = _targetTransform.position;
    }

    void FollowWayPoint()
    {
        // - Pick the Closest waypoint if none are selected yet
        if (_currentWaypointNode == null)
            _currentWaypointNode = FindClosestWaypoint();

        // - Set Target to next waypoint position
        if (_currentWaypointNode != null)
        {
            _targetPosition = _currentWaypointNode.transform.position;
            
            // - Store distnace to target position
            float distanceToWayPoint = (transform.position - _targetPosition).magnitude;

            if (distanceToWayPoint <= _currentWaypointNode.minDistanceToWayPoint)
            {
                if (_currentWaypointNode.maxSpeed > 0)
                    maxSpeed = _currentWaypointNode.maxSpeed;
                else maxSpeed = 100; // - Set high number to make car travel max speed
                
                _currentWaypointNode = _currentWaypointNode.nextWaypointNode[Random.Range(0, _currentWaypointNode.nextWaypointNode.Length)];
            }
        }
    }

    WaypointNode FindClosestWaypoint()
    {
        // - Find the next closest Waypoint
        return _allWaypoints.OrderBy(t => Vector3.Distance(transform.position, t.transform.position))
            .FirstOrDefault();
    }

    float TurnTowardsTarget()
    {
        Vector2 vectorToTarget = _targetPosition - transform.position;
        vectorToTarget.Normalize();
        
        if (isAvoidingCars)
            AvoidCars(vectorToTarget, out vectorToTarget);
        
        // - Calculate angle towards target
        float angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleToTarget *= -1;

        // - Makes car turn more and less depending on corner/angle
        float steerAmount = angleToTarget / (45.0f / personality.corneringSkill);
        
        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);
        return steerAmount;
    }

    float ApplyDrivingForce(float SteeringAngle)
    {
        float adjustedMaxSpeed = maxSpeed * personality.speedMultiplier;

        if (_carController.GetVelocityMagnitude() > adjustedMaxSpeed)
            return 0;
        
        float throttle = 1.05f - Mathf.Abs(SteeringAngle);
        return throttle * personality.aggression;
    }

    bool IsCarInfrontOfLine(out Vector3 position, out Vector3 otherCarRightVector)
    {
        _boxCollider2D.enabled = false;
        
        // - CircleCast to see if there is car infront
        RaycastHit2D raycastHit = Physics2D.CircleCast(transform.position + transform.up * 0.5f, 0.4f, 
            transform.up, 2, 1 << LayerMask.NameToLayer("ObjectUnderBridge"));
        
        // - Enable Car collider so the car can collide with objects
        _boxCollider2D.enabled = true;

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
        if (IsCarInfrontOfLine(out Vector3 otherCarPosition, out Vector3 otherCarRightVector))
        {
            Vector2 avoidanceVector = Vector2.zero;

            // - Calculate the reflecting vector if car were to hit other car
            avoidanceVector = Vector2.Reflect((otherCarPosition - transform.position).normalized, otherCarRightVector);
            
            float distanceToTarget = (_targetPosition - transform.position).magnitude;
            
            // - When AI car approuches the waypoint the Influence to go towards the Waypoint instead of avoiding the other car increases
            float driveTargetInfluence = 6.0f / distanceToTarget;
            
            driveTargetInfluence = Mathf.Clamp(driveTargetInfluence, 0.25f, 1.0f);
            
            float avoidanceInfluence = (1.0f - driveTargetInfluence) * personality.avoidanceStrength;

            _avoidanceVectorLerp = Vector2.Lerp(_avoidanceVectorLerp, avoidanceVector, Time.fixedDeltaTime * 3);
            
            // - AvoidanceVector
            newVectorToTarget = vectorToTarget * driveTargetInfluence + avoidanceVector * avoidanceInfluence;
            newVectorToTarget.Normalize();
            
            // - Avoidance Vector - Debug Line
            Debug.DrawRay(transform.position, avoidanceVector * 5, Color.green);
            
            // - New path - Debug Line
            Debug.DrawRay(transform.position, newVectorToTarget * 5, Color.yellow);

            return;
        }
        
        // - No car was detected so out is Default
        newVectorToTarget = vectorToTarget;
    }
}

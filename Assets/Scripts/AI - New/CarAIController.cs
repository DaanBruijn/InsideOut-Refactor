using UnityEngine;
using System.Linq;

// - Controller class for the Car AI
// - Uses Personality and Behaviour dependinging on the player
// - Daniel Bruijn

public class CarAIController : MonoBehaviour
{
    // - Variables
    // - Enums
    public enum AIMode
    {
        followPlayer,
        followWaypoints
    }

    public enum PlayerSituation
    {
        AheadFar,
        AheadNear,
        BehindFar,
        BehindNear
    }

    // - Settings
    [Header("AI Settings")]
    public AIMode aiMode;
    public float maxSpeed = 6.5f;
    public bool isAvoidingCars = true;

    [Header("Personality")]
    public AIPersonalitySO personality;

    [Header("Behaviour SOs")]
    public AIBehaviourSO attackBehaviour;
    public AIBehaviourSO defendBehaviour;
    public AIBehaviourSO catchUpBehaviour;
    public AIBehaviourSO normalBehaviour;

    AIBehaviourSO _currentBehaviour;

    // - Targeting
    Vector3 _targetPosition;
    Transform _targetTransform;

    WaypointNode _currentWaypointNode;
    WaypointNode[] _allWaypoints;
    
    float _steerVelocity;
    float _smoothedSteer;

    // - Components
    CarController _carController;
    CarLapCounter _myLapCounter;
    CarLapCounter _playerLapCounter;
    BoxCollider2D _boxCollider2D;

    // - Player State
    PlayerSituation _playerSituation;
    bool _isAheadOfPlayer;
    bool _isNearPlayer;
    float _distanceToPlayer;
    
    // - State
    AIStateMachine _stateMachine;
    AIState _lastState;
    float _lastStateChangeTime;
    float _stateCooldown;
    
    void Awake()
    {
        // - References
        _carController = GetComponent<CarController>();
        _myLapCounter = GetComponent<CarLapCounter>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _allWaypoints = FindObjectsOfType<WaypointNode>();
        
        // - Setup StateMachine
        _stateMachine = new AIStateMachine();
        _stateMachine.Initialize(new NormalState(), this);

        _currentBehaviour = normalBehaviour;
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            _playerLapCounter = player.GetComponent<CarLapCounter>();

        _lastStateChangeTime = Time.time;
    }

    void FixedUpdate()
    {
        EvaluatePlayerSituation();
        UpdateStateFromSituation();
        
        _stateMachine.Tick(this);
        
        switch (aiMode)
        {
            case AIMode.followPlayer:
                FollowPlayer();
                break;

            case AIMode.followWaypoints:
                FollowWayPoint();
                break;
        }

        Vector2 input;
        input.x = TurnTowardsTarget();
        input.y = ApplyDrivingForce(input.x);

        _carController.SetInputVector(input);
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
        if (_currentWaypointNode == null)
            _currentWaypointNode = FindClosestWaypoint();

        if (_currentWaypointNode == null) return;

        _targetPosition = _currentWaypointNode.transform.position;

        float dist = Vector3.Distance(transform.position, _targetPosition);

        if (dist <= _currentWaypointNode.minDistanceToWayPoint)
        {
            if (_currentWaypointNode.nextWaypointNode.Length > 0)
            {
                _currentWaypointNode = _currentWaypointNode.nextWaypointNode[Random.Range(0, _currentWaypointNode.nextWaypointNode.Length)];
            }
        }
    }

    WaypointNode FindClosestWaypoint()
    {
        return _allWaypoints.OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).FirstOrDefault();
    }
    
    float TurnTowardsTarget()
    {
        Vector2 dir = (_targetPosition - transform.position).normalized;

        if (isAvoidingCars)
            AvoidCars(dir, out dir);

        float angle = Vector2.SignedAngle(transform.up, dir) * -1f;

        float steer = angle / (45f / personality.corneringSkill);

        steer *= _currentBehaviour.steeringMultiplier;

        // - Simulation human error
        steer *= Random.Range(0.97f, 1.03f);
        steer = Mathf.Clamp(steer, -0.85f, 0.85f);

        return steer;
    }

    float ApplyDrivingForce(float steering)
    {
        float speedMultiplier = personality.speedMultiplier * _currentBehaviour.speedMultiplier * GetPlayerPressure();

        float adjustedMaxSpeed = maxSpeed * speedMultiplier;

        if (_carController.GetVelocityMagnitude() > adjustedMaxSpeed)
            return 0;

        float throttle = 1.05f - Mathf.Abs(steering);

        // - Simulation human error
        float mistakeFactor = Random.Range(0.92f, 1.0f);

        return throttle * personality.aggression * mistakeFactor;
    }
    
    void AvoidCars(Vector2 targetDir, out Vector2 result)
    {
        float avoidanceMultiplier = _currentBehaviour.avoidanceMultiplier;
        Vector2 finalDir = targetDir;

        if (IsCarInFront(out Vector3 otherPos, out Vector3 otherRight))
        {
            Vector2 avoidDir = Vector2.Reflect((otherPos - transform.position).normalized, otherRight);
            
            float dist = Vector2.Distance(transform.position, _targetPosition);
            float targetInfluence = Mathf.Clamp(6f / dist, 0.25f, 1f);
            float avoidanceInfluence = (1f - targetInfluence) * personality.avoidanceStrength * avoidanceMultiplier;

            finalDir = (targetDir * targetInfluence + avoidDir * avoidanceInfluence);
            finalDir.Normalize();
        }

        result = finalDir;
    }
    
    bool IsCarInFront(out Vector3 pos, out Vector3 right)
    {
        _boxCollider2D.enabled = false;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position + transform.up * 0.5f, 0.4f, transform.up, 2f, 1 << LayerMask.NameToLayer("ObjectUnderBridge"));

        _boxCollider2D.enabled = true;

        if (hit.collider != null)
        {
            pos = hit.collider.transform.position;
            right = hit.collider.transform.right;
            return true;
        }

        pos = Vector3.zero;
        right = Vector3.zero;
        return false;
    }
    
    void EvaluatePlayerSituation()
    {
        if (_playerLapCounter == null || _myLapCounter == null)
            return;

        int myPos = _myLapCounter.GetCarPosition();
        int playerPos = _playerLapCounter.GetCarPosition();

        _isAheadOfPlayer = myPos < playerPos;

        _distanceToPlayer = Vector2.Distance(transform.position, _playerLapCounter.transform.position);
        _isNearPlayer = _distanceToPlayer < 1.5f;

        if (_isAheadOfPlayer)
            _playerSituation = _isNearPlayer ? PlayerSituation.AheadNear : PlayerSituation.AheadFar;
        else
            _playerSituation = _isNearPlayer ? PlayerSituation.BehindNear : PlayerSituation.BehindFar;
    }
    
    public void SetState(AIState newState)
    {
        _stateMachine.ChangeState(newState, this);
    }
    
    public void SetBehaviour(AIBehaviourSO behaviour)
    {
        _currentBehaviour = behaviour;
    }
    
    void UpdateStateFromSituation()
    {
        AIState desiredState = null;

        // - Switch Behaviour depending on Player Location / Position
        switch (_playerSituation)
        {
            case PlayerSituation.BehindFar: 
                desiredState = new CatchUpState();
                break;

            case PlayerSituation.BehindNear:
                desiredState = personality.aggression > 1.2f ? new AttackState() : new CatchUpState();
                break;

            case PlayerSituation.AheadNear:
                desiredState = personality.aggression < 1f ? new DefendState() : new NormalState();
                break;

            case PlayerSituation.AheadFar:
                desiredState = new NormalState();
                break;
        }

        if (_lastState == null)
        {
            _stateMachine.ChangeState(desiredState, this);
            _lastState = desiredState;
            _lastStateChangeTime = Time.time;
            return;
        }

        // - Switch behaviour after cooldown
        if (desiredState.GetType() != _lastState.GetType())
        {
            if (Time.time - _lastStateChangeTime >= _stateCooldown)
            {
                Debug.Log($"{name} -> {desiredState.GetType().Name}");
                _stateMachine.ChangeState(desiredState, this);
                _lastState = desiredState;
                _lastStateChangeTime = Time.time;
            }
        }
    }
    
    float GetPlayerPressure()
    {
        if (_playerLapCounter == null || _myLapCounter == null)
            return 1f;

        int myPos = _myLapCounter.GetCarPosition();
        int playerPos = _playerLapCounter.GetCarPosition();

        int diff = playerPos - myPos;

        // - Player ahead - AI goes faster
        if (diff < 0)
            return 1f;

        switch (diff)
        {
            case 0: return 1.00f;
            case 1: return 1.03f;
            case 2: return 1.05f;
            case 3: return 1.07f;
            default: return 1.10f;
        }
    }
}
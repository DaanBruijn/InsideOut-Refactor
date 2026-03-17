using System;
using System.Collections;
using UnityEngine;

// - CarController script that manages the input and controls the car
// - Daniel Bruijn

[Obsolete("Obsolete")]
public class CarController : MonoBehaviour
{
    // - Public Variables
    [Header("Car settings")] 
    public float carAccelerationFactor;
    public float carTurnFactor;
    public float carDriftFactor;
    public float maxSpeed;
    public float offRoadDrag;
    public float offRoadDrift;
    
    [Header("Sprites")]
    public SpriteRenderer carSprite;
    public SpriteRenderer carShadowSprite;
    
    [Header("Jumping")]
    public AnimationCurve jumpingCurve;
    public ParticleSystem landingParticleSystem;
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    
    // - Local Variables
    float carDragFactor = 3f;
    float accelerationInput = 0f;
    float steeringInput = 0f;
    float rotationAngle;
    float velocityVsUp = 0f;
    
    // - State
    bool isJumping = false;
    

    // - Components
    Rigidbody2D carRigidBody2D;
    Collider2D carCollider2D;
    CarAudioHandler carAudioHandler;
    CarSurfaceHandler carSurfaceHandler;

    void Awake()
    {
        // - References - Sets all the components 
        carRigidBody2D = GetComponent<Rigidbody2D>();
        carCollider2D = GetComponentInChildren<Collider2D>();
        carAudioHandler = GetComponent<CarAudioHandler>();
        carSurfaceHandler = GetComponent<CarSurfaceHandler>();
    }

    private void FixedUpdate()
    {
        // - Checks if the Countdown is active - If Active, dont let the cars Move
        if (GameManager.instance.GetGameState() == GameStates.countDown)
            return;
        
        ApplyEngineForce();
        ApplySteeringForce();
        KillSidewaysVelocity();
    }

    
    void ApplyEngineForce()
    {
        // - Can't brake in mid air but will add small drag
        if (isJumping && accelerationInput < 0)
            accelerationInput = 0;
        
        // - Calculates how much the car goes forward depending on velocity
        velocityVsUp = Vector2.Dot(transform.up, carRigidBody2D.velocity);
        
        // - TestCases
        // - Limit for MaxSpeed
        if (velocityVsUp > maxSpeed && accelerationInput > 0)
            return;
        // - Limit for MaxSpeed (Going backwards)
        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
            return;
        // - Limit for directional accelerating
        if (carRigidBody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0 && !isJumping)
            return;
        
        // - Apply drag when there is no player input
        if (accelerationInput == 0)
            carRigidBody2D.drag = Mathf.Lerp(carRigidBody2D.drag, carDragFactor, Time.fixedDeltaTime * 3);
        else carRigidBody2D.drag = Mathf.Lerp(carRigidBody2D.drag, 0, Time.fixedDeltaTime * 10);

        switch (GetSurfaceType())
        {
            case Surface.SurfaceTypes.Mud:
                carRigidBody2D.drag = Mathf.Lerp(carRigidBody2D.drag, offRoadDrag, Time.fixedDeltaTime * 3);
                break;
        }
        
        // - Create a force for the cars engine
        Vector2 engineForce = transform.up * accelerationInput * carAccelerationFactor;
        
        // - Apply force to the Rigidbody
        carRigidBody2D.AddForce(engineForce, ForceMode2D.Force);
    }

    void ApplySteeringForce()
    {
        // - Limits the cars turn when moving slower
        float minSpeedBeforeTurnFactor = (carRigidBody2D.velocity.magnitude / 5);
        minSpeedBeforeTurnFactor = Mathf.Clamp01(minSpeedBeforeTurnFactor);
        
        // - Update the rotation angle based on the input from the player
        rotationAngle -= steeringInput * carTurnFactor * minSpeedBeforeTurnFactor;
        
        // - Apply steering by rotating the car object
        carRigidBody2D.MoveRotation(rotationAngle);
    }
    
    void KillSidewaysVelocity()
    {
        // - Calculates the Forward and Right velocity for the drift
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidBody2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidBody2D.velocity, transform.right);

        float currentDriftFactor = carDriftFactor;
        
        // - Apply more drag on surfaces
        switch (GetSurfaceType())
        {
            case Surface.SurfaceTypes.Mud:
                currentDriftFactor *= offRoadDrift;
                break;
        }
        
        // - Adjusts the sliding of the car based on the CarDriftFactor
        carRigidBody2D.velocity = forwardVelocity + rightVelocity * currentDriftFactor;;
    }

    float GetLateralVelocity()
    {
        // - Returns how fast the car is drifting sideways
        return Vector2.Dot(transform.right, carRigidBody2D.velocity);
    }

    public bool TireScreech(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;
        
        if (isJumping)
            return false;

        // - Check if car is moving forwards and hitting brake
        if (accelerationInput < 0 && velocityVsUp > 0)
        {
            isBraking = true;
            return true;
        }

        // - If lateral movement is high, tires start screeching
        if (Mathf.Abs(GetLateralVelocity()) > 4.0f)
            return true;
        
        return false;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        // - Sets varibalbes based on input vector * Don't touch
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    public float GetVelocityMagnitude()
    {
        // - Returns the Velocity Magnitude - Used in AudioHandler
        return carRigidBody2D.velocity.magnitude;
    }

    public Surface.SurfaceTypes GetSurfaceType()
    {
        return carSurfaceHandler.GetSurfaceType();
    }
    
    // - Function to start Coroutine
    public void Jump(float jumpHeightScale, float jumpPushScale)
    {
        if (!isJumping)
            StartCoroutine(JumpCoroutine(jumpHeightScale, jumpPushScale));
    }
    
    // - Coroutine for the Jump
    public IEnumerator JumpCoroutine(float jumpHeightScale, float jumpPushScale)
    {
        isJumping = true;

        float jumpStartTime = Time.time;
        float jumpDuration = carRigidBody2D.velocity.magnitude * 0.05f;
        
        jumpHeightScale = jumpHeightScale * carRigidBody2D.velocity.magnitude * 0.05f;
        jumpHeightScale = Mathf.Clamp(jumpHeightScale, 0.0f, 1.0f);
        
        // - Dissable Collsions
        carCollider2D.enabled = false;
        
        carAudioHandler.PlayJumpSFX();
        
        // - Change sorting layer to flying
        carSprite.sortingLayerName = "Flying";
        carSprite.sortingLayerName = "Flying";
        
        
        // - Push the car forward
        carRigidBody2D.AddForce(carRigidBody2D.velocity.normalized * jumpDuration * 5, ForceMode2D.Impulse);
        
        while (isJumping)
        {
            // - Percantage of jump progress
            float jumpCompletedPerc = (Time.time - jumpStartTime) / jumpDuration;
            jumpCompletedPerc = Mathf.Clamp01(jumpCompletedPerc);
             
            // - Base scale of one nad how much we increase the scale with.
            carSprite.transform.localScale = Vector3.one + Vector3.one * jumpingCurve.Evaluate(jumpCompletedPerc) * jumpHeightScale;

            // - Shadowsprite 0.75% of car Sprite
            carShadowSprite.transform.localScale = carSprite.transform.localScale * 0.75f;

            // - Shadowsprite offset
            carShadowSprite.transform.localPosition = new Vector3(1, -1, 0.0f) * 0.25f * jumpingCurve.Evaluate(jumpCompletedPerc) * jumpHeightScale;
               
            // - When jumpPerc is 100% jump is done
            if (jumpCompletedPerc == 1.0f)
                break;
            
            yield return null;
        }
        
        // - Check if landing is good
        bool isGrounded = Physics2D.OverlapCircle(transform.position, 0.3f, groundLayer);
        if (isGrounded)
        {
            // - Something is below the car
            isJumping = false;
            
            // - Add small jump
            Jump(0.2f, 0.6f);
            Debug.Log("Extra Jump");
        }
        else
        {
            // - Scale back Sprites and position
            carSprite.transform.localScale = Vector3.one;

            carShadowSprite.transform.localScale = carSprite.transform.localScale;
            carSprite.transform.localPosition = Vector3.zero;
            
            // - Car is landing, enable collider
            carCollider2D.enabled = true;
            
            carSprite.sortingLayerName = "Default";
            carSprite.sortingLayerName = "Default";

            if (jumpHeightScale > 0.2f)
            {
                landingParticleSystem.Play();
                carAudioHandler.PlayLandingSFX();
            }
            
            // - Change State
            isJumping = false;
        }
    }
    
    // - Detect jump
    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("JumpObject"))
        {
            // - Get JumpData from Object
            JumpData jumpData = collider2D.GetComponent<JumpData>();
            Jump(jumpData.jumpHeightScale, jumpData.jumpPushScale);
        }
    }
}

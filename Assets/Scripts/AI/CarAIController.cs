using System;
using UnityEngine;

// - Script for handling the AI cars in the Game
// - Daniel Bruijn

public class CarAIController : MonoBehaviour
{
    // - Variables
    public enum AIMode
    {
        followPlayer,
        followWaypoints
    };
    
    // - Public
    [Header("AI Settings")]
    public AIMode aiMode;
    
    // - Private
    private Vector3 targetPosition = Vector3.zero;
    private Transform targetTransform = null;
    
    // - Components
    CarController carController;

    void Awake()
    {
        // - References - Sets all the components 
        carController = GetComponent<CarController>();
    }

    void FixedUpdate()
    {
        Vector2 inputVector = Vector2.zero;

        switch (aiMode)
        {
            case AIMode.followPlayer:
                FollowPlayer();
                break;
        }

        inputVector.x = turnTowardsTarget();
        inputVector.y = 1.0f;

        // - Send Input to CarController
        carController.SetInputVector(inputVector);
    }

    void FollowPlayer()
    {
        // - Handles the Tranform to Player Transform
        if (targetTransform == null)
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (targetTransform != null)
            targetPosition = targetTransform.position;
    }

    float turnTowardsTarget()
    {
        Vector2 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.Normalize();
        
        // - Calculate angle towards target
        float angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleToTarget *= -1;

        // - Makes car turn more and less depending on corner/angle
        float steerAmount = angleToTarget / 45.0f;
        
        // - Clamp Steering
        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);
        
        return steerAmount;
    }
}

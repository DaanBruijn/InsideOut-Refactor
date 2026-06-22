using UnityEngine;

[CreateAssetMenu(menuName = "AI/AIBehaviourSO")]
public class AIBehaviourSO : ScriptableObject
{
    [Header("Speed")]
    public float speedMultiplier = 1f;
    [Header("Steering")]
    public float steeringMultiplier = 1f;
    [Header("Avoidance")]
    public float avoidanceMultiplier = 1f;
}
using UnityEngine;

[CreateAssetMenu(menuName = "AI/AIPersonalitySO")]
public class AIPersonalitySO : ScriptableObject
{
    [Header("Driving")]
    [Range(0.5f, 1.2f)]
    public float speedMultiplier = 1f;
    [Range(0.5f, 2f)]
    public float aggression = 1f;
    [Range(0.5f, 2f)]
    public float corneringSkill = 1f;
    [Range(0f, 1f)]
    public float avoidanceStrength = 1f;
}

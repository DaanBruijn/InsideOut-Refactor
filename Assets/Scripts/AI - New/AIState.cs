using UnityEngine;

// - States for the AI
// - Daniel Bruijn

public abstract class AIState
{
    public abstract void Enter(CarAIController ai);
    public abstract void Tick(CarAIController ai);
    public abstract void Exit(CarAIController ai);
}

using UnityEngine;

// - Defend State for the AI
// - Daniel Bruijn

public class DefendState : AIState
{
    public override void Enter(CarAIController ai)
    {
        ai.SetBehaviour(ai.defendBehaviour);
    }

    public override void Tick(CarAIController ai)
    {
    }

    public override void Exit(CarAIController ai)
    {
    }
}
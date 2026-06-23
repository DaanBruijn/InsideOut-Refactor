using UnityEngine;

// - Normal State for the AI
// - Daniel Bruijn

public class NormalState : AIState
{
    public override void Enter(CarAIController ai)
    {
        ai.SetBehaviour(ai.normalBehaviour);
    }

    public override void Tick(CarAIController ai)
    {
    }

    public override void Exit(CarAIController ai)
    {
    }
}

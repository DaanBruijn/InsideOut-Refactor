using UnityEngine;

// - CatchUp State for the AI
// - Daniel Bruijn

public class CatchUpState : AIState
{
    public override void Enter(CarAIController ai)
    {
        ai.SetBehaviour(ai.catchUpBehaviour);
    }

    public override void Tick(CarAIController ai)
    {
    }

    public override void Exit(CarAIController ai)
    {
    }
}
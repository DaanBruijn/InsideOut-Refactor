using UnityEngine;

// - Attack State for the AI
// - Daniel Bruijn

public class AttackState : AIState
{
    public override void Enter(CarAIController ai)
    {
        ai.SetBehaviour(ai.attackBehaviour);
    }

    public override void Tick(CarAIController ai)
    {
    }

    public override void Exit(CarAIController ai)
    {
    }
}
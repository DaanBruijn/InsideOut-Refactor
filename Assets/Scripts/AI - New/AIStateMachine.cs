using UnityEngine;

// - StateMachine used to switched between different States / Behaviours
// - Daniel Bruijn

public class AIStateMachine
{
    // - Variables
    public AIState CurrentState { get; private set; }

    public void Initialize(AIState startState, CarAIController ai)
    {
        CurrentState = startState;
        CurrentState.Enter(ai);
    }

    public void ChangeState(AIState newState, CarAIController ai)
    {
        CurrentState.Exit(ai);
        CurrentState = newState;
        CurrentState.Enter(ai);
    }

    public void Tick(CarAIController ai)
    {
        if (CurrentState != null)
            CurrentState.Tick(ai);
    }
}
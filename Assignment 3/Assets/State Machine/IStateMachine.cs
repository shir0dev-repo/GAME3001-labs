using System;
using System.Collections;

public interface IStateMachine<State> where State : Enum
{
    void SetState(State state);
    State GetCurrentState();
    State GetNextState();
    IEnumerator TransitionState(State nextState);
}

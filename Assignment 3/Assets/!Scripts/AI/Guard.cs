using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GuardStateMachine : MonoBehaviour, IStateMachine<GuardStateMachine.State>
{
    public enum State { Idle, Patrol, Chase }
    private State _currentState = State.Idle;
    public Action<State> OnStateChanged, OnStateChangeAttempted;
    public bool PlayerVisible { get; set; }

    private Coroutine _stateCO = null;

    private void Awake()
    {
        _stateCO = StartCoroutine(TransitionState(_currentState));
    }

    public State GetCurrentState() => _currentState;
    public State GetNextState()
    {
        return _currentState switch
        {
            State.Idle => State.Patrol,
            State.Patrol => State.Idle,
            State.Chase => State.Idle,
            _ => throw new InvalidOperationException("Invalid state value!")
        };
    }
    public IEnumerator TransitionState(State currentState)
    {
        yield return currentState switch
        {
            State.Idle => RandomStateChange(),
            State.Patrol => RandomStateChange(),
            State.Chase => ChaseState(),
            _ => throw new InvalidOperationException("Invalid state value!")
        };

    }

    private IEnumerator RandomStateChange()
    {
        if (_currentState == State.Idle) // delay at start of behaviour
            yield return new WaitForSeconds(3f);

        int attempts = 0;
        while (attempts < 3) // force state change after 3 attempts (9 seconds)
        {
            OnStateChangeAttempted?.Invoke(_currentState);
            yield return new WaitForSeconds(3f);
            if (Random.value > 0.75f)
            {
                break;
            }

            attempts++;
        }

        State next = _currentState == State.Idle ? State.Patrol : State.Idle;
        SetState(next);
    }

    private IEnumerator ChaseState()
    {
        yield return new WaitUntil(() => PlayerVisible == false);
        SetState(State.Idle);
    }

    public void SetState(State state)
    {
        if (_currentState == state) return; // state will not change

        _currentState = state;
        OnStateChanged?.Invoke(_currentState);

        StopAllCoroutines(); // avoid multiple coroutines running simultaneously
        _stateCO = StartCoroutine(TransitionState(_currentState));
    }
}

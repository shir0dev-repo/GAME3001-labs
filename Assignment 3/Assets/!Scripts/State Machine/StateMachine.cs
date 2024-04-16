using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class StateMachine<TState> : MonoBehaviour where TState : Enum
{
    public readonly struct StateTransition
    {
        public readonly TState Next;

        public delegate bool TransitionRequirement();
        public readonly TransitionRequirement CheckTransition;
     
        public readonly int Priority;

        public StateTransition(TState nextState, Func<bool> transitionRequirement, int priority = 0)
        {
            Next = nextState;
            CheckTransition = transitionRequirement.Invoke;
            Priority = priority;
        }
    }

    private readonly Dictionary<TState, List<StateTransition>> m_stateTransitionDictionary = new();
    public Dictionary<TState, List<StateTransition>> StateTransitionDictionary => m_stateTransitionDictionary;

    protected TState m_currentState;
    protected bool m_isTransitioningState = false;
    protected float m_updateTimer = 0.5f;

    public TState GetCurrentState() => m_currentState;

    public void AddTransitions(TState enterState, params StateTransition[] transitions) => m_stateTransitionDictionary.Add(enterState, new(transitions));

    protected virtual IEnumerator Start()
    {
        while (true)
        {
            if (!m_isTransitioningState && m_stateTransitionDictionary.ContainsKey(m_currentState))
            {
                foreach (StateTransition transition in m_stateTransitionDictionary[m_currentState].OrderByDescending(t => t.Priority))
                {
                    if (transition.CheckTransition())
                    {
                        m_currentState = transition.Next;
                        HandleTransition(transition.Next);
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(1);
        }        
    }

    protected abstract void HandleTransition(TState nextState);
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class StateMachine<TState> : MonoBehaviour where TState : Enum
{

    public enum CHECK_TYPE { UPDATE, PERIODIC }
    public readonly struct StateTransition
    {
        public readonly TState Next;

        public delegate bool TransitionRequirement();
        public readonly TransitionRequirement CheckTransition;
        
        public readonly int Priority;
        public readonly float WaitDuration;
        public readonly CHECK_TYPE CheckType;

        public StateTransition(TState nextState, Func<bool> transitionRequirement, CHECK_TYPE checkType, int priority = 0, float wait = 1)
        {
            Next = nextState;
            CheckTransition = transitionRequirement.Invoke;
            Priority = priority;
            WaitDuration = wait;
            CheckType = checkType;
        }
    }

    private readonly Dictionary<TState, List<StateTransition>> m_stateTransitionDictionary = new();
    private Dictionary<TState, List<StateTransition>> m_checkOnUpdateDictionary = new();
    private Dictionary<TState, List<StateTransition>> m_checkPeriodicDictionary = new();

    public Dictionary<TState, List<StateTransition>> StateTransitionDictionary => m_stateTransitionDictionary;

    protected TState m_currentState;
    protected bool m_isTransitioningState = false;
    protected float m_updateTimer = 0.5f;

    public TState GetCurrentState() => m_currentState;

    public void AddTransitions(TState enterState, params StateTransition[] transitions) => m_stateTransitionDictionary.Add(enterState, new(transitions));

    protected virtual IEnumerator Start()
    {
        m_checkPeriodicDictionary = m_stateTransitionDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Where(tr => tr.CheckType == CHECK_TYPE.PERIODIC).ToList());
        m_checkOnUpdateDictionary = m_stateTransitionDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Where(tr => tr.CheckType == CHECK_TYPE.UPDATE).ToList());


        while (true)
        {
            float waitDuration = 1f;
            if (!m_isTransitioningState && m_checkPeriodicDictionary.ContainsKey(m_currentState))
            {
                foreach (StateTransition transition in m_checkPeriodicDictionary[m_currentState].OrderByDescending(t => t.Priority))
                {
                    waitDuration = transition.WaitDuration;
                    if (transition.CheckTransition())
                    {
                        m_currentState = transition.Next;
                        HandleTransition(transition.Next);
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(waitDuration);
        }        
    }

    protected virtual void Update()
    {
        if (!m_isTransitioningState && m_checkOnUpdateDictionary.ContainsKey(m_currentState))
        {
            foreach (StateTransition transition in m_checkOnUpdateDictionary[m_currentState].OrderByDescending(t => t.Priority))
            {
                if (transition.CheckTransition())
                {
                    m_currentState = transition.Next;
                    HandleTransition(transition.Next);
                    break;
                }
            }
        }
    }

    protected abstract void HandleTransition(TState nextState);
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuardStateMachine : StateMachine<GuardStateMachine.State>
{
    public enum State
    {
        No_Action = -1,
        Idle,
        Patrol,
        Chase,
        Searching
    }

    public static event EventHandler<State> OnStateChanged;

    [Header("Search")]
    [SerializeField] private FieldOfView m_fieldOfView;
    [SerializeField] private LayerMask m_targetLayer;
    [SerializeField] private float m_viewRadius = 90f;
    [SerializeField] private float m_viewDistance = 3f;

    float m_giveUpTimer = 0, m_giveUpThreshold = 4;

    [Space]
    [SerializeField] private GuardMovement m_movement;


    private void Awake()
    {
        m_currentState = State.Idle;
        m_movement = GetComponent<GuardMovement>();
        StateTransition idleToChase = new StateTransition(State.Chase, SearchForTargets, CHECK_TYPE.UPDATE,priority: 1);
        StateTransition idleToPatrol = new StateTransition(State.Patrol, m_movement.ShouldMoveToNextPatrolPoint, CHECK_TYPE.PERIODIC);

        StateTransition patrolToChase = new StateTransition(State.Chase, SearchForTargets, CHECK_TYPE.UPDATE, 1);
        StateTransition patrolToIdle = new StateTransition(State.Idle, m_movement.NearStoppingPoint, CHECK_TYPE.PERIODIC);

        StateTransition chaseToSearching = new StateTransition(State.Searching, () => !SearchForTargets(), CHECK_TYPE.PERIODIC, 1);

        StateTransition searchingToChase = new StateTransition(State.Chase, SearchForTargets, CHECK_TYPE.UPDATE, 1);
        StateTransition searchingToIdle = new StateTransition(State.Idle, GiveUpSearching, CHECK_TYPE.PERIODIC);

        AddTransitions(State.Idle, idleToChase, idleToPatrol);
        AddTransitions(State.Patrol, patrolToChase, patrolToIdle);
        AddTransitions(State.Chase, chaseToSearching);
        AddTransitions(State.Searching, searchingToChase, searchingToIdle);
    }

    protected override void Update()
    {
        base.Update();
        switch (m_currentState)
        {
            case State.Idle:
                break;
            case State.Patrol:
                m_movement.MoveToTarget(m_currentState);
                m_movement.RotateToTarget();
                break;
            case State.Chase:
                m_movement.MoveToTarget(m_currentState);
                m_movement.RotateToTarget();
                break;
            case State.Searching:
                m_movement.MoveToTarget(m_currentState);
                m_movement.RotateToTarget();
                break;
            default:
                break;
        }
    }

    protected override void HandleTransition(State nextState)
    {
        OnStateChanged?.Invoke(this, nextState);
        switch (nextState)
        {
            case State.Idle:
                break;
            case State.Patrol:               
                m_movement.ResumePatrol();
                break;
            case State.Chase:
                break;
            case State.Searching:
                break;
            default:
                m_currentState = State.Idle;
                break;
        }
    }

    private bool SearchForTargets()
    {
        if (m_fieldOfView.CheckForTarget(m_targetLayer, out GameObject foundTarget))
        {
            Debug.Log("found target!");
            m_movement.SetTarget(foundTarget.transform);
            return true;
        }

        return false;
    }

    private bool GiveUpSearching()
    {
        if (Vector3.Distance(transform.position, m_movement.GetLastKnownPosition()) < 0.1f)
            m_giveUpTimer += m_updateTimer;

        // keep last known position of target
        if (SearchForTargets())
        {
            m_giveUpTimer = 0;
            return false;
        }
        // if target cannot be found within x amount of time, give up
        else if (m_giveUpTimer > m_giveUpThreshold)
        {
            m_giveUpTimer = 0;
            m_movement.ResumePatrol();
            return true;
        }
        // return if agent gave up

        return true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (!collision.gameObject.CompareTag("Player")) return;

        switch (m_currentState)
        {
            case State.No_Action:
                break;
            case State.Idle:
                GameManager.OnGameWin?.Invoke();
                break;
            case State.Patrol:
                GameManager.OnGameWin?.Invoke();
                break;
            case State.Chase:
                GameManager.OnGameLose?.Invoke();
                break;
            case State.Searching:
                GameManager.OnGameLose?.Invoke();
                break;
        }
    }
}

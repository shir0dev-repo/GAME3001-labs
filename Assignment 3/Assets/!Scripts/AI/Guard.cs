using System.Collections;
using System.Linq;
using UnityEngine;

public class GuardStateMachine : StateMachine<GuardStateMachine.State>
{
    public enum State
    {
        No_Action = -1,
        Idle,
        Patrol,
        Chase
    }

    // idle -> patrol
    private const int MAX_PATROL_ATTEMPTS = 3;
    private int m_patrolStartCounter = 0;

    [Header("Movement")]
    [SerializeField] private float m_moveSpeed, m_rotationSpeed;
    private Transform m_currentTarget;

    [Header("Search")]
    [SerializeField] private FieldOfView m_fieldOfView;
    [SerializeField] private LayerMask m_targetLayer;

    // patrol -> idle
    [Header("Patrolling")]
    [SerializeField] private Transform[] m_patrolPoints;
    [SerializeField] private float m_stoppingRadius;
    private Transform m_currentPatrolPoint;
    private int m_currentPatrolIndex = 0;



    private void Awake()
    {
        m_currentState = State.Idle;
        m_currentTarget = m_currentPatrolPoint = m_patrolPoints.OrderBy(pp => Vector3.Distance(transform.position, pp.position)).First();

        StateTransition idleToChase = new StateTransition(State.Chase, SearchForTargets, priority: 1);
        StateTransition idleToPatrol = new StateTransition(State.Patrol, ShouldMoveToNextPatrolPoint);

        StateTransition patrolToChase = new StateTransition(State.Chase, SearchForTargets, 1);
        StateTransition patrolToIdle = new StateTransition(State.Idle, NearStoppingPoint);

        StateTransition chaseToIdle = new StateTransition(State.Idle, SearchLastKnownLocation, 1);

        AddTransitions(State.Idle, idleToChase, idleToPatrol);
        AddTransitions(State.Patrol, patrolToChase, patrolToIdle);
    }

    private void Update()
    {
        switch (m_currentState)
        {
            case State.Idle:
                break;
            case State.Patrol:
                MoveToTarget();
                break;
            case State.Chase:
                break;
            default:
                break;
        }
    }

    protected override void HandleTransition(State nextState)
    {
        switch (nextState)
        {
            case State.Idle:
                break;
            case State.Patrol:
                Debug.Log("assignment");
                m_currentPatrolPoint = m_patrolPoints[m_currentPatrolIndex];
                m_currentTarget = m_currentPatrolPoint;
                StartCoroutine(RotateToTargetCoroutine());
                break;
            case State.Chase:
                break;
            default:
                m_currentState = State.Idle;
                break;
        }


    }


    private bool ShouldMoveToNextPatrolPoint()
    {
        m_patrolStartCounter++;

        if (Random.value > 0.6f || m_patrolStartCounter >= MAX_PATROL_ATTEMPTS)
        {
            m_patrolStartCounter = 0;
            m_currentPatrolIndex++;
            Debug.Log("increment");
            m_currentPatrolIndex %= m_patrolPoints.Length;
            return true;
        }

        return false;
    }

    private bool NearStoppingPoint()
    {
        Vector2 horizontalPos = new(transform.position.x, transform.position.z);
        Vector2 patrolPos = new(m_currentPatrolPoint.position.x, m_currentPatrolPoint.position.z);

        return Vector2.Distance(horizontalPos, patrolPos) < m_stoppingRadius;


    }

    private bool SearchForTargets()
    {
        return m_fieldOfView.CheckForTarget(m_targetLayer);
    }

    private bool SearchLastKnownLocation()
    {
        bool giveup = false;

        // keep last known position of target
        // if target cannot be found within x amount of time, give up
        // return if agent gave up

        return giveup;
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_currentTarget.position, m_moveSpeed * Time.deltaTime);
    }

    private IEnumerator RotateToTargetCoroutine()
    {
        Debug.Log("rotating!");
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation((m_currentTarget.position - transform.position).normalized, Vector3.up);

        float timeElapsed = 0;

        while (timeElapsed < m_rotationSpeed)
        {
            timeElapsed += Time.deltaTime;

            Debug.Log(timeElapsed);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / m_rotationSpeed);
            yield return null;
        }
    }
}

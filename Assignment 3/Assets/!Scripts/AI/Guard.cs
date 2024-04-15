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
        Chase,
        Searching
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

    [Header("Chasing")]
    [SerializeField] private float m_viewRadius = 90f;
    [SerializeField] private float m_viewDistance = 3f;
    Vector3 m_lastKnownTargetPosition = Vector3.zero;
    float m_giveUpTimer = 0, m_giveUpThreshold = 3;


    [Space]
    [SerializeField] private Animator m_anim;



    private void Awake()
    {
        m_currentState = State.Idle;
        m_currentTarget = m_currentPatrolPoint = m_patrolPoints.OrderBy(pp => Vector3.Distance(transform.position, pp.position)).First();

        StateTransition idleToChase = new StateTransition(State.Chase, SearchForTargets, priority: 1);
        StateTransition idleToPatrol = new StateTransition(State.Patrol, ShouldMoveToNextPatrolPoint);

        StateTransition patrolToChase = new StateTransition(State.Chase, SearchForTargets, 1);
        StateTransition patrolToIdle = new StateTransition(State.Idle, NearStoppingPoint);

        StateTransition chaseToSearching = new StateTransition(State.Searching, () => !SearchForTargets(), 1);

        StateTransition searchingToChase = new StateTransition(State.Chase, SearchForTargets, 1);
        StateTransition searchingToIdle = new StateTransition(State.Idle, GiveUpSearching);

        AddTransitions(State.Idle, idleToChase, idleToPatrol);
        AddTransitions(State.Patrol, patrolToChase, patrolToIdle);
        AddTransitions(State.Chase, chaseToSearching);
        AddTransitions(State.Searching, searchingToChase, searchingToIdle);
    }

    private void Update()
    {
        switch (m_currentState)
        {
            case State.Idle:
                break;
            case State.Patrol:
                MoveToTarget();
                RotateToTarget();
                break;
            case State.Chase:
                break;
            case State.Searching:
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
                Debug.Log("Setting state to Idle");
                m_anim.SetBool("_isWalking", false);
                break;
            case State.Patrol:
                Debug.Log("Setting state to Patrol");
                m_currentPatrolPoint = m_patrolPoints[m_currentPatrolIndex];
                m_currentTarget = m_currentPatrolPoint;
                m_anim.SetBool("_isWalking", true);
                break;
            case State.Chase:
                Debug.Log("Setting state to Chase");
                break;
            case State.Searching:
                Debug.Log("Setting state to Searching");
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
            m_currentPatrolIndex %= m_patrolPoints.Length;
            return true;
        }

        return false;
    }

    private bool NearStoppingPoint()
    {
        Vector2 horizontalPos = new(transform.position.x, transform.position.z);
        Vector2 patrolPos = new(m_currentPatrolPoint.position.x, m_currentPatrolPoint.position.z);

        if (Vector2.Distance(horizontalPos, patrolPos) < m_stoppingRadius)
        {
            m_anim.SetBool("_isWalking", false);
            return true;
        }

        return false;
    }
    public void FoundTarget(Transform target)
    {
        m_currentTarget = target;
    }
    private bool SearchForTargets()
    {
        if (m_fieldOfView.CheckForTarget(m_targetLayer, out GameObject foundTarget))
        {
            Debug.Log("found target!");
            m_currentTarget = foundTarget.transform;
            return true;
        }

        return false;
    }

    private bool GiveUpSearching()
    {
        bool giveup = false;

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
            m_giveUpThreshold = 0;
            return true;
        }
        // return if agent gave up

        return giveup;
    }


    private void MoveToTarget()
    {
        m_anim.SetBool("_isWalking", true);
        transform.position = Vector3.MoveTowards(transform.position, m_currentTarget.position, m_moveSpeed * Time.deltaTime);
    }

    private void RotateToTarget()
    {
        transform.rotation = Quaternion.LookRotation((m_currentTarget.position - transform.position).normalized, Vector3.up);
    }
}

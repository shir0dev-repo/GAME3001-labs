using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using State = GuardStateMachine.State;

public class GuardMovement : MonoBehaviour
{
    private const int MAX_PATROL_ATTEMPTS = 3;
    private int m_patrolStartCounter = 0;

    [SerializeField] private Animator m_anim;
    
    [Space]
    [SerializeField] private float m_patrolSpeed, m_chaseSpeed, m_rotationSpeed;
    [SerializeField] private float m_stoppingRadius;

    [SerializeField] private Transform[] m_patrolPoints;
    private Transform m_currentPatrolPoint;

    private Transform m_currentTarget;
    private Vector3 m_lastKnownTargetPosition = Vector3.zero;

    private int m_currentPatrolIndex = 0;

    public bool CanMove { get; set; }

    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        m_currentTarget = m_currentPatrolPoint = m_patrolPoints.OrderBy(pp => Vector3.Distance(transform.position, pp.position)).First();
    }
    public Vector3 GetLastKnownPosition() => m_lastKnownTargetPosition;
    public void SetTarget(Transform target)
    {
        m_currentTarget = target;
        m_lastKnownTargetPosition = target.position;
    }
        

    public bool ShouldMoveToNextPatrolPoint()
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

    public bool NearStoppingPoint()
    {
        if (Vector3.Distance(transform.position, m_currentTarget.position) <= m_stoppingRadius)
        {
            m_anim.SetBool("_isWalking", false);
            m_currentPatrolPoint = m_currentTarget;
            return true;
        }

        return false;
    }

    public void ResumePatrol()
    {
        m_currentTarget = m_patrolPoints[m_currentPatrolIndex];
        m_anim.SetBool("_isWalking", true);
    }
    public void MoveToTarget(State state)
    {
        float speed = state == State.Chase || state == State.Searching ? m_chaseSpeed : m_patrolSpeed;
        m_anim.SetBool("_isWalking", true);
        transform.position = Vector3.MoveTowards(transform.position, m_currentTarget.position, speed * Time.deltaTime);
    }

    public void RotateToTarget()
    {
        if (Vector3.Distance(transform.position, m_currentTarget.position) > 0)
            transform.rotation = Quaternion.LookRotation((m_currentTarget.position - transform.position).normalized, Vector3.up);
    }
}

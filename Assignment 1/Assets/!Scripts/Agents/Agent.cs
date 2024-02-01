using System;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public enum AgentState
    {
        Idle = 0,
        Seek = 1,
        Flee = 2,
        Arrival = 3,
        Avoid = 4
    }

    [SerializeField] private LayerMask _environmentLayer;
    [Space, SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 65f;
    [SerializeField] private float _satisfactionRadius = 5f;

    public Transform Target { get; set; }
    public bool AvoidObstacles { get; set; }

    private delegate void AgentBehaviourDelegate();
    private AgentBehaviourDelegate _agentStateBehaviour;

    private AgentState _currentState = AgentState.Idle;
    private Vector3 _targetDirection = Vector3.zero;

    private void Update()
    {
        if (Target == null) return;

        _agentStateBehaviour?.Invoke();
    }

    public void SetState(AgentState agentState)
    {
        _currentState = agentState;

        _agentStateBehaviour = _currentState switch
        {
            AgentState.Idle => null,
            AgentState.Seek => SeekToTarget,
            AgentState.Flee => FleeFromTarget,
            AgentState.Arrival => ArriveAtTarget,
            _ => null
        };
    }

    private void SeekToTarget()
    {
        Vector3 position = transform.position + (Time.deltaTime * _moveSpeed * transform.forward);
        transform.SetPositionAndRotation(position, RotateToDirection(towardsTarget: true));
    }

    private Quaternion RotateToDirection(bool towardsTarget)
    {
        if (towardsTarget)
            _targetDirection = GetDirection(from: transform.position, to: Target.position);
        else
            _targetDirection = GetDirection(from: Target.position, to: transform.position);
        _targetDirection.y = 0;
        _targetDirection.Normalize();

        float targetAngle = Mathf.Atan2(_targetDirection.x, _targetDirection.z) * Mathf.Rad2Deg;

        return Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetAngle, 0), _rotationSpeed * Time.deltaTime);
    }

    private void FleeFromTarget()
    {
        if (Vector3.Distance(Target.position, transform.position) < 10f)
        {
            transform.rotation = RotateToDirection(towardsTarget: false);
            transform.position += Time.deltaTime * _moveSpeed * transform.forward;
        }
        else if (Quaternion.Angle(transform.rotation, Quaternion.Euler(GetDirection(from: transform.position, to: Target.position))) < 1f)
        {
            transform.rotation = RotateToDirection(towardsTarget: true);
        }
    }

    private void ArriveAtTarget()
    {
        if (Vector3.Distance(transform.position, Target.position) < 2f) return;
        transform.rotation = RotateToDirection(true);

        _targetDirection = Target.position - transform.position;
        float distance = _targetDirection.magnitude;

        if (distance < _satisfactionRadius)
            _targetDirection = _targetDirection.normalized * _moveSpeed * (distance / _satisfactionRadius);
        else
            _targetDirection = _targetDirection.normalized * _moveSpeed;
        transform.position += transform.forward * _targetDirection.magnitude * Time.deltaTime;
    }

    private Vector3 GetDirection(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        dir.y = 0;
        return (dir).normalized;
    }

    private void OnDrawGizmosSelected()
    {
        if (Target == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, Target.position);
    }
}

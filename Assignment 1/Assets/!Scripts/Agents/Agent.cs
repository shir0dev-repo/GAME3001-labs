using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public enum AgentState
    {
        Idle = 0,
        Seek = 1,
        Flee = 2,
        Arrival = 3,
        Length
    }
    delegate void AgentBehaviourDelegate();

    [SerializeField] private LayerMask _groundLayer;
    [Space]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 65f;

    private AgentBehaviourDelegate _agentStateBehaviour;
    private AgentState _currentState = AgentState.Idle;
    private Transform _target;
    private Vector3 _targetDirection = Vector3.zero;

    private void Update()
    {
        if (Vector3.Distance(transform.position, _target.position) < 0.1f) return;


        _agentStateBehaviour?.Invoke();
    }

    public void SetState(AgentState agentState)
    {
        _currentState = agentState;

        _agentStateBehaviour = _currentState switch
        {
            AgentState.Idle => null,
            AgentState.Seek => MoveToTarget,
            AgentState.Flee => FleeFromTarget,
            AgentState.Arrival => ArriveAtTarget,
            _ => null
        };
    }

    private void ArriveAtTarget()
    {
        throw new NotImplementedException();
    }

    private void FleeFromTarget()
    {
        throw new NotImplementedException();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void SetBehaviour(AgentState behaviour)
    {
        _currentState = behaviour;
    }

    private void MoveToTarget()
    {
        transform.position += Time.deltaTime * _moveSpeed * transform.forward;
    }

    private void RotateToDirection()
    {
        _targetDirection = GetDirection(transform.position, _target.position);
        _targetDirection.y = 0;
        _targetDirection.Normalize();

        float targetAngle = Mathf.Atan2(_targetDirection.x, _targetDirection.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetAngle, 0), _rotationSpeed * Time.deltaTime);
    }

    private Vector3 GetDirection(Vector3 from, Vector3 to)
    {
        Vector3 dir = from - to;
        dir.y = 0;
        return (dir).normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _target.position);
    }
}

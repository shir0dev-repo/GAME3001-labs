using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [SerializeField] private LayerMask _groundLayer;
    [Space]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 65f;

    delegate void AgentBehaviourDelegate();
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

    private void MoveToTarget()
    {
        transform.rotation = RotateToDirection(towardsTarget: true);
        transform.position += Time.deltaTime * _moveSpeed * transform.forward;
    }

    private Quaternion RotateToDirection(bool towardsTarget)
    {
        if (towardsTarget)
            _targetDirection = GetDirection(from: transform.position, to: _target.position);
        else
            _targetDirection = GetDirection(from: _target.position, to: transform.position);
        _targetDirection.y = 0;
        _targetDirection.Normalize();

        float targetAngle = Mathf.Atan2(_targetDirection.x, _targetDirection.z) * Mathf.Rad2Deg;

        return Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetAngle, 0), _rotationSpeed * Time.deltaTime);
    }

    private void FleeFromTarget()
    {
        if (Vector3.Distance(_target.position, transform.position) < 10f)
        {
            transform.rotation = RotateToDirection(towardsTarget: false);
            transform.position += Time.deltaTime * _moveSpeed * transform.forward;
        }
        else if (Quaternion.Angle(transform.rotation, Quaternion.Euler(GetDirection(from: transform.position, to: _target.position))) < 1f)
        {
            transform.rotation = RotateToDirection(towardsTarget: true);
        }
    }

    private void ArriveAtTarget()
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

    private Vector3 GetDirection(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        dir.y = 0;
        return (dir).normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _target.position);
    }
}

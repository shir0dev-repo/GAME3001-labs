using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public enum AgentBehaviour
    {
        Idle,
        Seek,
        Flee,
        Arrival
    }

    [SerializeField] private LayerMask _groundLayer;
    [Space]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 65f;

    private AgentBehaviour _agentBehaviour;

    private Vector3 _targetPosition = Vector3.zero;
    private Vector3 _targetDirection = Vector3.zero;

    private void Awake()
    {
        _targetPosition = new(-20, 0, -5);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _targetPosition) < 0.1f) return;

        switch (_agentBehaviour)
        {
            case AgentBehaviour.Idle:
                return;
                case AgentBehaviour.Seek:
                MoveToPosition();
                break;
            case AgentBehaviour.Flee:
                break;
            case AgentBehaviour.Arrival:
                break;
            default:
                break;
        }
        RotateToDirection();
        MoveToPosition();
    }

    public void SetTarget(Transform target)
    {
        _targetPosition = target.position;
    }

    public void SetBehaviour(AgentBehaviour behaviour)
    {
        _agentBehaviour = behaviour;
    }

    private void MoveToPosition()
    {
        transform.position += Time.deltaTime * _moveSpeed * transform.forward;
    }

    private void RotateToDirection()
    {
        _targetDirection = _targetPosition - transform.position;
        _targetDirection.y = 0;
        _targetDirection.Normalize();

        float targetAngle = Mathf.Atan2(_targetDirection.x, _targetDirection.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetAngle, 0), _rotationSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _targetPosition);
    }
}

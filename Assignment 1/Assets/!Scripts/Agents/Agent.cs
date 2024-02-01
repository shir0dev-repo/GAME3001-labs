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

    private delegate void AgentBehaviourDelegate();
    private AgentBehaviourDelegate _agentStateBehaviour;

    [Header("Movement")]
    [Space, SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 65f;

    [Header("Flee")]
    [SerializeField] private float _maxFleeDistance = 5f;

    [Header("Arrival")]
    [SerializeField] private float _arrivalRadius = 5f;
    [SerializeField] private float _stoppingRadius = 0.75f;

    [Header("Avoidance")]
    [SerializeField] private LayerMask _environmentLayer;

    public Transform Target { get; set; }
    public bool AvoidObstacles { get; set; }

    private void Update()
    {
        if (Target == null) return;

        _agentStateBehaviour?.Invoke();
    }

    public void SetState(AgentState agentState)
    {
        _agentStateBehaviour = agentState switch
        {
            AgentState.Idle => null,
            AgentState.Seek => SeekTarget,
            AgentState.Flee => FleeTarget,
            AgentState.Arrival => ArriveTarget,
            _ => null
        };
    }

    private Vector3 GetDirectionToTarget()
    {
        if (Target == null)
            return transform.position;
        else
            return Target.position - transform.position;
    }

    private void MoveToDirection(Vector3 direction, float maxDisplacement)
    {
        transform.position += direction * maxDisplacement;
    }

    private Quaternion GetRotationToTarget(Vector3 targetPosition)
    {
        if (Target == null)
            return transform.rotation;

        float targetAngle = Mathf.Atan2(targetPosition.x, targetPosition.z) * Mathf.Rad2Deg;

        return Quaternion.AngleAxis(targetAngle, Vector3.up);
    }

    private void RotateToDirection(Vector3 targetDirection, float maxDegreeDelta)
    {
        if (targetDirection.sqrMagnitude > 1)
            targetDirection.Normalize();

        Quaternion targetRotation = GetRotationToTarget(targetDirection);

        if (Quaternion.Angle(targetRotation, transform.rotation) > 1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxDegreeDelta);
        }
    }

    private void SeekTarget()
    {
        RotateToDirection(GetDirectionToTarget(), _rotationSpeed * Time.deltaTime);
        MoveToDirection(transform.forward, _moveSpeed * Time.deltaTime);
    }

    private void FleeTarget()
    {
        Vector3 targetDirection = GetDirectionToTarget();

        if (targetDirection.sqrMagnitude > _maxFleeDistance * _maxFleeDistance) 
            return;

        targetDirection = -1f * targetDirection.normalized;
        RotateToDirection(targetDirection, _rotationSpeed * Time.deltaTime);
        MoveToDirection(transform.forward, _moveSpeed * Time.deltaTime);
    }

    private void ArriveTarget()
    {
        Vector3 targetDirection = GetDirectionToTarget();
        RotateToDirection(targetDirection, _rotationSpeed * Time.deltaTime);
        float distanceToTarget = targetDirection.magnitude;
        float targetVelocity = _moveSpeed * Time.deltaTime;

        if (distanceToTarget <= _stoppingRadius)
            return;
        
        else if (distanceToTarget < _arrivalRadius + _stoppingRadius)
            targetVelocity *= distanceToTarget / _arrivalRadius;

        MoveToDirection(transform.forward, targetVelocity);
    }


}

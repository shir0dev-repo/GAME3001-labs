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
    [SerializeField] int _whiskerCount = 4;
    [SerializeField] float _whiskerLength = 1f;
    [SerializeField, Range(90, 360)] float _whiskerMaxAngle = 90f;
    [SerializeField] private float _avoidanceWeight = 2f;

    public Transform Target { get; set; }

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
            AgentState.Avoid => HandleAvoidance,
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

    private void HandleAvoidance()
    {
        ArriveTarget();
        AvoidObstacles();
    }

    private void AvoidObstacles()
    {
        float angleIncrement = _whiskerMaxAngle / _whiskerCount;
        float initialAngle = (-_whiskerMaxAngle / 2f) + angleIncrement / 2f;
        float currentAngle = initialAngle;
        float targetRotation = 0;


        for (int i = 0; i < _whiskerCount; i++)
        {
            if (CastWhisker(currentAngle, out Vector3 whiskerDirection))
            {
                float whiskerAngle = Vector3.SignedAngle(transform.forward, whiskerDirection, Vector3.up);

                targetRotation -= whiskerAngle;
            }

            currentAngle += angleIncrement;
        }

        transform.Rotate(Vector3.up, _avoidanceWeight * targetRotation * Time.deltaTime);
    }

    private bool CastWhisker(float angle, out Vector3 whiskerDirection)
    {
        Color rayColor = Color.red;

        whiskerDirection = Quaternion.Euler(0, angle, 0) * transform.forward;

        Physics.Raycast(new Ray(transform.position + Vector3.up * 0.5f, whiskerDirection), out RaycastHit hit, _whiskerLength, _environmentLayer);

        if (hit.collider != null)
            rayColor = Color.green;

        Debug.DrawRay(transform.position + Vector3.up * 0.5f, whiskerDirection * _whiskerLength, rayColor);
        return hit.collider != null;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

using Random = UnityEngine.Random;

public class AgentManager : MonoBehaviour
{
    [SerializeField] private GameObject _agentPrefab;
    [SerializeField] private GameObject _targetPrefab;
    [SerializeField] private GameObject _obstaclePrefab;
    [Space]
    [SerializeField] private float _spawnRadius = 25f;
    [Space]
    [SerializeField] InputAction _agentStateAction;

    private Transform _target;
    private Agent _agent;
    private Transform _obstacle;

    private void Awake()
    {
        Setup();
    }

    private void OnEnable()
    {
        _agentStateAction.started += HandleStateChange;
        _agentStateAction.Enable();
    }

    private void OnDisable()
    {
        _agentStateAction.started -= HandleStateChange;
        _agentStateAction.Disable();
    }

    private void Setup()
    {
        _agent = Instantiate(_agentPrefab).GetComponent<Agent>();
        _target = Instantiate(_targetPrefab).transform;
        _obstacle = Instantiate(_obstaclePrefab).transform;
        _agent.Target = _target;

        DisableActors();
    }

    private void HandleStateChange(InputAction.CallbackContext context)
    {
        ToggleObstacles(false);

        Agent.AgentState behaviourState = (Agent.AgentState)(int)context.ReadValue<float>();

        switch (behaviourState)
        {
            case Agent.AgentState.Seek:
                StartSeek();
                break;
            case Agent.AgentState.Flee:
                StartFlee();
                break;
            case Agent.AgentState.Arrival:
                StartSeek();
                break;
            case Agent.AgentState.Avoid:
                ToggleObstacles(true);
                StartSeek();
                _obstacle.right = Vector3.Cross(_target.position - _agent.transform.position, Vector3.up);
                break;
            default:
                DisableActors();
                break;
        }

        _agent.SetState(behaviourState);
    }

    private void StartSeek()
    {
        _agent.transform.position = GetRandomPointAlongCircle(transform.position, _spawnRadius);
        _target.position = Vector3.Reflect(-_agent.transform.position, Vector3.up);

        _agent.transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, 60, 0) * (_target.position - _agent.transform.position), Vector3.up);

        _agent.gameObject.SetActive(true);
        _target.gameObject.SetActive(true);
    }

    private void StartFlee()
    {
        _target.position = Vector3.zero;
        _agent.transform.position = GetRandomPointAlongCircle(transform.position, _spawnRadius);
        _agent.transform.LookAt(_target.position + transform.position, Vector3.up);

        _agent.gameObject.SetActive(true);
        _target.gameObject.SetActive(true);
    }

    private void StartAvoidanceSeek()
    {
        _agent.transform.position = new(8, 0, -7);
        _agent.transform.rotation = Quaternion.Euler(0, -90f, 0);
        _target.transform.position = new(8, 0, 7);

        _agent.gameObject.SetActive(true);
        _target.gameObject.SetActive(true);
    }

    private void ToggleObstacles(bool toggle)
    {
        _obstacle.gameObject.SetActive(toggle);
    }

    private void DisableActors()
    {
        _obstacle.gameObject.SetActive(false);
        _target.gameObject.SetActive(false);
        _agent.gameObject.SetActive(false);
        _agent.SetState(Agent.AgentState.Idle);
    }

    private Vector3 GetRandomPointAlongCircle(Vector3 center, float radius)
    {
        Vector3 point = center + Vector3.forward * radius;
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        return randomRotation * point;
    }

    private Vector3[] GetPointsAlongCircle(Vector3 center, float radius)
    {
        Vector3[] points = new Vector3[36];
        for (int i = 0; i < 36; i++)
        {
            int increment = (i & 1) == 1 ? i : i - 1;
            points[i] = Quaternion.Euler(0, (360 / 36f) * increment, 0) * ((center - Vector3.forward) * radius);
        }

        return points;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLineList(GetPointsAlongCircle(transform.position, _spawnRadius));
    }
}

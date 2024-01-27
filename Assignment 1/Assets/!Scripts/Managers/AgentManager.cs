using UnityEngine;
using UnityEngine.InputSystem;

public class AgentManager : MonoBehaviour
{
    [SerializeField] private GameObject _agentPrefab;
    [SerializeField] private GameObject _targetPrefab;
    [SerializeField] private Collider _groundCollider;
    [SerializeField] InputAction _agentStateAction;

    private Transform _target;
    private Agent _agent;

    public Transform AgentTransform => _agent.transform;
    public Transform TargetTransform => _target.transform;

    private void Awake()
    {
        SpawnAgent();
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

    private void HandleStateChange(InputAction.CallbackContext context)
    {
        int stateInput = (int)context.ReadValue<float>();
        Debug.Log(stateInput);
        StartBehaviour(stateInput);
    }

    public void SpawnAgent()
    {
        _agent = Instantiate(_agentPrefab).GetComponent<Agent>();
        _target = Instantiate(_targetPrefab).transform;
        _agent.SetTarget(_target);
        _target.gameObject.SetActive(false);
        _agent.gameObject.SetActive(false);
    }

    public void StartBehaviour(int behaviourIndex)
    {
        ToggleObstacles(behaviourIndex == 5);
        switch ((Agent.AgentState)behaviourIndex)
        {
            case Agent.AgentState.Idle:
                DisableActors();
                break;
            case Agent.AgentState.Seek:
                StartSeek();
                break;
            case Agent.AgentState.Flee:
                StartFlee();
                break;
            case Agent.AgentState.Arrival:
                StartArrival();
                break;
        }
    }

    private void ToggleObstacles(bool toggle)
    {

    }

    private void DisableActors()
    {
        _target.gameObject.SetActive(false);
        _agent.gameObject.SetActive(false);
        _agent.SetState(Agent.AgentState.Idle);
    }

    private void StartSeek()
    {
        Vector2 extents = new Vector2(_groundCollider.bounds.extents.x, _groundCollider.bounds.extents.z);
        Vector3 randomPosition = new Vector3(Random.Range(-extents.x, extents.x), 0, Random.Range(-extents.x, extents.x));

        _target.position = randomPosition;

        _agent.transform.SetPositionAndRotation(GetOutskirtPositionForAgent(), Quaternion.identity);

        _target.gameObject.SetActive(true);
        _agent.gameObject.SetActive(true);

        _agent.SetState(Agent.AgentState.Seek);
    }

    private void StartFlee()
    {
        Vector2 extents = new Vector2(_groundCollider.bounds.extents.x, _groundCollider.bounds.extents.z);
        Vector3 randomPosition = new Vector3(Random.Range(-extents.x, extents.x), 0, Random.Range(-extents.x, extents.x));
        _target.position = randomPosition;

        Vector3 closeToTarget = _target.position + (Vector3.left * 3f);
        Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0, 359f), _target.up);

        _agent.transform.SetPositionAndRotation(randomRotation * closeToTarget, Quaternion.identity);

        _agent.SetState(Agent.AgentState.Flee);
    }

    private void StartArrival()
    {
        throw new System.NotImplementedException();
    }

    private Vector3 GetOutskirtPositionForAgent()
    {
        float distanceToCenter = _groundCollider.bounds.extents.x;

        Vector3 agentPosition = Vector3.left * distanceToCenter;
        Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0, 359f), Vector3.up);
        return randomRotation * agentPosition;
    }
}

using System;
using System.Collections.Generic;
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

    public Transform AgentTransform => _agent.transform;
    public Transform TargetTransform => _target.transform;
    public Transform ObstacleTransform => _obstacle.transform;

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

        _target.gameObject.SetActive(false);
        _agent.gameObject.SetActive(false);
        _obstacle.gameObject.SetActive(false);
    }

    private void HandleStateChange(InputAction.CallbackContext context)
    {
        ToggleObstacles(false);

        Agent.AgentState behaviourState = (Agent.AgentState)(int)context.ReadValue<float>();
        Debug.Log(behaviourState.ToString());

        switch (behaviourState)
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
            case Agent.AgentState.Avoid:
                ToggleObstacles(true);
                StartAvoidanceSeek();
                break;
            default:
                DisableActors();
                break;
        }
    }

    private void StartSeek()
    {
        Vector3[] positions = GetPointsAlongCircle(transform.position, _spawnRadius);
        _agent.transform.position = positions[Random.Range(0, 36)];
        _target.position = Vector3.Reflect(-_agent.transform.position, Vector3.up);

        _agent.transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, 60, 0) * (_target.position -  _agent.transform.position), Vector3.up);

        _agent.SetState(Agent.AgentState.Seek);

        _agent.gameObject.SetActive(true);
        _target.gameObject.SetActive(true);
    }

    private void StartFlee()
    {
        _target.position = Vector3.zero;
        Vector3[] positions = GetPointsAlongCircle(_target.position, 5);
        _agent.transform.position = positions[Random.Range(0, 36)];
        _agent.transform.LookAt(_target.position + transform.position, Vector3.up);

        _agent.SetState(Agent.AgentState.Flee);

        _agent.gameObject.SetActive(true);
        _target.gameObject.SetActive(true);
    }

    private void StartArrival()
    {
        Vector3[] positions = GetPointsAlongCircle(transform.position, _spawnRadius);
        _agent.transform.position = positions[Random.Range(0, 36)];
        _target.position = Vector3.Reflect(-_agent.transform.position, Vector3.up);

        _agent.transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, 60, 0) * (_target.position - _agent.transform.position), Vector3.up);

        _agent.SetState(Agent.AgentState.Arrival);

        _agent.gameObject.SetActive(true);
        _target.gameObject.SetActive(true);
    }

    private void StartAvoidanceSeek()
    {
        _agent.SetState(Agent.AgentState.Avoid);
    }

    private void ToggleObstacles(bool toggle)
    {
        _obstacle.gameObject.SetActive(toggle);
    }

    private void DisableActors()
    {
        _obstacle.gameObject.SetActive(!_agent.gameObject.activeSelf);
        _target.gameObject.SetActive(false);
        _agent.gameObject.SetActive(false);
        _agent.SetState(Agent.AgentState.Idle);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLineList(GetPointsAlongCircle(transform.position, _spawnRadius));
    }
}

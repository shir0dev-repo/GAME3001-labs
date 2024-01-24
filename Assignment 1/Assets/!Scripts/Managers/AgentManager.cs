using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField] private GameObject _agentPrefab;
    [SerializeField] private GameObject _targetPrefab;

    private Transform _target;
    private Agent _agent;

    public void SpawnAgent()
    {
        _agent = Instantiate(_agentPrefab).GetComponent<Agent>();
        _target = Instantiate(_targetPrefab).transform;
        _agent.SetTarget(_target);
        _target.gameObject.SetActive(false);
        _agent.gameObject.SetActive(false);
    }

    public void SetAgentState(Agent.AgentBehaviour agentBehaviour)
    {
        _agent.SetBehaviour(agentBehaviour);
    }
}

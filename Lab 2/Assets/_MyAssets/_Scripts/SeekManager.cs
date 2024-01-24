using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekManager : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private Starship _agent;

    private Vector3 _initialTargetPosition = Vector3.zero;

    private void Awake()
    {
        _initialTargetPosition = _target.transform.position;
    }

    public void ResetSeekState()
    {
        _agent.ResetState();

        _target.transform.position = _initialTargetPosition;
    }

    public void StartSeeking()
    {
        _agent.StartSeeking();
    }
}

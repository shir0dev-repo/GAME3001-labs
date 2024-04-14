using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropManager : Singleton<PropManager>
{

    [SerializeField] private GameObject[] _obstacles;
    [SerializeField] private GameObject _targetProp;
    public GameObject TargetProp => _targetProp;

    public GameObject GetRandomObstacle()
    {
        return _obstacles[Random.Range(0, _obstacles.Length)];
    }
}

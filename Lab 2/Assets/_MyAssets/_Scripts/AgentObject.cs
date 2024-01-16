using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentObject : MonoBehaviour
{
    [SerializeField] Transform m_target;

    public Vector3 TargetPosition
    {
        get { return m_target.position; }
        set { m_target.position = value; }
    }

    private void Start()
    {
        
    }
}

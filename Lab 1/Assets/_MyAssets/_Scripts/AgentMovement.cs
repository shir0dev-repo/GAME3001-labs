using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    [SerializeField] private Camera m_mainCam;
    [SerializeField] private Transform m_targetIcon;
    [Space]
    [SerializeField] private Vector3 m_targetPosition;
    [SerializeField] private float m_moveSpeed = 10f;

    private void Awake()
    {
        if (m_mainCam == null) m_mainCam = Camera.main;    
    }

    private void Update()
    {
        // left mouse up.
        if (Input.GetMouseButtonUp(0))
        {
            // get mouse click in world space.
            m_targetPosition = m_mainCam.ScreenToWorldPoint(Input.mousePosition);
            // ignore z-value.
            m_targetPosition.z = 0;

            // set target visualizer if possible.
            if (m_targetIcon != null)
                m_targetIcon.position = m_targetPosition;
        }

        // only move if farther than 0.1 units away.
        if (Vector3.Distance(transform.position, m_targetPosition) > 0.1f)
        {
            // move transform toward target position.
            transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, m_moveSpeed * Time.deltaTime);

            // rotate plane towards target.
            transform.right = m_targetPosition - transform.position;
        }
    }
}

/*
 
eric.blanchard@georgebrown.ca
connor.smiley@georgebrown.ca
sinan.kolip@georgebrown.ca
 
 */

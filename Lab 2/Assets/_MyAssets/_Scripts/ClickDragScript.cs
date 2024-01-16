using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDragScript : MonoBehaviour
{
    bool m_isDragging = false;
    private Rigidbody2D m_currentDraggedBody;
    private Vector3 m_dragOffset = Vector3.zero;
    private Vector2 m_mousePosition = Vector2.zero;

    private Camera m_mainCam;


    private void Awake()
    {
        m_mainCam = Camera.main;
    }

    private void Update()
    {
        // update current mouse position.
        m_mousePosition = m_mainCam.ScreenToWorldPoint(Input.mousePosition);

        // listen for mouse click
        if(Input.GetMouseButtonDown(0))
        {
            // raycast from mouse position
            RaycastHit2D hit = Physics2D.Raycast(m_mousePosition, Vector2.zero);

            // gameobject has both a collider and a rigidbody2D
            if (hit.collider != null && hit.rigidbody != null && hit.rigidbody != m_currentDraggedBody)
            {
                // start dragging if no other object is being dragged.
                m_isDragging = true;
                m_currentDraggedBody = hit.rigidbody;
                m_dragOffset = m_currentDraggedBody.position - m_mousePosition;
            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            m_isDragging = false;
            m_currentDraggedBody = null;
            m_dragOffset = Vector2.zero;
        }

        if (m_isDragging && m_currentDraggedBody != null)
        {
            m_currentDraggedBody.MovePosition(m_dragOffset + m_mainCam.ScreenToWorldPoint(Input.mousePosition));
        }
    }

}

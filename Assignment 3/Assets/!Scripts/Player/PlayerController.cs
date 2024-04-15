using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_moveSpeed, m_maxSpeed;
    [SerializeField] private float m_rotationSpeed;

    private Rigidbody m_rigidbody;
    private Animator m_anim;
    private Camera m_mainCam;

    private Vector3 m_forceDirection = Vector3.zero;

    private float m_timeSpentIdle = 0;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_anim = GetComponent<Animator>();
        m_mainCam = Camera.main;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector3 horizontalInput = Input.GetAxisRaw("Horizontal") * GetCameraRight() * m_moveSpeed;
        Vector3 verticalInput = Input.GetAxisRaw("Vertical") * GetCameraForward() * m_moveSpeed;

        if (horizontalInput.sqrMagnitude < 0.1f && verticalInput.sqrMagnitude < 0.1f)
        {
            if (Random.value > 0.995f)
                m_anim.SetTrigger("_idleThreshold");
        }


        m_forceDirection += horizontalInput;
        m_forceDirection += verticalInput;

        m_rigidbody.AddForce(m_forceDirection, ForceMode.Impulse);
        m_forceDirection = Vector3.zero;

        Vector3 horizontalVelocity = m_rigidbody.velocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.sqrMagnitude > m_maxSpeed * m_maxSpeed)
            m_rigidbody.velocity = horizontalVelocity.normalized * m_maxSpeed + Vector3.up * m_rigidbody.velocity.y;

        m_anim.SetFloat("_velocity", m_rigidbody.velocity.sqrMagnitude);
    }

    private void HandleRotation()
    {
        Vector3 direction = m_rigidbody.velocity;

        if ((Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f) && direction.sqrMagnitude > 0.1f)
            m_rigidbody.MoveRotation(Quaternion.LookRotation(direction, Vector3.up));
        else
            m_rigidbody.angularVelocity = Vector3.zero;
    }

    private Vector3 GetCameraForward()
    {
        Vector3 forward = m_mainCam.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }
    private Vector3 GetCameraRight()
    {
        Vector3 right = m_mainCam.transform.right;
        right.y = 0;
        return right.normalized;
    }

}

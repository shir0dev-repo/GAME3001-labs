using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starship : AgentObject
{
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float whiskerLength = 1f;
    [SerializeField, Range(90, 360)] float whiskerMaxAngle = 90f;
    [SerializeField] int whiskerCount = 4;
    
    float whiskerAngleIncrement;

    // Add fields for whisper length, angle and avoidance weight.
    //
    //
    //
    private Rigidbody2D rb;

    new void Start() // Note the new.
    {
        base.Start(); // Explicitly invoking Start of AgentObject.
        Debug.Log("Starting Starship.");
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (TargetPosition != null)
        {
            // Seek();
            SeekForward();
            // Add call to AvoidObstacles.
            AvoidObstacles();
        }
    }

    private void AvoidObstacles()
    {
        whiskerAngleIncrement = whiskerMaxAngle / whiskerCount;

        for (int i = -whiskerCount / 2; i < whiskerCount / 2; i++)
        {
            CastWhisker(whiskerAngleIncrement * i);
        }
        //bool hitLeft = CastWhisker(35f);
        //bool hitRight = CastWhisker(-35f);
    }

    private bool CastWhisker(float angle)
    {
        Color rayColor = Color.red;

        // Cast whiskers to detect obstacles.
        Vector2 whiskerDirection = Quaternion.Euler(0, 0, angle) * transform.up;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, whiskerDirection, whiskerLength);
        
        if (hit.collider != null)
            rayColor = Color.green;
        
        Debug.DrawRay(transform.position, whiskerDirection * whiskerLength, rayColor);
        return hit.collider != null;
    }

    private void RotateCounterClockwise()
    {
        // Rotate counterclockwise based on rotationSpeed and a weight.
        // 
    }

    private void RotateClockwise()
    {
        // Rotate clockwise based on rotationSpeed and a weight.
        // 
    }

    // Add CastWhisker method. I removed it entirely.
    //
    //
    //
    //

    private void SeekForward() // A seek with rotation to target but only moving along forward vector.
    {
        // Calculate direction to the target.
        Vector2 directionToTarget = (TargetPosition - transform.position).normalized;

        // Calculate the angle to rotate towards the target.
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg + 90.0f; // Note the +90 when converting from Radians.

        // Smoothly rotate towards the target.
        float angleDifference = Mathf.DeltaAngle(targetAngle, transform.eulerAngles.z);
        float rotationStep = rotationSpeed * Time.deltaTime;
        float rotationAmount = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
        transform.Rotate(Vector3.forward, rotationAmount);

        // Move along the forward vector using Rigidbody2D.
        rb.velocity = transform.up * movementSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Target")
        {
            GetComponent<AudioSource>().Play();
            // What is this!?! Didn't you learn how to create a static sound manager last week in 1017?
        }
    }
}

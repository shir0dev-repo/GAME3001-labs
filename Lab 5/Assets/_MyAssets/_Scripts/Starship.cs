using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starship : AgentObject
{
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float whiskerLength;
    [SerializeField] float whiskerAngle;
    [SerializeField] float avoidanceWeight;
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
            AvoidObstacles();
        }
    }

    private void AvoidObstacles()
    {
        // Cast whiskers to detect obstacles
        bool hitLeft = CastWhisker(whiskerAngle, Color.red);
        bool hitRight = CastWhisker(-whiskerAngle, Color.blue);

        // Adjust rotation based on detected obstacles
        if (hitLeft)
        {
            // Rotate counterclockwise if the left whisker hit
            RotateClockwise();
        }
        else if (hitRight && !hitLeft)
        {
            // Rotate clockwise if the right whisker hit
            RotateCounterClockwise();
        }
    }

    private void RotateCounterClockwise()
    {
        // Rotate counterclockwise based on rotationSpeed and a weight.
        transform.Rotate(Vector3.forward, rotationSpeed * avoidanceWeight * Time.deltaTime);
    }

    private void RotateClockwise()
    {
        // Rotate clockwise based on rotationSpeed.
        transform.Rotate(Vector3.forward, -rotationSpeed * avoidanceWeight * Time.deltaTime);
    }

    private bool CastWhisker(float angle, Color color)
    {
        bool hitResult = false;
        Color rayColor = color;
        // Calculate the direction of the whisker.
        Vector2 whiskerDirection = Quaternion.Euler(0, 0, angle) * transform.up;

        // Cast a ray in the whisker direction.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, whiskerDirection, whiskerLength);

        // Check if the ray hits an obstacle.
        if (hit.collider != null)
        {
            Debug.Log("Obstacle detected!");
            rayColor = Color.green;
            hitResult = true;
        }
        // Debug ray visualization
        Debug.DrawRay(transform.position, whiskerDirection * whiskerLength, rayColor);
        return hitResult;
    }

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

using System;
using UnityEngine;

public class Starship : AgentObject
{
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;

    [SerializeField] float whiskerLength = 1f;
    [SerializeField, Range(90f, 360f)] float whiskerAngleMax = 90f;
    [SerializeField] float avoidanceWeight = 2f;
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
        whiskerAngleIncrement = whiskerAngleMax / whiskerCount;
        float currentAngle = (-whiskerAngleMax / 2f) + whiskerAngleIncrement / 2f;

        float weightedDirection = 0;
        for (int i = 0; i < whiskerCount; i++)
        {
            if (CastWhisker(currentAngle, out Vector2 whiskerDirection))
            {
                float angleDir = GetAngleDirection(whiskerDirection);
                weightedDirection += angleDir;
                Debug.Log(i + ": " + angleDir);
            }

            currentAngle += whiskerAngleIncrement;
        }

        Debug.Log(weightedDirection);


        if (weightedDirection < 0)
            RotateCounterClockwise();
        else if (weightedDirection > 0)
            RotateClockwise();
    }

    private float GetAngleDirection(Vector3 target)
    {
        if (Vector3.Angle(transform.up, target.normalized) < whiskerAngleIncrement/2f)
            return 0f;

        Vector3 perp = Vector3.Cross(transform.up, target);
        float dirAngle = Vector3.Dot(perp, transform.forward);

        if (dirAngle > 0f)
            return 1f;
        else if (dirAngle < 0f)
            return -1f;
        else
            return 0f;
    }

    private bool CastWhisker(float angle, out Vector2 whiskerDirection)
    {
        Color rayColor = Color.red;

        // Cast whiskers to detect obstacles.
        whiskerDirection = Quaternion.Euler(0, 0, angle) * transform.up;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, whiskerDirection, whiskerLength);

        if (hit.collider != null)
            rayColor = Color.green;

        Debug.DrawRay(transform.position, whiskerDirection * whiskerLength, rayColor);
        return hit.collider != null;
    }

    private void RotateCounterClockwise()
    {
        // Rotate counterclockwise based on rotationSpeed and a weight.
        transform.Rotate(Vector3.forward, rotationSpeed * avoidanceWeight * Time.deltaTime);
    }

    private void RotateClockwise()
    {
        transform.Rotate(Vector3.forward, -rotationSpeed * avoidanceWeight * Time.deltaTime);
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
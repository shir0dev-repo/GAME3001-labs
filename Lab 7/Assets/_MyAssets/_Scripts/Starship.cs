using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starship : AgentObject
{
    // TODO: Add for Lab 7a.
    //
    //
    [SerializeField] float movementSpeed; // TODO: Uncomment for Lab 7a.
    [SerializeField] float rotationSpeed;
    [SerializeField] float whiskerLength;
    [SerializeField] float whiskerAngle;
    // [SerializeField] float avoidanceWeight;
    private Rigidbody2D rb;
    private NavigationObject no;
    // Decision Tree. TODO: Add for Lab 7a.
    //
    //
    //

    new void Start() // Note the new.
    {
        base.Start(); // Explicitly invoking Start of AgentObject.
        Debug.Log("Starting Starship.");
        rb = GetComponent<Rigidbody2D>();
        no = GetComponent<NavigationObject>();
        // TODO: Add for Lab 7a.
        //
        //
        //
    }

    void Update()
    {
        // bool hit = CastWhisker(whiskerAngle, Color.red);
        // transform.Rotate(0f, 0f, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);

        //if (TargetPosition != null)
        //{
        //    // Seek();
        //    SeekForward();
        //    AvoidObstacles();
        //}

        // TODO: Add for Lab 7a. Add seek target for tree temporarily to planet.
        //

        // TODO: Update for Lab 7a.
        // 
        // 
        //
        //
        //
        //
    }

    //private void AvoidObstacles()
    //{
    //    // Cast whiskers to detect obstacles
    //    bool hitLeft = CastWhisker(whiskerAngle, Color.red);
    //    bool hitRight = CastWhisker(-whiskerAngle, Color.blue);

    //    // Adjust rotation based on detected obstacles
    //    if (hitLeft)
    //    {
    //        // Rotate counterclockwise if the left whisker hit
    //        RotateClockwise();
    //    }
    //    else if (hitRight && !hitLeft)
    //    {
    //        // Rotate clockwise if the right whisker hit
    //        RotateCounterClockwise();
    //    }
    //}

    //private void RotateCounterClockwise()
    //{
    //    // Rotate counterclockwise based on rotationSpeed and a weight.
    //    transform.Rotate(Vector3.forward, rotationSpeed * avoidanceWeight * Time.deltaTime);
    //}

    //private void RotateClockwise()
    //{
    //    // Rotate clockwise based on rotationSpeed.
    //    transform.Rotate(Vector3.forward, -rotationSpeed * avoidanceWeight * Time.deltaTime);
    //}

    private bool CastWhisker(float angle, Color color)
    {
        bool hitResult = false;
        Color rayColor = color;

        // Calculate the direction of the whisker.
        Vector2 whiskerDirection = Quaternion.Euler(0, 0, angle) * transform.right;

        if (no.HasLOS(gameObject, "Planet", whiskerDirection, whiskerLength))
        {
            // Debug.Log("Obstacle detected!");
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

        // TODO: New for Lab 7a. Continue patrol.
        //
        //
        //
    }

    // TODO: Add for Lab 7a.
    public void StartPatrol()
    {
        //
    }

    // TODO: Add for Lab 7a.
    private Transform GetNextPatrolPoint()
    {
        //
        //
        //
        //
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.gameObject.tag == "Target")
    //    {
    //        GetComponent<AudioSource>().Play();
    //    }
    //}

    // TODO: Fill in for Lab 7a.
    private void BuildTree()
    {
        // Root condition node.
        //
        //

        // Second level.

        // PatrolAction leaf.
        //
        //
        //

        // LOSCondition node.
        //
        //

        // Third level.

        // MoveToLOSAction leaf.
        //
        //
        //

        // CloseCombatCondition node.
        //
        //

        // Fourth level.

        // MoveToPlayerAction leaf.
        //
        //
        //

        // AttackAction leaf.
        //
        //
        //
    }
}

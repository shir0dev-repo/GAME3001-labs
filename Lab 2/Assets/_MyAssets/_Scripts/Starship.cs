using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Starship : AgentObject
{
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _rotationSpeed = 90.0f;
    [SerializeField] private Rigidbody2D _rigidbody;

    protected override void Start()
    {
        base.Start();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //Seek();
        SeekForward();
    }

    private void Seek()
    {
        Vector2 linearVelocity = (TargetPosition - transform.position).normalized * _moveSpeed;

        Vector2 angularVelocity = linearVelocity - _rigidbody.velocity;

        _rigidbody.AddForce(angularVelocity);
    }

    private void SeekForward()
    {
        Vector3 lookDir = (TargetPosition - transform.position).normalized;

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90.0f;
        float angleDifference = Mathf.DeltaAngle(angle, transform.eulerAngles.z);
        float rotationStep = _rotationSpeed * Time.deltaTime;
        float rotationAngle = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);

        transform.Rotate(Vector3.forward, rotationAngle);

        _rigidbody.velocity = transform.up * _moveSpeed;
    }
}

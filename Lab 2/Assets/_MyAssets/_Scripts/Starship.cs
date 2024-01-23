using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Starship : AgentObject
{
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private Rigidbody2D _rigidbody;

    protected override void Start()
    {
        base.Start();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Seek();
    }

    private void Seek()
    {
        Vector2 linearVelocity = (TargetPosition - transform.position).normalized * _moveSpeed;

        Vector2 angularVelocity = linearVelocity - _rigidbody.velocity;

        _rigidbody.AddForce(angularVelocity);
    }
}

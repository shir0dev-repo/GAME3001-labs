using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Starship : AgentObject
{
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _rotationSpeed = 90.0f;
    [SerializeField] private Rigidbody2D _rigidbody;

    private Vector3 _initialPosition;

    protected override void Start()
    {
        base.Start();
        _rigidbody = GetComponent<Rigidbody2D>();
        ShouldSeek = false;
        _initialPosition = transform.position;
    }

    private void Update()
    {
        //Seek();
        if (ShouldSeek)
            SeekForward();
    }

    public void ResetState()
    {
        ShouldSeek = false;

        
        _rigidbody.velocity = Vector3.zero;
        transform.SetPositionAndRotation(_initialPosition, Quaternion.identity);
    }

    public void StartSeeking() => ShouldSeek = true;

    // private void Seek()
    // {
    //     Vector2 linearVelocity = (TargetPosition - transform.position).normalized * _moveSpeed;
    // 
    //     Vector2 angularVelocity = linearVelocity - _rigidbody.velocity;
    // 
    //     _rigidbody.AddForce(angularVelocity);
    // }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Target"))
            GetComponent<AudioSource>().Play();
    }
}

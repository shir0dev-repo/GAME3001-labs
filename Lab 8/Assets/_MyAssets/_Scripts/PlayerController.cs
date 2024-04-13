using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 4f, _rotationSpeed = 12f;

    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileCooldown = 2f;
    private bool _canFire = true;

    private void Update()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W))
            transform.position += Input.GetAxisRaw("Vertical") * _moveSpeed * Time.deltaTime * transform.up;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, 0, -Input.GetAxisRaw("Horizontal") * _rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            FireProjectile();
    }

    private void FireProjectile()
    {
        if (!_canFire) return;

        _canFire = false;
        Invoke(nameof(ResetCooldown), _projectileCooldown);

        Torpedo t = Instantiate(_projectilePrefab, transform.position, Quaternion.identity).GetComponent<Torpedo>();
        t.SetDirection(transform.up);
    }

    private void ResetCooldown() => _canFire = true;
}

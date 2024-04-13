using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    Vector2 _direction = Vector2.zero;
    float _moveSpeed = 4f;

    private void Update()
    {
        transform.Translate(_moveSpeed * Time.deltaTime * _direction);
    }

    public void SetDirection(Vector3 direction) => _direction = direction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out AgentObject ao)) return;

        ao.health -= 30;
    }
}

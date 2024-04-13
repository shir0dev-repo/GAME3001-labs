using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTorpedo : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private Vector2 directionToTarget;

    private void Update()
    {
        transform.Translate(directionToTarget.x, directionToTarget.y, 0f);
    }

    public void LockOnTarget(Transform target)
    {
        Vector2 dir = (target.position - transform.position).normalized;
        directionToTarget = dir * _moveSpeed * Time.deltaTime;
    }
}

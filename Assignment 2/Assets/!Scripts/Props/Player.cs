using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : ActorBase
{
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField] private float _rotationDuration = 0.3f;
    [SerializeField] private Chest _chest;

    private Coroutine _currentCO = null;

    public bool CurrentlyMoving => _currentCO != null;

    public void StartPathing(List<Node> path)
    {
        if (_currentCO == null)
            _currentCO = StartCoroutine(FollowPathCoroutine(path));
    }

    private IEnumerator FollowPathCoroutine(List<Node> path)
    {
        Queue<Node> pathQueue = new(path);

        while (pathQueue.Count > 1 && pathQueue.TryDequeue(out Node nextNode)) // last elem is target, which i did not want on same tile as player
        {
            AudioManager.Instance.PlaySoundEffect("Walk", 0.1f);
            float timeElapsed = 0;

            while (timeElapsed < _moveDuration)
            {
                timeElapsed += Time.deltaTime;
                _anim.SetFloat("_MoveBlend", 1);
                float progress = timeElapsed / _moveDuration;

                transform.position = Vector3.Lerp(CurrentNode.PropPosition, nextNode.PropPosition, progress);

                yield return new WaitForEndOfFrame();
            }

            transform.position = nextNode.transform.position + Vector3.up * 0.5f;
            CurrentNode.ToggleGlow(false);
            CurrentNode = nextNode;

            _anim.SetFloat("_MoveBlend", 0);

            if (pathQueue.TryPeek(out Node peek))
                yield return RotateToNodeCoroutine(peek);
            else
                yield return RotateToNodeCoroutine(NodeGrid.Instance.CurrentTarget);
        }

        NodeGrid.Instance.SetStart(CurrentNode);
        NodeGrid.Instance.ClearGlow();

        _anim.SetFloat("_MoveBlend", 0);
        _anim.SetTrigger("_OpenChest");
        _chest.Open();
        yield return new WaitForSeconds(4f);
        _currentCO = null;
    }

    private IEnumerator RotateToNodeCoroutine(Node nextNode)
    {
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(nextNode.PropPosition - CurrentNode.PropPosition, Vector3.up);
        if (Quaternion.Angle(currentRotation, targetRotation) < 5f) yield break;


        float timeRemaining = _rotationDuration;

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            float progress = timeRemaining / _rotationDuration;

            if (_anim != null)
            {
                _anim.SetFloat("_MoveBlend", 0.5f - (0.5f * Mathf.Cos(2 * Mathf.PI * progress + Mathf.PI)));
            }

            transform.rotation = Quaternion.Slerp(targetRotation, currentRotation, progress);
            yield return new WaitForEndOfFrame();
        }

    }
}

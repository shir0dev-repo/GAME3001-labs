using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public Node CurrentNode { get; private set; }

    private Coroutine _currentCO = null;

    public void SetNode(Node node)
    {
        CurrentNode = node;
    }

    public void StartPathing(List<Node> path)
    {
        if (_currentCO == null)
            _currentCO = StartCoroutine(FollowPathCoroutine(path));
    }

    private IEnumerator FollowPathCoroutine(List<Node> path)
    {
        Queue<Node> pathQueue = new Queue<Node>(path);
        Node currentNode;
        while (pathQueue.TryDequeue(out currentNode))
        {
            float timeElapsed = 0;
            float moveDuration = 1;
            while (timeElapsed < moveDuration)
            {
                timeElapsed += Time.deltaTime;
                float progress = timeElapsed / moveDuration;
                transform.position = Vector3.Lerp(transform.position, currentNode.transform.position + Vector3.up * 0.5f, progress);

                yield return new WaitForEndOfFrame();
            }

            transform.position = currentNode.transform.position + Vector3.up * 0.5f;
            CurrentNode = currentNode;
        }

        Debug.Log("finished!");
        _currentCO = null;
    }

    public void Initialize(Node current, Index directionNodeIndex)
    {
        CurrentNode = current;
        Node firstAdjacent = NodeGrid.Instance.CurrentPath[directionNodeIndex];
        transform.position = CurrentNode.PropPosition;
        transform.rotation = Quaternion.LookRotation(firstAdjacent.transform.position - CurrentNode.transform.position, Vector3.up);
    }

    public void Initialize(Node current, Node lookNode)
    {
        CurrentNode = current;
        transform.position = CurrentNode.PropPosition;
        transform.rotation = Quaternion.LookRotation(lookNode.transform.position - CurrentNode.transform.position, Vector3.up);
    }
}

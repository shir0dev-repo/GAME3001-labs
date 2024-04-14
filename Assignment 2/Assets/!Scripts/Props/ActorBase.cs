using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorBase : MonoBehaviour
{
    public Node CurrentNode { get; protected set; }
    [SerializeField] protected Animator _anim;

    public void Initialize(Node current, int directionNodeIndex)
    {
        try
        {
            Initialize(current, NodeGrid.Instance.CurrentPath[directionNodeIndex]);
        }
        catch (System.Exception)
        {
            transform.position = NodeGrid.Instance.CurrentStart.PropPosition;
        }
    }

    public void Initialize(Node current, Node lookNode)
    {
        CurrentNode = current;
        transform.SetPositionAndRotation(CurrentNode.PropPosition,
            Quaternion.LookRotation(lookNode.transform.position - CurrentNode.transform.position, Vector3.up));
    }
}

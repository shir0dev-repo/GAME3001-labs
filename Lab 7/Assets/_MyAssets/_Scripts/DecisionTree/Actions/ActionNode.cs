using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionNode : TreeNode
{
    public GameObject Agent { get; set; }

    public ActionNode()
    {
        isLeaf = true;
        Agent = null;
    }

    public abstract void Action(); // Abstract method for tree.
}

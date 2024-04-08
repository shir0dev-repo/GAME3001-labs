using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConditionNode : TreeNode
{
    public ConditionNode()
    {
        isLeaf = false;
    }

    public abstract bool Condition(); // Abstract method for tree.
}

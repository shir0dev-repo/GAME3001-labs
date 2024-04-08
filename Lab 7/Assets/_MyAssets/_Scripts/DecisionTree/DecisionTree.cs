using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Fill in for Lab 7a.
public class DecisionTree
{
    // TODO: Fill in properties.
    public GameObject Agent { get; set; }
    public LOSCondition LOSNode { get; set; }
    public RadiusCondition RadiusNode { get; set; }
    public CloseCombatCondition CCNode { get; set; }
    public List<TreeNode> TreeNodeList;
    public DecisionTree(GameObject agent)
    {
        // TODO: Fill in for Lab 7a.
        Agent = agent;
        TreeNodeList = new();
    }

    public void MakeDecision()
    {
        // TODO: Fill in for Lab 7a.
        TreeNode currentNode = TreeNodeList[0];
        while (!currentNode.isLeaf)
        {
            currentNode = ((ConditionNode)currentNode).Condition() ? currentNode.right : currentNode.left;
        }

        ((ActionNode)currentNode).Action();
    }

    public TreeNode AddNode(TreeNode parent, TreeNode child, TreeNodeType type)
    {
        // TODO: Fill in for Lab 7a.
        switch (type)
        {
            case TreeNodeType.LEFT_TREE_NODE:
                parent.left = child;
                break;
            case TreeNodeType.RIGHT_TREE_NODE:
                parent.right = child;
                break;
        }
        child.parent = parent;

        return child;
    }
}

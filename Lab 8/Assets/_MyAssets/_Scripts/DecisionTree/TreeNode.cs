using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TreeNodeType
{
    LEFT_TREE_NODE,
    RIGHT_TREE_NODE
};

public abstract class TreeNode
{
    public string name;
    public TreeNode left = null;
    public TreeNode right = null;
    public TreeNode parent = null;
    public bool isLeaf = false;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public GameObject Tile { get; private set; }
    public List<PathConnection> Connections;

    public PathNode(GameObject tile)
    {
        Tile = tile;
        Connections = new List<PathConnection>();
    }

    public void AddConnection(PathConnection pc) => Connections.Add(pc);
}

[System.Serializable]
public class PathConnection
{
    public float Cost { get; set; }
    public PathNode FromNode { get; private set; }
    public PathNode ToNode { get; private set; }

    public PathConnection(PathNode from, PathNode to, float cost = 1)
    {
        FromNode = from;
        ToNode = to; 
        Cost = cost;
    }
}

public class NodeRecord
{
    public PathNode Node { get; set; }
    public NodeRecord FromRecord { get; set; }
    public PathConnection PathConnection { get; set; }
    public float CurrentCost { get; set; }

    public NodeRecord(PathNode node = null)
    {
        Node = node;
        PathConnection = null;
        FromRecord = null;
        CurrentCost = 0;
    }
}

[System.Serializable]
public class PathManager : MonoBehaviour
{

}

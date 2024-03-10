using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathNode
{
    public GameObject Tile { get; private set; }
    public List<PathConnection> connections;
    
    public PathNode(GameObject tile)
    {
        Tile = tile;
        connections = new List<PathConnection>();
    }

    public void AddConnection(PathConnection c)
    {
        connections.Add(c);
    }
}

[System.Serializable]
public class PathConnection
{
    public float Cost { get; set; } // This is a new cost from tile to tile. We'll use distance in units.
    public PathNode FromNode { get; private set; }
    public PathNode ToNode { get; private set; }
    public PathConnection(PathNode from, PathNode to, float cost = 1f)
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
    public float CostSoFar { get; set; }

    public NodeRecord(PathNode node = null)
    {
        Node = node;
        PathConnection = null;
        FromRecord = null;
        CostSoFar = 0f;
    }
};

[System.Serializable]
public class PathManager : MonoBehaviour
{
    public List<NodeRecord> openList;
    public List<NodeRecord> closedList;
    // What will be the shortest path.
    public List<PathConnection> path;
    public static PathManager Instance { get; private set; } // Static object of the class.

    void Awake()
    {
        if (Instance == null) // If the object/instance doesn't exist yet.
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void Initialize()
    {
        openList = new List<NodeRecord>();
        closedList = new List<NodeRecord>();
        path = new List<PathConnection>();
    }

    // The big algorithm.
    public void GetShortestPath(PathNode start, PathNode goal)
    {
        if (path.Count > 0)
        {
            GridManager.Instance.SetTileStatuses();
            path.Clear(); // Clear previous path.
        }
        NodeRecord currentRecord = null;
        openList.Add(new NodeRecord(start)); // But don't set status of start tile.
        while (openList.Count > 0)
        {
            currentRecord = GetSmallestNode();
            if (currentRecord.Node == goal)
            {
                // We found the goal!
                openList.Remove(currentRecord);
                closedList.Add(currentRecord);
                currentRecord.Node.Tile.GetComponent<TileScript>().SetStatus(TileStatus.CLOSED);
                break;
            }
            List<PathConnection> connections = currentRecord.Node.connections;
            for (int i = 0; i < connections.Count; i++)
            {
                PathNode endNode = connections[i].ToNode;
                NodeRecord endNodeRecord;
                float endNodeCost = currentRecord.CostSoFar + connections[i].Cost;

                if (ContainsNode(closedList, endNode)) continue;
                else if (ContainsNode(openList, endNode))
                {
                    endNodeRecord = GetNodeRecord(openList, endNode);
                    if (endNodeRecord.CostSoFar <= endNodeCost)
                        continue;
                }
                else
                {
                    endNodeRecord = new NodeRecord();
                    endNodeRecord.Node = endNode;
                }
                endNodeRecord.CostSoFar = endNodeCost;
                endNodeRecord.PathConnection = connections[i];
                endNodeRecord.FromRecord = currentRecord;
                if (!ContainsNode(openList, endNode))
                {
                    openList.Add(endNodeRecord);
                    endNodeRecord.Node.Tile.GetComponent<TileScript>().SetStatus(TileStatus.CLOSED);
                }
            }
            openList.Remove(currentRecord);
            closedList.Add(currentRecord);
            currentRecord.Node.Tile.GetComponent<TileScript>().SetStatus(TileStatus.CLOSED);
        }
        if (currentRecord == null) return;
        if (currentRecord.Node != goal)
        {
            Debug.LogError("Could not find path to goal!");
        }
        else
        {
            Debug.Log("Found path to goal!");
            while (currentRecord.Node != start)
            {
                path.Add(currentRecord.PathConnection);
                currentRecord.Node.Tile.GetComponent<TileScript>().SetStatus(TileStatus.PATH);
                currentRecord = currentRecord.FromRecord;
            }
            path.Reverse();
        }
        openList.Clear();
        closedList.Clear();
    }
    // Start of some utility methods.
    public NodeRecord GetSmallestNode()
    {
        NodeRecord smallestNode = openList[0];
        // Iterate through the rest of the NodeRecords in the list.
        for (int i = 1; i < openList.Count; i++)
        {
            // If the current NodeRecord has a smaller CostSoFar than the smallestNode, update smallestNode.
            if (openList[i].CostSoFar < smallestNode.CostSoFar)
            {
                smallestNode = openList[i];
            }
            // If they're the same, flip a coin. Optional, but looks better for Dijkstra.
            else if (openList[i].CostSoFar == smallestNode.CostSoFar)
            {
                smallestNode = (Random.value < 0.5 ? openList[i] : smallestNode);
            }
        }
        return smallestNode; // Return the NodeRecord with the smallest CostSoFar.
    }

    public bool ContainsNode(List<NodeRecord> list, PathNode node)
    {
        foreach (NodeRecord record in list)
        {
            if (record.Node == node) return true;
        }
        return false;
    }

    public NodeRecord GetNodeRecord(List<NodeRecord> list, PathNode node)
    {
        foreach (NodeRecord record in list)
        {
            if (record.Node == node) return record;
        }
        return null;
    }
}

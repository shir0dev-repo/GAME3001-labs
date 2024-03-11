using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public static PathManager Instance { get; private set; }

    public List<NodeRecord> openList;
    public List<NodeRecord> closedList;
    public List<PathConnection> path;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
        else
            Destroy(gameObject);
    }

    private void Initialize()
    {
        openList = new();
        closedList = new();
        path = new();
    }

    public void CalculateShortestPath(PathNode start, PathNode end)
    {
        //reset prev. path
        if (path.Count > 0)
        {
            GridManager.Instance.SetTileStatuses();
            path.Clear();
        }

        NodeRecord currentRecord = null;
        openList.Add(new NodeRecord(start));
        while (openList.Count > 0)
        {
            currentRecord = GetCheapestNode();

            if (currentRecord.Node == end)
            {
                // goal found
                openList.Remove(currentRecord);
                closedList.Add(currentRecord);
                currentRecord.Node.Tile.GetComponent<TileScript>().SetStatus(TileStatus.CLOSED);
                break;
            }

            List<PathConnection> connections = currentRecord.Node.Connections;
            for (int i = 0; i < connections.Count; i++)
            {
                PathNode endNode = connections[i].ToNode;
                NodeRecord endNodeRecord;
                float endNodeCost = currentRecord.CurrentCost + connections[i].Cost;

                if (ContainsNode(closedList, endNode)) continue;
                else if (ContainsNode(openList, endNode))
                {
                    endNodeRecord = GetNodeRecord(openList, endNode);
                    if (endNodeRecord.CurrentCost <= endNodeCost) continue;
                }
                else
                {
                    endNodeRecord = new NodeRecord()
                    {
                        Node = endNode
                    };
                }
                endNodeRecord.CurrentCost = endNodeCost;
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
        if (currentRecord.Node != end)
            Debug.LogError("Could not find path to goal.");
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

    #region Utilities

    public NodeRecord GetCheapestNode()
    {
        NodeRecord cheapestNode = openList[0];

        for (int i = 1; i < openList.Count; i++)
        {
            if (openList[i].CurrentCost < cheapestNode.CurrentCost)
            {
                cheapestNode = openList[i];
            }
            else if (openList[i].CurrentCost == cheapestNode.CurrentCost)
            {
                cheapestNode = Random.value > 0.5 ? openList[i] : cheapestNode;
            }
        }

        return cheapestNode;
    }

    public bool ContainsNode(List<NodeRecord> nodeRecords, PathNode node)
    {
        foreach (NodeRecord record in nodeRecords)
        {
            if (record.Node == node) return true;
        }

        return false;
    }

    public NodeRecord GetNodeRecord(List<NodeRecord> nodeRecords, PathNode node)
    {
        foreach (NodeRecord record in nodeRecords)
        {
            if (record.Node == node) return record;
        }

        return null;
    }

    #endregion
}

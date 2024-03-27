using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;
public class NodeGrid : Singleton<NodeGrid>
{
    private const int _SIZE = 10;

    [SerializeField] private GameObject _nodePrefab;
    [SerializeField] private Pathfinding.Heuristic _heuristic;

    public Node[,] Nodes { get; private set; }
    public List<Node> CurrentPath { get; private set; }

    public Node CurrentStart { get; private set; }
    public Node CurrentTarget { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Nodes = new Node[_SIZE, _SIZE];
        PopulateGrid();

        CurrentStart = GetRandomNode();
        CurrentTarget = GetRandomNode();

        CurrentPath = Pathfinding.GetPath(CurrentStart, CurrentTarget, _heuristic);
    }

    public void PopulateGrid()
    {
        for (int y = 0; y < _SIZE; y++)
        {
            for (int x = 0; x < _SIZE; x++)
            {
                Node node = Instantiate(_nodePrefab, transform).GetComponent<Node>();
                node.transform.position = new Vector3(x, 0, y);
                node.Position = new(x, y);
                node.gameObject.name = node.ToString();
                Nodes[x, y] = node;
            }
        }

        // populate after loop to ensure neighbours cannot be null (except the edges ;P)
        PopulateNeighbours();
    }

    private void PopulateNeighbours()
    {
        foreach (Node node in Nodes)
        {
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                Vector2Int v = dir.GetVector();
                try
                {
                    node.Neighbours.Add(Nodes[node.Position.x + v.x, node.Position.y + v.y]);
                }
                catch { continue; }
            }
        }
    }

    public void GeneratePath()
    {
        ResetGrid();
        CurrentPath = Pathfinding.GetPath(CurrentStart, CurrentTarget, _heuristic);
        RefreshDebugDisplay();
    }
    public void GeneratePath(bool isRandom)
    {
        if (isRandom)
        {
            CurrentStart = GetRandomNode();
            CurrentTarget = GetRandomNode();
        }

        GeneratePath();
    }
    public void GeneratePath(Node start, Node target)
    {
        CurrentPath = Pathfinding.GetPath(start, target, _heuristic);
    }

    private Node GetRandomNode()
    {
        return Nodes[Random.Range(0, Nodes.GetLength(0)), Random.Range(0, Nodes.GetLength(1))];
    }

    public void SetStart(Node node)
    {
        CurrentStart = node;
        ResetGrid();
        CurrentPath = Pathfinding.GetPath(CurrentStart, CurrentTarget, _heuristic);
    }
    public void SetTarget(Node node)
    {
        CurrentTarget = node;
        ResetGrid();
        CurrentPath = Pathfinding.GetPath(CurrentStart, CurrentTarget, _heuristic);
    }

    public void ResetGrid()
    {
        foreach (Node node in Nodes)
        {
            node.NodeType = Node.Type.DEFAULT;
            node.G = 0;
            node.H = 0;
        }
    }

    public void RefreshDebugDisplay()
    {
        foreach (Node node in Nodes)
        {
            node.ToggleDebug(true);
        }
    }
}

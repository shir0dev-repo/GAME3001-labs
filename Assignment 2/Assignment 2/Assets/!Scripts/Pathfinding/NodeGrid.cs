using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;
public class NodeGrid : Singleton<NodeGrid>
{
    private const int _SIZE = 10;

    [SerializeField] private GameObject _nodePrefab;
    [SerializeField] private Pathfinding.Heuristic _heuristic;
    [SerializeField] private int _randObstacles = 5;

    public Node[,] Nodes { get; private set; }
    public List<Node> CurrentPath { get; private set; }

    public Node CurrentStart { get; private set; }
    public Node CurrentTarget { get; private set; }
    public Pathfinding.Heuristic CurrentHeuristic => _heuristic;

    private bool _isGlowing = false;

    protected override void Awake()
    {
        base.Awake();
        Nodes = new Node[_SIZE, _SIZE];
        PopulateGrid();

        for (int i = 0; i < _randObstacles; i++)
        {
            GetRandomNode().ToggleObstacle();
        }

        GeneratePath(isRandom: true);
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

    public void GeneratePath(bool isRandom = false)
    {
        ResetGrid();

        if (isRandom)
        {
            CurrentStart = GetRandomNode();
            CurrentTarget = GetRandomNode();
        }

        CurrentPath = Pathfinding.GetPath(CurrentStart, CurrentTarget, _heuristic);
        RefreshDebugDisplay();
        TogglePathHighlight(_isGlowing);
    }
    public void GeneratePath(Node start, Node target)
    {
        CurrentPath = Pathfinding.GetPath(start, target, _heuristic);
    }

    private Node GetRandomNode()
    {
        int timeout = 500;
        int attempts = 0;
        do
        {
            attempts++;

            Node n = Nodes[Random.Range(0, Nodes.GetLength(0)), Random.Range(0, Nodes.GetLength(1))];
            if (n.IsObstacle == false)
                return n;
        } while (attempts < timeout);
        throw new StackOverflowException("Could not select a random node within 500 attempts. Please try again.");
    }

    public void SetStart(Node node)
    {
        CurrentStart = node;
        ResetGrid();
        CurrentPath = Pathfinding.GetPath(CurrentStart, CurrentTarget, _heuristic);
        RefreshDebugDisplay();
        TogglePathHighlight(_isGlowing);
    }
    public void SetTarget(Node node)
    {
        CurrentTarget = node;
        ResetGrid();
        CurrentPath = Pathfinding.GetPath(CurrentStart, CurrentTarget, _heuristic);
        RefreshDebugDisplay();
        TogglePathHighlight(_isGlowing);
    }

    public void TogglePathHighlight()
    {
        TogglePathHighlight(!_isGlowing);
    }
    public void TogglePathHighlight(bool state)
    {
        ClearGlow();
        _isGlowing = state;
        foreach (Node node in CurrentPath)
            node.ToggleGlow(_isGlowing);
    }

    public List<Node> GetObstacleNodes()
    {
        List<Node> obstNodes = new();

        foreach (Node node in Nodes)
        {
            if (node.IsObstacle)
                obstNodes.Add(node);
        }

        return obstNodes;
    }

    public void SwapHeuristicType()
    {
        if (_heuristic == Pathfinding.Heuristic.Manhattan)
            _heuristic = Pathfinding.Heuristic.Euclidean;
        else
            _heuristic = Pathfinding.Heuristic.Manhattan;
    }

    public void ResetGrid()
    {
        foreach (Node node in Nodes)
        {
            if (node.NodeType != Node.Type.OBSTACLE)
                node.NodeType = Node.Type.DEFAULT;
            node.G = 0;
            node.H = 0;
            node.ToggleGlow(false);
        }
    }

    public void ClearGlow()
    {
        foreach (Node node in Nodes)
            node.ToggleGlow(false);
        _isGlowing = false;
    }

    public void RefreshDebugDisplay()
    {
        if (GameController.Instance == null || GameController.Instance.InDebugMode == false)
            return;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdatePathCost(CurrentPath.Count);
        }
        foreach (Node node in Nodes)
        {
            node.ToggleDebug(true);
        }
    }
}

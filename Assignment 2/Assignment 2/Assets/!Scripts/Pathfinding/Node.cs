using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Node : MonoBehaviour
{
    public enum Type { DEFAULT, PATH, START, TARGET, OBSTACLE }
    public Type NodeType { get; set; }

    private static Color _glowColor;

    /// <summary>
    /// Distance to Starting Node.
    /// </summary>
    public float G;
    /// <summary>
    /// Distance to Target Node.
    /// </summary>
    public float H;

    /// <summary>
    /// Total cost of the node, i.e. distance from start + distance to end.
    /// </summary>
    public float F => G + H; // total cost

    /// <summary>
    /// Grid position of the node.
    /// </summary>
    /// <remarks>
    /// As this is stored in a 2D Node[], the position doubles as a place to store the index.
    ///</remarks>
    public Vector2Int Position;

    /// <summary>
    /// List of Nodes within the four Cardinal directions.
    /// </summary>
    [HideInInspector] public List<Node> Neighbours = new List<Node>();

    /// <summary>
    /// Prevents player from traversing over tile.
    /// </summary>
    [HideInInspector] public bool IsObstacle = false;

    [SerializeField] private NodeDebugDisplay DebugDisplay;
    [SerializeField] private Transform _prop;
    private MeshRenderer _tileMeshRenderer;
    private bool _isGlowing;
    public Vector3 PropPosition => _prop.position;

    private void Awake()
    {
        DebugDisplay = transform.GetChild(0).GetComponent<NodeDebugDisplay>();
        DebugDisplay.gameObject.SetActive(false);
        _tileMeshRenderer = GetComponentInChildren<MeshRenderer>();

        if (_tileMeshRenderer != null)
        {
            if (_glowColor == null)
                _glowColor = _tileMeshRenderer.materials[1].GetColor("_GlowColor");
            ToggleGlow(false);
        }
    }

    public void ToggleGlow()
    {
        _isGlowing = !_isGlowing;
        ToggleGlow(_isGlowing);
    }

    public void ToggleGlow(bool active)
    {
        if (active)
            _tileMeshRenderer.materials[1].SetColor("_GlowColor", _glowColor);
        else
            _tileMeshRenderer.materials[1].SetColor("_GlowColor", Color.black);
    }

    public void ToggleDebug(bool visible)
    {
        DebugDisplay.RefreshState(this);
        DebugDisplay.gameObject.SetActive(visible);
    }

    public override string ToString()
    {
        return $"Node: [{Position.x}, {Position.y}]";
    }

    public void ToggleObstacle()
    {
        IsObstacle = !IsObstacle;

        NodeType = IsObstacle ? Type.OBSTACLE : Type.DEFAULT;
    }

    private Quaternion GetTargetPropRotation()
    {
        int index = NodeGrid.Instance.CurrentPath.Count - 2;
        if (index < 0) return Quaternion.LookRotation(NodeGrid.Instance.CurrentStart.transform.position - transform.position, Vector3.up);

        Node adjacent = NodeGrid.Instance.CurrentPath[index];
        return Quaternion.LookRotation(adjacent.transform.position - transform.position, Vector3.up);
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[SelectionBase]
public class Node : MonoBehaviour, IPointerClickHandler
{
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
    public List<Node> Neighbours = new List<Node>();

    /// <summary>
    /// Prevents player from traversing over tile.
    /// </summary>
    public bool IsObstacle = false;

    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        NodeGrid.Instance.SetTarget(this);
    }

    public void ToggleDebug(bool visible)
    {
        NodeDebugDisplay db = GetComponentInChildren<NodeDebugDisplay>(true);
        db.RefreshState();
        db.gameObject.SetActive(visible);
    }

    public override string ToString()
    {
        return $"Node: [{Position.x}, {Position.y}]";
    }
}

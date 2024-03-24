using System.Collections.Generic;
using UnityEngine;

public class NodeDebugDisplay : MonoBehaviour
{
  public enum DebugType { DEFAULT, PATH, OBSTACLE, TARGET }
  [SerializeField] private Node _parent;
  [SerializeField] private MeshRenderer _meshRenderer;
  private DebugType _db;
  private static Dictionary<DebugType, Color> _debugColorDict = new()
  {
    { DebugType.DEFAULT, Color.blue },
    { DebugType.TARGET, Color.magenta },
    { DebugType.PATH, Color.green },
    { DebugType.OBSTACLE, Color.red },
  };

  private void Start()
  {
    _meshRenderer = GetComponent<MeshRenderer>();
  }

  public void RefreshState()
  {
    if (NodeGrid.Instance.CurrentPath.Contains(_parent))
      _db = DebugType.PATH;
    else if (NodeGrid.Instance.CurrentTarget == _parent)
      _db = DebugType.TARGET;
    else if (_parent.IsObstacle)
      _db = DebugType.OBSTACLE;
    else
      _db = DebugType.DEFAULT;

    _meshRenderer.material.color = _debugColorDict[_db];
  }
}
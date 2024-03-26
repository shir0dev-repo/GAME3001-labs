using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeDebugDisplay : MonoBehaviour
{
  [SerializeField] private Image _borderPanel;

  private void Awake()
  {
    GetComponent<Canvas>().worldCamera = Camera.main;
  }

  public void RefreshState(Node parentNode)
  {
    _borderPanel.color = parentNode.NodeType switch
    {
      Node.Type.DEFAULT => Color.grey,
      Node.Type.PATH => Color.green,
      Node.Type.START => Color.cyan,
      Node.Type.TARGET => Color.red,
      Node.Type.OBSTACLE => Color.magenta,
      _ => Color.grey
    };
  }
}
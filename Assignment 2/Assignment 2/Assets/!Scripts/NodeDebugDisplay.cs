using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeDebugDisplay : MonoBehaviour
{
  [SerializeField] private Image _borderPanel;
  [SerializeField] private TextMeshProUGUI _cost, _label;
  private void Awake()
  {
    GetComponent<Canvas>().worldCamera = Camera.main;
  }

  public void RefreshState(Node parentNode)
  {
    _borderPanel.color = parentNode.NodeType switch
    {
      Node.Type.DEFAULT => Color.grey,
      Node.Type.START => Color.green,
      Node.Type.PATH => Color.cyan,
      Node.Type.TARGET => Color.red,
      Node.Type.OBSTACLE => Color.magenta,
      _ => Color.grey
    };

    if (parentNode.NodeType == Node.Type.DEFAULT)
      _label.text = "";
    else
      _label.text = parentNode.NodeType.ToString();

    _cost.text = parentNode.G.ToString("F1");
  }
}
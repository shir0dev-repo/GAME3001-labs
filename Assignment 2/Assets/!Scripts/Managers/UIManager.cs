using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    private const string _DEBUG_STR = "Toggle Debug Mode (H): ";
    private const string _HEURISTIC_STR = "Current Heuristic (Q): ";
    private const string _DEBUG_INSTRUCTION =
    "Reset Map: (R)\n" +
    "Set Starting Tile: (LMB)\n" +
    "Add/Remove Obstacle: (MMB/O)\n" +
    "Set Target Tile: (RMB)";

    private const string _GAME_INSTRUCTION =
    "Move to Target: (M)\n" +
    "Toggle Path Highlight: (F)";

    [SerializeField] private TextMeshProUGUI _debugText, _heuristicText, _pathCostText;
    [SerializeField] private TextMeshProUGUI _instructionText;

    public void UpdateDebugText(bool isDebug)
    {
        _heuristicText.gameObject.SetActive(isDebug);
        if (isDebug)
        {
            _debugText.text = _DEBUG_STR + "<color=\"green\">True";
            _instructionText.text = _DEBUG_INSTRUCTION;
        }
        else
        {
            _debugText.text = _DEBUG_STR + "<color=\"red\">False";
            _instructionText.text = _GAME_INSTRUCTION;
        }
    }
    public void UpdateHeuristicText()
    {
        _heuristicText.text = _HEURISTIC_STR + $"<color=\"yellow\">{NodeGrid.Instance.CurrentHeuristic}";
    }

    public void UpdatePathCost(int cost)
    {
        _pathCostText.text = "Total Cost:\n" + cost.ToString();
    }
}

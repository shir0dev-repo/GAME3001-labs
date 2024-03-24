using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    [SerializeField] private bool _inDebugMode = false;
    [SerializeField] private NodeGrid _nodeGrid;

    private void Start()
    {
        _nodeGrid = NodeGrid.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleDebugMode();
        }
    }

    private void ToggleDebugMode()
    {
        _inDebugMode = !_inDebugMode;

        foreach (Node node in _nodeGrid.Nodes)
        {
            node.ToggleDebug(_inDebugMode);
        }
    }
}
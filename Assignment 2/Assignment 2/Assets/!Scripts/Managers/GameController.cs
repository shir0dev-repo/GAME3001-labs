using UnityEngine;
using Cinemachine;
using System;
using System.Collections.Generic;

public class GameController : Singleton<GameController>
{
    [SerializeField] private bool _inDebugMode = false;
    [SerializeField] private NodeGrid _nodeGrid;
    [SerializeField] private LayerMask _tileLayer;
    [SerializeField] private Actor _player;
    [SerializeField] private Actor _target;

    private List<GameObject> _propSpawningPool = new();
    private Camera _mainCam;
    public bool InDebugMode => _inDebugMode;

    private void Start()
    {
        _nodeGrid = NodeGrid.Instance;
        _mainCam = Camera.main;
        UpdateActors();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // move actor along path
        {
            _player.StartPathing(_nodeGrid.CurrentPath);
        }
        if (Input.GetKeyDown(KeyCode.F)) // find and display path
        {
            _nodeGrid.GeneratePath();
            _nodeGrid.HighlightPath();
        }

        if (Input.GetKeyDown(KeyCode.H)) // toggle debug mode
        {
            ToggleDebugMode();
        }

        if (!_inDebugMode) return;

        if (Input.GetKeyDown(KeyCode.Q)) // swap heuristic
        {
            _nodeGrid.SwapHeuristicType();
            UIManager.Instance.UpdateHeuristicText();
            _nodeGrid.GeneratePath();
        }

        if (Input.GetKeyDown(KeyCode.R)) // reset map and player
        {
            _nodeGrid.GeneratePath(isRandom: true);
            _nodeGrid.RefreshDebugDisplay();
        }
        if (Input.GetKeyDown(KeyCode.O)) // toggle obstacle state
        {
            SetNodeObstacle(DoMouseRaycast(Input.mousePosition));
            UpdateActors();
        }
        if (Input.GetMouseButtonDown(0)) /// LMB - Set Start
        {
            SetNodeStart(DoMouseRaycast(Input.mousePosition));
        }
        else if (Input.GetMouseButtonDown(1)) // RMB - Set Goal
        {
            SetNodeTarget(DoMouseRaycast(Input.mousePosition));
        }
    }

    private void SetNodeStart(RaycastHit rc)
    {
        if (rc.collider == null || !rc.collider.TryGetComponent(out Node start)) return;
        else if (start.IsObstacle) return;
        else if (start == _nodeGrid.CurrentStart || start == _nodeGrid.CurrentTarget) return;

        _nodeGrid.SetStart(start);
        UpdateActors();
        _nodeGrid.RefreshDebugDisplay();
    }

    private void SetNodeTarget(RaycastHit rc)
    {
        if (rc.collider == null || !rc.collider.TryGetComponent(out Node target)) return;
        else if (target.IsObstacle) return;
        else if (target == _nodeGrid.CurrentStart || target == _nodeGrid.CurrentTarget) return;

        _nodeGrid.SetTarget(target);
        UpdateActors();
        _nodeGrid.RefreshDebugDisplay();
    }

    private void SetNodeObstacle(RaycastHit rc)
    {
        if (rc.collider == null || !rc.collider.TryGetComponent(out Node obst)) return;
        else if (obst == _nodeGrid.CurrentStart || obst == _nodeGrid.CurrentTarget) return;

        obst.ToggleObstacle();
        _nodeGrid.GeneratePath();
        UpdateObstaclePool();

        _nodeGrid.RefreshDebugDisplay();
    }

    private void UpdateObstaclePool()
    {
        var obstacleNodes = _nodeGrid.GetObstacleNodes();

        int i;
        for (i = 0; i < obstacleNodes.Count; i++)
        {
            if (i >= _propSpawningPool.Count)
            {
                GameObject newProp = Instantiate(PropManager.Instance.GetRandomObstacle(), transform);
                _propSpawningPool.Add(newProp);
            }

            _propSpawningPool[i].transform.position = obstacleNodes[i].PropPosition;
            _propSpawningPool[i].gameObject.SetActive(true);
        }

        if (i < _propSpawningPool.Count)
        {
            for (int j = i; j < _propSpawningPool.Count; j++)
            {
                _propSpawningPool[j].SetActive(false);
            }
        }
    }

    private void UpdateActors()
    {
        _player.Initialize(_nodeGrid.CurrentStart, 0);
        if (_nodeGrid.CurrentPath.Count > 1)
            _target.Initialize(_nodeGrid.CurrentTarget, _nodeGrid.CurrentPath.Count - 2);
        else
            _target.Initialize(_nodeGrid.CurrentTarget, _nodeGrid.CurrentStart);
    }

    private RaycastHit DoMouseRaycast(Vector3 mousePos)
    {
        Physics.Raycast(_mainCam.ScreenPointToRay(mousePos), out RaycastHit rc, Mathf.Infinity, _tileLayer);
        return rc;
    }

    private void ToggleDebugMode()
    {
        _inDebugMode = !_inDebugMode;
        UIManager.Instance.UpdateDebugText(_inDebugMode);
        CameraController.Instance.SetActiveCamera(_inDebugMode);

        foreach (Node node in _nodeGrid.Nodes)
        {
            node.ToggleDebug(_inDebugMode);
        }
    }
}
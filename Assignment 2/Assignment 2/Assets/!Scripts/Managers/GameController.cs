using UnityEngine;
using Cinemachine;
using System;
using System.Collections.Generic;

public class GameController : Singleton<GameController>
{
    [SerializeField] private bool _inDebugMode = false;
    [SerializeField] private NodeGrid _nodeGrid;
    [SerializeField] private LayerMask _tileLayer;
    [SerializeField] private Player _player;
    [SerializeField] private Chest _target;

    private List<GameObject> _propSpawningPool = new();
    private Camera _mainCam;
    public bool InDebugMode => _inDebugMode;

    private void Start()
    {
        _nodeGrid = NodeGrid.Instance;
        _mainCam = Camera.main;
        UpdateActors();
        UpdateObstaclePool();
        UIManager.Instance.UpdatePathCost(_nodeGrid.CurrentPath.Count);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // toggle debug mode
        {
            ToggleDebugMode();
        }
        if (_player.CurrentlyMoving == false)
        {
            if (Input.GetKeyDown(KeyCode.M)) // move actor along path
            {
                _player.StartPathing(_nodeGrid.CurrentPath);
            }
            if (Input.GetKeyDown(KeyCode.F)) // find and display path
            {
                _nodeGrid.TogglePathHighlight();
                UIManager.Instance.UpdatePathCost(_nodeGrid.CurrentPath.Count);
            }
        }

        if (!_inDebugMode || _player.CurrentlyMoving) return;

        if (Input.GetKeyDown(KeyCode.Q)) // swap heuristic
        {
            _nodeGrid.SwapHeuristicType();
            UIManager.Instance.UpdateHeuristicText();
            _nodeGrid.GeneratePath();
        }

        if (Input.GetKeyDown(KeyCode.R)) // reset map and player
        {
            _nodeGrid.GeneratePath(isRandom: true);
            UpdateActors();
            _nodeGrid.RefreshDebugDisplay();
        }
        if (Input.GetKeyDown(KeyCode.O) || Input.GetMouseButtonDown(2)) // toggle obstacle state
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
        else if (start.IsObstacle || start == _nodeGrid.CurrentStart || start == _nodeGrid.CurrentTarget)
        {
            AudioManager.Instance.PlaySoundEffect("Error");
            return;
        }

        _nodeGrid.SetStart(start);
        UpdateActors();
        _nodeGrid.RefreshDebugDisplay();
        AudioManager.Instance.PlaySoundEffect("Select");
    }

    private void SetNodeTarget(RaycastHit rc)
    {
        if (rc.collider == null || !rc.collider.TryGetComponent(out Node target)) return;
        else if (target.IsObstacle || target == _nodeGrid.CurrentStart || target == _nodeGrid.CurrentTarget)
        {
            AudioManager.Instance.PlaySoundEffect("Error");
            return;
        }

        _nodeGrid.SetTarget(target);
        UpdateActors();
        _nodeGrid.RefreshDebugDisplay();
        AudioManager.Instance.PlaySoundEffect("Select");
    }

    private void SetNodeObstacle(RaycastHit rc)
    {
        if (rc.collider == null || !rc.collider.TryGetComponent(out Node obst)) return;
        else if (obst == _nodeGrid.CurrentStart || obst == _nodeGrid.CurrentTarget)
        {
            AudioManager.Instance.PlaySoundEffect("Error");
            return;
        }

        obst.ToggleObstacle();
        _nodeGrid.GeneratePath();
        UpdateObstaclePool();

        _nodeGrid.RefreshDebugDisplay();
        AudioManager.Instance.PlaySoundEffect("Select");
    }

    public void UpdateObstaclePool()
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
        _target.Close();
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
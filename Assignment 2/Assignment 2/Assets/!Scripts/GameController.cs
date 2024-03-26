using UnityEngine;

public class GameController : Singleton<GameController>
{
    [SerializeField] private bool _inDebugMode = false;
    [SerializeField] private NodeGrid _nodeGrid;
    [SerializeField] private LayerMask _tileLayer;
    private Camera _mainCam;

    private void Start()
    {
        _nodeGrid = NodeGrid.Instance;
        _mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // toggle debug mode
        {
            ToggleDebugMode();
        }
        if (Input.GetKeyDown(KeyCode.F)) // find and display path
        {
            _nodeGrid.GeneratePath();
        }
        if (Input.GetKeyDown(KeyCode.M)) // move actor along path
        {

        }
        if (Input.GetKeyDown(KeyCode.R)) // reset map and player
        {
            _nodeGrid.GeneratePath(isRandom: true);
            _nodeGrid.RefreshDebugDisplay();
        }

        if (_inDebugMode)
        {
            if (Input.GetMouseButtonDown(0)) /// LMB - Set Start
            {
                SetNodeStart(DoMouseRaycast(Input.mousePosition));
            }
            else if (Input.GetMouseButtonDown(1)) // RMB - Set Goal
            {
                SetNodeTarget(DoMouseRaycast(Input.mousePosition));
            }
        }
    }

    private RaycastHit DoMouseRaycast(Vector3 mousePos)
    {
        Debug.Log(Input.mousePosition);
        Physics.Raycast(_mainCam.ScreenPointToRay(mousePos), out RaycastHit rc, Mathf.Infinity, _tileLayer);
        return rc;
    }

    private void SetNodeStart(RaycastHit rc)
    {
        if (rc.collider == null || !rc.collider.TryGetComponent(out Node start))
        {
            Debug.LogWarning("Could not find node from click position!");
            return;
        }
        else if (start.IsObstacle)
        {
            Debug.Log("Selected node is an obstacle!");
            return;
        }

        Debug.Log("Clicked " + start.ToString());
        _nodeGrid.SetStart(start);
        _nodeGrid.RefreshDebugDisplay();
    }

    private void SetNodeTarget(RaycastHit rc)
    {
        if (rc.collider == null || !rc.collider.TryGetComponent(out Node target))
        {
            Debug.LogWarning("Could not find a node from click position!");
            return;
        }
        else if (target.IsObstacle)
        {
            Debug.Log("Selected node is an obstacle!");
            return;
        }

        _nodeGrid.SetTarget(target);
        _nodeGrid.RefreshDebugDisplay();
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
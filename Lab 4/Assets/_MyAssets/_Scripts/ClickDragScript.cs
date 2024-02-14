using UnityEngine;

public class ClickDragScript : MonoBehaviour
{
    [SerializeField] private bool lockToGrid = true;
    private bool isDragging = false;
    private Vector2 offset;
    private Rigidbody2D currentlyDraggedObject;

    void Update()
    {
        StartDrag(Input.GetMouseButtonDown(0));

        HandleDrag();

        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging) return;

            string objTag = currentlyDraggedObject.tag;

            TileStatus draggedObjectStatus = objTag switch
            {
                "Mines" => TileStatus.IMPASSABLE,
                "Ship" => TileStatus.START,
                "Planet" => TileStatus.GOAL,
                _ => TileStatus.UNVISITED
            };

            Vector2 tileIndex = currentlyDraggedObject.gameObject.GetComponent<NavigationObject>().GetGridIndex();
            GridManager.Instance.GetGrid()[(int)tileIndex.y, (int)tileIndex.x].GetComponent<TileScript>().SetStatus(draggedObjectStatus);

            if (draggedObjectStatus == TileStatus.START)
                GridManager.Instance.SetTileCosts(tileIndex);

            foreach (GameObject node in GridManager.Instance.GetGrid())
            {
                TileScript ts = node.GetComponent<TileScript>();

                if (ts.status != TileStatus.IMPASSABLE && ts.status != TileStatus.GOAL && ts.status != TileStatus.START)
                    ts.SetStatus(TileStatus.UNVISITED);
            }

            isDragging = false;
            currentlyDraggedObject = null;
        }        
    }

    private void StartDrag(bool mouseClicked)
    {
        if (!mouseClicked || isDragging) return;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider == null) return;
        if (!hit.collider.TryGetComponent(out Rigidbody2D rb2d)) return;

        isDragging = true;
        currentlyDraggedObject = rb2d;

        if (!currentlyDraggedObject.TryGetComponent(out NavigationObject navObject)) return;

        Vector2 tileIndex = navObject.GetGridIndex();
        GridManager.Instance.GetGrid()[(int)tileIndex.y, (int)tileIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.UNVISITED);
    }
    private void HandleDrag()
    {
        if (!isDragging || currentlyDraggedObject == null) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 movePosition = lockToGrid ? GridManager.Instance.GetGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition)) : mousePosition + offset;

        currentlyDraggedObject.MovePosition(movePosition);

        currentlyDraggedObject.GetComponent<NavigationObject>().SetGridIndex();
    }

}

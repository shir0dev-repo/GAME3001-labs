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

        if (isDragging && currentlyDraggedObject != null)
        {
            if (!lockToGrid) // Note the new selection structure for lock to grid.
            {
                // Move the dragged GameObject normally based on the mouse position.
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentlyDraggedObject.MovePosition(mousePosition + offset);
            }
            else
            {
                Vector2 gridPosition = GridManager.Instance.GetGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                currentlyDraggedObject.MovePosition(gridPosition);
            }

            currentlyDraggedObject.GetComponent<NavigationObject>().SetGridIndex();
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

        if (currentlyDraggedObject.tag is not "Mines" or "Ship" or "Planet") return;

        Vector2 tileIndex = currentlyDraggedObject.gameObject.GetComponent<NavigationObject>().GetGridIndex();
        GridManager.Instance.GetGrid()[(int)tileIndex.y, (int)tileIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.UNVISITED);
    }
}

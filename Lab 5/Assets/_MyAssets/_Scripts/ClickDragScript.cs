using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDragScript : MonoBehaviour
{
    [SerializeField] private bool lockToGrid = true;
    private bool isDragging = false;
    private Vector2 offset;
    private Rigidbody2D currentlyDraggedObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            // Raycast to check if the mouse is over a collider.
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                // Check if the clicked GameObject has a Rigidbody2D.
                Rigidbody2D rb2d = hit.collider.GetComponent<Rigidbody2D>();
                if (rb2d != null)
                {
                    // Start dragging only if no object is currently being dragged.
                    isDragging = true;
                    currentlyDraggedObject = rb2d;
                    offset = rb2d.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (currentlyDraggedObject.gameObject.tag == "Mines" ||
                        currentlyDraggedObject.gameObject.tag == "Ship"  ||
                        currentlyDraggedObject.gameObject.tag == "Planet") 
                    { 
                        Vector2 tileIndex = currentlyDraggedObject.gameObject.GetComponent<NavigationObject>().GetGridIndex();
                        GridManager.Instance.GetGrid()[(int)tileIndex.y, (int)tileIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.UNVISITED);
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging) return; // Early-exit if.
            Vector2 tileIndex = currentlyDraggedObject.gameObject.GetComponent<NavigationObject>().GetGridIndex();
            if (currentlyDraggedObject.gameObject.tag == "Mines") // We've released a mines tile.
            { 
                GridManager.Instance.GetGrid()[(int)tileIndex.y, (int)tileIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.IMPASSABLE);
                GridManager.Instance.ConnectGrid();
            }
            else if (currentlyDraggedObject.gameObject.tag == "Ship") // We've released a ship tile.
            {
                GridManager.Instance.GetGrid()[(int)tileIndex.y, (int)tileIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.START);
            }
            else if (currentlyDraggedObject.gameObject.tag == "Planet") // We've released a planet tile.
            {
                GridManager.Instance.SetTileCosts(currentlyDraggedObject.GetComponent<NavigationObject>().GetGridIndex());
                GridManager.Instance.GetGrid()[(int)tileIndex.y, (int)tileIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.GOAL);
            }
            // Stop dragging.
            isDragging = false;
            currentlyDraggedObject = null;
        }
        if (isDragging && currentlyDraggedObject != null)
        {
            if (!lockToGrid)
            {
                // Move the dragged GameObject normally based on the mouse position.
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentlyDraggedObject.MovePosition(mousePosition + offset);
            }
            else
            {
                // Move the dragged GameObject and lock it to a grid position.
                Vector2 gridPosition = GridManager.Instance.GetGridPosition( Camera.main.ScreenToWorldPoint(Input.mousePosition) );
                currentlyDraggedObject.MovePosition(gridPosition);
            }
            currentlyDraggedObject.GetComponent<NavigationObject>().SetGridIndex();
        }
    }
}

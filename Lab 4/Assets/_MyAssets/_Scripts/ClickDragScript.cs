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
                    // Add extra behaviour for mines in Lab 4 part 1.
                    //
                    //
                    //
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Add extra behaviour for mines in Lab 4 part 1.
            //
            //
            //
            // Stop dragging.
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
                // Move the dragged GameObject and lock it to a grid position.
                // Add extra behaviour for lock to grid in Lab 4 part 1.
                //
                //
                //
            }
            // Uncomment the below line for Lab 4 part 1.
            //
            // currentlyDraggedObject.GetComponent<NavigationObject>().SetGridIndex();
        }
    }
}

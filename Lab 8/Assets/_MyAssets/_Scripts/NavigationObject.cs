using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationObject : MonoBehaviour
{
    public Vector2 gridIndex;
    void Awake()
    {
        gridIndex = new Vector2();
        SetGridIndex();
    }

    public Vector2 GetGridIndex()
    {
        return gridIndex;
    }

    public void SetGridIndex() 
    {
        float originalX = Mathf.Floor(transform.position.x) + 0.5f;
        gridIndex.x = (int)Mathf.Floor((originalX + 7.5f) / 15 * 15);
        float originalY = Mathf.Floor(transform.position.y) + 0.5f;
        gridIndex.y = 11 - (int)Mathf.Floor(originalY + 5.5f);
    }

    public bool HasLOS(GameObject source, string targetTag, Vector2 whiskerDirection, float whiskerLength)
    {
        // Set the layer of the source to Ignore Linecast.
        source.layer = 3;

        // Create the layermask for the ship.
        int layerMask = ~(1 << LayerMask.NameToLayer("Ignore Linecast"));

        // Cast a ray in the whisker direction.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, whiskerDirection, whiskerLength, layerMask);

        // Reset the source's layer.
        source.layer = 0;

        if (hit.collider != null && hit.collider.CompareTag(targetTag))
        {
            return true;
        }
        return false;
    }
}

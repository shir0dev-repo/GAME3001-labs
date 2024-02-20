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

    public void SetGridIndex() // TODO: replace ugly real numbers.
    {
        float originalX = Mathf.Floor(transform.position.x) + 0.5f;
        gridIndex.x = (int)Mathf.Floor((originalX + 7.5f) / 15 * 15);
        float originalY = Mathf.Floor(transform.position.y) + 0.5f;
        gridIndex.y = 11 - (int)Mathf.Floor(originalY + 5.5f);
    }
}

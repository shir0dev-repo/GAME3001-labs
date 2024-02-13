using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationObject : MonoBehaviour
{
    public Vector2 GridIndex;
    private void Awake()
    {
        GridIndex = new Vector2();
        SetGridIndex();
    }
    public Vector2 GetGridIndex()
    {
        return GridIndex;
    }

    public void SetGridIndex()
    {
        float originalX = Mathf.Floor(transform.position.x) + 0.5f;
        GridIndex.x = (int)Mathf.Floor((originalX + 7.5f) / 15 * 15);
        float originalY = Mathf.Floor(transform.position.y) + 0.5f;
        GridIndex.y = 11 - (int)Mathf.Floor(originalY + 5.5f);
    }
}

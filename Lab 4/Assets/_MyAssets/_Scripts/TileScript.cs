using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    [SerializeField] private GameObject[] neighbourTiles;
    [SerializeField] private Color original;
    public TileStatus status = TileStatus.UNVISITED;
    
    public void SetNeighbourTile(int index, GameObject tile)
    {
        neighbourTiles[index] = tile;
    }

    public void SetColor(Color color, bool newColor = false)
    {
        if (!newColor)
            original = color;
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    public void ToggleImpassable(bool impass = true)
    {
        if (impass)
        {
            status = TileStatus.IMPASSABLE;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0f, 0f, 0.5f);
        }
        else
        {
            status = TileStatus.UNVISITED;
            gameObject.GetComponent<SpriteRenderer>().color = original;
        }
    }
}

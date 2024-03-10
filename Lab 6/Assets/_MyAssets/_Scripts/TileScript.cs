using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    [SerializeField] private GameObject[] neighbourTiles; // Top, Right, Bottom, Left.
    [SerializeField] private Color original;
    public TilePanelScript tilePanel; // Used for UI only.
    public TileStatus status = TileStatus.UNVISITED;
    public float cost = 1f;
    // New PathNode for pathfinding.
    public PathNode Node { get; set; }
    
    // New for Lab 5.
    public void ResetNeighbourConnections()
    {
        neighbourTiles = new GameObject[4];
        Node.connections.Clear();
    }

    public void SetNeighbourTile(int index, GameObject tile)
    {
        neighbourTiles[index] = tile;
    }

    // New GetNeighbourTileNode method for PathConnection.
    public PathNode GetNeighbourTileNode(int index)
    {
        return neighbourTiles[index].GetComponent<TileScript>().Node;
    }

    public void SetColor(Color color, bool newColor = false)
    {
        if (!newColor)
            original = color;
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    // Removed ToggleImpassable.

    internal void SetStatus(TileStatus stat)
    {
        status = stat;
        switch (stat)
        {
            case TileStatus.UNVISITED:
                gameObject.GetComponent<SpriteRenderer>().color = original;
                tilePanel.statusText.text = "U";
                break;
            case TileStatus.OPEN:
                gameObject.GetComponent<SpriteRenderer>().color = original;
                tilePanel.statusText.text = "O";
                break;
            case TileStatus.CLOSED:
                gameObject.GetComponent<SpriteRenderer>().color = original;
                tilePanel.statusText.text = "C";
                break;
            case TileStatus.IMPASSABLE:
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0f, 0f, 0.5f);
                tilePanel.statusText.text = "I";
                break;
            case TileStatus.GOAL:
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0f, 0.5f);
                tilePanel.statusText.text = "G";
                break;
            case TileStatus.START:
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0f, 0.5f, 0f, 0.5f);
                tilePanel.statusText.text = "S";
                break;
            // Added new case for Lab 5.
            case TileStatus.PATH:
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
                tilePanel.statusText.text = "P";
                break;
        }
    }
}

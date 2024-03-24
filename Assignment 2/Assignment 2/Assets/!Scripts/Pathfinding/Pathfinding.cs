using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding
{

  /// <summary>
  /// Uses the A-Star algorithm to find the shortest path (Manhattan-based) between two nodes.
  /// </summary>
  /// <param name="startingNode">The node to start the search from.</param>
  /// <param name="targetNode">The target node to reach.</param>
  /// <returns>A list of nodes containing the shortest path between the start and end nodes.</returns>
  public static List<Node> PathTo(Node startingNode, Node targetNode, bool useManhattan)
  {

    float getTileCost(Vector2 start, Vector2 end)
      => useManhattan ? GetTileCostManhattan(start, end) : GetTileCostEuclidean(start, end);

    List<Node> openPathNodes = new();
    List<Node> closedPathNodes = new();

    Node currentNode = startingNode;

    currentNode.G = 0;
    currentNode.H = getTileCost(currentNode.Position, targetNode.Position);

    openPathNodes.Add(currentNode);

    while (openPathNodes.Count > 0)
    {
      openPathNodes = openPathNodes.OrderBy(node => node.F).ToList(); // sort list by lowest total cost
      currentNode = openPathNodes[0];

      openPathNodes.Remove(currentNode);
      closedPathNodes.Add(currentNode);

      float g = currentNode.G + 1;

      if (closedPathNodes.Contains(targetNode))
        break;

      foreach (Node neighbour in currentNode.Neighbours)
      {
        if (neighbour.IsObstacle) continue;
        if (closedPathNodes.Contains(neighbour)) continue;

        if (!openPathNodes.Contains(neighbour))
        {
          neighbour.G = g;
          neighbour.H = getTileCost(neighbour.Position, targetNode.Position);
          openPathNodes.Add(neighbour);
        }
        else if (neighbour.F > g + neighbour.H)
          neighbour.G = g;
        else if (neighbour.F == currentNode.F)
        {
          neighbour.H += BreakTie(startingNode.Position, currentNode.Position, targetNode.Position);
        }
      }
    }

    List<Node> finalPath = new();

    if (closedPathNodes.Contains(targetNode))
    {
      currentNode = targetNode;

      while (currentNode != null && currentNode != startingNode)
      {
        finalPath.Add(currentNode);
        closedPathNodes.Remove(currentNode);

        currentNode = closedPathNodes.Find(n => n.G < currentNode.G && currentNode.Neighbours.Contains(n));
      }

      finalPath.Reverse();
    }
    return finalPath;
  }

  private static float BreakTie(Vector2 startPos, Vector2 currentPos, Vector2 targetPos)
  {
    float dx1 = currentPos.x - targetPos.x;
    float dy1 = currentPos.y - targetPos.y;
    float dx2 = startPos.x - targetPos.x;
    float dy2 = startPos.y - targetPos.y;
    return Mathf.Abs(dx1 * dy2 - dx2 * dy1) * 0.001f;
  }

  /// <summary>
  /// Helper function for getting the cost of an adjacent tile.
  /// </summary>
  /// <returns>The Manhattan distance between the start and end positions.</returns>
  public static float GetTileCostManhattan(Vector2 startPos, Vector2 targetPos)
  {
    float xCost = Mathf.Abs(startPos.x - targetPos.x);
    float yCost = Mathf.Abs(startPos.y - targetPos.y);
    return xCost + yCost;
  }



  public static float GetTileCostEuclidean(Vector2 startPos, Vector2 targetPos)
  {
    float x = Mathf.Pow(targetPos.x - startPos.x, 2f);
    float y = Mathf.Pow(targetPos.y - startPos.y, 2f);

    return Mathf.Sqrt(x + y);
  }
}
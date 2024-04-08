using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding
{
  public enum Heuristic { Manhattan, Euclidean }

  private delegate float costDelegate(Vector2 start, Vector2 end);

  /// <summary>
  /// Uses the A-Star algorithm to find the shortest path between two nodes.
  /// </summary>
  /// <param name="startingNode">The node to start the search from.</param>
  /// <param name="targetNode">The target node to reach.</param>
  /// <param name="heuristic">The type of heuristic to use for the algorithm.</param>
  /// <returns>A list of nodes containing the shortest path between the start and end nodes.</returns>
  public static List<Node> GetPath(Node startingNode, Node targetNode, Heuristic heuristic)
  {

    costDelegate getTileCost = heuristic switch
    {
      Heuristic.Manhattan => GetTileCostManhattan,
      Heuristic.Euclidean => GetTileCostEuclidean,
      _ => (_, _) => { return 0; }
    };

    List<Node> openPathNodes = new();
    List<Node> closedPathNodes = new();

    Node currentNode = startingNode;
    currentNode.NodeType = Node.Type.START;

    currentNode.G = 0;
    currentNode.H = getTileCost(currentNode.Position, targetNode.Position);

    targetNode.NodeType = Node.Type.TARGET;

    openPathNodes.Add(currentNode);

    while (openPathNodes.Count > 0)
    {
      openPathNodes = openPathNodes
        .OrderBy(node => node.F)
        .ToList();
      // sort list by lowest total cost, then by distance from start, then by distance from target
      // sorting this way ensures a strict preference to the lowest G value, meaning the algorithm will prefer paths with
      // the least distance from the starting node

      currentNode = openPathNodes[0];

      openPathNodes.Remove(currentNode);
      closedPathNodes.Add(currentNode);

      // the distance from start that the following nodes will have
      float g = currentNode.G + 1;

      if (closedPathNodes.Contains(targetNode))
        break;

      foreach (Node neighbour in currentNode.Neighbours)
      {
        if (neighbour.IsObstacle)
        {
          neighbour.NodeType = Node.Type.OBSTACLE;
          continue;
        }

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
        if (currentNode.Position != targetNode.Position)
          currentNode.NodeType = Node.Type.PATH;

        closedPathNodes.Remove(currentNode);

        foreach (Node n in closedPathNodes)
        {
          if (n.G <= currentNode.G && currentNode.Neighbours.Contains(n))
          {
            currentNode = n;
            continue;
          }
        }
      }

      finalPath.Reverse();
    }
    return finalPath;
  }

  /// <summary>
  ///
  /// </summary>
  /// <remarks></remarks>
  /// <returns></returns>
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
using UnityEngine;

public enum Direction { Left, Right, Up, Down }

public static class DirectionUtils
{
  public static Vector2Int GetVector(this Direction direction)
  {
    return direction switch
    {
      Direction.Left => Vector2Int.left,
      Direction.Right => Vector2Int.right,
      Direction.Up => Vector2Int.up,
      Direction.Down => Vector2Int.down,
      _ => Vector2Int.zero
    };
  }
}
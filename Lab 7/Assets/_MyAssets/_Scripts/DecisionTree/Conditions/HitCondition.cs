using UnityEngine;

public class HitCondition : ConditionNode
{
  public bool IsHit { get; set; }
  public HitCondition()
  {
    name = "Hit Condition";
    IsHit = false;
  }
  public override bool Condition()
  {
    Debug.Log("Checking " + name);
    return IsHit;
  }
}
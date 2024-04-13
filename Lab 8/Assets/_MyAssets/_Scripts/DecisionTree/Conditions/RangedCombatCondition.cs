

using UnityEngine;

public class RangedCombatCondition : ConditionNode
{
  public bool IsWithinCombatRange { get; set; }
  public RangedCombatCondition()
  {
    name = "Ranged Combat Condition";
    IsWithinCombatRange = false;
  }
  public override bool Condition()
  {
    Debug.Log("Checking " + name);
    return IsWithinCombatRange;
  }
}

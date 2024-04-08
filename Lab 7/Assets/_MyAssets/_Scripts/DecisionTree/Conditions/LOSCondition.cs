using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Fill in for Lab 7a.
public class LOSCondition : ConditionNode
{
  public bool HasLOS { get; set; }
  public LOSCondition()
  {
    name = "LOS Condition";
    HasLOS = false;
  }

  public override bool Condition()
  {
    Debug.Log("Checking " + name);
    return HasLOS;
  }
}

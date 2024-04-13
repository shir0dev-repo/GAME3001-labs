using UnityEngine;

public class HealthCondition : ConditionNode
{
  public bool IsHealthy { get; set; }
  public HealthCondition()
  {
    name = "Health Condition";
    IsHealthy = true;
  }
  public override bool Condition()
  {
    Debug.Log("Checking " + name);
    return IsHealthy;
  }
}
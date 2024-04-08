using UnityEngine;

public class FleeAction : ActionNode
{
  public FleeAction()
  {
    name = "Flee Action";
  }
  public override void Action()
  {
    if (Agent.GetComponent<AgentObject>().state != ActionState.FLEE)
    {
      Debug.Log("Starting " + name);
      AgentObject ao = Agent.GetComponent<AgentObject>();
      ao.state = ActionState.FLEE;


      if (AgentScript is RangedCombatEnemy rce)
      {

      }
    }
  }
}
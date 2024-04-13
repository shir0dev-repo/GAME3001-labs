using UnityEngine;

public class MoveToRangeAction : ActionNode
{
  public MoveToRangeAction()
  {
    name = "Move to Range Action";
  }

  public override void Action()
  {
    if (Agent.GetComponent<AgentObject>().state != ActionState.MOVE_TO_RANGE)
    {
      Debug.Log("Starting " + name);
      AgentObject ao = Agent.GetComponent<AgentObject>();
      ao.state = ActionState.MOVE_TO_RANGE;


      if (AgentScript is RangedCombatEnemy rce)
      {

      }
    }
  }
}
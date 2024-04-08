using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Fill in for Lab 7a.
public class MoveToPlayerAction : ActionNode
{
  public MoveToPlayerAction()
  {
    name = "Move to Player Action";
  }
  public override void Action()
  {
    if (Agent.GetComponent<AgentObject>().state != ActionState.MOVE_TO_PLAYER)
    {
      Debug.Log("Starting " + name);
      AgentObject ao = Agent.GetComponent<AgentObject>();
      ao.state = ActionState.MOVE_TO_PLAYER;

      if (AgentScript is CloseCombatEnemy cce)
      {

      }
    }
  }
}

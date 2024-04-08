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
    if (Agent.GetComponent<Starship>().state != ActionState.MOVE_TO_PLAYER)
    {
      Debug.Log("Starting " + name);
      Starship ss = Agent.GetComponent<Starship>();
      ss.state = ActionState.MOVE_TO_PLAYER;
    }
  }
}

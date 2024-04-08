using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Fill in for Lab 7a.
public class MoveToLOSAction : ActionNode
{
  public MoveToLOSAction()
  {
    name = "Move to LOS Action";
  }
  public override void Action()
  {
    if (Agent.GetComponent<Starship>().state != ActionState.MOVE_TO_LOS)
    {
      Debug.Log("Starting " + name);
      Starship ss = Agent.GetComponent<Starship>();
      ss.state = ActionState.MOVE_TO_LOS;
    }
  }
}

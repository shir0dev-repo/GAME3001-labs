using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Fill in for Lab 7a.
public class PatrolAction : ActionNode
{
  public PatrolAction()
  {
    name = "Patrol Action";
  }
  public override void Action()
  {
    if (Agent.GetComponent<Starship>().state != ActionState.PATROL)
    {
      Debug.Log("Starting " + name);
      Starship ss = Agent.GetComponent<Starship>();
      ss.state = ActionState.PATROL;

      ss.StartPatrol();
    }
  }
}

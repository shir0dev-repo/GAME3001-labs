using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionNode : TreeNode
{
    public GameObject Agent { get; set; }
    public Component AgentScript { get; set; }
    public ActionNode()
    {
        isLeaf = true;
        Agent = null;
        AgentScript = null;
    }
    public void SetAgent(GameObject agent, System.Type agentScript)
    {
        Agent = agent;
        AgentScript = agent.GetComponent(agentScript);
    }
    public abstract void Action(); // Abstract method for tree.
}

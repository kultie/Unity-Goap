using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kultie.Goap;
public class TestGoap : MonoBehaviour,IGoap {

    GoapPlanner planner;
    GoapAgent agent;

	private void Start()
	{      
        agent = new GoapAgent(new List<GoapAction>(), new StateMachine(new Dictionary<string, SMState>()), this, gameObject);
	}

	public void ActionsFinished()
    {
        throw new System.NotImplementedException();
    }

    public Dictionary<string, object> GetGoalState()
    {
        Dictionary<string, object> goalData = new Dictionary<string, object>();
        goalData.Add("value1", false);
        goalData.Add("value2", false);
        return goalData;
    }

    public Dictionary<string, object> GetWorldState()
    {
        Dictionary<string, object> worldData = new Dictionary<string, object>();
        worldData.Add("value1", true);
        worldData.Add("value2", true);
        return worldData;
    }

    public bool PerformNext(GoapAction nextAction)
    {
        throw new System.NotImplementedException();
    }

    public void PlanAborted(GoapAction action)
    {
        throw new System.NotImplementedException();
    }

    public void PlanFailed(Dictionary<string, object> failedGoal)
    {
        throw new System.NotImplementedException();
    }

    public void PlanFound(Dictionary<string, object> goal, Queue<GoapAction> actions)
    {
        throw new System.NotImplementedException();
    }

	private void Update()
	{
        agent.Update(Time.deltaTime);
	}
}

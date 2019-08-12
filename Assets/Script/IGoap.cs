using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kultie.Goap
{
    public interface IGoap
    {
        Dictionary<string, object> GetWorldState();

        Dictionary<string, object> GetGoalState();

        void PlanFailed(Dictionary<string, object> failedGoal);

        void PlanFound(Dictionary<string, object> goal, Queue<GoapAction> actions);

        void ActionsFinished();

        void PlanAborted(GoapAction action);

        bool PerformNext(GoapAction nextAction);
    }
}


using System;
using System.Collections.Generic;
using UnityEngine;
namespace Kultie.Goap
{
    public class GoapAgent
    {
        private StateMachine stateMachine;

        private List<GoapAction> availableActions;
        private Queue<GoapAction> currentActions;

        private IGoap goapData;

        private GoapPlanner planner;

        GameObject _agent;
        public GameObject agent{
            get{
                return _agent;
            }
        }

        public GoapAgent(List<GoapAction> _availableActions, StateMachine _stateMachine, IGoap _goapData, GameObject __agent)
        {
            availableActions = _availableActions;
            stateMachine = _stateMachine;
            goapData = _goapData;
            _agent = __agent;
            planner = new GoapPlanner();
            stateMachine.states.Add("perform_action_state", new PerformActionState(HasPlan, goapData, stateMachine, currentActions, agent));
            stateMachine.states.Add("idle", new IdleState(goapData,stateMachine,availableActions,planner,this));
            stateMachine.Change("idle");
        }

        public void AddAction(GoapAction action)
        {
            availableActions.Add(action);
        }

        public void RemoveAction(GoapAction action)
        {
            availableActions.Remove(action);
        }

        public void Update(float dt)
        {
            stateMachine.Update(dt);
        }

        private bool HasPlan()
        {
            return currentActions.Count > 0;
        }

        public void SetCurrentAction(Queue<GoapAction> actions)
        {
            currentActions = actions;
        }

        private class PerformActionState : SMState
        {
            BoolDelegate hasActionPlan;
            IGoap goapData;
            StateMachine controller;
            Queue<GoapAction> currentActions;
            GameObject agent;

            public PerformActionState(BoolDelegate _hasActionPlan, IGoap _goapData, StateMachine _controller, Queue<GoapAction> _currentActions, GameObject _agent)
            {
                currentActions = _currentActions;
                hasActionPlan = _hasActionPlan;
                goapData = _goapData;
                controller = _controller;
                agent = _agent;
            }

            public override void Update(float dt)
            {

                if (!hasActionPlan())
                {
                    Debug.Log("<color=red>Done actions</color>");
                    controller.Change("idle");
                    goapData.ActionsFinished();
                    return;
                }

                GoapAction action = currentActions.Peek();
                if (!action.IsDone())
                {
                    bool success = action.PerformAction(agent);
                    if (!success)
                    {
                        controller.Change("idle");
                        goapData.ActionsFinished();
                    }
                }
                if (action.IsDone())
                {
                    currentActions.Dequeue();
                }
            }
        }

        private class IdleState : SMState
        {
            Dictionary<string, object> worldState;
            Dictionary<string, object> goal;
            IGoap goapData;
            StateMachine controller;
            GoapPlanner planner;
            GoapAgent agent;
            List<GoapAction> availableActions;

            public IdleState(IGoap _goapData, StateMachine _controller, List<GoapAction> _availableActions,GoapPlanner _planner, GoapAgent _agent)
            {
                goapData = _goapData;
                worldState = goapData.GetWorldState();
                goal = goapData.GetGoalState();
                controller = _controller;
                planner = _planner;
                agent = _agent;
                availableActions = _availableActions;
            }

            public override void Update(float dt)
            {

                Queue<GoapAction> plan = planner.Plan(agent, availableActions, worldState, goal);
                if (plan != null)
                {
                    agent.SetCurrentAction(plan);
                    controller.Change("perform_action_state");
                }
                else{
                    Debug.Log("<color=orange>Failed Plan:</color>");
                    goapData.PlanFailed(goal);
                    controller.Change("idle");
                }
            }
        }
    }
}


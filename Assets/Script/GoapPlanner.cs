using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kultie.Goap
{
    public class GoapPlanner
    {
        public Queue<GoapAction> Plan(GoapAgent agent, List<GoapAction> availableActions, Dictionary<string, object> worldState, Dictionary<string, object> goal)
        {
            foreach (GoapAction action in availableActions)
            {
                action.ForceReset();
            }

            List<GoapAction> usableActions = new List<GoapAction>();
            foreach (GoapAction act in availableActions)
            {
                if (act.CheckPrecondition(agent.agent))
                {
                    usableActions.Add(act);
                }
            }

            List<Node> leaves = new List<Node>();

            // build graph
            Node start = new Node(null, 0, worldState, null);
            bool success = BuildGraph(start, leaves, usableActions, goal);

            if (!success)
            {
                // oh no, we didn't get a plan
                Debug.Log("NO PLAN");
                return null;
            }

            // get the cheapest leaf
            Node cheapest = null;
            foreach (Node leaf in leaves)
            {
                if (cheapest == null)
                    cheapest = leaf;
                else
                {
                    if (leaf.runningCost < cheapest.runningCost)
                        cheapest = leaf;
                }
            }

            // get its node and work back through the parents
            List<GoapAction> result = new List<GoapAction>();
            Node n = cheapest;
            while (n != null)
            {
                if (n.action != null)
                {
                    result.Insert(0, n.action); // insert the action in the front
                }
                n = n.parent;
            }
            // we now have this action list in correct order

            Queue<GoapAction> queue = new Queue<GoapAction>();
            foreach (GoapAction a in result)
            {
                queue.Enqueue(a);
            }

            // hooray we have a plan!
            return queue;
        }

        private bool BuildGraph(Node parent, List<Node> leaves, List<GoapAction> usableActions, Dictionary<string, object> goal)
        {
            bool foundOne = false;

            // go through each action available at this node and see if we can use it here
            foreach (GoapAction action in usableActions)
            {

                // if the parent state has the conditions for this action's preconditions, we can use it here
                if (InState(action.Preconditions, parent.state))
                {

                    // apply the action's effects to the parent state
                    Dictionary<string, object> currentState = populateState(parent.state, action.Effects);
                    //Debug.Log(GoapAgent.prettyPrint(currentState));
                    Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);

                    if (InState(goal, currentState))
                    {
                        // we found a solution!
                        leaves.Add(node);
                        foundOne = true;
                    }
                    else
                    {
                        // not at a solution yet, so test all the remaining actions and branch out the tree
                        List<GoapAction> subset = ActionSubset(usableActions, action);
                        bool found = BuildGraph(node, leaves, subset, goal);
                        if (found)
                            foundOne = true;
                    }
                }
            }

            return foundOne;
        }

        private List<GoapAction> ActionSubset(List<GoapAction> actions, GoapAction removeMe)
        {
            List<GoapAction> subset = new List<GoapAction>();
            foreach (GoapAction a in actions)
            {
                if (!a.Equals(removeMe))
                    subset.Add(a);
            }
            return subset;
        }

        private bool InState(Dictionary<string, object> test, Dictionary<string, object> state)
        {
            bool allMatch = true;
            foreach (KeyValuePair<string, object> t in test)
            {
                bool match = false;
                foreach (KeyValuePair<string, object> s in state)
                {
                    if (s.Equals(t))
                    {
                        match = true;
                        break;
                    }
                }
                if (!match)
                    allMatch = false;
            }
            return allMatch;
        }

        private Dictionary<string, object> populateState(Dictionary<string, object> currentState, Dictionary<string, object> stateChange)
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            // copy the KVPs over as new objects
            foreach (KeyValuePair<string, object> s in currentState)
            {
                state[s.Key] = s.Value;
            }

            foreach (KeyValuePair<string, object> change in stateChange)
            {
                if (state.ContainsKey(change.Key))
                {
                    state.Remove(change.Key);
                    KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, change.Value);
                    state[updated.Key] = updated.Value;
                }
                else
                {
                    state[change.Key] = change.Value;
                }
            }
            return state;
        }


        private class Node
        {
            public Node parent;
            public float runningCost;
            public Dictionary<string, object> state;
            public GoapAction action;

            public Node(Node parent, float runningCost, Dictionary<string, object> state, GoapAction action)
            {
                this.parent = parent;
                this.runningCost = runningCost;
                this.state = state;
                this.action = action;
            }
        }
    }

}


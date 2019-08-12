using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public Dictionary<string, SMState> states;
    public SMState currentState;
    public StateMachine(Dictionary<string, SMState> _states)
    {
        states = _states;
        currentState = null;
    }

    public void Change(string _stateName)
    {
        if (!states.ContainsKey(_stateName))
        {
            return;
        }
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = states[_stateName];
        currentState.Enter();
    }

    public void Update(float dt)
    {
        currentState.Update(dt);
    }

}

public class SMState
{
    //public GameState gameState;
    public virtual void Update(float dt) { }
    public virtual void Enter() { }
    public virtual bool IsFinished() { return false; }
    public virtual void Exit() { }
}

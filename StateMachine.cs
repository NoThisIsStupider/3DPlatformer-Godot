using System;
using Godot;

public class StateMachine : Node
{
    // No idea what type these should be yet, might even work to leave them as object so you can mix different styles 
    // of state machine (eg one thing uses a dictionary to store states, other thing uses enums)
    public object currentState = null; 
    public object previousState = null; 

    public override void _PhysicsProcess(float delta)
    {
        if (currentState != null)
        {
            _StateLogic(delta);
            object transition = _GetTransition(delta);
            if (transition != null)
            {
                SetState(transition);
            }
        }
    }

    public virtual void _StateLogic(float delta) {}

    public virtual object _GetTransition(float delta) {return null;}

    public virtual void _EnterState(object newState, object oldState) {}

    public virtual void _ExitState(object oldState, object newState) {}

    public void SetState(object newState)
    {
        if (currentState != null)
        {
            _ExitState(currentState, newState);
        }

        previousState = currentState;
        currentState = newState;
        
        if (currentState != null) //seems redundant but both if statements are required regardless of placement, since they involve different values
        {
            _EnterState(currentState, previousState);
        }
    }

    public bool CheckForState(params object[] states)
    {
        foreach (object state in states)
        {
            if (state.Equals(currentState))
            {
                return true;
            }
        }
        return false;
    }
}
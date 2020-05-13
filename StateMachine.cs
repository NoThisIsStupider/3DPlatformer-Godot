using System;

class StateMachine
{
    // No idea what type these should be yet, might even work to leave them as object so you can mix different styles 
    // of state machine (eg one thing uses a dictionary to store states, other thing uses enums)
    object currentState = null; 
    object previousState = null; 

    public virtual void _StateLogic(float delta) {}

    public virtual void _GetTransition(float delta) {}

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
}
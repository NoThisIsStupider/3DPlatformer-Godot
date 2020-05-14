using System;
using Godot.Collections;
using Godot;

public class PlayerFSM : StateMachine
{
    public Player player = null;
    bool doGroundSnap = false;

    enum States
    {
        Stand,
        Walk,
        Jump
    }

    public override void _Ready()
    {
        player = (Player)GetParent();
        currentState = States.Stand;
    }

    public override void _StateLogic(float delta)
    {
        player.CalculateNormalizedMoveInput();
        player.CalculateMoveInputRelativeToYRot();
        player.ApplyGravity();

        //https://github.com/godotengine/godot/issues/34117
        //Not super clean but basically this ensures the velocity is actually 0 when slow enough so slope stopping works
        if (Mathf.Abs(player.velocity.x) < 1)
        {
            player.velocity.x = 0;
        }
        if (Mathf.Abs(player.velocity.z) < 1)
        {
            player.velocity.z = 0;
        }
        //keeping the y velocity above 0.5f on the ground makes it so the speed on slopes feels uniform instead of you accelerating downhill
        if (CheckForState(States.Stand, States.Walk) && player.velocity.y < -0.5f)
        {
            player.velocity.y = -0.5f;
        }

        //apply horizontal velocity using lerp to basically simulate friction and such
        player.velocity.x = Mathf.Lerp(player.velocity.x, player.moveInputYRotated.x * player.MOVE_SPEED, 0.2f);
        player.velocity.z = Mathf.Lerp(player.velocity.z, player.moveInputYRotated.z * player.MOVE_SPEED, 0.2f);

        //re-enable ground snapping when moving down, so that when you fall after jumping ground snapping will be enabled when you land
        if (player.velocity.y < 0)
        {
            doGroundSnap = true;
        }

        
        if (CheckForState(States.Stand, States.Walk))
        {
            player.velocity = player.MoveAndSlideWithSnap(player.velocity, (doGroundSnap) ? Vector3.Down / 5 : Vector3.Zero, Vector3.Up, true);

            if (Input.IsActionJustPressed("ui_select"))
            {
                player.velocity.y = 20;
                doGroundSnap = false;
            }
        }
        else if ((States)currentState == States.Jump)
        {
            player.velocity = player.MoveAndSlide(player.velocity, Vector3.Up, true);
        }
    }

    public override object _GetTransition(float delta)
    {
        switch ((States)currentState)
        {
            case States.Stand:
                if (!player.IsOnFloor())
                {
                    return States.Jump;
                }
                else if (player.velocity.x != 0 || player.velocity.z != 0)
                {
                    return States.Walk;
                }
                break;
            case States.Walk:
                if (!player.IsOnFloor())
                {
                    return States.Jump;
                }
                else if (player.velocity.x == 0 || player.velocity.z == 0)
                {
                    return States.Stand;
                }
                break;
            case States.Jump:
                if (player.IsOnFloor())
                {
                    return States.Stand;
                }
                break;
        }
        return null; //no transition was hit, so stay in the same state
    }

    public override void _EnterState(object newState, object oldState)
    {
        switch ((States)currentState)
        {
        }
    }

    public override void _ExitState(object oldState, object newState)
    {
        switch ((States)currentState)
        {
        }
    }
}
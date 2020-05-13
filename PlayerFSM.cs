using System;
using Godot.Collections;
using Godot;

public class PlayerFSM : StateMachine
{
    public Player player = null;
    bool disableGroundSnap = false;

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

        player.velocity = new Vector3(player.moveInputYRotated.x * 15, player.velocity.y, player.moveInputYRotated.z * 15);
        if (player.velocity.y < 0)
        {
            disableGroundSnap = false;
        }

        if (CheckForState(States.Stand, States.Walk))
        {
            player.velocity = player.MoveAndSlideWithSnap(player.velocity, (!disableGroundSnap) ? Vector3.Down / 5 : Vector3.Zero, Vector3.Up, true);

            if (Input.IsActionJustPressed("ui_select"))
            {
                player.velocity.y = 20;
                disableGroundSnap = true;
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
        switch ((States) currentState)
        {
        }
    }

    public override void _ExitState(object oldState, object newState)
    {
        switch ((States) currentState)
        {
        }
    }
}
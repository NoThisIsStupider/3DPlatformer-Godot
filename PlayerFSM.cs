using System;
using Godot.Collections;
using Godot;

//TODO: Make the player not use RayShapes when in the air (to fix some jank) 
//TODO: Incorporate the raycast that checks if the player only slightly left the floor (will be especially important for the above)

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

        //apply horizontal velocity using lerp to basically simulate friction and such
        player.velocity.x = Mathf.Lerp(player.velocity.x, player.moveInputYRotated.x * player.MOVE_SPEED, 0.2f);
        player.velocity.z = Mathf.Lerp(player.velocity.z, player.moveInputYRotated.z * player.MOVE_SPEED, 0.2f);

        EnsureSlopeStop();

        //re-enable ground snapping when moving down, so that when you fall after jumping ground snapping will be enabled when you land
        if (player.velocity.y < 0)
        {
            doGroundSnap = true;
        }

        player.velocity = player.MoveAndSlideWithSnap(player.velocity, (doGroundSnap) ? Vector3.Down / 5 : Vector3.Zero, Vector3.Up, true);

        if (CheckForState(States.Stand, States.Walk))
        {
            if (Input.IsActionJustPressed("ui_select"))
            {
                player.velocity.y = 20;
                doGroundSnap = false;
            }
        }

        GD.Print(player.IsOnFloor());
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

    private void EnsureSlopeStop()
    {
        //https://github.com/godotengine/godot/issues/34117
        //Not super clean but basically this ensures the velocity is actually 0 when going slow enough on a slope so slope stopping works
        var floor_angle = Mathf.Rad2Deg(Mathf.Acos(GetNode<RayCast>("../RayCast").GetCollisionNormal().Dot(new Vector3(0, 1, 0))));
        if (floor_angle > 1)
        {
            if (!(player.moveInputYRotated.Dot(player.velocity) > 0))
            {
                if (Mathf.Abs(player.velocity.x) < 1f)
                {
                    player.velocity.x = 0;
                }
                if (Mathf.Abs(player.velocity.z) < 1f)
                {
                    player.velocity.z = 0;
                }
            }
        }
    }
}
using Godot;
using System;

public class Player : KinematicBody
{
    Spatial cameraBase;
    float velocityY = 0;

    Vector2 MOUSE_LOOK_SPEED = new Vector2(0.0025f, 0.0025f);

    public override void _Ready()
    {
        cameraBase = GetNode<Spatial>("./CameraBase");
        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventKey)
        {
            if (OS.GetScancodeString((inputEvent as InputEventKey).Scancode) == "Escape")
            {
                GetTree().Quit();
            }
        }

        if (inputEvent is InputEventMouseMotion)
        {
            //Vertical camera movement
            cameraBase.RotateX((inputEvent as InputEventMouseMotion).Relative.y * -MOUSE_LOOK_SPEED.y);
            if (cameraBase.RotationDegrees.x > 90)
            {
                cameraBase.RotationDegrees = new Vector3(90, 0, 0);
            }
            else if (cameraBase.RotationDegrees.x < -90)
            {
                cameraBase.RotationDegrees = new Vector3(-90, 0, 0);
            }
            //Horizontal camera movement
            RotateY((inputEvent as InputEventMouseMotion).Relative.x * -MOUSE_LOOK_SPEED.x);
        }
    }

    private Vector2 CalculateNormalizedMoveInput()
    {
        Vector2 input;
        input.x = (Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"));
	    input.y = (Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up"));
        input = input.Normalized();
        return input;
    }

    public override void _PhysicsProcess(float delta)
    {
        Vector2 input = CalculateNormalizedMoveInput();
        velocityY -= 0.98f;
        if (Input.IsActionJustPressed("ui_select"))
        {
            velocityY += 30;
        }

        Vector3 velocity = new Vector3(input.x * 15, velocityY, input.y * 15).Rotated(Vector3.Up, Rotation.y);

        MoveAndSlideWithSnap(velocity, Vector3.Down, Vector3.Up, true);

        if (IsOnFloor())
        {
            velocityY = 0;
        }
    }
}

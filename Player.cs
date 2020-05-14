using Godot;
using System;

public class Player : KinematicBody
{
    public Spatial cameraBase;
    public Vector2 moveInput;
    public Vector3 moveInputYRotated;

    public Vector3 velocity = new Vector3();

    [Export] public Vector2 MOUSE_LOOK_SPEED = new Vector2(0.0025f, 0.0025f);
    [Export] public float GRAVITY = 0.98f;
    [Export] public float MOVE_SPEED = 12;

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

    public void CalculateNormalizedMoveInput()
    {
        moveInput.x = (Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"));
	    moveInput.y = (Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up"));
        moveInput = moveInput.Normalized();
    }

    public void CalculateMoveInputRelativeToYRot()
    {
        moveInputYRotated = new Vector3(moveInput.x, 0, moveInput.y).Rotated(Vector3.Up, Rotation.y);
    }

    public void ApplyGravity()
    {
        velocity.y -= GRAVITY;
    }
}

using System;
using System.Collections.Generic;
using Godot;

public class Player : KinematicBody2D
{
    [Export] private float speed = 150f;
    [Export] private float runningSpeed = 250f;
    [Export] private bool shouldSlide = false;

    private bool isRunning = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        base._Process(delta);
        //
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        isRunning = Input.IsActionPressed("run");

        var input = new Vector2(
            Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
            Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
        );
        var direction = input.Normalized();
        var movement = (isRunning ? runningSpeed : speed) * direction;

        if (shouldSlide)
        {
            MoveAndSlide(movement);
        }
        else
        {
            MoveAndCollide(movement);
        }
    }
}

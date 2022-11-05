using System;
using System.Collections.Generic;
using Godot;

public class Player : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export] private float speed = 500f;
    [Export] private bool shouldSlide = false;

    private List<string> directions = new List<string> { };

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

        var input = new Vector2(
            Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
            Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
        );
        var direction = input.Normalized();
        var movement = speed * direction;
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

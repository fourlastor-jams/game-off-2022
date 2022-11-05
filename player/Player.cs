using System;
using System.Collections.Generic;
using Godot;

public class Player : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

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

        if (Input.IsActionPressed("ui_down"))
        {
            directions.Add("down");
            // move player down
        }
        else
        {
            directions.Remove("down");
        }
        if (Input.IsActionPressed("ui_up"))
        {
            directions.Add("up");
        }
        else
        {
            directions.Remove("up");
        }
        if (Input.IsActionPressed("ui_left"))
        {
            directions.Add("left");
        }
        else
        {
            directions.Remove("left");
        }
        if (Input.IsActionPressed("ui_right"))
        {
            directions.Add("right");
        }
        else
        {
            directions.Remove("right");
        }
    }
}

using System;
using Godot;

public class Player : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

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
        if (Input.IsActionJustPressed("ui_down"))
        {
            // move player down
        }
        else if (Input.IsActionJustPressed("ui_up"))
        {
            //
        }
        else if (Input.IsActionJustPressed("ui_left"))
        {
            //
        }
        else if (Input.IsActionJustPressed("ui_right"))
        {
            //
        }
    }
}

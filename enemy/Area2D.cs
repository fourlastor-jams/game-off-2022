using System;
using Godot;

public class Area2D : Godot.Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //
    }

    public void _OnAreaEntered(Area2D area)
    {
        GD.Print(">>>");
    }
}

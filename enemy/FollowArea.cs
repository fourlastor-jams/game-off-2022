using System;
using Godot;

public class FollowArea : Godot.Area2D
{
    private Shape shape;

    public override void _Ready()
    {
        // shape = GetNode<Shape>("Shape");
        // GD.Print(">>>>>>>");
        // GD.Print(">>> " + shape);
    }

    public void _OnAreaEntered(Area2D area)
    {
        //
    }
}

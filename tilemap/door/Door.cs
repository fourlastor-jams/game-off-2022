using Godot;
using System;


public class Door : Node2D
{
    [Signal] public delegate void StepOnDoor();

    public override void _Ready()
    {
        GetNode<Area2D>("Area2D").Connect("body_entered", this, nameof(OnPlayerEntered));
    }

    private void OnPlayerEntered(Node node)
    {
        if (node is Player)
        {
            EmitSignal(nameof(StepOnDoor));
        }
    }
}

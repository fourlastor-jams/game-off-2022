using Godot;
using System;

public class Map : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    private Player player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetNode<Player>("YSort/Player");
        player.Connect("OnAction", this, nameof(PlayerAction));
    }

    public void PlayerAction(Vector2 direction)
    {
        GD.Print("Player acting at " + player.Position + ", direction" + direction);
    }
}

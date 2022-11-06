using Godot;
using System;

public class Map : Node2D
{
    private Player player;

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

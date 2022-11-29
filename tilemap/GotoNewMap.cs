using Godot;
using System;

public class GotoNewMap : Area2D
{
    [Export] public String sceneName;
    private PackedScene mapScene;

    [Signal] public delegate void PlayerEntered(PackedScene mapScene);

    public override void _Ready()
    {
        Connect("body_entered", this, "_On_Body_Entered");
        mapScene = GD.Load<PackedScene>("res://maps/" + sceneName + ".tscn");
    }

    private void _On_Body_Entered(Node2D node)
    {
        if (node is Player)
        {
            EmitSignal(nameof(PlayerEntered), mapScene);
        }
    }
}

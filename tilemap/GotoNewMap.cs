using Godot;
using System;

public class GotoNewMap : Area2D
{
    [Export] public String sceneName;
    private PackedScene mapScene;

    [Signal] public delegate void PlayerEntered(PackedScene mapScene);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("body_entered", this, "_on_GotoNewMap_area_entered");
        mapScene = GD.Load<PackedScene>("res://maps/" + sceneName + ".tscn");
    }

    private void _on_GotoNewMap_area_entered(object area)
    {
        EmitSignal(nameof(PlayerEntered), mapScene);
    }

}

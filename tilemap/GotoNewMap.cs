using Godot;
using System;

public class GotoNewMap : Area2D
{
    [Export] public String sceneName;
    private PackedScene mapScene;

    [Signal] public delegate void PlayerEntered(PackedScene mapScene);

    [Signal] public delegate void TryUpgradeInventory();

    public override void _Ready()
    {
        Connect("body_entered", this, nameof(OnBodyEntered));
        mapScene = GD.Load<PackedScene>("res://maps/" + sceneName + ".tscn");
    }

    private void OnBodyEntered(Node2D node)
    {
        if (node is Player)
        {
            EmitSignal(nameof(PlayerEntered), mapScene);

            // If going back to the start, try to upgrade the inventory.
            if (sceneName.Equals("map1"))
            {
                EmitSignal(nameof(TryUpgradeInventory));
            }
        }
    }
}

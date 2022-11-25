using Godot;
using System;

public class GameOver : MarginContainer
{
    [Signal] public delegate void OnRetry();

    public override async void _Ready()
    {
        // Visual - don't show text yet.
        Visible = false;
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        Visible = true;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        // todo: this needs to be 'any key'
        if (!@event.IsActionPressed("debug_hit")) return;
        EmitSignal(nameof(OnRetry));
        QueueFree();
    }
}

using Godot;
using System;

public class GameOver : MarginContainer
{
    [Signal] public delegate void OnRetry();

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (!@event.IsActionPressed("debug_hit")) return;
        EmitSignal(nameof(OnRetry));
        QueueFree();
    }
}

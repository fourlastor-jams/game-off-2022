using Godot;
using System;

public class GameOver : MarginContainer
{
    [Signal] public delegate void OnRetry();

    private int timer;

    public override void _Ready()
    {
        // Visual - don't show text yet.
        Visible = false;
    }

    public override void _Process(float delta)
    {
        // Show text after waiting 2 seconds.
        if (timer < 180)
        {
            ++timer;
            return;
        }
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

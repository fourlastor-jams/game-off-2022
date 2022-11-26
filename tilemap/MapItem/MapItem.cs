using Godot;
using System;


public class MapItem : RigidBody2D
{
    // Godot supports selecting the item-type via a dropdown.
    [Export] public Item item;

    [Signal] public delegate void StepOnItem();

    private static float gravityDampening = 100000f;
    private Node2D nearPlayer;

    public override void _Ready()
    {
        GetNode<Area2D>("Area2D").Connect("body_entered", this, nameof(OnPlayerEntered));
        if (item.HasGravity())
        {
            GetNode<Area2D>("GravityArea").Connect("body_entered", this, nameof(OnPlayerNear));
            GetNode<Area2D>("GravityArea").Connect("body_exited", this, nameof(OnPlayerNotNear));
        }

        // Use _IntegrateForces() instead.
        CustomIntegrator = true;
    }

    public override void _IntegrateForces(Physics2DDirectBodyState state)
    {
        if (nearPlayer != null)
        {
            var dt = state.Step;
            Vector2 distance = nearPlayer.GlobalPosition - GlobalPosition;
            float force = (-1) * gravityDampening / (distance.Length() * distance.Length());
            Vector2 gravity = distance * force;
            ///Vector2 gravity = gravityDampening * (distance / (distance.LengthSquared() * distance.Length()));
            var velocity = state.LinearVelocity;
            state.LinearVelocity = ((velocity + gravity * -1) * dt);
        }
    }

    private void OnPlayerEntered(Node node)
    {
        if (node is Player)
        {
            EmitSignal(nameof(StepOnItem));
        }
    }

    private void OnPlayerNear(Node2D node)
    {
        if (node is Player)
        {
            nearPlayer = node;
            Sleeping = false;
        }
    }

    private void OnPlayerNotNear(Node2D node)
    {
        if (node is Player)
        {
            nearPlayer = null;
            Sleeping = true;
        }
    }
}

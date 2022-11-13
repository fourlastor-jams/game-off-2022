using Godot;
using System;

public class Enemy : KinematicBody2D
{
    [Export] private float speed = 150f;

    private AnimationTree animationTree;
    private AnimationNodeStateMachinePlayback animationStateMachine;

    public override void _Ready()
    {
        animationTree = GetNode<AnimationTree>("AnimationTree");
        animationStateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
        GD.Print(animationTree);
        GD.Print(animationStateMachine);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        var input = new Vector2(
            Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
            Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
        );
        var velocity = input.Normalized();

        if (velocity == Vector2.Zero)
        {
            animationStateMachine.Travel("Idle");
            return;
        }

        animationStateMachine.Travel("Walk");
        animationTree.Set("parameters/Walk/blend_position", velocity);
        animationTree.Set("parameters/Idle/blend_position", velocity);

        var movement = speed * velocity;
        MoveAndSlide(movement);
    }
}

using Godot;

public class Player : KinematicBody2D
{
    [Export] private float speed = 150f;
    [Export] private float runningSpeed = 250f;
    [Export] private bool shouldSlide = false;

    private AnimationTree animationTree;
    private AnimationNodeStateMachinePlayback animationPlayback;
    private bool isRunning = false;

    public override void _Ready()
    {
        base._Ready();
        animationTree = (AnimationTree)GetNode("AnimationTree");
        animationPlayback = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        isRunning = Input.IsActionPressed("run");

        var input = new Vector2(
            Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
            Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
        );
        var velocity = input.Normalized();

        if (velocity == Vector2.Zero)
        {
            animationPlayback.Travel("Idle");
            return;
        }

        animationPlayback.Travel("Walk");
        animationTree.Set("parameters/Walk/blend_position", velocity);
        animationTree.Set("parameters/Idle/blend_position", velocity);

        var movement = (isRunning ? runningSpeed : speed) * velocity;
        if (shouldSlide)
        {
            MoveAndSlide(movement);
        }
        else
        {
            MoveAndCollide(movement);
        }
    }
}

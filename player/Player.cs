using Godot;

public class Player : KinematicBody2D
{
    [Export] private float speed = 150f;
    [Export] private float runningSpeed = 250f;
    [Export] private bool shouldSlide = false;

    [Signal] public delegate void OnAction(Vector2 direction);

    private AnimationTree animationTree;
    private AnimationNodeStateMachinePlayback animationStateMachine;
    private bool isRunning = false;
    private Vector2 facingDirection = Vector2.One;

    public override void _Ready()
    {
        base._Ready();
        animationTree = GetNode<AnimationTree>("AnimationTree");
        animationStateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
    }

    public void ChangeDirection(Vector2 newDirection)
    {
        facingDirection = newDirection;
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
            animationStateMachine.Travel("Idle");
            return;
        }

        animationStateMachine.Travel("Walk");
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

    public override void _UnhandledKeyInput(InputEventKey @event)
    {
        base._UnhandledKeyInput(@event);
        if (Input.IsActionJustPressed("ui_select"))
        {
            EmitSignal(nameof(OnAction), facingDirection);
        }
    }
}

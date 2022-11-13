using Godot;

public class Player : KinematicBody2D
{
    [Export] private float speed = 100f;
    [Export] private float runningSpeed = 130f;
    [Export] private bool shouldSlide;

    [Signal] public delegate void OnAction(Vector2 direction);

    private AnimationTree animationTree;
    private AnimationPlayer animationPlayer;
    private AnimationNodeStateMachinePlayback animationStateMachine;
    private bool isRunning;
    private bool attackQueued;
    private Vector2 facingDirection = Vector2.One;

    public override void _Ready()
    {
        base._Ready();
        animationTree = GetNode<AnimationTree>("AnimationTree");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationStateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
    }

    public override async void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        isRunning = Input.IsActionPressed("run");

        var input = new Vector2(
            Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
            Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
        );
        var velocity = input.Normalized();

        if (attackQueued)
        {
            attackQueued = false;
            // Swing sword.
            animationStateMachine.Start("Attack");
            await ToSignal(animationPlayer, "animation_finished");
        }

        if (velocity == Vector2.Zero)
        {
            animationStateMachine.Travel("Idle");
            return;
        }

        animationStateMachine.Travel("Walk");
        animationTree.Set("parameters/Walk/blend_position", velocity);
        animationTree.Set("parameters/Idle/blend_position", velocity);
        animationTree.Set("parameters/Attack/blend_position", velocity);

        var movement = (isRunning ? runningSpeed : speed) * velocity;
        if (shouldSlide)
        {
            MoveAndSlide(movement);
        }
        else
        {
            MoveAndCollide(movement);
        }

        facingDirection = new Vector2(
            x: Mathf.Sign(movement.x),
            y: Mathf.Sign(movement.y)
        );
    }

    public override void _UnhandledKeyInput(InputEventKey @event)
    {
        base._UnhandledKeyInput(@event);
        if (Input.IsActionJustPressed("ui_select"))
        {
            EmitSignal(nameof(OnAction), facingDirection);
        }
        else if (Input.IsActionJustPressed("attack"))
        {
            attackQueued = true;
        }
    }
}

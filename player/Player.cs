using Godot;

public class Player : KinematicBody2D
{
    [Export] private float speed = 100f;
    [Export] private float runningSpeed = 130f;
    [Export] private bool shouldSlide;

    [Signal] public delegate void OnAction(Vector2 direction);

    [Signal] public delegate void OnDeductHealth(int amount);

    public bool isDead = false;
    private int health;
    private AnimationTree animationTree;
    private AnimationPlayer animationPlayer;
    private AnimationNodeStateMachinePlayback animationStateMachine;
    private bool isRunning;
    private bool attackQueued;
    private int hitsQueued;
    private bool hitAnimationQueued;
    private Vector2 facingDirection = Vector2.One;
    private AudioStreamPlayer heartbeatPlayer;

    public override void _Ready()
    {
        base._Ready();
        animationTree = GetNode<AnimationTree>("AnimationTree");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationStateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
        heartbeatPlayer = GetNode<AudioStreamPlayer>("HeartBeat");
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

        animationTree.Set("parameters/Hit/blend_position", velocity);
        if (hitsQueued > 0)
        {
            QueueHitAnimation();
            // Deduct health.
            EmitSignal(nameof(OnDeductHealth), Item.Heart, hitsQueued);
            hitsQueued = 0;
        }

        if (hitAnimationQueued)
        {
            animationStateMachine.Start("Hit");
            hitAnimationQueued = false;
            return;
        }

        if (animationStateMachine.GetCurrentNode().Equals("Hit")) return;

        // I tried for a while to get this to work using signals.
        // The AnimationTree kept randomly playing the Dead state when it
        // wasn't supposed to.
        if (isDead)
        {
            animationStateMachine.Start("Dead");
            // No longer accept input.
            this.SetPhysicsProcess(false);
            return;
        }

        if (attackQueued)
        {
            attackQueued = false;
            animationTree.Set("parameters/Attack/blend_position", velocity);
            animationStateMachine.Start("Attack");
            return;
        }

        if (animationStateMachine.GetCurrentNode().Equals("Attack")) return;

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

        facingDirection = new Vector2(
            x: Mathf.Sign(movement.x),
            y: Mathf.Sign(movement.y)
        );
    }

    public void QueueHitAnimation()
    {
        hitAnimationQueued = true;
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
        else if (Input.IsActionJustPressed("debug_hit"))
        {
            hitsQueued = 1;
        }
    }

    public void SetHealth(int amount)
    {
        health = amount;
        if (health == 1)
            heartbeatPlayer.Play();
        else
            heartbeatPlayer.Stop();
    }

    public void OnAttacked(Vector2 fromDirection)
    {
        hitsQueued = 1;
    }
}

using System.Collections.Generic;
using Godot;

public enum State
{
    IDLE,
    WANDER,
    FOLLOW,
    ATTACK,
    HURT,
    GOTO // TODO: go to a certain position
}

public class Enemy : KinematicBody2D
{
    [Signal] public delegate void OnAttack(Vector2 facing);
    [Signal] public delegate void OnCollision();
    // [Signal] public delegate void OnChangeDirection();

    [Export] private float speed;

    private AnimationTree animationTree;
    private AnimationNodeStateMachinePlayback animationStateMachine;

    [Export] private List<Vector2> wanderDirections;

    private int wanderDirectionIndex = 0;
    private Vector2 wanderDirection = Vector2.Zero;
    private Timer wanderTimer;
    private State currentState = State.IDLE;
    private State previousState = State.IDLE;
    private Node2D followingTarget = null;
    [Export] private float attackDistanceThreshold = 2f;

    private Vector2 facingDirection = Vector2.Zero;

    public override async void _Ready()
    {
        animationTree = GetNode<AnimationTree>("AnimationTree");
        animationStateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
        wanderTimer = GetNode<Timer>("WanderTimer");
        wanderDirectionIndex = Game.random.Next(wanderDirections.Capacity);
        wanderDirection = wanderDirections.Capacity > 0 ? wanderDirections[wanderDirectionIndex] : Vector2.Zero;
        wanderTimer.OneShot = true;
        _ResetWanderTimer();

        await ToSignal(GetTree().CreateTimer(1), "timeout");
        ChangeStateTo(State.WANDER);
    }

    public void _OnBodyEntered(Node2D other)
    {
        if (other is Player)
        {
            GD.Print("Body " + other.Name + " entered.");
            // TODO we need to update this position more often!
            followingTarget = other;
            ChangeStateTo(State.FOLLOW);
        }
        else
        {
            //
        }
    }

    public void _OnBodyExited(Node2D other)
    {
        if (other is Player)
        {
            GD.Print("Body " + other.Name + " exited.");
            // ChangeStateTo(State.WANDER);
            followingTarget = null;
            ChangeStateTo(State.IDLE);
        }
    }

    public void _OnCollision()
    {
        _ChangeWanderDirection();

        // More complex behaviour (wip):
        var count = GetSlideCount();
        for (int i = 0; i < count; i++)
        {
            KinematicCollision2D collision = GetSlideCollision(i);
            var side = "";
            if (collision.Normal.x > 0) // collided to the left
            {
                side = "L";
            }
            else if (collision.Normal.x < 0) // collided to the right
            {
                side = "R";
            }
            else if (collision.Normal.y > 0) // collided to the top
            {
                side = "T";
            }
            else if (collision.Normal.y < 0) // collided to the bottom
            {
                side = "B";
            }
            // GD.Print("collision " + i + ": " + side);
        }
    }

    public void ChangeStateTo(State newState)
    {
        wanderTimer.Stop();
        previousState = currentState;
        currentState = newState;
        
        switch (currentState)
        {
            case State.IDLE:
                animationStateMachine.Travel("Idle");
                _ResetWanderTimer();
                break;
            case State.WANDER:
                animationStateMachine.Travel("Walk");
                StartWandering();
                break;
            case State.FOLLOW:
                animationStateMachine.Travel("Walk");
                break;
            case State.ATTACK:
                animationStateMachine.Travel("Attack");
                break;
        }
    }

    private void StartWandering()
    {
        wanderTimer.Start();
    }

    public void _OnWanderTimerTimeout()
    {
        switch (currentState)
        {
            case State.IDLE:
                ChangeStateTo(State.WANDER);
                _ChangeWanderDirection();
                break;
            case State.WANDER:
                ChangeStateTo(State.IDLE);
                break;
        }
    }

    public void _ResetWanderTimer()
    {
        wanderTimer.Stop();
        float scaling = 1.4f;
        if (currentState == State.IDLE)
            scaling = 1.8f;
        wanderTimer.WaitTime = (float)(Game.random.NextDouble() * scaling);
        wanderTimer.Start();
    }

    public void _ChangeWanderDirection()
    {
        wanderDirectionIndex = Game.random.Next(wanderDirections.Capacity);
        wanderDirection = wanderDirections[wanderDirectionIndex];
        _ResetWanderTimer();
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        Vector2 velocity = new Vector2(0, 0);
        var distanceToTarget = Mathf.Floor(GetDistanceToFollowingTarget());

        switch (currentState)
        {
            case State.IDLE:
                velocity = new Vector2(0, 0);
                break;
            case State.WANDER:
                velocity = wanderDirection;
                break;
            case State.FOLLOW:
                if (distanceToTarget <= attackDistanceThreshold)
                {
                    ChangeStateTo(State.ATTACK);
                }
                else
                {
                    var followingPosition = followingTarget.GlobalPosition;
                    velocity = GlobalPosition.DirectionTo(followingPosition);
                }
                break;
            case State.ATTACK:
                velocity = Vector2.Zero;
                if (distanceToTarget > attackDistanceThreshold)
                {
                    ChangeStateTo(State.FOLLOW);
                }
                break;
            default:
                break;
        }

        if (velocity == Vector2.Zero)
        {
            return;
        }

        animationTree.Set("parameters/Walk/blend_position", velocity.Normalized());
        animationTree.Set("parameters/Idle/blend_position", velocity.Normalized());
        animationTree.Set("parameters/Attack/blend_position", velocity.Normalized());

        var movement = speed * velocity.Normalized();
        var result = MoveAndSlide(movement);
        if (result == Vector2.Zero)
        {
            EmitSignal(nameof(OnCollision));
        }

        facingDirection = new Vector2(
            x: Mathf.Sign(movement.x),
            y: Mathf.Sign(movement.y)
        );
    }

    private float GetDistanceToFollowingTarget()
    {
        if (followingTarget != null)
        {
            var followingPosition = followingTarget.GlobalPosition;
            var distance = followingPosition.DistanceTo(GlobalPosition);
            return distance;
        }
        else
        {
            return float.NaN;
        }
    }

    public void OnAttacking()
    {
        EmitSignal(nameof(OnAttack), facingDirection);
    }
}

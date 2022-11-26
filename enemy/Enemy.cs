using System.Collections.Generic;
using Godot;

public enum State
{
    IDLE,
    WANDER,
    FOLLOW,
    ATTACK,
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

    [Export] private List<Vector2> wanderDirections; // = new List<Vector2> { Vector2.Right, Vector2.Down, Vector2.Left, Vector2.Up };
    [Export] private List<int> wanderDurations; // = new List<int> {  };

    private int wanderDirectionIndex = 0;
    private Vector2 wanderDirection = Vector2.Zero;
    private Timer wanderTimer;
    private State currentState = State.IDLE;
    private State previousState = State.IDLE;
    private Area2D followingTarget = null;
    [Export] private float attackDistanceThreshold = 2f;

    private Vector2 facingDirection = Vector2.Zero;

    public override async void _Ready()
    {
        GD.Print(wanderDirections.Capacity);
        GD.Print(wanderDurations.Capacity);

        animationTree = GetNode<AnimationTree>("AnimationTree");
        animationStateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
        wanderTimer = GetNode<Timer>("WanderTimer");
        wanderDirection = wanderDirections.Capacity > 0 ? wanderDirections[0] : Vector2.Zero;
        wanderTimer.WaitTime = wanderDurations[wanderDirectionIndex];
        wanderTimer.OneShot = true;
        // debug:
        // await ToSignal(GetTree().CreateTimer(1), "timeout");
        ChangeStateTo(State.WANDER);
    }

    public void _OnAreaEntered(Area2D other)
    {
        GD.Print("Area " + other.Name + " entered.");
        if (other.Name == "PlayerArea")
        {
            // TODO we need to update this position more often!
            followingTarget = other;
            // ChangeStateTo(State.FOLLOW);
        }
        else
        {
            //
        }
    }

    public void _OnAreaExited(Area2D other)
    {
        GD.Print("Area " + other.Name + " exited.");
        if (other.Name == "PlayerArea")
        {
            // ChangeStateTo(State.WANDER);
            followingTarget = null;
        }
    }

    public void _OnCollision()
    {
        GD.Print("Collided!");
        _ChangeWanderDirection();
        // more complex behaviour (wip):
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
        GD.Print("Changing state from " + currentState + " to " + newState);
        wanderTimer.Stop();
        previousState = currentState;
        currentState = newState;

        if (currentState == State.WANDER)
        {
            StartWandering();
        }
    }

    private void StartWandering()
    {
        wanderTimer.Start();
    }

    public void _OnWanderTimerTimeout()
    {
        _ChangeWanderDirection();
    }

    public void _ChangeWanderDirection()
    {
        wanderTimer.Stop();
        if (wanderDirectionIndex > wanderDirections.Capacity - 1)
        {
            wanderDirectionIndex = 0;
        }
        wanderDirection = wanderDirections[wanderDirectionIndex];
        wanderTimer.WaitTime = wanderDurations[wanderDirectionIndex];
        wanderDirectionIndex += 1;
        wanderTimer.Start();
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        Vector2 velocity = new Vector2(0, 0);
        var distanceToTarget = Mathf.Floor(GetDistanceToFollowingTarget());

        switch (currentState)
        {
            case State.IDLE:
                animationStateMachine.Travel("Idle");
                velocity = new Vector2(0, 0);
                break;
            case State.WANDER:
                animationStateMachine.Travel("Walk");
                velocity = wanderDirection;
                break;
            case State.FOLLOW:
                animationStateMachine.Travel("Walk");
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
                animationStateMachine.Travel("Attack");
                velocity = Vector2.Zero;

                // TODO attack! signal?

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
            // animationStateMachine.Travel("Idle");
            return;
        }

        animationTree.Set("parameters/Walk/blend_position", velocity.Normalized());
        animationTree.Set("parameters/Idle/blend_position", velocity.Normalized());
        animationTree.Set("parameters/Attack/blend_position", velocity.Normalized());

        var movement = speed * velocity.Normalized();
        var result = MoveAndSlide(movement);
        // if (result == Vector2.Zero)
        // {
        // EmitSignal(nameof(OnCollision));
        // }

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

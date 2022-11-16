using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

enum State
{
    IDLE,
    WANDER,
    FOLLOW,
    ATTACK
}

public class Enemy : KinematicBody2D
{
    [Signal] public delegate void OnCollision();
    // [Signal] public delegate void OnChangeDirection();

    [Export] private float speed = 80f;

    private State currentState = State.IDLE;

    private AnimationTree animationTree;
    private AnimationNodeStateMachinePlayback animationStateMachine;

    [Export] private List<Vector2> wanderDirections = new List<Vector2> { Vector2.Right, Vector2.Down, Vector2.Left, Vector2.Up };
    [Export] private List<int> wanderDurations = new List<int> { 1, 1, 1, 1 };
    private int wanderDirectionIndex = 0;
    private Vector2 wanderDirection = Vector2.Zero;
    private Timer timer;

    public override async void _Ready()
    {
        animationTree = GetNode<AnimationTree>("AnimationTree");
        animationStateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
        timer = GetNode<Timer>("WanderTimer");
        wanderDirection = wanderDirections.Capacity > 0 ? wanderDirections[0] : Vector2.Zero;
        timer.WaitTime = wanderDurations[wanderDirectionIndex];
        timer.OneShot = true;
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        ChangeState();
    }

    public void _OnAreaEntered(Area2D other)
    {
        GD.Print("Area " + other + " entered.");
    }

    public void _OnAreaExited(Area2D other)
    {
        GD.Print("Area " + other + " exited.");
    }

    public void _OnCollision()
    {
        GD.Print("collision");
        _ChangeDirection();
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

    public void ChangeState()
    {
        // TODO
        currentState = State.WANDER;
        if (currentState == State.WANDER)
        {
            StartWandering();
        }
    }

    private void StartWandering()
    {
        timer.Start();
    }

    public void _OnWanderTimerTimeout()
    {
        _ChangeDirection();
    }

    public void _ChangeDirection()
    {
        timer.Stop();
        if (wanderDirectionIndex > wanderDirections.Capacity - 1)
        {
            wanderDirectionIndex = 0;
        }
        wanderDirection = wanderDirections[wanderDirectionIndex];
        timer.WaitTime = wanderDurations[wanderDirectionIndex];
        GD.Print("Switching to direction " + wanderDirection + " (index is " + wanderDirectionIndex + ") ");
        wanderDirectionIndex += 1;
        timer.Start();
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        Vector2 velocity = new Vector2(0, 0);
        switch (currentState)
        {
            case State.IDLE:
                velocity = new Vector2(0, 0);
                break;
            case State.WANDER:
                velocity = wanderDirection;
                // do we need anything else?
                break;
            case State.FOLLOW:
                velocity = new Vector2(0, 0);
                // move_towards()
                break;
            case State.ATTACK:
                velocity = new Vector2(0, 0);
                //
                break;
            default:
                break;
        }

        // var input = new Vector2(
        //     Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
        //     Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
        // );
        // velocity = input.Normalized();

        if (velocity == Vector2.Zero)
        {
            animationStateMachine.Travel("Idle");
            return;
        }

        animationStateMachine.Travel("Walk");
        animationTree.Set("parameters/Walk/blend_position", velocity);
        animationTree.Set("parameters/Idle/blend_position", velocity);

        var movement = speed * velocity.Normalized();
        var result = MoveAndSlide(movement);
        if (result == Vector2.Zero)
        {
            EmitSignal(nameof(OnCollision));
        }
    }
}

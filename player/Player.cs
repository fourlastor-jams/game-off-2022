using Godot;

public class Player : KinematicBody2D
{
    [Export] private float speed = 150f;
    [Export] private float runningSpeed = 250f;
    [Export] private bool shouldSlide = false;

    private bool isRunning = false;
    private bool teste = false;

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        isRunning = Input.IsActionPressed("run");

        var input = new Vector2(
            Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
            Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
        );
        var direction = input.Normalized();
        var movement = (isRunning ? runningSpeed : speed) * direction;

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

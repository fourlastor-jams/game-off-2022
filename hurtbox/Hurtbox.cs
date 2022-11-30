using Godot;

public class Hurtbox : Area2D
{
    [Signal] public delegate void OnHit();

    public override void _Ready()
    {
        Connect("body_entered", this, nameof(OnBodyEntered));
    }

    private void OnBodyEntered(Node body)
    {
        if (body.Name == "Sword")
        {
            EmitSignal(nameof(OnHit));
        }
    }
}

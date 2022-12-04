using Godot;

public class Hurtbox : Area2D
{
    [Signal] public delegate void OnHit();

    public override void _Ready()
    {
        Connect("area_entered", this, nameof(OnBodyEntered));
    }

    private void OnBodyEntered(Area2D body)
    {
        if (body.Name == "Sword")
        {
            EmitSignal(nameof(OnHit));
        }
    }
}

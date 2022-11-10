using Godot;

public class Game : Node
{
    private AudioStreamPlayer musicPlayer;
    private Control inventory;

    public override void _Ready()
    {
        musicPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        inventory = GetNode<Control>("UI/Inventory");
    }

    public override void _UnhandledKeyInput(InputEventKey @event)
    {
        base._UnhandledKeyInput(@event);
        if (Input.IsActionJustPressed("toggle_audio"))
        {
            musicPlayer.Playing = !musicPlayer.Playing;
        }
        else if (Input.IsActionJustPressed("toggle_inventory"))
        {
            inventory.Visible = !inventory.Visible;
        }
    }
}

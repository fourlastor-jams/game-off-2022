using Godot;

public class Game : Node2D
{
    private AudioStreamPlayer musicPlayer;

    public override void _Ready()
    {
        var scene = ResourceLoader.Load<PackedScene>("res://tilemap/Map.tscn").Instance<Map>();
        GetNode<Viewport>("ViewportContainer/Viewport").AddChild(scene);
        musicPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
    }

    public override void _UnhandledKeyInput(InputEventKey @event)
    {
        base._UnhandledKeyInput(@event);
        if (Input.IsActionJustPressed("toggle_audio"))
        {
            musicPlayer.Playing = !musicPlayer.Playing;
        }
    }
}

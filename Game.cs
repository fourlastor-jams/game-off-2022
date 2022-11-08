using Godot;

public class Game : Node2D
{
    private Node2D node;
    private AudioStreamPlayer musicPlayer;

    public override void _Ready()
    {
        // node = GetNode<Node2D>("Node");
        // musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");

        // debug: we immediately load the map scene
        // GetTree().ChangeScene("res://tilemap/Map.tscn");
        // var map = ResourceLoader.load("res://tilemap/Map.tscn").instance();
        // node.add_child(map);
    }

    public override void _UnhandledKeyInput(InputEventKey @event)
    {
        // base._UnhandledKeyInput(@event);
        // if (Input.IsActionJustPressed("toggle_audio"))
        // {
        // musicPlayer.Playing = !musicPlayer.Playing;
        // }
    }
}

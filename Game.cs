using Godot;

public class Game : Node2D
{
    public override void _Ready()
    {
        // debug: we immediately load the map scene
        GetTree().ChangeScene("res://tilemap/Map.tscn");
    }
}

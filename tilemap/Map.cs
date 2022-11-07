using Godot;
using Godot.Collections;

public class Map : Node2D
{
    private Player player;
    private TileMap walls;
    private AudioStreamPlayer musicPlayer;
    private bool hasKey;

    private readonly Dictionary<string, int> tileIds = new Dictionary<string, int>();

    public override void _Ready()
    {
        player = GetNode<Player>("YSort/Player");
        walls = GetNode<TileMap>("YSort/Walls");
        musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
        player.Connect("OnAction", this, nameof(PlayerAction));
        tileIds[KeyTile] = walls.TileSet.FindTileByName(KeyTile);
        tileIds[DoorTile] = walls.TileSet.FindTileByName(DoorTile);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        var position = PlayerMapPosition();
        if (walls.GetCellv(position) == tileIds[KeyTile])
        {
            hasKey = true;
            walls.SetCellv(position, TileMap.InvalidCell);
        }
    }

    public override void _UnhandledKeyInput(InputEventKey @event)
    {
        base._UnhandledKeyInput(@event);
        if (Input.IsActionJustPressed("toggle_audio"))
        {
            musicPlayer.Playing = !musicPlayer.Playing;
        }
    }

    public void PlayerAction(Vector2 direction)
    {
        var position = PlayerMapPosition() + direction;
        var tileId = walls.GetCellv(position);
        if (tileId == TileMap.InvalidCell)
        {
            GD.Print($"No tile found at {position}");
            return;
        }
        var tileName = walls.TileSet.TileGetName(tileId);
        GD.Print($"Found '{tileName}' [{tileId}] at {position}");
        if (tileId == tileIds[DoorTile] && hasKey)
        {
            walls.SetCellv(position, TileMap.InvalidCell);
            hasKey = false;
        }
    }

    private Vector2 PlayerMapPosition()
    {
        return walls.WorldToMap(walls.ToLocal(player.GlobalPosition));
    }
    
    private const string KeyTile = "key";
    private const string DoorTile = "door";
}

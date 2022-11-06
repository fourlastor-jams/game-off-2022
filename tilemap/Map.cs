using Godot;

public class Map : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    private Player player;
    private TileMap walls;
    private bool hasKey;
    
    private int keyId;
    private int doorId;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetNode<Player>("YSort/Player");
        walls = GetNode<TileMap>("YSort/Walls");
        player.Connect("OnAction", this, nameof(PlayerAction));
        keyId = walls.TileSet.FindTileByName("key");
        doorId = walls.TileSet.FindTileByName("door");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        var position = PlayerMapPosition();
        if (walls.GetCellv(position) == keyId)
        {
            hasKey = true;
            walls.SetCellv(position, TileMap.InvalidCell);
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
        if (tileId == doorId && hasKey)
        {
            walls.SetCellv(position, TileMap.InvalidCell);
            hasKey = false;
        }
    }

    private Vector2 PlayerMapPosition()
    {
        return walls.WorldToMap(walls.ToLocal(player.GlobalPosition));
    }
}

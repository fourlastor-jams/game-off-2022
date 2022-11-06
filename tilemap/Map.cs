using Godot;

public class Map : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    private Player player;
    private TileMap walls;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetNode<Player>("YSort/Player");
        walls = GetNode<TileMap>("YSort/Walls");
        player.Connect("OnAction", this, nameof(PlayerAction));
    }

    public void PlayerAction(Vector2 direction)
    {
        var position = walls.WorldToMap(walls.ToLocal(player.GlobalPosition)) + direction;
        var tileId = walls.GetCellv(position);
        if (tileId == TileMap.InvalidCell)
        {
            GD.Print($"No tile found at {position}");
            return;
        }
        var tileName = walls.TileSet.TileGetName(tileId);
        GD.Print($"Tile with at position {position}: ID {tileId} - Name {tileName}");
    }
}

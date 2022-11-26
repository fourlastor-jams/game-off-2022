using Godot;
using Godot.Collections;

public class Map : Node2D
{
    public Player Player { get; private set; }
    private TileMap walls;
    private bool playerHasKey;

    private readonly Dictionary<string, int> tileIds = new Dictionary<string, int>();

    [Signal] public delegate void OnItemPickedUp(Item item);

    public override void _Ready()
    {
        Player = GetNode<Player>("Walls/Player");
        walls = GetNode<TileMap>("Walls");
        Player.Connect("OnAction", this, nameof(PlayerAction));
        tileIds[KeyTile] = walls.TileSet.FindTileByName(KeyTile);
        tileIds[DoorTile] = walls.TileSet.FindTileByName(DoorTile);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        var position = PlayerMapPosition();
        if (walls.GetCellv(position) == tileIds[KeyTile])
        {
            EmitSignal(nameof(OnItemPickedUp), Item.Key);
            walls.SetCellv(position, TileMap.InvalidCell);
        }
    }

    public void PlayerAction(Vector2 direction)
    {
        var playerPosition = PlayerMapPosition() + direction;
        var tileId = walls.GetCellv(playerPosition);
        if (tileId == TileMap.InvalidCell)
        {
            GD.Print($"No tile found at {playerPosition}");
            return;
        }

        var tileName = walls.TileSet.TileGetName(tileId);
        GD.Print($"Found '{tileName}' [{tileId}] at {playerPosition}");
        if (tileId == tileIds[DoorTile] && playerHasKey)
        {
            walls.SetCellv(playerPosition, TileMap.InvalidCell);
            playerHasKey = false;
        }
    }

    private Vector2 PlayerMapPosition()
    {
        return walls.WorldToMap(walls.ToLocal(Player.GlobalPosition));
    }

    private const string KeyTile = "key";
    private const string DoorTile = "door";
}

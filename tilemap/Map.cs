using Godot;
using Godot.Collections;

public class Map : Node2D
{
    public Player player { get; private set; }
    private TileMap walls;
    private bool playerHasKey;
    private YSort enemies;
    private YSort mapItems;

    private readonly Dictionary<string, int> tileIds = new Dictionary<string, int>();

    [Signal] public delegate void OnItemPickedUp(Item item);

    [Signal] public delegate void AttemptPickupItem(MapItem mapItem);

    public override void _Ready()
    {
        player = GetNode<Player>("Walls/Player");
        walls = GetNode<TileMap>("Walls");
        enemies = GetNode<YSort>("Walls/Enemies");
        mapItems = GetNode<YSort>("Walls/MapItems");
        player.Connect("OnAction", this, nameof(PlayerAction));
        for (int i = 0; i < enemies.GetChildCount(); i++)
        {
            var enemy = enemies.GetChild(i);
            enemy.Connect(nameof(Enemy.OnAttack), player, nameof(Player.OnAttacked));
            enemy.Connect(nameof(Enemy.OnDead), this, nameof(OnEnemyDead));
        }

        tileIds[KeyTile] = walls.TileSet.FindTileByName(KeyTile);
        tileIds[DoorTile] = walls.TileSet.FindTileByName(DoorTile);
        var items = new Array<MapItem>(mapItems.GetChildren());
        foreach (var mapItem in items)
        {
            mapItem.Connect(nameof(MapItem.StepOnItem), this, nameof(OnPlayerStepOnItem), new Array() { mapItem });
        }
    }

    private void OnPlayerStepOnItem(MapItem mapItem)
    {
        EmitSignal(nameof(AttemptPickupItem), mapItem);
    }

    private void OnEnemyDead(Enemy enemy)
    {
        PackedScene itemScene = GD.Load<PackedScene>("res://tilemap/MapItem/Rupee.tscn");
        int numEnemies = enemies.GetChildCount();
        if (numEnemies < 2)
        {
            itemScene = GD.Load<PackedScene>("res://tilemap/MapItem/Key.tscn");
        }

        Node2D mapItem = itemScene.Instance<Node2D>();
        mapItem.GlobalPosition = enemy.GlobalPosition;
        mapItems.AddChild(mapItem);
        mapItem.Connect(nameof(MapItem.StepOnItem), this, nameof(OnPlayerStepOnItem), new Array() { mapItem });
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
        return walls.WorldToMap(walls.ToLocal(player.GlobalPosition));
    }

    private const string KeyTile = "key";
    private const string DoorTile = "door";
    private const string RupeeTile = "rupee";
    private const string HeartTile = "heart";
}

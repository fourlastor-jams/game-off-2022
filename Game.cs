using Godot;
using Godot.Collections;
using JetBrains.Annotations;

[UsedImplicitly] public class Game : Node
{
    private AudioStreamPlayer musicPlayer;
    private Inventory inventory;
    [CanBeNull] private Map map;
    private Viewport viewport;
    private Transition transition;

    private PackedScene mapScene = GD.Load<PackedScene>("res://maps/map1.tscn");
    private readonly PackedScene gameOverScene = GD.Load<PackedScene>("res://game-over/GameOver.tscn");
    private readonly PackedScene introScene = GD.Load<PackedScene>("res://intro/Intro.tscn");

    public static System.Random random = new System.Random();

    public override void _EnterTree()
    {
        var screenSize = OS.GetScreenSize();
        OS.WindowSize = screenSize * 0.8f;
    }

    public override void _Ready()
    {
        musicPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        inventory = GetNode<Inventory>("UI/Inventory");
        viewport = GetNode<Viewport>("ViewportContainer/Viewport");
        transition = GetNode<Transition>("Transition");

        inventory.Connect(nameof(Inventory.HeartsRanOut), this, nameof(OnGameOver));
        StartIntro();
        return;
    }

    private void StartIntro()
    {
        var intro = introScene.Instance();
        intro.Connect(nameof(Intro.IntroFinished), this, nameof(FirstStart));
        viewport.AddChild(intro);
    }

    private void FirstStart()
    {
        var intro = viewport.GetChild(0);
        intro.QueueFree();
        GetNode<Control>("UI").Visible = true;
        StartGame();
        AddInitialItemsToInventory();
    }

    private async void TransitionToMap(PackedScene newScene)
    {
        mapScene = newScene;
        transition.RefreshImage(viewport);
        transition.Start();
        if (map != null)
        {
            map?.QueueFree();
            map = null;
        }

        await ToSignal(transition, nameof(Transition.TransitionMidPoint));
        // Start the game.
        StartGame();
        // Can't control player yet.
        map.player.SetPhysicsProcess(false);
        await ToSignal(GetTree(), "idle_frame"); // Required, othewise the transition texture will be a grey screen.
        transition.RefreshImage(viewport);
        await ToSignal(transition, nameof(Transition.TransitionEnd));
        // Can control player after the transition is over.
        map.player.SetPhysicsProcess(true);
    }

    private void Retry(GameOver gameOver)
    {
        gameOver.Disconnect(nameof(GameOver.OnRetry), this, nameof(Retry));

        TransitionToMap(mapScene);
        AddInitialItemsToInventory();

        // Start the music.
        musicPlayer.Play();
    }

    private void StartGame()
    {
        map?.QueueFree();
        var newMap = mapScene.Instance<Map>();
        viewport.AddChild(newMap);
        newMap.player.Connect(nameof(Player.OnDeductHealth), inventory, nameof(Inventory.DeductItem),
            new Array { Item.Heart });
        inventory.Connect(nameof(Inventory.NumHearts), newMap.player, nameof(Player.SetHealth));
        inventory.Connect(nameof(Inventory.HeartLostFromPickup), newMap.player, nameof(Player.QueueHitAnimation));
        newMap.Connect(nameof(Map.OnItemPickedUp), inventory, nameof(Inventory.AddItem));
        newMap.GetNode<Area2D>("GotoNewMap").Connect(nameof(GotoNewMap.PlayerEntered), this, nameof(GotoToNewMap));
        newMap.GetNode<Area2D>("GotoNewMap").Connect(nameof(GotoNewMap.TryUpgradeInventory), inventory,
            nameof(Inventory.TryUpgradeSlots));
        newMap.Connect(nameof(Map.AttemptPickupItem), this, nameof(AttemptPickupItem));
        map = newMap;
    }

    private void AttemptPickupItem(MapItem mapItem)
    {
        switch (mapItem.item)
        {
            // Don't add doors to inventory, use a key.
            case Item.Door:
                if (inventory.HasItem(Item.Key))
                {
                    inventory.RemoveItem(Item.Key);
                    mapItem.QueueFree();
                }

                break;
            // Pick up everything else.
            case Item.Heart:
            case Item.Key:
            case Item.Rupee:
                inventory.AddItem(mapItem.item);
                mapItem.QueueFree();
                break;
        }
    }

    private void AddInitialItemsToInventory()
    {
        inventory.Clear();
        for (var i = 0; i < 4; i++)
        {
            inventory.AddItem(Item.Heart);
        }

        // TODO: remove
        for (var i = 0; i < 4; i++)
        {
            inventory.AddItem(Item.Key);
        }

        // TODO: remove
        for (var i = 0; i < 8; i++)
        {
            inventory.AddItem(Item.Rupee);
        }
    }

    private void OnGameOver()
    {
        map.player.isDead = true;
        musicPlayer.Stop();
        var gameOver = gameOverScene.Instance<GameOver>();
        AddChild(gameOver);
        gameOver.Connect(nameof(GameOver.OnRetry), this, nameof(Retry), new Array { gameOver });
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

    public void GotoToNewMap(PackedScene mapScene)
    {
        TransitionToMap(mapScene);
    }
}

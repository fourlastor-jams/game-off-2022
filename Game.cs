using Godot;
using Godot.Collections;
using JetBrains.Annotations;

[UsedImplicitly]
public class Game : Node
{

    private AudioStreamPlayer musicPlayer;
    private Inventory inventory;
    [CanBeNull] private Map map;
    private Viewport viewport;
    private Transition transition;

    private PackedScene mapScene = GD.Load<PackedScene>("res://maps/map1.tscn");
    private readonly PackedScene gameOverScene = GD.Load<PackedScene>("res://game-over/GameOver.tscn");

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
        StartGame();
    }

    private async void TransitionToMap(PackedScene mapScene)
    {
        this.mapScene = mapScene;
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
        map.Player.SetPhysicsProcess(false);
        await ToSignal(GetTree(), "idle_frame");  // Required, othewise the transition texture will be a grey screen.
        transition.RefreshImage(viewport);
        await ToSignal(transition, nameof(Transition.TransitionEnd));
        // Can control player after the transition is over.
        map.Player.SetPhysicsProcess(true);
    }

    private void Retry(GameOver gameOver)
    {
        gameOver.Disconnect(nameof(GameOver.OnRetry), this, nameof(Retry));

        TransitionToMap(mapScene);

        // Start the music.
        musicPlayer.Play();
    }

    private void StartGame()
    {
        viewport.RemoveChild(map);
        var newMap = mapScene.Instance<Map>();
        viewport.AddChild(newMap);
        newMap.Player.Connect(nameof(Player.OnDeductHealth), inventory, nameof(Inventory.DeductHealth));
        inventory.Connect(nameof(Inventory.NumHearts), newMap.Player, nameof(Player.SetHealth));
        newMap.Connect(nameof(Map.OnItemPickedUp), inventory, nameof(Inventory.AddItem));
        newMap.GetNode<Area2D>("GotoNewMap").Connect(nameof(GotoNewMap.PlayerEntered), this, nameof(GotoToNewMap));
        map = newMap;
        AddInitialItemsToInventory();
    }

    private void AddInitialItemsToInventory()
    {
        inventory.Clear();
        for (var i = 0; i < 4; i++)
        {
            inventory.AddItem(Item.Heart);
        }

        for (var i = 0; i < 4; i++)
        {
            inventory.AddItem(Item.Key);
        }
    }

    private void OnGameOver()
    {
        map.Player.isDead = true;
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

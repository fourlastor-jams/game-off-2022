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

    private readonly PackedScene mapScene = GD.Load<PackedScene>("res://tilemap/Map.tscn");
    private readonly PackedScene gameOverScene = GD.Load<PackedScene>("res://game-over/GameOver.tscn");

    public override void _Ready()
    {
        musicPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        inventory = GetNode<Inventory>("UI/Inventory");
        viewport = GetNode<Viewport>("ViewportContainer/Viewport");
        transition = GetNode<Transition>("Transition");

        inventory.Connect(nameof(Inventory.HeartsRanOut), this, nameof(OnGameOver));
        StartGame();
    }

    private async void Retry(GameOver gameOver)
    {
        gameOver.Disconnect(nameof(GameOver.OnRetry), this, nameof(Retry));
        var img = viewport.GetTexture().GetData();
        transition.Start(img);
        if (map != null)
        {
            map?.QueueFree();
            map = null;
        }

        // await ToSignal(transition, nameof(Transition.TransitionMidPoint));
        await ToSignal(transition, nameof(Transition.TransitionEnd));
        StartGame();
    }

    private void StartGame()
    {
        var newMap = mapScene.Instance<Map>();
        viewport.AddChild(newMap);
        newMap.Player.Connect(nameof(Player.OnDeductHealth), inventory, nameof(Inventory.DeductHealth));
        newMap.Connect(nameof(Map.OnItemPickedUp), inventory, nameof(Inventory.AddItem));
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
}

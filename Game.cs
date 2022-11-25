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
        map = GetNode<Map>("ViewportContainer/Viewport/Map");
        for (int i = 0; i < 4; i++)
        {
            inventory.AddItem(Item.Heart);
        }
        for (int i = 0; i < 4; i++)
        {
            inventory.AddItem(Item.Key);
        }

        map.Connect(nameof(Map.OnItemPickedUp), inventory, nameof(Inventory.AddItem));

        // TODO: remove once merged
        map.GetNode<Area2D>("GotoNewMap").Connect(nameof(GotoNewMap.PlayerEntered), this, nameof(GotoToNewMap));
    }

    private void StartGame(PackedScene mapScene)
    {
        var newMap = mapScene.Instance<Map>();
        viewport.AddChild(newMap);
        newMap.GetNode<Area2D>("GotoNewMap").Connect(nameof(GotoNewMap.PlayerEntered), this, nameof(GotoToNewMap));
        map = newMap;
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
        //map.GetNode<Area2D>("GotoNewMap").Disconnect(nameof(GotoNewMap.PlayerEntered), this, nameof(GotoToNewMap));
        viewport.RemoveChild(map);
        if (map != null)
        {
            map?.QueueFree();
            map = null;
        }
        GD.Print("hi");
        StartGame(mapScene);
    }
}

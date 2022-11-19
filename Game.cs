using Godot;
using Godot.Collections;
using JetBrains.Annotations;

[UsedImplicitly] public class Game : Node
{
    private AudioStreamPlayer musicPlayer;
    private Inventory inventory;
    private Map map;

    public override void _Ready()
    {
        musicPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        inventory = GetNode<Inventory>("UI/Inventory");
        map = GetNode<Map>("ViewportContainer/Viewport/Map");
        map.Player.Connect(nameof(Player.OnDeductHealth), inventory, nameof(Inventory.DeductHealth));
        for (var i = 0; i < 4; i++)
        {
            inventory.AddItem(Item.Heart);
        }

        for (var i = 0; i < 4; i++)
        {
            inventory.AddItem(Item.Key);
        }

        map.Connect(nameof(Map.OnItemPickedUp), inventory, nameof(Inventory.AddItem));
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

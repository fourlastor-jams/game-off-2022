using Godot;
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
        for (var i = 0; i < 4; i++)
        {
            inventory.AddItem(Item.Heart);
        }

        for (var i = 0; i < 4; i++)
        {
            inventory.AddItem(Item.Key);
        }

        inventory.Connect(nameof(Inventory.HeartsRanOut), this, nameof(GameOver));

        map = GetNode<Map>("ViewportContainer/Viewport/Map");
        map.Player.Connect(nameof(Player.OnDeductHealth), inventory, nameof(Inventory.DeductHealth));
        map.Connect(nameof(Map.OnItemPickedUp), inventory, nameof(Inventory.AddItem));
    }

    private void GameOver()
    {
        GD.Print("Oh no!");
        GD.Print("Oh no!");
        GD.Print("Oh no!");
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

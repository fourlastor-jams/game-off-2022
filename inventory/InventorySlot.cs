using Godot;
using JetBrains.Annotations;

public class InventorySlot : Control
{
    [CanBeNull] private Item? item;

    private TextureRect icon;

    public override void _Ready()
    {
        icon = GetNode<TextureRect>("MarginContainer/ItemIcon");
        icon.Connect("gui_input", this, nameof(OnInput));
    }

    public bool AddItem(Item newItem)
    {
        if (item != null)
        {
            return false;
        }

        item = newItem;
        icon.Texture = newItem.Texture();
        return true;
    }

    public void OnInput([UsedImplicitly] InputEvent @event)
    {
        if (Input.IsActionJustPressed("item_click") && item != null)
        {
            GD.Print($"Click on item {item.Value.Name()}");
        }
    }
}

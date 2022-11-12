using Godot;
using JetBrains.Annotations;

public class InventorySlot : Control
{
    [CanBeNull] private Item? item;

    private TextureRect icon;

    public override void _Ready()
    {
        icon = GetNode<TextureRect>("MarginContainer/ItemIcon");
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
}

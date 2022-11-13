using Godot;
using JetBrains.Annotations;

public class InventorySlot : Control
{
    [CanBeNull] public Item? Item { get; private set; }
    public TextureRect Icon { get; private set; }

    private readonly Vector2 offset = new Vector2(1, 2);

    public override void _Ready()
    {
        Icon = GetNode<TextureRect>("MarginContainer/ItemIcon");
    }

    public bool AddItem(Item newItem)
    {
        if (Item != null)
        {
            return false;
        }

        SetItem(newItem);

        return true;
    }

    public void SetItem(Item? newItem)
    {
        Item = newItem;
        if (newItem.HasValue) Icon.Texture = newItem.Value.Texture();
        else Icon.Texture = null; // TODO: I don't think this is a memory leak but I'm not positive.
        Icon.RectPosition = offset;
    }
}

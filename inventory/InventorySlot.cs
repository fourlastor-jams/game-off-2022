using Godot;
using JetBrains.Annotations;

public class InventorySlot : Control
{
    [CanBeNull] public Item? item { get; set; }
    public TextureRect icon { get; set; }

    private Inventory Inventory;
    private Vector2 Offset = new Vector2(1, 2);

    public override void _Ready()
    {
        icon = GetNode<TextureRect>("MarginContainer/ItemIcon");
        icon.Connect("gui_input", this, nameof(OnInput));
        Inventory = (Inventory)FindParent("Inventory");
    }

    public bool AddItem(Item newItem)
    {
        if (item != null)
        {
            return false;
        }

        this.SetItem(newItem);

        return true;
    }

    public void SetItem(Item? newItem)
    {
        item = newItem;
        if (item.HasValue) icon.Texture = newItem.Value.Texture();
        else icon.Texture = null;  // TODO: I don't think this is a memory leak but I'm not positive.
        icon.RectPosition = Offset;
    }


    public void OnInput([UsedImplicitly] InputEvent @event)
    {
        if (item == null) return;

        if (Inventory.Swapping) return;

        if (Input.IsActionJustPressed("item_click"))
        {
            //GD.Print($"Click on item {item.Value.Name()}");
            Inventory.DragFrom = this;
        }

        // Snap the item to the cursor to drag it around.
        if (Input.IsActionPressed("item_click") && Inventory.DragFrom == this)
        {
            Vector2 position = GetViewport().GetMousePosition() - GetGlobalTransformWithCanvas().origin - new Vector2(8, 8);
            icon.SetPosition(position);
        }

        if (Inventory.DragFrom == null) return;

        // Snap the item to the cursor to drag it around.
        if (Input.IsActionJustReleased("item_click"))
        {
            // Check if there's another element at this position.
            GridContainer gridContainer = (GridContainer)GetParent();
            InventorySlot foundSlot = null;
            foreach (InventorySlot slot in gridContainer.GetChildren())
            {
                GD.Print(slot.GetRect());
                Vector2 position = RectPosition + icon.RectPosition + new Vector2(8, 8);
                if (slot.GetRect().HasPoint(position))
                {
                    foundSlot = slot;
                    break;
                }

            }

            if (foundSlot == null) foundSlot = this;

            GD.Print($"Drag to item {item.Value.Name()}");
            Inventory.DragTo = foundSlot;
            Inventory.Swapping = true;
        }
    } 
}

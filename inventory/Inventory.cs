using System.Collections.Generic;
using System.Linq;
using Godot;

public class Inventory : Control
{
    private List<InventorySlot> slots;

    // Inventory dragging.
    public InventorySlot DragFrom;
    public InventorySlot DragTo;
    public bool Swapping;

    public override void _Ready()
    {
        slots = new List<InventorySlot>(8);
        for (var i = 0; i < 16; i++)
        {
            slots.Add(Slot(i));
        }
    }

    public override void _Process(float delta)
    {
        if (this.DragFrom != null && this.DragTo != null)
        {
            this._SwapItems();
        }
    }

    public bool AddItem(Item item)
    {
        return slots.Any(slot => slot.AddItem(item));
    }

    private InventorySlot Slot(int slot)
    {
        return GetNode<InventorySlot>($"MarginContainer/InventoryGrid/InventorySlot{slot + 1}");
    }

    private async void _SwapItems()
    {
        // Don't call _Process each frame until finished.
        SetProcess(false);
        Swapping = true;

        int scale = 24;

        // Get velocity vectors for the 'swap' animation.
        Vector2 vectorFrom = this.DragTo.RectPosition - (this.DragFrom.RectPosition + this.DragFrom.icon.RectPosition);
        vectorFrom = vectorFrom / scale;
        Vector2 vectorTo = this.DragFrom.RectPosition - (this.DragTo.RectPosition + this.DragTo.icon.RectPosition);
        vectorTo = vectorTo / scale;

        for (int i = 0; i < scale; ++i)
        {
            this.DragFrom.icon.RectPosition += vectorFrom;
            if (this.DragFrom != this.DragTo) this.DragTo.icon.RectPosition += vectorTo;

            // Wait one frame.
            await ToSignal(GetTree(), "idle_frame");
        }

        // Actually swap the items at their respective positions now.
        Item? temp = this.DragFrom.item;
        this.DragFrom.SetItem(this.DragTo.item);
        this.DragTo.SetItem(temp);

        // Reset DragFrom and DragTo.
        this.DragFrom = null;
        this.DragTo = null;

        // Call _Process each frame from now on.
        SetProcess(true);
        Swapping = false;
    }
}

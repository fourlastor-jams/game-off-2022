using System.Collections.Generic;
using System.Linq;
using Godot;
using JetBrains.Annotations;
using Array = Godot.Collections.Array;

public class Inventory : Control
{
    private const int AnimationSteps = 24;

    private GridContainer inventoryGrid;
    private List<InventorySlot> slots;

    // Inventory dragging.
    [CanBeNull] private InventorySlot dragFrom;
    [CanBeNull] private InventorySlot dragTo;
    private bool swapping;

    [Signal] public delegate void NumHearts(int amount);
    [Signal] public delegate void HeartLostFromPickup();
    [Signal] public delegate void HeartsRanOut();

    public override void _Ready()
    {
        inventoryGrid = GetNode<GridContainer>("MarginContainer/InventoryGrid");
        var slotsCount = inventoryGrid.GetChildCount();
        slots = new List<InventorySlot>(slotsCount);
        for (var i = 0; i < slotsCount; i++)
        {
            var slot = Slot(i);
            slots.Add(slot);
            slot.Icon.Connect("gui_input", this, nameof(OnInput), new Array { slot });
        }
    }

    public void OnInput([UsedImplicitly] InputEvent @event, InventorySlot sourceSlot)
    {
        if (sourceSlot.Item == null || swapping) return;

        if (Input.IsActionJustPressed("item_click"))
        {
            //GD.Print($"Click on item {item.Value.Name()}");
            dragFrom = sourceSlot;
        }

        // Snap the item to the cursor to drag it around.
        if (Input.IsActionPressed("item_click") && dragFrom == sourceSlot)
        {
            var position = GetViewport().GetMousePosition() - sourceSlot.GetGlobalTransformWithCanvas().origin -
                           new Vector2(8, 8);
            sourceSlot.Icon.SetPosition(position);
        }

        if (dragFrom == null) return;

        // Snap the item to the cursor to drag it around.
        if (!Input.IsActionJustReleased("item_click")) return;

        // Check if there's another element at this position.
        var foundSlot = FindSlotAtPosition(sourceSlot) ?? sourceSlot;
        // GD.Print($"Drag to item {item.Value.Name()}");
        dragTo = foundSlot;
        swapping = true;
    }

    private InventorySlot FindSlotAtPosition(InventorySlot sourceSlot)
    {
        return (
            from InventorySlot slot in inventoryGrid.GetChildren()
            let position = sourceSlot.RectPosition + sourceSlot.Icon.RectPosition + new Vector2(8, 8)
            where slot.GetRect().HasPoint(position)
            select slot
        ).FirstOrDefault();
    }


    public override void _Process(float delta)
    {
        if (dragFrom != null && dragTo != null)
        {
            _SwapItems(dragFrom, dragTo);
        }
    }

    public void AddItem(Item item)
    {
        var noHealthDeductionRequired = slots.Any(slot => slot.AddItem(item));
        if (noHealthDeductionRequired) return;
        DeductHealth(1);
        AddItem(item);
        EmitSignal(nameof(HeartLostFromPickup));
    }

    private InventorySlot Slot(int slot)
    {
        return GetNode<InventorySlot>($"MarginContainer/InventoryGrid/InventorySlot{slot + 1}");
    }

    public int GetItemCount(Item item)
    {
        bool SlotsPredicate(InventorySlot slot) => slot.Item.HasValue && slot.Item.Value == item;
        return slots.Count(SlotsPredicate);
    }

    public void DeductHealth(int amount)
    {
        var numDeducted = 0;

        bool SlotsWithHeartsPredicate(InventorySlot slot) => slot.Item.HasValue && slot.Item.Value == Item.Heart;

        foreach (var slot in slots.Where(SlotsWithHeartsPredicate))
        {
            slot.SetItem(null);
            if (++numDeducted >= amount) break;
        }

        var heartsRemaining = slots.Count(SlotsWithHeartsPredicate);
        EmitSignal(nameof(NumHearts), heartsRemaining);
        if (heartsRemaining == 0)
        {
            EmitSignal(nameof(HeartsRanOut));
        }
    }

    public void TryUpgradeSlots()
    {
        // TODO: not working yet.
        /*int numRupees = GetItemCount(Item.Rupee);
        if (numRupees >= slots.Count - 4)
        {

            for (var i = 0; i < 4; i++)
            {
                var slot = Slot(i);
                slots.Add(slot);
                slot.Icon.Connect("gui_input", this, nameof(OnInput), new Array { slot });
            }
        }*/
    }

    private async void _SwapItems(InventorySlot origin, InventorySlot destination)
    {
        // Don't call _Process each frame until finished.
        SetProcess(false);
        swapping = true;

        // Get velocity vectors for the 'swap' animation.
        var vectorFrom = destination.RectPosition - (origin.RectPosition + origin.Icon.RectPosition);
        vectorFrom /= AnimationSteps;
        var vectorTo = origin.RectPosition - (destination.RectPosition + destination.Icon.RectPosition);
        vectorTo /= AnimationSteps;

        for (var i = 0; i < AnimationSteps; ++i)
        {
            origin.Icon.RectPosition += vectorFrom;
            if (dragFrom != dragTo) destination.Icon.RectPosition += vectorTo;

            // Wait one frame.
            await ToSignal(GetTree(), "idle_frame");
        }

        // Actually swap the items at their respective positions now.
        var temp = origin.Item;
        origin.SetItem(destination.Item);
        destination.SetItem(temp);

        // Reset DragFrom and DragTo.
        dragFrom = null;
        dragTo = null;

        // Call _Process each frame from now on.
        SetProcess(true);
        swapping = false;
    }

    public void Clear()
    {
        foreach (var slot in slots)
        {
            slot.SetItem(null);
        }
    }

    public bool HasItem(Item item)
    {
        return slots.Any(it => it.Item == item);
    }

    public void RemoveItem(Item item)
    {
        slots.First(it => it.Item == item).SetItem(null);
    }
}

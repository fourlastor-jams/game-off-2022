using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using JetBrains.Annotations;

public class Inventory : Control
{
    private readonly PackedScene slotPackedScene = GD.Load<PackedScene>("res://inventory/InventorySlot.tscn");
    private readonly PackedScene backgroundScene = GD.Load<PackedScene>("res://inventory/Background.tscn");

    private const int AnimationSteps = 24;

    private GridContainer backgroundGrid;
    private GridContainer inventoryGrid;
    private readonly List<InventorySlot> slots = new List<InventorySlot>();

    // Inventory dragging.
    [CanBeNull] private InventorySlot dragFrom;
    [CanBeNull] private InventorySlot dragTo;
    private bool swapping;

    [Signal] public delegate void NumHearts(int amount);

    [Signal] public delegate void HeartLostFromPickup();

    [Signal] public delegate void HeartsRanOut();

    public override void _Ready()
    {
        backgroundGrid = GetNode<GridContainer>("MarginContainer/BackgroundGrid");
        inventoryGrid = GetNode<GridContainer>("MarginContainer/InventoryGrid");
        foreach (var slot in inventoryGrid.GetChildren().OfType<InventorySlot>())
        {
            SetupSlot(slot);
        }

        UpdateInventorySize();
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
        if (noHealthDeductionRequired || item == Item.Heart) return;
        DeductItem(1, Item.Heart);
        AddItem(item);
        EmitSignal(nameof(HeartLostFromPickup));
    }

    private int GetItemCount(Item item)
    {
        bool SlotsPredicate(InventorySlot slot) => slot.Item.HasValue && slot.Item.Value == item;
        return slots.Count(SlotsPredicate);
    }

    public void DeductItem(int amount, Item item)
    {
        var numDeducted = 0;
        bool SlotsWithPredicate(InventorySlot slot) => slot.Item.HasValue && slot.Item.Value == item;

        foreach (var slot in slots.Where(SlotsWithPredicate))
        {
            slot.SetItem(null);
            if (++numDeducted >= amount) break;
        }

        if (item == Item.Heart)
        {
            var heartsRemaining = slots.Count(SlotsWithPredicate);
            EmitSignal(nameof(NumHearts), heartsRemaining);
            if (heartsRemaining == 0)
            {
                EmitSignal(nameof(HeartsRanOut));
            }
        }
    }

    public void Reset()
    {
        slots.Clear();
        foreach (var node in backgroundGrid.GetChildren().OfType<Node>())
        {
            node.QueueFree();
        }

        foreach (var node in inventoryGrid.GetChildren().OfType<Node>())
        {
            node.QueueFree();
        }

        const int columns = 4;
        const int slotsToAdd = 16;

        backgroundGrid.Columns = columns;
        inventoryGrid.Columns = columns;

        for (var i = 0; i < slotsToAdd; i++)
        {
            AddSlotToInventory();
        }

        UpdateInventorySize();
    }

    private void AddSlotToInventory()
    {
        backgroundGrid.AddChild(backgroundScene.Instance());
        var slot = slotPackedScene.Instance<InventorySlot>();
        inventoryGrid.AddChild(slot);
        SetupSlot(slot);
    }

    private void SetupSlot(InventorySlot slot)
    {
        slots.Add(slot);
        slot.Icon.Connect("gui_input", this, nameof(OnInput), new Array { slot });
    }

    private void UpdateInventorySize()
    {
        var columns = backgroundGrid.Columns;
        var rows = slots.Count / columns;
        const float cellSize = 18f;
        const float hMargin = 14f;
        const float vMargin = 6f;
        RectSize = new Vector2(cellSize * columns + columns - 1 + hMargin, cellSize * rows + rows - 1 + vMargin);
    }

    public void TryUpgradeSlots()
    {
        var numRupees = GetItemCount(Item.Rupee);
        if (numRupees >= slots.Count - 8)
        {
            // Remove slots.Count - 4 rupees
            DeductItem(slots.Count - 8, Item.Rupee);

            // Add 1 column.
            backgroundGrid.Columns += 1;
            inventoryGrid.Columns += 1;

            // Add 4 InventorySlot nodes.
            for (var i = 0; i < 4; ++i)
            {
                AddSlotToInventory();
            }

            UpdateInventorySize();
        }
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

using System.Collections.Generic;
using System.Linq;
using Godot;

public class Inventory : Control
{
    private List<InventorySlot> slots;

    public override void _Ready()
    {
        slots = new List<InventorySlot>(8);
        for (var i = 0; i < 16; i++)
        {
            slots.Add(Slot(i));
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
}

using System;
using Godot;

public enum Item
{
    Key,
    Heart,
    Rupee,
    Door
}


public static class ItemExtension
{
    public static string Name(this Item item)
    {
        return item.OnItem(
            key: () => "Key",
            heart: () => "Heart",
            rupee: () => "Rupee",
            door: () => "Door"
        );
    }
    public static bool HasGravity(this Item item)
    {
        return item.OnItem(
            key: () => true,
            heart: () => true,
            rupee: () => true,
            door: () => false
        );
    }

    public static Texture Texture(this Item item)
    {
        return GD.Load<Texture>($"inventory/items/{item.Name()}.tres");
    }

    private static T OnItem<T>(this Item item, Func<T> key, Func<T> heart, Func<T> rupee, Func<T> door)
    {
        switch (item)
        {
            case Item.Key:
                return key();
            case Item.Heart:
                return heart();
            case Item.Rupee:
                return rupee();
            case Item.Door:
                return door();
            default:
                throw new Exception($"Missing name for {item}");
        }
    }
}

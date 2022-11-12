using System;
using Godot;

public enum Item
{
    Key,
    Hearth
}


public static class ItemExtension
{
    public static string Name(this Item item)
    {
        return item.OnItem(
            key: () => "Key",
            heart: () => "Heart"
        );
    }

    public static Texture Texture(this Item item)
    {
        return GD.Load<Texture>($"inventory/items/{item.Name()}.tres");
    }

    private static T OnItem<T>(this Item item, Func<T> key, Func<T> heart)
    {
        switch (item)
        {
            case Item.Key:
                return key();
            case Item.Hearth:
                return heart();
            default:
                throw new Exception($"Missing name for {item}");
        }
    }
}

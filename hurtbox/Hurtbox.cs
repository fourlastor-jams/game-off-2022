using System;
using Godot;

public class Hurtbox : Area2D
{
    public override void _Ready()
    {

    }

    public void _OnAreaEntered(Area2D other)
    {
        if (other.Name == "Sword")
        {
            GD.Print("HIT!");
        }
        else
        {
            //
        }
    }

    public void _OnAreaExited(Area2D other)
    {
        //
    }
}

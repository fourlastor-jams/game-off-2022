using Godot;
using System;
using JetBrains.Annotations;

[UsedImplicitly] public class Intro : Control
{
    [Signal] public delegate void IntroFinished();

    public override void _Ready()
    {
        var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.Play("Intro");
        animationPlayer.Connect("animation_finished", this, nameof(AnimationFinished));
    }

    private void AnimationFinished([UsedImplicitly] string animationName)
    {
        EmitSignal(nameof(IntroFinished));
    }
}

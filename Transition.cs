using Godot;
using System;
using System.Threading.Tasks;

public class Transition : Control
{
    private AnimationPlayer player;
    private TextureRect texture;

    [Signal] public delegate void TransitionMidPoint();

    [Signal] public delegate void TransitionEnd();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        texture = GetNode<TextureRect>("TransitionTexture");
        player = GetNode<AnimationPlayer>("TransitionPlayer");
    }

    public void RefreshImage(Viewport viewport)
    {
        var image = viewport.GetTexture().GetData();
        image.FlipY();
        var newTexture = new ImageTexture();
        newTexture.CreateFromImage(image);
        texture.Texture = newTexture;
    }

    public async void Start()
    {
        player.Play("transition_in");
        await ToSignal(player, "animation_finished");
        EmitSignal(nameof(TransitionMidPoint));
        player.Play("transition_out");
        await ToSignal(player, "animation_finished");
        EmitSignal(nameof(TransitionEnd));
    }
}

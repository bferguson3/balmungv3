using Godot;
using System;

public class mapSelector_effect : Sprite
{
    private float newAlpha = 0.5f;

    private bool fadeOut = true;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
    }

    public override void _Process(float delta)
    {
        if (this.Visible)
        {
            if (newAlpha > 0.1f && fadeOut)
            {
                newAlpha = newAlpha - (0.5f * delta);
            }
            else
            {
                fadeOut = false;
            }

            if (newAlpha < 0.5f && !fadeOut)
            {
                newAlpha = newAlpha + (0.5f * delta);
            }
            else
            {
                fadeOut = true;
            }

            var newC = this.Modulate;
            newC.a = newAlpha;
            this.SetModulate(newC);
        }
    }
}

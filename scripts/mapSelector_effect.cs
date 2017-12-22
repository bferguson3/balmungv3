using Godot;
using System;

public class mapSelector_effect : Sprite
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    float newAlpha = 0.5f;
    bool fadeOut = true;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here

    }

    public override void _Process(float delta)
    {
        //        // Called every frame. Delta is time since last frame.
        //        // Update game logic here.
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

            Color newC = this.Modulate;
            newC.a = newAlpha;
            this.SetModulate(newC);

        }
    }
}

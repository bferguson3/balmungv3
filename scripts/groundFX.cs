using Godot;
using System;

public class groundFX : Sprite
{
    private bool drawingMoveArea;

    private player p;

    public override void _Ready()
    {
        p = GetNode("../playerSprite") as player;
    }

    public void DrawPlayerMoveZone()
    {
        drawingMoveArea = true;
        Update();
    }

    public void HidePlayerMoveZone()
    {
        drawingMoveArea = false;
        Update();
    }

    public override void _Draw()
    {
        if (drawingMoveArea)
        {
            //Determine what is the area that needs to be colored and fill it in.
            //Draw grid by grid. e.g. one square up, one square down, 1 by 1.
            //but first, check to see if it's within "combat bounds".
            //to that end, create a 'combat bounds' colision object on combat start
            //and only draw 'move grid' if it doesn't overlap. (Somehow)

            //-> Planning ahead further, create an optional seperate map for combat.
            //It can still be set up the same way, but there (+) is a tilemap that is
            //shown or hidden based on a random(int) and terrain type.

            Vector2 rPos;
            var rSize = new Vector2(32, 32);

            for (var r = -2; r < 3; r++)
            {
                for (var q = -2; q < 3; q++)
                {
                    if (Mathf.Abs(r) + Mathf.Abs(q) < 3)
                    {
                        rPos = p.GetPosition();
                        rPos.x += (r * 32) - 16;
                        rPos.y += (q * 32) - 16;

                        var newR = new Rect2(rPos, rSize);
                        DrawRect(newR, new Color(0, 1, 0, 0.33f));
                    }
                }
            }
        }
    }
    //    public override void _Process(float delta)
    //    {
    //        
    //    }
}

using Godot;
using System;

public class playerRay : RayCast2D
{
    globals g;

    public override void _Ready()
    {
        g = GetNode("/root/globals") as globals;
    }

    public override void _Process(float delta)
    {
        if (GetName() == "playerRayDown")
        {
            if (IsColliding())
            {
                g.playerBlockedDown = true;
            }
            else
                g.playerBlockedDown = false;
        }
        else if (GetName() == "playerRayUp")
        {
            if (IsColliding())
                g.playerBlockedUp = true;
            else
                g.playerBlockedUp = false;
        }
        else if (GetName() == "playerRayLeft")
        {
            if (IsColliding())
                g.playerBlockedLeft = true;
            else
                g.playerBlockedLeft = false;

        }
        else if(GetName() == "playerRayRight")
        {
            if (IsColliding())
                g.playerBlockedRight = true;
            else 
                g.playerBlockedRight = false;
        }
    }
}

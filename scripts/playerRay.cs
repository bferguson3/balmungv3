using Godot;
using System;

public class playerRay : RayCast2D
{
    globals g;

    public override void _Ready()
    {
        g = GetNode("/root/globals") as globals;
    }

    private void CheckCombat(Node col)
    {
        npcScript n = col.GetParent() as npcScript;
        if(n.myType == npcType.enemy){
            //GD.Print("BATTLE!");
        }
    }

    public override void _Process(float delta)
    {
        if (GetName() == "playerRayDown")
        {
            if (IsColliding())
            {
                CheckCombat(GetCollider() as Node);
                g.playerBlockedDown = true;
            }
            else
                g.playerBlockedDown = false;
        }
        else if (GetName() == "playerRayUp")
        {
            if (IsColliding()){
                CheckCombat(GetCollider() as Node);
                g.playerBlockedUp = true;
            }
            else
                g.playerBlockedUp = false;
        }
        else if (GetName() == "playerRayLeft")
        {
            if (IsColliding()){
                CheckCombat(GetCollider() as Node);
                g.playerBlockedLeft = true;
            }
            else
                g.playerBlockedLeft = false;

        }
        else if(GetName() == "playerRayRight")
        {
            if (IsColliding()){
                CheckCombat(GetCollider() as Node);
                g.playerBlockedRight = true;
            }
            else 
                g.playerBlockedRight = false;
        }
    }
}

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
            
            g.inCombat = true;

            Sprite ui = GetNode("../../combatUI") as Sprite;
            
            Camera2D cam = GetNode("../Camera2D") as Camera2D;
            Vector2 oldpos = cam.GetGlobalPosition();
            oldpos.x -= 130;
            ui.Show();
            ui.SetPosition(oldpos);
            cam.SetOffset(new Vector2(-80, 0));
            cam.SetDragMargin(0, 1);
            cam.SetDragMargin(1, 1);
            cam.SetDragMargin(2, 1);
            cam.SetDragMargin(3, 1);
        
        }
    }

    public override void _Process(float delta)
    {
        if (GetName() == "playerRayDown")
        {
            if (IsColliding())
            {
                if(!g.inCombat)
                    CheckCombat(GetCollider() as Node);
                g.playerBlockedDown = true;
            }
            else
                g.playerBlockedDown = false;
        }
        else if (GetName() == "playerRayUp")
        {
            if (IsColliding()){
                if(!g.inCombat)
                    CheckCombat(GetCollider() as Node);
                g.playerBlockedUp = true;
            }
            else
                g.playerBlockedUp = false;
        }
        else if (GetName() == "playerRayLeft")
        {
            if (IsColliding()){
                if(!g.inCombat)
                    CheckCombat(GetCollider() as Node);
                g.playerBlockedLeft = true;
            }
            else
                g.playerBlockedLeft = false;

        }
        else if(GetName() == "playerRayRight")
        {
            if (IsColliding()){
                if(!g.inCombat)
                    CheckCombat(GetCollider() as Node);
                g.playerBlockedRight = true;
            }
            else 
                g.playerBlockedRight = false;
        }
    }
}

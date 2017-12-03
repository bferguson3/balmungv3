using Godot;
using System;

public class playerRay : RayCast2D
{
    globals g;
    player p;
    Sprite ui;
    Camera2D cam;

    public override void _Ready()
    {
        g = GetNode("/root/globals") as globals;
        ui = GetNode("../../combatUI") as Sprite;
        cam = GetNode("../Camera2D") as Camera2D;
        p = GetParent() as player;

    }

    private void SetBattlePositions(battlePositions bpos, player p, npcScript n)
    {
        if(bpos == battlePositions.standard)
        {
            p.SetPosition(n.GetPosition() + new Vector2(0, 98));
            n.SetPosition(n.GetPosition() + new Vector2(0, -98));
        }
    }

    private void CheckCombat(Node col)
    {
        npcScript n = col.GetParent() as npcScript;
        if(n.myType == npcType.enemy){
            
            g.inCombat = true;
            
            ui.Show();
            ui.SetPosition(n.GetPosition() + new Vector2(120, 40));
            
            cam.SetOffset(new Vector2(200, 40));
            
            SetBattlePositions(battlePositions.standard, p, n);
           
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

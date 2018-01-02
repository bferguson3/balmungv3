using Godot;
using System;

public class playerRay : RayCast2D
{
    private globals g;

    private player p;

    private UI ui;

    private Camera2D cam;

    private npc collidingWith;

    private bool drawingFadeOut;

    public override void _Ready()
    {
        g = GetNode("/root/globals") as globals;
        ui = GetNode("../../UI") as UI;
        cam = GetNode("../Camera2D") as Camera2D;
        p = GetParent() as player;
        AddException(GetNode("../Area2D"));
    }

    private void CheckCombat(Node col)
    {
        collidingWith = col.GetParent() as npc;

        if (collidingWith == null)
        {
            return;
        }

        ui.collidingWith = collidingWith;

        if (ui.collidingWith.myType == npcType.enemy)
        {
            //TODO: Move elsewhere?
            g.inCombat = true;
            g.inputMode = inputModes.noInput;
            ui.StartFadeOut();
        }
    }

    public override void _Process(float delta)
    {
        if (GetName() == "playerRayDown")
        {
            if (IsColliding())
            {
                var col = GetCollider() as Node;

                if (GetParent() == col.GetParent())
                {
                    //do something?
                }
                else
                {
                    if (!g.inCombat)
                    {
                        CheckCombat(col);
                    }
                    if (col.GetName() == "combatFleeZone")
                    { }//g.tryToFlee = true;}
                    else
                    {
                        //    g.tryToFlee = false;
                        g.playerBlockedDown = true;
                    }
                }
            }
            else
            {
                g.playerBlockedDown = false;
            }
        }
        else if (GetName() == "playerRayUp")
        {
            if (IsColliding())
            {
                var col = GetCollider() as Node;

                if (GetParent() == col.GetParent())
                {
                    //do something?
                }
                else
                {
                    if (!g.inCombat)
                    {
                        CheckCombat(col);

                        if (col.GetName() == "combatFleeZone")
                        { }//g.tryToFlee = true;}
                        else
                        {
                            //   g.tryToFlee = false;
                            g.playerBlockedUp = true;
                        }
                    }
                }
            }
            else
            {
                g.playerBlockedUp = false;
            }
        }
        else if (GetName() == "playerRayLeft")
        {
            if (IsColliding())
            {
                var col = GetCollider() as Node;

                if (GetParent() == col.GetParent())
                {
                    //do something?
                }
                else
                {
                    if (!g.inCombat)
                    {
                        CheckCombat(col);
                    }
                    if (col.GetName() == "combatFleeZone")
                    { }//g.tryToFlee = true;}
                    else
                    {
                        //    g.tryToFlee = false;
                        g.playerBlockedLeft = true;
                    }
                }
            }
            else
            {
                g.playerBlockedLeft = false;
            }
        }
        else if (GetName() == "playerRayRight")
        {
            if (IsColliding())
            {
                var col = GetCollider() as Node;

                if (GetParent() == col.GetParent())
                {
                    //do something?
                }
                else
                {
                    if (!g.inCombat)
                    {
                        CheckCombat(col);
                    }
                    if (col.GetName() == "combatFleeZone")
                    { }//g.tryToFlee = true;}
                    else
                    {
                        //    g.tryToFlee = false;
                        g.playerBlockedRight = true;
                    }
                }
            }
            else
            {
                g.playerBlockedRight = false;
            }
        }
    }
}
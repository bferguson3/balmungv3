using Godot;
using System;
using System.Collections.Generic;

public class player : Sprite
{
[Export]
    public string myName;

    [Export]
    public int ATK;

    [Export]
    public int DEF;

    [Export]
    public int STR;
    [Export]
    public int DEX;

    [Export]
    public int INT;

    [Export]
    public int EXP;

    [Export]
    public int MaxHP;

    [Export]
    public int HP;

    [Export]
    public int MaxMP;

    [Export]
    public int MP;

    public int actionWeight;

    public Vector2 roundStartPos;

    public bool turnTaken;

    public List<Node> targets = new List<Node>();

    private Vector2 mypos;

    private globals g;

    private UI gui;

    private int udMove;
    
    private int lrMove;
    
    private int moveRange;

    private RayCast2D[] diagonals = new RayCast2D[4];

    private RayCast2D[] laterals = new RayCast2D[4];

    public override void _Ready()
    {
        g = GetNode("/root/globals") as globals;
        gui = GetNode("../UI") as UI;

        for (var c = 1; c <= 4; c++)
        {
            diagonals[c - 1] = GetNode("diagonal" + c.ToString()) as RayCast2D;
            diagonals[c - 1].SetRotationDegrees(((c * 2) - 1) * 45f);
            diagonals[c - 1].AddException(this.GetNode("Area2D"));
        }

        laterals[0] = GetNode("playerRayDown") as RayCast2D;
        laterals[1] = GetNode("playerRayUp") as RayCast2D;
        laterals[2] = GetNode("playerRayLeft") as RayCast2D;
        laterals[3] = GetNode("playerRayRight") as RayCast2D;
    }

    public void SelectAdjacentNPCs()
    {
        //Use 8 directional rays
        //TODO: fix collision udlr?
        //For now iterate differently
        //add to List<> of possibilities
        //Selector position = target.position
        targets.Clear();
        FindCollidingTargets();
        
        if (targets.Count > 0)
        {
            gui.MapSelect(targets[0] as Sprite);
        }
        else
        {
            g.inputMode = inputModes.moving;
        }
    }

    private void FindCollidingTargets()
    {
        if (g.inputMode == inputModes.selectToTalk)
        {
            for (var c = 0; c < 4; c++)
            {
                if (diagonals[c].IsColliding())
                {
                    var col = diagonals[c].GetCollider() as Node;

                    if (col.GetParent() is npc)
                    {
                        var me = col.GetParent() as npc;

                        if (me.myType == npcType.talker)
                        {
                            //Do I talk?
                            //Yes, so I can return myself as a GOOD target for selectToTalk
                            targets.Add(me);
                        }
                    }
                }
                if (laterals[c].IsColliding())
                {
                    var col = laterals[c].GetCollider() as Node;

                    if (col.GetParent() is npc)
                    {
                        var me = col.GetParent() as npc;

                        if (me.myType == npcType.talker)
                        {
                            targets.Add(me);
                        }
                    }
                }
            }
        }
    }

    public bool CheckCollision(string direction)
    {
        if (direction == g.downButton)
        {
            return !g.playerBlockedDown;
        }
        else if (direction == g.rightButton)
        {
            return !g.playerBlockedRight;
        }
        else if (direction == g.leftButton)
        {
            return !g.playerBlockedLeft;
        }
        else if (direction == g.upButton)
        {
            return !g.playerBlockedUp;
        }
        
        return true;
    }

    public bool CheckMoveBoundary(string direction)
    {
        if (direction == g.upButton)
        {
            if (udMove < moveRange && lrMove == 0 || (udMove < (moveRange - 1) && Mathf.Abs(lrMove) < moveRange))
            {
                return true;
            }

            return false;
        }
        else if (direction == g.downButton)
        {
            if (udMove > -moveRange && lrMove == 0 || (udMove > -(moveRange - 1) && Mathf.Abs(lrMove) < moveRange))
            {
                return true;
            }
            
            return false;
        }
        else if (direction == g.leftButton)
        {
            if (lrMove > -moveRange && udMove == 0 || (lrMove > -(moveRange - 1) && Mathf.Abs(udMove) < moveRange))
            {
                return true;
            }
            
            return false;
        }
        else if (direction == g.rightButton)
        {
            if (lrMove < moveRange && udMove == 0 || (lrMove < (moveRange - 1) && Mathf.Abs(udMove) < moveRange))
            {
                return true;
            }
            
            return false;
        }

        return false;
    }

    public void MoveThisSprite(string direction)
    {
        var x = 0;
        var y = 0;

        if (direction == g.downButton)
        {
            y += 32;
            udMove--;
        }
        else if (direction == g.upButton)
        {
            y -= 32;
            udMove++;
        }
        else if (direction == g.leftButton)
        {
            x -= 32;
            lrMove--;
        }
        else if (direction == g.rightButton)
        {
            x += 32;
            lrMove++;
        }

        mypos = GetPosition();
        mypos.x += x;
        mypos.y += y;
        SetPosition(mypos);
    }

    public void InitializeTurn()
    {
        roundStartPos = GetPosition();
        udMove = 0;
        lrMove = 0;
        moveRange = 2;
    }

    //    public override void _Process(float delta)
    //    {
    //        // Called every frame. Delta is time since last frame.
    //        // Update game logic here.
    //        
    //    }
}

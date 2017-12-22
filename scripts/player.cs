using Godot;
using System;
using System.Collections.Generic;

public class player : Sprite
{ 
    Vector2 mypos;
    public Vector2 roundStartPos;
    public bool turnTaken;
    globals g;
    private int udMove, lrMove, moveRange;
	
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
    RayCast2D[] diagonals = new RayCast2D[4];
    RayCast2D[] laterals = new RayCast2D[4];
    List<Node> targets = new List<Node>();

    public override void _Ready()
    {
        g = GetNode("/root/globals") as globals;
        for(int c = 1; c <= 4; c++){
            diagonals[c-1] = GetNode("diagonal" + c.ToString()) as RayCast2D;
            diagonals[c-1].SetRotationDegrees(((c*2)-1)*45f);
            diagonals[c-1].AddException(this.GetNode("Area2D"));
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
        //
        foreach(var m in targets)
            GD.Print(m);
        //
    }

    private void FindCollidingTargets()
    {
        if(g.inputMode == inputModes.selectToTalk){
            for(int c = 0; c < 4; c++){
                if(diagonals[c].IsColliding()){
                    var col = diagonals[c].GetCollider() as Node;
                    if(col.GetParent() is npc){
                        var me = col.GetParent() as npc;
                        if(me.myType == npcType.talker){
                            //Do I talk?
                            //Yes, so I can return myself as a GOOD target for selectToTalk
                            targets.Add(me);
                        }
                    }
                }
                if(laterals[c].IsColliding()){
                    var col = laterals[c].GetCollider() as Node;
                    if(col.GetParent() is npc){
                        var me = col.GetParent() as npc;
                        if(me.myType == npcType.talker)
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
            return !g.playerBlockedDown;
        else if (direction == g.rightButton)
            return !g.playerBlockedRight;
        else if (direction == g.leftButton)
            return !g.playerBlockedLeft;
        else if (direction == g.upButton)
            return !g.playerBlockedUp;
        else
            return true;
    }

    public bool CheckMoveBoundary(string direction)
    {
        if(direction == g.upButton){
            if(udMove < moveRange && lrMove == 0 || (udMove < (moveRange-1) && Mathf.Abs(lrMove) < moveRange)){
                return true;
            }
            else
                return false;
        }
        else if(direction == g.downButton){
            if(udMove > -moveRange && lrMove == 0 || (udMove > -(moveRange-1) && Mathf.Abs(lrMove) < moveRange)){
                return true;
            }
            else
                return false;
        }
        else if(direction == g.leftButton){
            if(lrMove > -moveRange && udMove == 0 || (lrMove > -(moveRange-1) && Mathf.Abs(udMove) < moveRange)){
                return true;
            }
            else   
                return false;
        }
        else if(direction == g.rightButton){
            if(lrMove < moveRange && udMove == 0 || (lrMove < (moveRange-1) && Mathf.Abs(udMove) < moveRange)){
                return true;
            }
            else
                return false;
        }
        return false;
        
    }

    public void MoveThisSprite(string direction)
    {
        int x = 0, y = 0;

        if (direction == g.downButton){
            y += 32;
            udMove--;
        }
        else if (direction == g.upButton){
            y -= 32;
            udMove++;
        }
        else if (direction == g.leftButton){
            x -= 32;
            lrMove--;
        }
        else if (direction == g.rightButton){
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

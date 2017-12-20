using Godot;
using System;

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

    public override void _Ready()
    {
        g = GetNode("/root/globals") as globals;
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

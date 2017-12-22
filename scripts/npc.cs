using Godot;
using System;

public enum npcType { enemy, shopKeep, talker }
public enum aITypes { melee, ranged }

public class npc : Sprite
{
    [Export]
    public npcType myType;
    [Export]
    public aITypes aIType;
    public bool turnTaken;
    [Export]
    public string myName;
    [Export]
    public int ATK;
    [Export]
    public int DEF;
    [Export]
    public int DEX;
    [Export]
    public int EXP;
    [Export]
    public int HP;
    [Export]
    public int MP;
    [Export]
    public string droppedItem;// = new string[2];
    [Export]
    public int dropRate;
    [Export]
    public int droppedG;
    public int actionWeight, moraleScore;
    public bool fleeing;
    RayCast2D[] rays = new RayCast2D[8];
    globals g;
    combatOps c;
    UI gui;
    private Timer newt = new Timer();

    public override void _Ready()
    {
        g = GetNode("/root/globals") as globals;
        c = GetNode("../combatOps") as combatOps;
        gui = GetNode("../UI") as UI;
        InitializeRays();

        newt.Connect("timeout", this, "combatBeginNextTurn");
        newt.OneShot = true;
        AddChild(newt);
    }

    public void MoveForEscape()
    {

    }

    public void AttackAdjacent()
    {
        gui.UpdateCombatFeedback(myName + " attacks!\n ");
        newt.SetWaitTime(1.0f);
        newt.Start();
        //turnTaken = true;
    }

    private bool CheckCollision(string dir)
    {
        if (dir == "right")
        {
            if (rays[1].IsColliding())
            {
                var col = rays[1].GetCollider() as Node;
                if (col.GetParent() == this) { }
                else
                    return false;
            }
        }
        else if (dir == "left")
        {
            if (rays[5].IsColliding())
            {
                var col = rays[5].GetCollider() as Node;
                if (col.GetParent() == this) { }
                else
                    return false;
            }
        }
        else if (dir == "up")
        {
            if (rays[3].IsColliding())
            {
                var col = rays[3].GetCollider() as Node;
                if (col.GetParent() == this) { }
                else
                    return false;
            }
        }
        else if (dir == "down")
        {
            if (rays[7].IsColliding())
            {
                var col = rays[7].GetCollider() as Node;
                if (col.GetParent() == this) { }
                else
                    return false;
            }
        }
        return true;
    }

    public void MoveToClosestPC()
    {
        float nearestDist = 999999999f;
        Sprite nearest = g.combatants[0];
        Vector2 xyDist = new Vector2(0, 0);
        foreach (Sprite cbt in g.combatants)
        {
            if ((GetPosition().DistanceSquaredTo(cbt.GetPosition()) <= nearestDist) && this != cbt)
            {
                nearestDist = GetPosition().DistanceSquaredTo(cbt.GetPosition());
                nearest = cbt;
                xyDist.x = (int)(cbt.GetPosition().x) - (int)(GetPosition().x);
                xyDist.y = (int)(cbt.GetPosition().y) - (int)(GetPosition().y);
            }
            //GD.Print("It is " + xyDist.x.ToString() + "," + xyDist.y.ToString() + " to " + nearest.GetName());   
        }

        if (Mathf.Abs(xyDist.x) > Mathf.Abs(xyDist.y))
        {
            if (xyDist.x < 0)
            {
                if (CheckCollision("left"))
                {
                    SetPosition(new Vector2(this.Position.x - 32, this.Position.y));
                }
            }
            else
            {
                if (CheckCollision("right"))
                {
                    SetPosition(new Vector2(this.Position.x + 32, this.Position.y));
                }
            }
        }
        else
        {
            if (xyDist.y < 0)
            {
                if (CheckCollision("up"))
                {
                    SetPosition(new Vector2(this.Position.x, this.Position.y - 32));
                }
            }
            else
            {
                if (CheckCollision("down"))
                {
                    SetPosition(new Vector2(this.Position.x, this.Position.y + 32));
                }
            }
        }

        newt.SetWaitTime(0.5f);
        newt.Start();
    }

    void combatBeginNextTurn()
    {
        c.BeginNextTurn();
    }

    private void InitializeRays()
    {
        for (int a = 1; a <= 8; a++)
        {
            rays[a - 1] = GetNode("npcRay" + a.ToString()) as RayCast2D;
            rays[a - 1].RotationDegrees = (a * 45);
            rays[a - 1].AddException(this.GetNode("Area2D"));
        }
    }

    public bool CheckAdjacentToPC()
    {
        for (int r = 0; r < rays.Length; r++)
        {
            if (rays[r].IsColliding())
            {
                var col = rays[r].GetCollider() as Node;
                //GD.Print(col);
                if (col.GetParent() == this) { }
                else
                {
                    if (g.combatants.Contains(col.GetParent() as Sprite))
                    {
                        GD.Print("collide with other: " + col.GetParent());
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //    public override void _Process(float delta)
    //    {
    //        // Called every frame. Delta is time since last frame.
    //        // Update game logic here.
    //        
    //    }
}

using Godot;
using System;

public enum npcType { enemy, shopKeep, talker }

public class npc : Sprite
{
    [Export]
    public npcType myType;
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
    public int initiative;
    
    public override void _Ready()
    {
        //register_property("hp", &npc::myType, 100);
        //SetMeta("actor_tag", actors.enemy);
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}

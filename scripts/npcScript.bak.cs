using Godot;
using System;

//public enum npcType { enemy, shopKeep, talker }

public class npcScript : Sprite
{
    [Export]
    public npcType myType;
    [Export]
    public string myName;
    [Export]
    public int ATK;
    [Export]
    public int DEF;
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
    
    public override void _Ready()
    {
        //register_property("hp", &npcScript::myType, 100);
        
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}

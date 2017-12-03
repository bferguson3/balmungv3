using Godot;
using System;

public enum npcType { enemy, shopKeep, talker }

public class npcScript : Sprite
{
    [Export]
    public npcType myType;
	[Export]
	int x = 0;

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

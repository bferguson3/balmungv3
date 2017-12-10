using Godot;
using System;

public class combatOps : Node
{
    public Timer combatWaitTimer;
    UI gui;
    player p;
    public void SetBattlePositions(battlePositions bpos, player pl, npc np)
    {
        if(bpos == battlePositions.standard)
        {
            //TODO
            //Properly position player and enemies. Atm splits +3/-3 squares.
            pl.SetPosition(np.GetPosition() + new Vector2(0, 98));
            np.SetPosition(np.GetPosition() + new Vector2(0, -98));
        }
    }
    public void SetupCombat(npc collidingWith)
    {
            SetBattlePositions(battlePositions.standard, p, collidingWith);
            gui.InitializeCombatUI();
            
            combatWaitTimer.Connect("timeout", this, "BeginNextTurn");
            combatWaitTimer.SetWaitTime(1.0f);
            combatWaitTimer.OneShot = true;
            combatWaitTimer.Start();
    }
    public void BeginNextTurn(){
        //Iterate through combatants
        //Determine initiative via DEX
        gui.UpdateCombatFeedback("ITS YOUR TURN");
    }
    public override void _Ready()
    {   
        
        gui = GetNode("../UI") as UI;
        p = GetNode("../playerSprite") as player;
        combatWaitTimer = new Timer();
        this.AddChild(combatWaitTimer);
    }

//    public override void _Process(float delta)
//    {
//    }
}

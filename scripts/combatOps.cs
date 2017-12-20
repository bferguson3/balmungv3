using Godot;
using System;

public class combatOps : Node
{
    public Timer combatWaitTimer;
    UI gui;
    player p;
    globals g;
    groundFX ground;
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
            //TODO: make this better
            g.combatants.Add(collidingWith);
            g.combatants.Add(p);
            foreach(Node n in g.combatants){
                if(n is player){
                    //player
                    var me = n as player;
                    me.actionWeight = 0;
                    me.turnTaken = false;
                }
                else if(n is npc){
                    //NPC
                    var me = n as npc;
                    me.actionWeight = 0;
                    me.turnTaken = false;
                }
            }

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
        //TODO: Fix this mess
        int highDex = 0;
        Node nextActor = p;
        foreach(Node n in g.combatants){
            if(n is player){
                var me = n as player;
                if(me.DEX - me.actionWeight >= highDex && !me.turnTaken){ //if my DEX is greater than or equal to last
                    highDex = me.DEX;
                    nextActor = n;
                }
                //player
            }
            else if(n is npc){
                var me = n as npc;
                if(me.DEX - me.actionWeight > highDex && !me.turnTaken){
                    highDex = me.DEX;
                    nextActor = n;
                }
                //NPC
            }
        }

        if(nextActor is player)
        {
            var me = nextActor as player;
            string f = me.myName;
            gui.UpdateCombatFeedback(f + "'s turn. \nCommand?");
            me.InitializeTurn();
            gui.InitializePlayerMenu();
            //DONT FORGET TO TOGGLE THE ACTEDTHISTURN VARIABLE AFTER ACTION??? or here?
        }
        else if(nextActor is npc)
        {
            var me = nextActor as npc;
            string f = me.myName;
            gui.UpdateCombatFeedback(f + "'s turn...\n ");
            //ENUM TYPE: MELEE, RANGED MONSTER.
            //IF MELEE, CHECK HP VS MORALE
            //IF HIGH ENOUGH, MOVE TOWARDS CLOSEST PC by x/y compare and collision check
            //IF WITHIN $8DIR, MELEE ATTACK INSTEAD
            //...IF RANGED, CHECK 8DIR, if false, RAYCAST TO NEARESTENEMY
            //IF BLOCKED, MOVE TOWARDS CLOSEST ENEMY
            //IF NOT BLOCKED, FIRE
            //TOGGLE ACTEDTHISTURN
        }

    }

    public void StartPlayerMovement()
    {
        g.inputMode = inputModes.combatMove;
        ground.DrawPlayerMoveZone();
        g.inputMode = inputModes.combatMove;
        gui.UpdateCombatFeedback("Moving.\nDestination?");
    }

    public void EndPlayerMovement()
    {
        ground.HidePlayerMoveZone();
        g.inputMode = inputModes.noInput;
        gui.UpdateCombatFeedback("");
        gui.HideSelectionText();
    }
    public override void _Ready()
    {   
        gui = GetNode("../UI") as UI;
        p = GetNode("../playerSprite") as player;
        ground = GetNode("../groundFX") as groundFX;
        g = GetNode("/root/globals") as globals;

        combatWaitTimer = new Timer();
        this.AddChild(combatWaitTimer);
    }

//    public override void _Process(float delta)
//    {
//    }
}

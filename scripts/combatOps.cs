using Godot;
using System;

public class combatOps : Node
{

    public Timer combatWaitTimer { get; set; }

    private UI gui;

    private player p;

    private globals g;

    private groundFX ground;
    
    Node battlemap;


 public void SetBattlePositions(battlePositions bpos, player pl, npc np)
    {
        if (bpos == battlePositions.standard)
        {
            //TODO
            //Properly position player and enemies. Atm splits +3/-3 squares.
            var p1pos = battlemap.GetNode("p1_combat_start") as Node2D;
            var e1pos = battlemap.GetNode("enemy_combat_start") as Node2D;
            pl.SetPosition(p1pos.GetGlobalPosition());
            np.SetPosition(e1pos.GetGlobalPosition());
        }
    }
    public void SetupCombat(npc collidingWith)
    {

            //TODO: make this better
            //TODO: figure out good way to determine battle background type
            battlemap = GetNode("../battlemap");
            g.combatants.Add(collidingWith);
            g.combatants.Add(p);
            foreach(Node n in g.combatants){
                if(n is player){
                    //player
                    var me = n as player;
                    me.actionWeight = 0;
                    me.turnTaken = false;
                }
                else if (n is npc)
               {
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
    
    public void BeginNextTurn()
    {
        //Iterate through combatants
        //Determine initiative via DEX
        //TODO: Fix this mess
        //GD.Print("Finding next highest speed who hasn't acted...");
        var highDex = 0;
        Node nextActor = g;

        foreach (Node n in g.combatants)
        {
            if (n is player)
            {
                var me = n as player;

                if (me.DEX - me.actionWeight >= highDex && !me.turnTaken)
                { //if my DEX is greater than or equal to last
                    highDex = me.DEX;
                    nextActor = n;
                }
                //player
            }
            else if (n is npc)
            {
                var me = n as npc;

                if (me.DEX - me.actionWeight > highDex && !me.turnTaken)
                {
                    highDex = me.DEX;
                    nextActor = n;
                }
                //NPC
            }
        }

        GD.Print("Next Turn: " + nextActor.GetName());

        if (nextActor is player)
        {
            var me = nextActor as player;
            var f = me.myName;

            gui.UpdateCombatFeedback(f + "'s turn. \nCommand?");
            me.InitializeTurn();
            gui.InitializePlayerMenu();
            //DONT FORGET TO TOGGLE THE ACTEDTHISTURN VARIABLE AFTER ACTION??? or here?
        }
        else if (nextActor is npc)
        {
            var me = nextActor as npc;
            var f = me.myName;

            gui.UpdateCombatFeedback(f + "'s turn...\n ");
            me.turnTaken = true;
            //ENUM TYPE: MELEE, RANGED MONSTER.
            //IF MELEE, CHECK HP VS MORALE

            if (me.aIType == aITypes.melee)
            {
                if (me.HP <= me.moraleScore)
                { //run away
                    me.fleeing = true; //for Virtue checks
                    me.MoveForEscape();
                }
                else //if I am brave
                {
                    if (me.CheckAdjacentToPC())
                    { //see if I'm near a player
                        me.AttackAdjacent(); //if I am, attack them
                    }
                    else
                    { //if I'm not
                        //GD.Print("attempting move");
                        me.MoveToClosestPC(); //move to the nearest one
                    }
                }
            }
            else if (me.aIType == aITypes.ranged)
            {
                //I am ranged, so
                //...IF RANGED, CHECK 8DIR, if false, RAYCAST TO NEARESTENEMY
                //IF BLOCKED, MOVE TOWARDS CLOSEST ENEMY
                //IF NOT BLOCKED, FIRE
            }
            //IF HIGH ENOUGH, MOVE TOWARDS CLOSEST PC by x/y compare and collision check
            //IF WITHIN $8DIR, MELEE ATTACK INSTEAD

            //TOGGLE ACTEDTHISTURN
        }
        else if (nextActor is globals)
        {
            GD.Print("resetting turn");

            foreach (Node j in g.combatants)
            {
                if (j is player)
                {
                    var me = j as player;
                    me.turnTaken = false;
                }
                else if (j is npc)
                {
                    var me = j as npc;
                    me.turnTaken = false;
                }
            }

            BeginNextTurn();
            return;
        }
    }

    public void StartPlayerMovement()
    {
        g.inputMode = inputModes.combatMove;
        ground.DrawPlayerMoveZone();
        //g.inputMode = inputModes.combatMove;
        gui.UpdateCombatFeedback("Moving.\nDestination?");
    }

    public void CancelPlayerMovement()
    {
        p.SetPosition(p.roundStartPos);
        g.inputMode = inputModes.combatCommand;
        ground.HidePlayerMoveZone();
        gui.UpdateCombatFeedback("Command?\n ");
        p.InitializeTurn();
    }

    public void EndPlayerMovement()
    {
        if (p.GetPosition() != p.roundStartPos)
        {
            ground.HidePlayerMoveZone();
            g.inputMode = inputModes.noInput;
            gui.UpdateCombatFeedback("");
            gui.HideSelectionText();
            p.actionWeight = 0;
            p.turnTaken = true;
            BeginNextTurn();
        }
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

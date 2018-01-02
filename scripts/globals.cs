using Godot;
using System;
using System.Collections.Generic;

public enum inputModes { noInput, moving, combatMove, combatCommand, oocMenuRoot, selectToTalk, convoListen, convoSpeak }
public enum battlePositions { standard, reversed, rtl, ltr, hasami, ambushed }
public enum actors { player, enemy }

public class globals : Node
{
    public bool acceptPressed,
                cancelPressed,
                leftPressed,
                rightPressed,
                upPressed,
                downPressed,
                playerBlockedDown,
                playerBlockedUp,
                playerBlockedLeft,
                playerBlockedRight,
                inCombat,
                acceptReleased = true,
                cancelReleased = true,
                leftReleased = true,
                rightReleased = true,
                upReleased = true,
                downReleased = true;

    public const string upButton = "ui_up",
                        leftButton = "ui_left",
                        rightButton = "ui_right",
                        downButton = "ui_down",
                        aButton = "ui_accept",
                        bButton = "ui_cancel";

    public inputModes inputMode;

    public bool inCombat;//, tryToFlee;
    public List<Sprite> combatants = new List<Sprite>();// combatants;
    public List<string> peopleMet = new List<string>();
    public List<string> globalKeywords = new List<string>();
    public List<string> currentVisibleKeywords = new List<string>();
    public Dictionary<string, string> privvyKeywords = new Dictionary<string, string>();

    public override void _Ready()
    {
        //TODO: deal with this over save files?
        globalKeywords.Add("name");
        globalKeywords.Add("job");
        //globalKeywords.Add("bye");
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }

    //    public override void _Process(float delta)
    //    {
    //        // Called every frame. Delta is time since last frame.
    //        // Update game logic here.
    //        
    //    }
}

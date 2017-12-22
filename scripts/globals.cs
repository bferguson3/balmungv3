using Godot;
using System;
using System.Collections.Generic;

public enum inputModes { noInput, moving, combatMove, combatCommand, oocMenuRoot, selectToTalk }
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

    public List<Sprite> combatants = new List<Sprite>();

    public override void _Ready()
    {

    }

    //    public override void _Process(float delta)
    //    {
    //        // Called every frame. Delta is time since last frame.
    //        // Update game logic here.
    //        
    //    }
}

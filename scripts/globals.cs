using Godot;
using System;

public enum inputModes { noInput, moving }
public enum battlePositions { standard, reversed, rtl, ltr, hasami, ambushed }

public class globals : Node
{
    public bool acceptPressed, cancelPressed, leftPressed, rightPressed, upPressed, downPressed;
    public bool acceptReleased = true, cancelReleased = true, leftReleased = true, rightReleased = true, upReleased = true, downReleased = true;

    public string upButton, leftButton, rightButton, downButton, aButton, bButton;
    public inputModes inputMode;

    public bool playerBlockedDown, playerBlockedUp, playerBlockedLeft, playerBlockedRight;

    public bool inCombat;

    public override void _Ready()
    {
        upButton = "ui_up";
        leftButton = "ui_left";
        rightButton = "ui_right";
        downButton = "ui_down";
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }

    
}

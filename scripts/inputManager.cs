using Godot;
using System;

public class inputManager : Node
{
    float currentTimer = 0;
    float initialRepeat = 0.750f;
    bool startRepeat, firstPress;
    float repeatTimer = 0.150f;
    string lastKey;
    bool repeatedOnce = false;

    player p;
    globals g;

    public override void _Ready()
    {
        g = GetNode("/root/globals") as globals;
        p = GetNode("../playerSprite") as player;
        g.inputMode = inputModes.moving;
    }

    void KeyAction(string key)
    {
        if(g.inputMode == inputModes.moving)
        {
            if(p.CheckCollision(key))
            {
                p.MoveThisSprite(key);
            }
        }
    }

    void FirstDepress(string key)
    {
        repeatedOnce = false;
        startRepeat = true;
        KeyAction(key);
    }

    public override void _PhysicsProcess(float delta)
    {
        UnpressAll();

        if(g.upReleased && Input.IsActionPressed(g.upButton))
        {
            lastKey = g.upButton;
            g.upPressed = true;
            g.upReleased = false;
            FirstDepress(lastKey);
        }
        else if (g.downReleased && Input.IsActionPressed(g.downButton))
        {
            lastKey = g.downButton;
            g.downPressed = true;
            g.downReleased = false;
            FirstDepress(lastKey);
        }
        else if (g.leftReleased && Input.IsActionPressed(g.leftButton))
        {
            lastKey = g.leftButton;
            g.leftPressed = true;
            g.leftReleased = false;
            FirstDepress(lastKey);
        }
        else if (g.rightReleased && Input.IsActionPressed(g.rightButton))
        {
            lastKey = g.rightButton;
            g.rightPressed = true;
            g.rightReleased = false;
            FirstDepress(lastKey);
        }

        if (startRepeat)
            currentTimer += delta;

        if (currentTimer >= initialRepeat)
        {
            if (repeatedOnce == false)
            {
                KeyAction(lastKey);
                repeatedOnce = true;
                currentTimer = 0;
            }
        }

        if(repeatedOnce && currentTimer > repeatTimer)
            RepeatPress(lastKey);

        if (!g.rightReleased && !Input.IsActionPressed(g.rightButton))
        {
            g.rightReleased = true;
            if(lastKey == g.rightButton){
                currentTimer = 0;
                startRepeat = false;
            }
        }
        if (!g.leftReleased && !Input.IsActionPressed(g.leftButton))
        {
            g.leftReleased = true;
            if (lastKey == g.leftButton)
            {
                currentTimer = 0;
                startRepeat = false;
            }
        }
        if (!g.upReleased && !Input.IsActionPressed(g.upButton))
        {
            g.upReleased = true;
            if (lastKey == g.upButton)
            {
                currentTimer = 0;
                startRepeat = false;
            
            }
        }
        if (!g.downReleased && !Input.IsActionPressed(g.downButton))
        {
            g.downReleased = true;
            if(lastKey == g.downButton)
            {
                currentTimer = 0;
                startRepeat = false;   
            }
        }
    }

    void RepeatPress(string key)
    {
        KeyAction(key);
        currentTimer = 0;
    }

    void UnpressAll()
    {
        g.upPressed = false;
        g.downPressed = false;
        g.leftPressed = false;
        g.rightPressed = false;
    }
}
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
    private Timer doubleProtTimer;

    player p;
    globals g;
    combatOps c;
    UI gui;
    private bool canPush = true;

    public override void _Ready()
    {
        g = GetNode("/root/globals") as globals;
        p = GetNode("../playerSprite") as player;
        c = GetNode("../combatOps") as combatOps;
        gui = GetNode("../UI") as UI;
        g.inputMode = inputModes.moving;

    }

    private void KeyOK(){
        GD.Print("ok");
        canPush = true;
    }

    void KeyAction(string key)
    {
        if(g.inputMode == inputModes.moving)
        {
            if(key == g.aButton){
                gui.OpenOOCMenu();
                g.inputMode = inputModes.oocMenuRoot;
            }
            if(p.CheckCollision(key))
            {
                p.MoveThisSprite(key);
            }
        }
        else if(g.inputMode == inputModes.combatMove){
            if(key == g.aButton){
                c.EndPlayerMovement();
            }
            else if(key == g.bButton)
            {
                c.CancelPlayerMovement();
            }
            else{
                if(p.CheckCollision(key) && p.CheckMoveBoundary(key)){
                    p.MoveThisSprite(key);
                }
            }
        }
        else if(g.inputMode == inputModes.combatCommand){
            if(key == g.upButton ||
                key == g.downButton ||
                key == g.leftButton ||
                key == g.rightButton){
                    gui.MoveCombatSel(key);
            }
            if(key == g.aButton)
            {
                gui.ConfirmCombatSel();
            }
            
        }
        else if(g.inputMode == inputModes.oocMenuRoot){
            if(key == g.aButton){
                gui.ConfirmOOCSel();
            }
            else if(key == g.bButton){
                gui.CloseOOCMenu();
                g.inputMode = inputModes.moving;
            }
        }
        else if(g.inputMode == inputModes.selectToTalk){
            if(key == g.bButton){
                gui.HideMapSelector();
                g.inputMode = inputModes.moving;
            }
            else if(key == g.aButton){
                gui.StartConversation();
            }
            else if(key == g.rightButton || key == g.upButton){
                gui.SelectNextOnMap(false);
            }
            else if(key == g.leftButton || key == g.downButton){
                gui.SelectNextOnMap(true);
            }
        }
    }

    void FirstDepress(string key)
    {
        repeatedOnce = false;
        startRepeat = true;
        canPush = false;
        KeyAction(key);
    }

    public override void _PhysicsProcess(float delta)
    {
        UnpressAll();

        if(g.acceptReleased && Input.IsActionPressed(g.aButton))
        {
            lastKey = g.aButton;
            g.acceptPressed = true;
            g.acceptReleased = false;
            FirstDepress(lastKey);
        }
        else if(g.cancelReleased && Input.IsActionPressed(g.bButton))
        {
            lastKey = g.bButton;
            g.cancelPressed = true;
            g.cancelReleased = false;
            FirstDepress(lastKey);
        }

        if(g.upReleased && Input.IsActionPressed(g.upButton))
        {
            lastKey = g.upButton;
            g.upPressed = true;
            g.upReleased = false;
            //if(canPush)
                FirstDepress(lastKey);
        }
        else if (g.downReleased && Input.IsActionPressed(g.downButton))
        {
            lastKey = g.downButton;
            g.downPressed = true;
            g.downReleased = false;
            //if(canPush)
                FirstDepress(lastKey);
        }
        else if (g.leftReleased && Input.IsActionPressed(g.leftButton))
        {
            lastKey = g.leftButton;
            g.leftPressed = true;
            g.leftReleased = false;
            //if(canPush)
                FirstDepress(lastKey);
        }
        else if (g.rightReleased && Input.IsActionPressed(g.rightButton))
        {
            lastKey = g.rightButton;
            g.rightPressed = true;
            g.rightReleased = false;
            //if(canPush)
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

        
        CheckDepress();

        //if(!canPush)
        //{
            //doubleProtTimer.SetWaitTime(0.05f);
            //doubleProtTimer.Start();
            //canPush = true;
        //}
       
    }

    void CheckDepress(){
         if(!g.acceptReleased && !Input.IsActionPressed(g.aButton))
        {
            g.acceptReleased = true;
            if(lastKey == g.aButton){
                currentTimer = 0;
                startRepeat = false;
            }
        }
        if(!g.cancelReleased && !Input.IsActionPressed(g.bButton))
        {
            g.cancelReleased = true;
            if(lastKey == g.bButton){
                currentTimer = 0;
                startRepeat = false;
            }
        }
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
        g.acceptPressed = false;
        g.cancelPressed = false;
        g.upPressed = false;
        g.downPressed = false;
        g.leftPressed = false;
        g.rightPressed = false;
    }
}

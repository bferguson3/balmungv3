using Godot;
using System;

public class UI : Sprite
{
    bool combatTransitionStart;
    Rect2 frame, blackSquare;
    Camera2D cam;
    Timer fadeTicker;//, combatWaitTimer;
    int fadeLoop = 0;
    int rectSize = 92;
    bool[,] fadeRects = new bool[22,22];
    Vector2 upLeft, bsqPos;
    public npc collidingWith;
    player p;
    globals g;
    Sprite combatUI;
    combatOps c;
    Label statLbl, menuLbl, feedbackLbl;

    public override void _Draw(){
       if(combatTransitionStart){
           for(int j = 0; j < fadeLoop; j++){
               for(int k = 0; k < fadeLoop; k++){
                   if(fadeRects[j, k] == true){
                       bsqPos = upLeft;
                       bsqPos.x += j * rectSize;
                       bsqPos.y += k * rectSize;
                       DrawRect(new Rect2(bsqPos, new Vector2(rectSize, rectSize)), new Color(0,0,0,1));
                   }
               }
           }
       }
    }

    public void StartFadeOut()
    {
        frame = cam.GetViewportRect();
        float leftBound = frame.Position.x - (frame.Size.x / 1.5f);
        float upBound = frame.Position.y - (frame.Size.y / 1.5f);
        upLeft = frame.Position;
        upLeft.x = leftBound;
        upLeft.y = upBound;

        blackSquare = new Rect2(upLeft, new Vector2(128, 128));
        
        combatTransitionStart = true;

        fadeTicker.Connect("timeout", this, "FadeTick");
        fadeTicker.SetWaitTime(0.075f);
        fadeTicker.Start();

        SetPosition(p.GetPosition() + new Vector2(-64, 0));
        Show();
    
    }

    public void StartFadeIn()
    {
        SetPosition(collidingWith.GetPosition() + new Vector2(120, 40));
        cam.Current = false;
        Camera2D battleCam = GetNode("combatUI/battleCam") as Camera2D;
        battleCam.Current = true;
        //TODO: Finish this animation
        c.SetupCombat(collidingWith);
    }

    public void FadeTick()
    {
        for(int n = 0; n < fadeLoop; n++){
                for(int c = fadeLoop; c >= 0; c--){
                    if(n+c <= fadeLoop)
                        fadeRects[n, c] = true;
                }
        }
        fadeLoop++;
        Update();
        if(fadeLoop > 21){
            fadeLoop = 0;
            combatTransitionStart = false;
            fadeTicker.Stop();
            //SetupCombat();
            StartFadeIn();
        }
    }

    public void InitializeCombatUI()
    {
        statLbl.Text = "Test PC\nHP: 100/100\nMP: 5/5";
        menuLbl.Text = "";
        feedbackLbl.Text = "ENCOUNTER!!";

        combatUI.Show();
    }

    public void UpdateCombatFeedback(string update)
    {
        feedbackLbl.Text = update;
    }
    
    public override void _Ready()
    {
        cam = GetNode("../playerSprite/Camera2D") as Camera2D;    
        p = GetNode("../playerSprite") as player;
        g = GetNode("/root/globals") as globals;
        combatUI = GetNode("combatUI") as Sprite;
        c = GetNode("../combatOps") as combatOps;

        statLbl = GetNode("combatUI/charStats") as Label;
        menuLbl = GetNode("combatUI/menuSelTxt") as Label;
        feedbackLbl = GetNode("combatUI/feedbackTxt") as Label;

        fadeTicker = new Timer();
        this.AddChild(fadeTicker);
    }

   public override void _Process(float delta)
    {
        
    }
}

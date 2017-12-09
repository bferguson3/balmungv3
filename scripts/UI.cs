using Godot;
using System;

public class UI : Sprite
{
    bool combatTransitionStart;
    Rect2 frame, blackSquare;
    Camera2D cam;
    Timer fadeTicker;
    int fadeLoop = 0;
    int rectSize = 92;
    bool[,] fadeRects = new bool[20,20];
    Vector2 upLeft, bsqPos;
    public npcScript collidingWith;
    player p;
    globals g;
    Sprite combatUI;

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
        float leftBound = frame.Position.x - (frame.Size.x / 1.8f);
        float upBound = frame.Position.y - (frame.Size.y / 1.8f);
        upLeft = frame.Position;
        upLeft.x = leftBound;
        upLeft.y = upBound;

        blackSquare = new Rect2(upLeft, new Vector2(128, 128));
        
        combatTransitionStart = true;

        fadeTicker = new Timer();
        this.AddChild(fadeTicker);
        fadeTicker.Connect("timeout", this, "FadeTick");
        fadeTicker.SetWaitTime(0.075f);
        fadeTicker.Start();

        SetPosition(p.GetPosition() + new Vector2(-64, 0));
        Show();
    
    }

    public void StartFadeIn()
    {
        SetPosition(collidingWith.GetPosition() + new Vector2(120, 40));
        cam.SetOffset(new Vector2(200, 40));
        SetupCombat();
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
        if(fadeLoop > 19){
            fadeLoop = 0;
            combatTransitionStart = false;
            fadeTicker.Stop();
            //SetupCombat();
            StartFadeIn();
        }
    }

    public void SetupCombat()
    {
            for(int v = 0; v < 4; v++)
                cam.SetDragMargin(v, 1);
            
            SetBattlePositions(battlePositions.standard, p, collidingWith);
           
            combatUI.Show();
    }

    private void SetBattlePositions(battlePositions bpos, player pl, npcScript np)
    {
        if(bpos == battlePositions.standard)
        {
            //TODO
            //Properly position player and enemies. Atm splits +3/-3 squares.
            pl.SetPosition(np.GetPosition() + new Vector2(0, 98));
            np.SetPosition(np.GetPosition() + new Vector2(0, -98));
        }
    }
    public override void _Ready()
    {
        cam = GetNode("../playerSprite/Camera2D") as Camera2D;    
        p = GetNode("../playerSprite") as player;
        g = GetNode("/root/globals") as globals;
        combatUI = GetNode("combatUI") as Sprite;
    }

   public override void _Process(float delta)
    {
        
    }
}

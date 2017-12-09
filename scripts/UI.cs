using Godot;
using System;

public class UI : Sprite
{
    bool drawTest;
    Rect2 frame, blackSquare;
    Camera2D cam;
    Timer fadeTicker;
    int fadeLoop = 0;
    int rectSize = 92;
    bool[,] fadeRects = new bool[20,20];
    Vector2 upLeft;
    public npcScript collidingWith;
    player p;
    globals g;

    public override void _Draw(){
       if(drawTest){
           for(int j = 0; j < fadeLoop; j++){
               for(int k = 0; k < fadeLoop; k++){
                   if(fadeRects[j, k] == true){
                       Vector2 placement = upLeft;
                       placement.x += j * rectSize;
                       placement.y += k * rectSize;
                       DrawRect(new Rect2(placement, new Vector2(rectSize, rectSize)), new Color(0,0,0,1));
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
        
        drawTest = true;

        fadeTicker = new Timer();
        this.AddChild(fadeTicker);
        fadeTicker.Connect("timeout", this, "TickHit");
        fadeTicker.SetWaitTime(0.075f);
        fadeTicker.Start();

        Show();
        SetPosition(collidingWith.GetPosition() + new Vector2(120, 40));
        
    }
    

    public void TickHit(){
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
            drawTest = false;
            fadeTicker.Stop();
            SetupCombat();
        }
    }
    public void SetupCombat()
    {
            cam.SetOffset(new Vector2(200, 40));
            cam.SetDragMargin(0, 1);
            cam.SetDragMargin(1, 1);
            cam.SetDragMargin(2, 1);
            cam.SetDragMargin(3, 1);
            
            SetBattlePositions(battlePositions.standard, p, collidingWith);
           
            Sprite n = GetNode("combatUI") as Sprite;
            n.Show();
    }

    private void SetBattlePositions(battlePositions bpos, player pl, npcScript np)
    {
        if(bpos == battlePositions.standard)
        {
            pl.SetPosition(np.GetPosition() + new Vector2(0, 98));
            np.SetPosition(np.GetPosition() + new Vector2(0, -98));
        }
    }
    public override void _Ready()
    {
        cam = GetNode("../playerSprite/Camera2D") as Camera2D;    
        p = GetNode("../playerSprite") as player;
        g = GetNode("/root/globals") as globals;
    }

   public override void _Process(float delta)
    {
        
    }
}

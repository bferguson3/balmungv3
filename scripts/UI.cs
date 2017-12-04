using Godot;
using System;

public class UI : Sprite
{
    bool drawTest;
    Rect2 frame, blackSquare;
    Camera2D cam;
    Timer fadeTicker;
    int fadeLoop;
    Rect2[] rects = new Rect2[200];
    Vector2 upLeft;

    public override void _Draw(){
       if(drawTest)
       {
           for(int i = 0; i < rects.Length; i++){
               DrawRect(rects[i], new Color(0,0,0,1));
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

        blackSquare = new Rect2(upLeft, new Vector2(64, 64));
        
        drawTest = true;

    }
    public override void _Ready()
    {
        cam = GetNode("../playerSprite/Camera2D") as Camera2D;    
    }

   public override void _Process(float delta)
    {
        if(drawTest){
            for(int i = fadeLoop; i >= 0; i--)
            {
                //rows to draw = fadeLoop
                //cols to draw = fadeloop + 1
                rects[i] = new Rect2(
                    new Vector2(upLeft.x, upLeft.y + (i*64)),
                     new Vector2(64*(i+1), 64));
                
            }
            Update();
            fadeLoop++;
        }
        if(fadeLoop >= 12){
            //drawTest = false;
            //fadeLoop = 0;
        }
    }
}

using Godot;
using System;

public class UI : Sprite
{
    public npc collidingWith;

    private bool combatTransitionStart;

    private Rect2 frame, blackSquare;

    private Camera2D cam;

    private Timer fadeTicker;//, combatWaitTimer;

    private int fadeLoop = 0, oocSelNo, mapSelNo;

    private int rectSize = 92;

    private bool[,] fadeRects = new bool[22, 22];

    private Vector2 upLeft, bsqPos;

    private player p;

    private globals g;

    private Sprite combatUI, combatSel, oocMenu, oocSel, dialogueWin, mapSel;

    private combatOps c;

    private Label statLbl, menuLbl, feedbackLbl, oocTxt;

    private int combatSelNo;

    public override void _Draw()
    {
        if (combatTransitionStart)
        {
            for (var j = 0; j < fadeLoop; j++)
            {
                for (var k = 0; k < fadeLoop; k++)
                {
                    if (fadeRects[j, k] == true)
                    {
                        bsqPos = upLeft;
                        bsqPos.x += j * rectSize;
                        bsqPos.y += k * rectSize;
                        DrawRect(new Rect2(bsqPos, new Vector2(rectSize, rectSize)), new Color(0, 0, 0, 1));
                    }
                }
            }
        }
    }

    public void StartFadeOut()
    {
        frame = cam.GetViewportRect();

        var leftBound = frame.Position.x - (frame.Size.x / 1.5f);
        var upBound = frame.Position.y - (frame.Size.y / 1.5f);

        upLeft = frame.Position;
        upLeft.x = leftBound;
        upLeft.y = upBound;

        blackSquare = new Rect2(upLeft, new Vector2(128, 128));

        combatTransitionStart = true;

        fadeTicker.Connect("timeout", this, "FadeTick");
        fadeTicker.SetWaitTime(0.075f);
        fadeTicker.Start();

        SetPosition(p.GetPosition() + new Vector2(-64, 0));
        //Show();
    }

    public void OpenOOCMenu()
    {
        oocMenu.Show();
        oocSel.Show();
        oocSelNo = 0;
        var cam = p.FindNode("Camera2D") as Camera2D;
        SetPosition(cam.GetCameraPosition());
        oocSel.SetPosition(new Vector2(oocSel.Position.x, -115f));
    }

    public void CloseOOCMenu()
    {
        oocMenu.Hide();
        oocSel.Hide();
    }

    public void StartFadeIn()
    {
        SetPosition(collidingWith.GetPosition() + new Vector2(120, 40));
        cam.Current = false;

        var battleCam = GetNode("combatUI/battleCam") as Camera2D;
        battleCam.Current = true;

        //TODO: Finish this animation
        c.SetupCombat(collidingWith);
    }

    public void FadeTick()
    {
        for (var n = 0; n < fadeLoop; n++)
        {
            for (var c = fadeLoop; c >= 0; c--)
            {
                if (n + c <= fadeLoop)
                {
                    fadeRects[n, c] = true;
                }
            }
        }

        fadeLoop++;
        Update();

        if (fadeLoop > 21)
        {
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

    public void InitializePlayerMenu()
    {
        menuLbl.Text = "Attack  Move\nCast     Use";
        combatSel.Show();
        g.inputMode = inputModes.combatCommand;
    }

    public void HideSelectionText()
    {
        menuLbl.Text = "";
        combatSel.Hide();
    }

    public void MoveCombatSel(string key)
    {
        if (key == g.upButton)
        {
            if (combatSelNo > 1)
            {
                combatSelNo = combatSelNo - 2;
            }
        }
        else if (key == g.downButton)
        {
            if (combatSelNo < 2)
            {
                combatSelNo = combatSelNo + 2;
            }
        }
        else if (key == g.rightButton)
        {
            if (combatSelNo == 0 || combatSelNo == 2)
            {
                combatSelNo++;
            }
        }
        else if (key == g.leftButton)
        {
            if (combatSelNo == 1 || combatSelNo == 3)
            {
                combatSelNo--;
            }
        }

        switch (combatSelNo)
        {
            //230,175 is efult pos
            case 0:
                combatSel.SetPosition(new Vector2(230, 175));
                break;
            case 1:
                combatSel.SetPosition(new Vector2(350, 175));
                break;
            case 2:
                combatSel.SetPosition(new Vector2(230, 235));
                break;
            case 3:
                combatSel.SetPosition(new Vector2(350, 235));
                break;
            default:
                GD.Print("Invalid combat selection.");
                break;
        }
    }

    public void ConfirmCombatSel()
    {
        if (combatSelNo == 1)
        {
            c.StartPlayerMovement();
        }
    }

    public void ConfirmOOCSel()
    {
        if (oocSelNo == 0)
        {
            g.inputMode = inputModes.selectToTalk;
            p.SelectAdjacentNPCs();
            oocMenu.Hide();
        }
    }

    public void UpdateCombatFeedback(string update)
    {
        feedbackLbl.Text = update;
    }

    public void MapSelect(Sprite target)
    {
        mapSel.Show();
        mapSel.SetGlobalPosition(target.GetGlobalPosition());
    }

    public override void _Ready()
    {
        Show();
        cam = GetNode("../playerSprite/Camera2D") as Camera2D;
        p = GetNode("../playerSprite") as player;
        g = GetNode("/root/globals") as globals;
        c = GetNode("../combatOps") as combatOps;

        oocMenu = GetNode("oocMenu") as Sprite;
        oocSel = GetNode("oocMenu/oocSel") as Sprite;
        oocTxt = GetNode("oocMenu/menuTxt") as Label;
        dialogueWin = GetNode("dialogueWin") as Sprite;
        mapSel = GetNode("mapSelector") as Sprite;

        combatUI = GetNode("combatUI") as Sprite;
        combatSel = GetNode("combatUI/selector") as Sprite;
        combatSelNo = 0;

        statLbl = GetNode("combatUI/charStats") as Label;
        menuLbl = GetNode("combatUI/menuSelTxt") as Label;
        feedbackLbl = GetNode("combatUI/feedbackTxt") as Label;

        combatUI.Hide();
        oocMenu.Hide();
        dialogueWin.Hide();

        fadeTicker = new Timer();
        this.AddChild(fadeTicker);
    }

    public void HideMapSelector()
    {
        mapSel.Hide();
    }

    public override void _Process(float delta)
    {

    }

    public void StartConversation()
    {

    }

    public void SelectNextOnMap(bool backwards)
    {
        if (!backwards)
        {
            mapSelNo++;
        }
        else
        {
            mapSelNo--;
        }

        if (mapSelNo < 0)
        {
            mapSelNo = p.targets.Count - 1;
        }

        if (mapSelNo > p.targets.Count - 1)
        {
            mapSelNo = 0;
        }

        MapSelect(p.targets[mapSelNo] as Sprite);
    }
}

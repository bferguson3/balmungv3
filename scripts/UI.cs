using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class UI : Sprite
{
    bool combatTransitionStart;
    Rect2 frame, blackSquare;
    Camera2D cam;
    Timer fadeTicker;//, combatWaitTimer;
    int fadeLoop = 0, oocSelNo, mapSelNo;
    int rectSize = 92;
    bool[,] fadeRects = new bool[22, 22];
    Vector2 upLeft, bsqPos,
        oocSelBasePos = new Vector2(-203,-115);
    public npc collidingWith;
    player p;
    globals g;
    Sprite combatUI, combatSel, oocMenu, oocSel, dialogueWin, mapSel;
    combatOps c;
    Label statLbl, menuLbl, feedbackLbl, oocTxt, dialogueTxt;
    private int combatSelNo, oocMaxSel, dbOffset;
    npc currentSpeaker;
    string[] scrip;
    private List<string> newTopics = new List<string>();

    public override void _Draw()
    {
        if (combatTransitionStart)
        {
            for (int j = 0; j < fadeLoop; j++)
            {
                for (int k = 0; k < fadeLoop; k++)
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
        //SetPosition(collidingWith.GetPosition() + new Vector2(120, 40));
        //SET MY POSITION TO BATTLEMAP
        var gpos = GetNode("../battlemap/gui_combat_pos") as Node2D;
        SetPosition(gpos.GetGlobalPosition());

        cam.Current = false;
        Camera2D battleCam = GetNode("combatUI/battleCam") as Camera2D;
        battleCam.Current = true;
        //TODO: Finish this animation
        c.SetupCombat(collidingWith);
    }

    public void FadeTick()
    {
        for (int n = 0; n < fadeLoop; n++)
        {
            for (int c = fadeLoop; c >= 0; c--)
            {
                if (n + c <= fadeLoop)
                    fadeRects[n, c] = true;
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
        dialogueTxt = GetNode("dialogueWin/dialogueTxt") as Label;
        mapSel = GetNode("mapSelector") as Sprite;

        combatUI = GetNode("combatUI") as Sprite;
        combatSel = GetNode("combatUI/selector") as Sprite;
        combatSelNo = 0;

        statLbl = GetNode("combatUI/charStats") as Label;
        menuLbl = GetNode("combatUI/menuSelTxt") as Label;
        feedbackLbl = GetNode("combatUI/feedbackTxt") as Label;

        scrip = System.IO.File.ReadAllLines("./scripts/dialogue.db");

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

    private void RunConvoAction(string command)
    {
        newTopics.Clear();
        //string comm = command.Substring(2);
        if (command.ToUpper().Contains("**NEW"))
        {
            //g.currentVisibleKeywords.Clear();
            string comm = command.substr(command.IndexOf(':'), (command.Length - command.IndexOf(':')));
            do
            {
                comm = FindNextKeyword(comm, true);
            }
            while (comm.Length > 1);
            NewConvoTree(newTopics);
        }
        else if(command.ToUpper().Contains("**ADD"))
        {
            string comm = command.substr(command.IndexOf(':'), (command.Length - command.IndexOf(':')));
            do
            {
                comm = FindNextKeyword(comm, false);
                //AddKeyword();
            }
            while (comm.Length > 1);
        }

    }

    private string FindNextKeyword(string comm, bool newBox)
    {
        int aa = 0;
        int bb = 0;
        if (comm.Contains(","))
        {
            //I contain plural tree choices!
            foreach (char n in comm)
            {
                if (n == ',')
                {
                    break;
                }
                if (n != ' ' && n != ':')
                {
                    bb++;
                }
                else
                    aa++;
            }
            if(newBox == false)
                //AddKeywordToMenu(comm.substr(aa, bb));
                g.currentVisibleKeywords.Add(comm.substr(aa, bb));
            else
                newTopics.Add(comm.substr(aa, bb));
            
            GD.Print(comm.substr(aa, bb) + " added to new keywords");
            comm = comm.Substring(aa + bb + 1);
        }
        else
        {
            aa = 0;
            foreach (char b in comm)
            {
                if (b == ',' || b == ' ' || b == ':')
                    aa++;
            }
            if(newBox == false)
                g.currentVisibleKeywords.Add(comm.Substring(aa));
            else
                newTopics.Add(comm.Substring(aa));
            
            GD.Print(comm.Substring(aa) + " added to new keywords");
            comm = "";
        }
        
        return comm;
    }

    private void NewConvoTree(List<string> topics)
    {
        oocTxt.Text = "";
        oocMaxSel = -1;
        oocSelNo = 0;
        oocSel.Position = oocSelBasePos;
        foreach(string c in topics){
            var c2 = c.substr(0,1).ToUpper();
            oocTxt.Text += c2 + c.Substring(1) + "\n";
            oocMaxSel++;
        }
    }

    public void StartConversation()
    {
        HideMapSelector();
        //GLOBALS KEYWORDS: (firstmeet), (hail), name, job, bye
        //After the line of dialogue has been grabbed,
        //Check the NEXT line
        //and make sure it doesn't contain a **.
        //If it does, perform action.
        g.inputMode = inputModes.convoListen;
        currentSpeaker = p.targets[mapSelNo] as npc;
        dialogueWin.Show();
        InitializeKeywords();
        bool npcfound = false;
        for (int c = 0; c < scrip.Length; c++)
        {
            if (scrip[c].Contains("NPC"))
            {
                string npcname = scrip[c].substr(scrip[c].IndexOf(':') + 1, scrip[c].Length - scrip[c].IndexOf(':') - 1);
                if (npcname == currentSpeaker.myName)
                {
                    npcfound = true;
                    dbOffset = c;
                    GD.Print("speaker dialogue found.");
                }
            }
            if (npcfound)
            {
                if (!g.peopleMet.Contains(currentSpeaker.myName))
                {
                    if (scrip[c].to_lower().Contains("firstmeet"))
                    { //KEYWORD
                        dialogueTxt.Text = scrip[c].substr(scrip[c].IndexOf(':') + 1, scrip[c].Length - scrip[c].IndexOf(':') - 1);
                        for (int j = 1; j < 9; j++)
                        {
                            //Check up to 9 rows ahead for commands.
                            if (scrip[c + j].Contains("**"))
                            {
                                RunConvoAction(scrip[c + j]);
                            }
                            else
                                break;
                        }
                        break;
                    }
                }
                else //AREADY MET PERSON CODE:
                {
                    if (scrip[c].to_lower().Contains("hail"))
                    { //KEYWORD
                        dialogueTxt.Text = scrip[c].substr(scrip[c].IndexOf(':') + 1, scrip[c].Length - scrip[c].IndexOf(':') - 1);
                        for (int j = 1; j < 9; j++)
                        {
                            //Check up to 9 rows ahead for commands.
                            if (scrip[c + j].Contains("**"))
                            {
                                RunConvoAction(scrip[c + j]);
                            }
                            else
                                break;
                        }
                        break;
                    }
                }
            }
        }
        oocMenu.Show();
        //TODO: Make another < indicator for dialogue
        var newoocselpos = new Vector2(160, 125);
        oocSel.Position = newoocselpos;
        //TODO:
        //add firstmeet to save
        //if any global keywords or switches are added, add to save
    }

    public void ConvoSel(string key)
    {
        var oocselpos = oocSel.Position;
        if (key == g.downButton)
        {
            if (oocSelNo < oocMaxSel)
            {
                oocSelNo++;
                oocselpos.y += 19f;
            }
        }
        else if (key == g.upButton)
        {
            if (oocSelNo > 0)
            {
                oocSelNo--;
                oocselpos.y -= 19f;
            }
        }
        else if (key == g.aButton) //Selected keyword.
        {
            if(newTopics.Count == 0){
                if(g.currentVisibleKeywords.Count == oocSelNo){
                    GD.Print("Selected keyword: 'Bye'");
                }
                else
                    GD.Print("Selected keyword:" + g.currentVisibleKeywords[oocSelNo]);
            }
            else
                GD.Print("Selected keyword:" + newTopics[oocSelNo]);
        }
        oocSel.SetPosition(oocselpos);
    }

    private void AddKeywordsToMenu()
    {
        oocMaxSel++;
        oocTxt.Text = "";
        foreach(string n in g.currentVisibleKeywords){
            var n2 = n.substr(0, 1).ToUpper() + n.Substring(1);
            oocTxt.Text += n2 + "\n";
        }
        oocTxt.Text += "Bye";
    }

    private void InitializeKeywords()
    {
        g.currentVisibleKeywords.Clear();
        oocMaxSel = 0;
        oocTxt.Text = "";
        foreach (string n in g.globalKeywords)
        {
            var n2 = n.substr(0, 1).ToUpper() + n.Substring(1);
            oocTxt.Text += n2 + "\n";
            oocMaxSel++;
            g.currentVisibleKeywords.Add(n);
        }
        if (g.privvyKeywords.ContainsKey(currentSpeaker.myName))
        {
            //TODO:Second-hand knowledge of this person is known, 
            //and the relevant keyword should be added automatically.
            //And maxsel inc.
            //g.currentVisibleKeywords.Add(g.privvyKeywords[currentSpeaker.myName]);
        }
        //if (g.currentVisibleKeywords.Count != 0)
        //{
        //current speaker's non-privvy, non-global keywords.
        //    foreach (string m in g.currentVisibleKeywords)
        //    {
        //        var m2 = m.substr(0, 1).ToUpper() + m.Substring(1);
        //        oocTxt.Text += m2 + "\n";
        //        oocMaxSel++;
        //    }
        //}
        //g.currentVisibleKeywords.Add("Bye");
        oocTxt.Text += "Bye";
        //oocMaxSel++;
        //oocMaxSel++; < not needed since it starts at 0
        //TODO: Add support for scrolling.
    }

    public void AdvanceConvo()
    {
        //FIRST, CHECK TO SEE IF HTE LINE IS DONE.
        //TODO: Add check for extra long strings.

        //if line is done and ready to ask question:
        //Also, add any keywords NOW to visible list that may be new.
        //OR: change to new tree.
        if(newTopics.Count == 0)
            AddKeywordsToMenu();
        oocSel.Position = oocSelBasePos;
        g.inputMode = inputModes.convoSpeak;
        
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
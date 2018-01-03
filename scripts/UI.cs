using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class UI : Sprite
{
    bool combatTransitionStart;
    Rect2 frame, blackSquare;
    private string dialogueReturnTo;
    Camera2D cam;
    Timer fadeTicker;//, combatWaitTimer;
    int fadeLoop = 0, oocSelNo, mapSelNo;
    int rectSize = 92;
    bool[,] fadeRects = new bool[22, 22];
    Vector2 upLeft, bsqPos,
        oocSelBasePos = new Vector2(-442,-244);
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
        
    }

    public void OpenOOCMenu()
    {
        oocTxt.Text = "Talk\nItems\nMagic\nStatus\nOpen\nAttack\nCamp\nSystem";
        oocMenu.Show();
        oocSel.Show();
        oocSelNo = 0;
        //TODO: Add support for saving last menu selection, possibly.
        var cam = p.FindNode("Camera2D") as Camera2D;
        SetPosition(cam.GetCameraPosition());
        oocSel.SetPosition(oocSelBasePos);
        oocSel.SetRotationDegrees(0f);
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
            oocSel.Hide();
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
        oocSel = GetNode("oocSel") as Sprite;
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

    //public override void _Process(float delta)
    //{

    //}

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
        else if(command.ToUpper().Contains("**RETURN")) //Bring me back to another tree!
        {
            string comm = command.substr(command.IndexOf(':'), (command.Length - command.IndexOf(':')));
            comm = TrimForKeyword(comm);
            //SpeakLine(comm);
            dialogueReturnTo = comm;
        }

    }

    private string TrimForKeyword(string trimstr)
    {
        //Trim ', :, " etc off of keywords to parse into SpeakLine()
        var bb = 0;
        foreach(char m in trimstr)
        {
            if(m == ':' || m == ' ' || m == '"' || m == '\'')
            {
                bb++;
            }
        }
        GD.Print("Returning, using kw:" + trimstr.Substring(bb));
        return trimstr.Substring(bb);
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

    private void CheckCommands(int c)
    {
        bool returnline = false;
        for (int j = 1; j < 9; j++)
        {
           //Check up to 9 rows ahead for commands.
            if (scrip[c + j].Contains("**"))
            {
                RunConvoAction(scrip[c + j]);
                if(scrip[c+j].ToUpper().Contains("RETURN")){
                    returnline = true;
                }
            }
            else
                break;
        }
        if(!returnline && newTopics.Count > 0)
        {
            dialogueReturnTo = "ROOT";
        }
    }

    private void DialogueTreeReturn(string line)
    {
        GD.Print("Returning to " + line);
        if(line.ToUpper() == "ROOT")
        {
            newTopics.Clear();
            InitializeKeywords();
        }
        else
        {
            //TODO: 
            //Here, search through lines to find the kw '$line'
            //Then act like npc was just asked about that kw.
            
        }
    }

    public void StartConversation()
    {
        HideMapSelector();
        newTopics.Clear();
        //GLOBALS KEYWORDS: (firstmeet), (hail), name, job, bye
        //After the line of dialogue has been grabbed,
        //Check the NEXT line
        //and make sure it doesn't contain a **.
        //If it does, perform action.
        //oocSelNo = 0;
        //oocSel.Position = oocSelBasePos;
        g.inputMode = inputModes.convoListen;
        currentSpeaker = p.targets[mapSelNo] as npc;
        dialogueWin.Show();
        oocSel.Show();
        InitializeKeywords();
        GD.Print("Keywords initialized OK");
        dialogueReturnTo = "";
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
                    if (scrip[c].to_lower().Contains("'firstmeet'"))
                    { //KEYWORD
                        dialogueTxt.Text = scrip[c].substr(scrip[c].IndexOf(':') + 1, scrip[c].Length - scrip[c].IndexOf(':') - 1);
                        CheckCommands(c);
                        break;
                    }
                }
                else //AREADY MET PERSON CODE:
                {
                    if (scrip[c].to_lower().Contains("'hail'"))
                    { //KEYWORD
                        dialogueTxt.Text = scrip[c].substr(scrip[c].IndexOf(':') + 1, scrip[c].Length - scrip[c].IndexOf(':') - 1);
                        CheckCommands(c);
                        break;
                    }
                }
            }
        }
        
        //TODO: Make another < indicator for dialogue, make this efficient(static)
        MoveSelectorToDialogueWindow();
        //TODO:
        //add firstmeet to save
        //if any global keywords or switches are added, add to save
    }

    private void MoveSelectorToDialogueWindow()
    {
        var newoocselpos = new Vector2(373, 280); //DialogueWin pos
        var newrot = 90f;
        oocSel.Position = newoocselpos;
        oocSel.SetRotationDegrees(newrot);
    }

    public void ConvoSel(string key)
    {
        var oocselpos = oocSel.Position;
        var spacing = 38f;
        if (key == g.downButton)
        {
            if (oocSelNo < oocMaxSel)
            {
                oocSelNo++;
                oocselpos.y += spacing;
            }
        }
        else if (key == g.upButton)
        {
            if (oocSelNo > 0)
            {
                oocSelNo--;
                oocselpos.y -= spacing;
            }
        }
        else if (key == g.aButton) //Selected keyword.
        {
            if(newTopics.Count == 0){
                if(g.currentVisibleKeywords.Count == oocSelNo){
                    
                    GD.Print("Selected keyword: 'Bye'");
                    SpeakLine("BYE"); //Single quotes around keywords!
                    g.inputMode = inputModes.convoEnding;
                    oocMenu.Hide();
                    MoveSelectorToDialogueWindow();
                }
                else{
                    GD.Print("Selected keyword:" + g.currentVisibleKeywords[oocSelNo]);
                    SpeakLine(g.currentVisibleKeywords[oocSelNo]);
                }
            }
            else{
                GD.Print("Selected NEW keyword:" + newTopics[oocSelNo]);
                SpeakLine(newTopics[oocSelNo]);
                DialogueTreeReturn(dialogueReturnTo);
            }
        }
        oocSel.SetPosition(oocselpos);
    }

    private void SpeakLine(string line)
    {
        //TODO: Add a "is this line too big?" check, and pagination.

        //TODO: Fix this loop for COUNTER based on dboffset
        for(int a = dbOffset; a < scrip.Length; a++) //99 is max lines for 1 npc atm.
        {
            //if(scrip[a].ToUpper().Contains("NPC:")){
            //    SpeakLine("ELSE");
            //    return;
            //}
            if(scrip[a].ToUpper().Contains("'"+line.ToUpper()+"'"))
            {
                GD.Print("What's that? You asked about: " + line);
                GD.Print(scrip[a]);
                int aa = 0;
                string subs = scrip[a].Substring(scrip[a].IndexOf(':'));
                foreach(char n in subs){
                    if(n == ' ' || n == ':')
                    {
                        aa++;
                    }
                    else
                        break;
                }
                
                dialogueTxt.Text = subs.Substring(aa);
                CheckCommands(a);
                return;
            }
        }
        //Getting this far means it was not found.
       SpeakLine("ELSE");
    }

    public void EndConvo()
    {
        dialogueWin.Hide();
        g.inputMode = inputModes.moving;
        oocSel.Hide();
        //oocMaxSel = 0;
        //oocSelNo = 0;
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
        if(!oocMenu.Visible)
            oocMenu.Show();
        //if line is done and ready to ask question:
        //Also, add any keywords NOW to visible list that may be new.
        //OR: change to new tree.
        if(newTopics.Count == 0)
            AddKeywordsToMenu();
        
        //oocSel = 0;
        //TODO:Fix the selection numbering here
        oocSel.Position = oocSelBasePos;
        oocSel.SetRotationDegrees(0f);
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
using Godot;
using System;
using System.Collections.Generic;

public class dialogue : Node {

    //First, check for GLOBAL KEYWORDS
    //populate global keywords to list
    //Then check for NPC-Specific keywords
    //secret keywords are words that are given elsewhere.
    //they need to be flagged with a global flag.
    

    //
    //Chef:Yes, I cook all day long.:All day
    //All day:What's it to you? You a cop?:Cop
    //Cook:It's hard work.:Work
    //Cop:I knew it, scram, copper.:
    //Work:I told you, I'm a chef.:
    //Bye:Farewell, Avatar.:
    //if no 2nd : found or after : is "" or " " then break

    //secret keywords: <secretkeyword>:bool for enabled
    //if no : or :0 then consider as hidden

}


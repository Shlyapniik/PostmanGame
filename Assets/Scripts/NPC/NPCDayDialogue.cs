using System;
using UnityEngine;

[Serializable]
public class NPCDayDialogue
{
    [Min(1)]
    public int day = 1;

    public DialogueData dialogue;
}
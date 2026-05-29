using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : NPC
{
    
    public DialogueData secondData;

    public void ClearQuest()
    {
        dialogueData = secondData;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObj/Dialogue")]
public class DialogueData : ScriptableObject
{
    public string speaker;
    public List<DialogueBlock> dialogues = new List<DialogueBlock>();
}

[Serializable]
public class DialogueBlock
{
    public List<string> npcLines;
    public List<PlayerChoice> choices = new List<PlayerChoice>();
}

[Serializable]
public class PlayerChoice
{
    public string choiceText;
    public int nextDialogueID;
}
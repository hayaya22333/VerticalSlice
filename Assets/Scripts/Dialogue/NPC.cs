using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public DialogueData dialogueData;
    public DialogueData otherDialogue;
    public int dialogueState = 0;
    public string _name;

    [SerializeField] GameObject talkHint;

    public void Talked()
    {
        if (dialogueState == 0)
        {
            dialogueState = 1;
        }
    }

    public void SetDialogueState(int x)
    {
        dialogueState = x;
    }

    void Update()
    {
        if (dialogueState == 0)
        {
            talkHint.SetActive(true);
        }
        else
        {
            talkHint.SetActive(false);
        }
    }

    public void ClearQuest()
    {
        dialogueData = otherDialogue;
        dialogueState = 0;
    }
}
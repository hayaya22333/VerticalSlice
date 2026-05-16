using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public DialogueData dialogueData;
    public int dialogueState = 0;

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
}
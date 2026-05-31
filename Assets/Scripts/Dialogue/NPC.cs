using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public List<DialogueData> dialogueList = new List<DialogueData>();
    public DialogueData dialogueData;
    public int dialogueState = 0;
    public int dialogueStage = 0;
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

    void Start()
    {
        Locator.Instance.Player.CheckQuest += HandleCheckQuest;
    }

    void Update()
    {
        dialogueData = dialogueList[dialogueStage];
        
        if (dialogueState == 0)
        {
            talkHint.SetActive(true);
        }
        else
        {
            talkHint.SetActive(false);
        }
    }

    void HandleCheckQuest(int _killCount)
    {
        if (gameObject.name != "QuestGiver") return;
        if (_killCount >= 5)
        {
            dialogueStage = 2;
            dialogueState = 0;
        }
        else if (_killCount >= 1)
        {
            if (dialogueStage == 1) return;
            dialogueStage = 1;
            dialogueState = 0;
        }
    }
}
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
        _name = gameObject.name;
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

    // Dialogue behavior -------------------------------------------------------
    [SerializeField] private float rotateSpeed = 360f;

    private Coroutine lookAtPlayerCoroutine;

    public void LookAtPlayerSmooth()
    {
        if (lookAtPlayerCoroutine != null)
        {
            StopCoroutine(lookAtPlayerCoroutine);
        }

        lookAtPlayerCoroutine = StartCoroutine(LookAtPlayerSmoothRoutine());
    }

    private IEnumerator LookAtPlayerSmoothRoutine()
    {
        Transform playerTransform = Locator.Instance.Player.transform;

        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            yield break;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.rotation = targetRotation;
    }
}
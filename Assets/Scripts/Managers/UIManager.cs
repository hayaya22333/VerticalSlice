using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private float woundDuration = 1f;
    private float woundProgression;

    [Header("General UI")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI expProgressionText;
    [SerializeField] GameObject lvlUpTextObject;
    [SerializeField] PlayerController player;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI loadedAmmoText;
    [SerializeField] GameObject woundOverlay;

    [Header("Death UI")]
    [SerializeField] GameObject deathUI;

    [Header("Dialoge UI")]
    [SerializeField] GameObject dialogueUI;

    void Start()
    {
        player = Locator.Instance.Player;
        player.LevelUp += HandleLevelUp;
        player.KilledPlayer += HandlePlayerDeath;
        player.PlayerTookDamage += HandlePlayerDamage;
        player.RunDialogue += HandleRunDialogue;
    }

    void Update()
    {
        expProgressionText.text = player.playerEXP + "/" + player.maxEXP;
        hpText.text = "HP: " + player.hp;
        loadedAmmoText.text = "ammo: " + player.loadedAmmo + "/" + player.maxAmmo;
        ammoText.text = player.ammo.ToString();

        if (woundProgression > 0)
        {
            woundProgression -= Time.deltaTime;
            if (woundProgression <= 0)
            {
                woundOverlay.SetActive(false);
            }
        }

        // Move this to player controller.

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            dialogueUI.SetActive(false);
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void HandleLevelUp(int lvl)
    {
        levelText.text = "Level " + lvl;
        Instantiate(lvlUpTextObject,
                    levelText.gameObject.transform.parent);
    }

    void HandlePlayerDamage(int i)
    {
        woundOverlay.SetActive(true);
        woundProgression = woundDuration;
    }

    void HandlePlayerDeath()
    {
        deathUI.SetActive(true);
    }

    void HandleRunDialogue()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        dialogueUI.SetActive(true);
    }
}

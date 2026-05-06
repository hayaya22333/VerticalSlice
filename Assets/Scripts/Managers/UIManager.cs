using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("General UI")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI expProgressionText;
    [SerializeField] GameObject lvlUpTextObject;
    [SerializeField] PlayerController player;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI loadedAmmoText;

    [Header("Death UI")]
    [SerializeField] GameObject deathUI;

    void Start()
    {
        player = Locator.Instance.Player;
        player.LevelUp += HandleLevelUp;
        player.KilledPlayer += HandlePlayerDeath;
    }

    void Update()
    {
        expProgressionText.text = player.playerEXP + "/" + player.maxEXP;
        hpText.text = "HP: " + player.hp;
        loadedAmmoText.text = player.loadedAmmo + "/" + player.maxAmmo;
        ammoText.text = player.ammo.ToString();
    }

    void HandleLevelUp(int lvl)
    {
        levelText.text = "Level " + lvl;
        Instantiate(lvlUpTextObject,
                    levelText.gameObject.transform.parent);
    }

    void HandlePlayerDeath()
    {
        deathUI.SetActive(true);
    }
}

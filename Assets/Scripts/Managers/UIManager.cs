using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI expProgressionText;
    [SerializeField] GameObject lvlUpTextObject;
    [SerializeField] PlayerController player;
    [SerializeField] TextMeshProUGUI hpText;

    void Start()
    {
        player = Locator.Instance.Player;
        player.LevelUp += HandleLevelUp;
    }

    void Update()
    {
        expProgressionText.text = player.playerEXP + "/" + player.maxEXP;
        hpText.text = "HP: " + player.hp;
    }

    void HandleLevelUp(int lvl)
    {
        levelText.text = "Level " + lvl;
        Instantiate(lvlUpTextObject,
                    levelText.gameObject.transform.parent);
    }
}

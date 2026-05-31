using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : MonoBehaviour
{
    private float woundDuration = 1f;
    private float woundProgression;

    [SerializeField] PlayerController player;

    [Header("General")]
    [SerializeField] GameObject generalUI;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI expProgressionText;
    [SerializeField] GameObject lvlUpTextObject;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI loadedAmmoText;
    [SerializeField] GameObject woundOverlay;
    [SerializeField] GameObject endUI;

    [Header("Temporary")]
    [SerializeField] GameObject talkHint;
    [SerializeField] GameObject itemPopup;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemCount;
    List<Item> displayItemQue = new List<Item>();
    [SerializeField] GameObject NoBullet;
    [SerializeField] GameObject NoAmmo;


    [Header("Death")]
    [SerializeField] GameObject deathUI;

    [Header("Dialogue")]
    [SerializeField] GameObject dialogueUI;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] List<GameObject> choiceButtons;
    private DialogueBlock activeDialogue;
    private DialogueData dialogueData;
    private int dialogueIndex;

    [Header("Shop")]
    public ShopData shopData;
    public TextMeshProUGUI moneyText;
    public GameObject shopUI;
    public GameObject buyUI;
    public GameObject sellUI;
    public List<GameObject> shopButtons;
    public List<GameObject> sellButtons;
    public List<ShopItem> shopItems = new List<ShopItem>();

// Start/Update *************************************************************************************************************
    #region Start/Update
    void Awake()
    {
        talkHint.SetActive(false);
        DisableChoices();

        foreach (ShopItem item in shopData.items)
        {
            ShopItem copy = new ShopItem
            {
                name = item.name,
                price = item.price,
                stock = item.stock
            };

            shopItems.Add(copy);
        }
    }

    void Start()
    {
        // Game
        player = Locator.Instance.Player;
        player.LevelUp += HandleLevelUp;
        player.KilledPlayer += HandlePlayerDeath;
        player.PlayerTookDamage += HandlePlayerDamage;
        player.EmptyShot += HandleNoBullet;
        player.NoAmmo += HandleNoAmmo;
        player.EndGame += HandleGameEnd;

        // NPC Interaction
        player.RunDialogue += HandleRunDialogue;
        player.AllowTalk += HandleAllowTalk;
        player.NoTalk += HandleNoTalk;
        player.CloseDialogue += CloseDialogue;

        // Item
        player.GetItem += HandleGetItem;
    }

    void Update()
    {
        ShowGenStats();
        ShowAmmo();

        if (woundProgression > 0)
        {
            woundProgression -= Time.deltaTime;
            if (woundProgression <= 0)
            {
                woundOverlay.SetActive(false);
            }
        }

        if (displayItemQue.Count != 0)
        {
            TryDisplayGetItem();
        }

        // Dialogue *************************************************************************
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (player.playerState != PlayerState.Talking) return;
            TryNextLine();
        }
    }
    #endregion


    void ShowAmmo()
    {
        loadedAmmoText.text = player.loadedAmmo + "/" + player.maxAmmo;
        ammoText.text = player.ammo.ToString();
    }

    void ShowGenStats()
    {
        expProgressionText.text = "EXP: " + player.playerEXP + "/" + player.maxEXP;
        if (player.hp > 0)
        {
            hpText.text = "HP: " + player.hp;
        }
        else
        {
            hpText.text = "HP: 0";
        }
    }



// Game *********************************************************************************************************************
    #region Game
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

    void HandleGameEnd()
    {
        endUI.SetActive(true);
        generalUI.SetActive(false);
        talkHint.SetActive(false);
        Time.timeScale = 0f;
    }
    #endregion



// Temporary ****************************************************************************************************************
    #region Temporary
    void HandleAllowTalk()
    {
        talkHint.SetActive(true);
    }

    void HandleNoTalk()
    {
        talkHint.SetActive(false);
    }

    void HandleGetItem(Item _item)
    {
        displayItemQue.Add(_item);
    }

    void TryDisplayGetItem()
    {
        if (itemPopup.activeSelf) return;

        itemPopup.SetActive(true);
        Item _item = displayItemQue[0];
        itemName.text = _item.displayName.ToString();
        itemCount.text = "(" + _item.count + ")";
        displayItemQue.Remove(_item);
    }

    void HandleNoBullet()
    {
        NoBullet.SetActive(true);
    }

    void HandleNoAmmo()
    {
        NoAmmo.SetActive(true);
    }
    #endregion



// Dialogue *****************************************************************************************************************
    #region Dialogue
    void HandleRunDialogue(NPC _npc)
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        dialogueData = _npc.dialogueData;
        activeDialogue = dialogueData.dialogues[_npc.dialogueState];
        dialogueIndex = -1;

        TryNextLine();
        dialogueUI.SetActive(true);
        talkHint.SetActive(false);
    }

    void CloseDialogue()
    {
        player.EndTalk();
        dialogueUI.SetActive(false);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        talkHint.SetActive(true);
    }

    void TryNextLine()
    {
        if (!(dialogueIndex +1 < activeDialogue.npcLines.Count)) return;

        dialogueIndex += 1;
        // Check End
        LoadLine(dialogueIndex);
        if (dialogueIndex + 1 >= activeDialogue.npcLines.Count)
        {
            LoadChoices();
        }
    }

    void LoadChoices()
    {
        int i = 0;
        foreach (PlayerChoice choice in activeDialogue.choices)
        {
            choiceButtons[i].SetActive(true);
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            Debug.Log(choice.choiceText);
            i += 1;
        }
    }

    void LoadLine(int _index)
    {
        switch (activeDialogue.npcLines[_index])
        {
            case "[END]":
                CloseDialogue();
                break;
            case "[SHOP]":
                dialogueUI.SetActive(false);
                OpenShop();
                break;
            case "[END GAME]":
                CloseDialogue();
                player.End();
                break;
            default:
                dialogueText.text = activeDialogue.npcLines[_index];
                break;
        }
    }

    public void MakeChoice(int i)
    {
        dialogueIndex = -1;
        activeDialogue = dialogueData.dialogues[activeDialogue.choices[i].nextDialogueID];
        DisableChoices();
        TryNextLine();
        EventSystem.current.SetSelectedGameObject(null);
    }

    void DisableChoices()
    {
        foreach (GameObject choiceButton in choiceButtons)
        {
            choiceButton.SetActive(false);
        }
    }
    #endregion



// Shop *********************************************************************************************************************
    #region Shop

    void OpenShop()
    {
        shopUI.SetActive(true);
        moneyText.text = "$" + player.wallet;
        int i = 0;
        foreach(ShopItem item in shopItems)
        {
            string _itemText = item.name + "\t$" + item.price + "\tstock: " + item.stock;
            shopButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = _itemText;
            i += 1;
        }

        LoadSell();
        SwitchBuy();
    }

    void LoadSell()
    {
        int i = 0;
        foreach(InventoryItem item in player.playerInventory.inventory)
        {
            string _itemText = item.itemName + "\t$" + item.price + "\tholding: " + item.count;
            sellButtons[i].SetActive(true);
            sellButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = _itemText;
            i += 1;
        }

        if (i == 1)
        {
            sellButtons[1].SetActive(false);
        }
        else if (i == 0)
        {
            sellButtons[1].SetActive(false);
            sellButtons[0].SetActive(false);
        }
    }

    public void CloseShop()
    {
        player.EndTalk();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        shopUI.SetActive(false);
    }

    public void SwitchBuy()
    {
        buyUI.SetActive(true);
        sellUI.SetActive(false);
    }

    public void SwitchSell()
    {
        sellUI.SetActive(true);
        buyUI.SetActive(false);
    }

    public void Sell(int i)
    {
        int _price = 50;

        switch(i)
        {
            case 0:
                if (player.playerInventory.inventory[0].count <= 0) return;
                player.playerInventory.inventory[0].count -= 1;
                _price = 50;
                break;
            case 1:
                if (player.playerInventory.inventory[1].count <= 0) return;
                player.playerInventory.inventory[1].count -= 1;
                _price = 100;
                break;
        }
        LoadSell();
        _price *= -1;
        player.Buy(_price);
        moneyText.text = "$" + player.wallet;
    }


    public void Buy(int i)
    {
        if (shopItems[i].stock <= 0) return;

        switch(i)
        {
            case 0:
                CloseShop();
                player.Die();
                break;
            case 1:
                if (player.wallet < 50) return;
                // FIX THIS
                player.flappy = true;
                break;
            case 2:
                if (player.wallet < 100) return;
                player.atk *= 2;
                break;
            case 3:
                if (player.wallet < 50) return;
                player.ammo += 3;
                break;
            default:
                return;
        }
        shopItems[i].stock -= 1;
        ShopItem item = shopItems[i];
        string _itemText = item.name + "\t$" + item.price + "\tstock: " + item.stock;
        shopButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = _itemText;
        player.Buy(item.price);
        moneyText.text = "$" + player.wallet;
    }
    #endregion

}

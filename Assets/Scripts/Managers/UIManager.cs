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
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI expProgressionText;
    [SerializeField] GameObject lvlUpTextObject;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI loadedAmmoText;
    [SerializeField] GameObject woundOverlay;

    [Header("Temporary")]
    [SerializeField] GameObject talkHint;
    [SerializeField] GameObject itemPopup;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemCount;
    List<Item> displayItemQue = new List<Item>();


    [Header("Death")]
    [SerializeField] GameObject deathUI;

    [Header("Dialoge")]
    [SerializeField] GameObject dialogueUI;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] List<GameObject> choiceButtons;
    private DialogueBlock activeDialogue;
    private DialogueData dialogueData;
    private int dialogueIndex;

    [Header("Shop")]
    public ShopData shopData;
    public TextMeshProUGUI moneyText;
    [SerializeField] GameObject shopUI;
    public List<GameObject> shopButtons;
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

        // NPC Interaction
        player.RunDialogue += HandleRunDialogue;
        player.AllowTalk += HandleAllowTalk;
        player.NoTalk += HandleNoTalk;

        // Item
        player.GetItem += HandleGetItem;
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
                player.EndTalk();
                dialogueUI.SetActive(false);
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                break;
            case "[SHOP]":
                dialogueUI.SetActive(false);
                OpenShop();
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
    }

    public void CloseShop()
    {
        player.EndTalk();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        shopUI.SetActive(false);
    }

    public void Buy(int i)
    {
        if (shopItems[i].stock <= 0 || player.wallet <= 0) return;

        switch(i)
        {
            case 0:
                CloseShop();
                player.Die();
                break;
            case 1:
                // FIX THIS
                player.flappy = true;
                break;
            case 2:
                player.atk *= 2;
                break;
            case 3:
                player.ammo += 3;
                break;
            default:
                return;
        }
        shopItems[i].stock -= 1;
        ShopItem item = shopItems[i];
        string _itemText = item.name + "\t$" + item.price + "\tstock: " + item.stock ;
        shopButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = _itemText;
        player.Buy(item.price);
        moneyText.text = "$" + player.wallet;
    }
    #endregion

}

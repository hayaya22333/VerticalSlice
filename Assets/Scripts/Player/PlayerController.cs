using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// This class should be connected to the Locator class.
/// Controls player movements, attack, and stat updates.
/// Invoke events with locator, therefore connects to AudioManager, GameController, Killable, etc.
/// </summary>

public enum PlayerState
{
    Idle,
    Walking,
    Talking
}

public class PlayerController : Character
{
    [Header("Player Properties")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private Transform cameraTransform;
    private Camera playerCam;
    private float xRotation = 0f;
    private float yRotation = 0f;
    public Inventory playerInventory;

    public GameObject gunModel;
    public GameObject canonModel;

    public override string description => "The lonely player";

    public int maxEXP = 50;
    public int playerEXP = 0;
    public int killCount = 0;
    
    // Gun
    public int maxAmmo = 12;
    public int ammo = 1;
    public float reloadDuration = 1.3f;
    private float reloadTimer;

    [Header("Player Status")]
    public PlayerState playerState = PlayerState.Idle;
    public float attackCD;
    public int loadedAmmo = 5;
    private bool canTalk = false;
    public bool flappy = false;
    public int wallet = 200;
    string currentWeapon = "gun";

    private NPC availableNPC;
    public List<Item> availableItems = new List<Item>();

    bool gameEnd = false;



// Events *********************************************************************************************************************
    #region Events
    public delegate void EmptyDelegate();
    public delegate void IntDelegate(int x);

    public event EmptyDelegate Shoot;
    public event EmptyDelegate EmptyShot;
    public event EmptyDelegate NoAmmo;
    public event EmptyDelegate KilledPlayer;
    public event EmptyDelegate PlayerReload;
    public event EmptyDelegate EndPlayerReload;

    public event EmptyDelegate AllowTalk;
    public event EmptyDelegate NoTalk;
    public event Action<NPC> RunDialogue;
    public event EmptyDelegate CloseDialogue;
    public event EmptyDelegate NPCSpeak;

    public event IntDelegate SpentMoney;
    public event IntDelegate KilledEnemy;
    public event IntDelegate LevelUp;
    public event IntDelegate PlayerTookDamage;

    public event Action<Item> GetItem;
    public event IntDelegate CheckQuest;

    public event EmptyDelegate EndGame;
    #endregion



// Start/Update ***************************************************************************************************************
    #region Start/Update

    void Awake()
    {
        lvl = 1;
        atk = 60;

        LevelUp += HandleLevelUp;
    }

    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        playerCam = GetComponentInChildren<Camera>();
        gunOriginalLocalPos = gunModel.transform.localPosition;
    }

    void Update()
    {
        if (gameEnd) return;
    // [WASD movements] + [Rotation]**********************************************
        if (playerState != PlayerState.Talking)
        {
            RotatePlayer();
            MovePlayer();

        // KEY-based controls*********************************************************
            if ((flappy) && (Input.GetKeyDown(KeyCode.Space)))
            {
                Jump();
            }

            // ATTACK
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (reloadTimer > 0 || attackCD > 0) return;

                if (loadedAmmo <= 0)
                {
                    EmptyShot.Invoke();
                    return;
                }

                attackCD = 0.5f;
                loadedAmmo -= 1;

                DrawATKRay();
                Shoot.Invoke();
                Attack();

                Recoil();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (reloadTimer > 0 || loadedAmmo >= maxAmmo) return;
                if (ammo <= 0)
                {
                    NoAmmo.Invoke();
                    return;
                }
                StartReload();
                PlayerReload.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!canTalk) return;

                StartCoroutine(LookAtNPCSmooth(availableNPC, 120f));
                availableNPC.LookAtPlayerSmooth();
                RunTalkSound();
                playerState = PlayerState.Talking;
                RunDialogue.Invoke(availableNPC);
                availableNPC.Talked();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (availableItems.Count <= 0) return;

                Item _target = availableItems[0];
                GetItem.Invoke(_target);
                availableItems.Remove(_target);
                Destroy(_target.gameObject);
            }
        }

    // Status check***************************************************************
        if (playerEXP >= maxEXP)
        {
            playerEXP -= maxEXP;
            lvl += 1;
            LevelUp.Invoke(lvl);
            atk = (int)(atk * 1.1f);
        }

        if (attackCD > 0)
        {
            attackCD -= Time.deltaTime;
        }

        if (reloadTimer > 0)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                Reload();
            }
        }
    }
    #endregion



// Movement *******************************************************************************************************************
    #region Movement

    void RotatePlayer()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    void MovePlayer()
    {
        float moveHoriz = Input.GetAxisRaw("Horizontal");
        float moveVerti = Input.GetAxisRaw("Vertical");

        Vector3 new_velocity = (rb.transform.forward * moveVerti + rb.transform.right * moveHoriz) * speed;
        rb.velocity = new Vector3(new_velocity.x, rb.velocity.y, new_velocity.z);
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + jumpForce, rb.velocity.z);
    }

    void GetGun()
    {
        canonModel.SetActive(false);
        gunModel.SetActive(true);
        currentWeapon = "gun";
    }

    void GetCanon()
    {
        gunModel.SetActive(false);
        canonModel.SetActive(true);
        currentWeapon = "canon";
    }
    #endregion



// Connect ********************************************************************************************************************
    #region Connect

    public void GetEXP(int exp)
    {
        Debug.Log("Player gained " + exp + "EXP!");
        playerEXP += exp;

        killCount += 1;
        KilledEnemy.Invoke(killCount);
        CheckQuest.Invoke(killCount);
    }
    
    public void HandleLevelUp(int lvl)
    {
        Debug.Log("Player reached lvl " + lvl);
        maxEXP += 50;
    }

    public void PlayerTakeDamage(int dmg)
    {
        if (hp == 0) return;
        hp -= dmg;
        Debug.Log("Player took " + dmg + "damage!!");
        PlayerTookDamage.Invoke(dmg);
        CloseDialogue.Invoke();

        if (hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        KilledPlayer.Invoke();
        Destroy(this);
    }

    public void EndTalk()
    {
        playerState = PlayerState.Idle;
    }

    public void Buy(int _price)
    {
        wallet -= _price;
        SpentMoney.Invoke(_price);
    }

    public void RunTalkSound()
    {
        NPCSpeak.Invoke();
    }

    private IEnumerator LookAtNPCSmooth(NPC npc, float rotateSpeed)
    {
        Vector3 direction = npc.transform.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.001f)
            yield break;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime);

            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public string currentQuest = "";
    public int questReq = 0;

    public void SetQuest(string _quest, int _req)
    {
        currentQuest = _quest;
        questReq = _req;
    }
    #endregion



// Actions ********************************************************************************************************************
    #region Actions

    void StartReload()
    {
        reloadTimer = reloadDuration;
    }

    void Reload()
    {
        if (loadedAmmo == maxAmmo || ammo == 0) return;
        // Play animation, wait for end
        EndPlayerReload.Invoke();
        int emptyAmmo = maxAmmo - loadedAmmo;
        if (ammo >= emptyAmmo)
        {
            ammo -= emptyAmmo;
            loadedAmmo = maxAmmo;
        }
        else
        {
            loadedAmmo += ammo;
            ammo = 0;
        }
    }

    void DrawATKRay()
    {
        Ray atkRay = new Ray(playerCam.transform.position, playerCam.transform.forward);
        Debug.DrawRay(playerCam.transform.position, playerCam.transform.forward * 20f, Color.red);

        RaycastHit hit;

        if (Physics.Raycast(atkRay, out hit, 100f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var target))
            {
                target.TakeDamage(atk, gameObject.name, hit);
            }
        }
    }

    [SerializeField] private float recoilDistance = 0.1f;
    [SerializeField] private float recoilSpeed = 20f;

    private Vector3 gunOriginalLocalPos;
    private Coroutine recoilCoroutine;

    private void Recoil()
    {
        if (recoilCoroutine != null)
        {
            StopCoroutine(recoilCoroutine);
        }

        recoilCoroutine = StartCoroutine(RecoilRoutine());
    }

    private IEnumerator RecoilRoutine()
    {
        Vector3 recoilPos = gunOriginalLocalPos + Vector3.back * recoilDistance;

        Quaternion originalRot = gunModel.transform.localRotation;
        Quaternion recoilRot =
            Quaternion.Euler(-8f, 0f, 0f) * originalRot;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;

            gunModel.transform.localPosition =
                Vector3.Lerp(gunOriginalLocalPos, recoilPos, t);

            gunModel.transform.localRotation =
                Quaternion.Lerp(originalRot, recoilRot, t);

            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;

            gunModel.transform.localPosition =
                Vector3.Lerp(recoilPos, gunOriginalLocalPos, t);

            gunModel.transform.localRotation =
                Quaternion.Lerp(recoilRot, originalRot, t);

            yield return null;
        }

        gunModel.transform.localPosition = gunOriginalLocalPos;
        gunModel.transform.localRotation = originalRot;
    }
    #endregion



// Collision *************************************************************************************************************
    #region Collision

    public void EnterNPC(Collider other)
    {
        availableNPC = other.GetComponent<NPC>();
        canTalk = true;
        AllowTalk.Invoke();
    }

    public void ExitNPC(Collider other)
    {
        canTalk = false;
        NoTalk.Invoke();
    }

    public void EnterItem(Collider other)
    {
        Item _item = other.GetComponent<Item>();
        _item.ShowText();
        availableItems.Add(_item);
    }

    public void ExitItem(Collider other)
    {
        Item _item = other.GetComponent<Item>();
        _item.HideText();
        availableItems.Remove(_item);
    }
    #endregion

    public void End()
    {
        gameEnd = true;
        EndGame.Invoke();
    }
}
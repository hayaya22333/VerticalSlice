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

    public override string description => "The lonely player";

    public int maxEXP = 300;
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

    private NPC availableNPC;

    private Camera playerCam;
    private float xRotation = 0f;
    private float yRotation = 0f;



// Events *********************************************************************************************************************
    #region Events
    public delegate void EmptyDelegate();
    public delegate void IntDelegate(int x);

    public event EmptyDelegate Shoot;
    public event EmptyDelegate KilledPlayer;
    public event EmptyDelegate PlayerReload;

    public event EmptyDelegate AllowTalk;
    public event EmptyDelegate NoTalk;

    public event IntDelegate SpentMoney;
    public event IntDelegate KilledEnemy;
    public event IntDelegate LevelUp;
    public event IntDelegate PlayerTookDamage;

    public event Action<NPC> RunDialogue;
    #endregion



// Start/Update ***************************************************************************************************************
    #region Start/Update

    void Awake()
    {
        lvl = 1;
        atk = 70;

        LevelUp += HandleLevelUp;
    }

    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        playerCam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
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

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (reloadTimer > 0) return;
                if (attackCD > 0 || loadedAmmo <= 0) return;
                attackCD = 0.5f;
                loadedAmmo -= 1;

                DrawATKRay();
                Shoot.Invoke();
                Attack();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (reloadTimer > 0 || loadedAmmo >= maxAmmo || ammo <= 0) return;
                StartReload();
                PlayerReload.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!canTalk) return;
                playerState = PlayerState.Talking;
                RunDialogue.Invoke(availableNPC);
                availableNPC.Talked();
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

    void Crouch()
    {
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
    }
    
    public void HandleLevelUp(int lvl)
    {
        Debug.Log("Player reached lvl " + lvl);
    }

    public void PlayerTakeDamage(int dmg)
    {
        if (hp == 0) return;
        hp -= dmg;
        Debug.Log("Player took " + dmg + "damage!!");
        PlayerTookDamage.Invoke(dmg);

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

        if (Physics.Raycast(atkRay, out hit))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var target))
            {
                target.TakeDamage(atk, gameObject.name);
            }
        }
    }
/*
    void DrawRay()
    {
        Ray playerRay = new Ray(playerCam.transform.position, playerCam.transform.forward);
        Debug.DrawRay(playerCam.transform.position, playerCam.transform.forward * 20f, Color.red);
        RaycastHit hit;

        if (Physics.Raycast(playerRay, out hit))
        {
            // Debug.Log("Hitting " + hit.collider.name);
            GameObject hitObject = hit.collider.gameObject;
            switch (hitObject.tag)
            {
                case "Item":
                // UI allow pickup
                    break;
                case "Enemy":
                // Add aggro
                    break;
            }
            if (hit.collider.TryGetComponent<IInteractable>(out var target))
            {
                AllowInteract(target);
            }
        }
    }
*/
    #endregion



// Auto Detection *************************************************************************************************************
    #region Auto Detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            availableNPC = other.GetComponent<NPC>();
            canTalk = true;
            AllowTalk.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            canTalk = false;
            NoTalk.Invoke();
        }
    }
    #endregion
}
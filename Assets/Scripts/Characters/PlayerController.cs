using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class should be connected to the Locator class.
/// Controls player movements, attack, and stat updates.
/// Invoke events with locator, therefore connects to AudioManager, GameController, Killable, etc.
/// </summary>
public class PlayerController : Character
{
    #region Variables

    [Header("Player Properties")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private Transform cameraTransform;

    public override string description => "The lonely player";

    public int maxEXP = 300;
    public int playerEXP = 0;
    public int killCount = 0;

    public delegate void EmptyDelegate();
    public delegate void IntDelegate(int x);

    public event EmptyDelegate Shoot;
    public event IntDelegate KilledEnemy;
    public event IntDelegate LevelUp;

    private Camera playerCam;
    private float xRotation = 0f;
    private float yRotation = 0f;

    #endregion

    #region StartUpdate

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
        // [WASD movements] + [Rotation]
        RotatePlayer();
        MovePlayer();

        // KEY-based controls
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            DrawATKRay();
            Shoot.Invoke();
            Attack();
        }

        if (playerEXP >= maxEXP)
        {
            playerEXP -= maxEXP;
            lvl += 1;
            LevelUp.Invoke(lvl);
        }
    }
    #endregion

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
    #endregion

    #region Actions
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
    #endregion

}
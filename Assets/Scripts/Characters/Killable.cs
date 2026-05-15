using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class Killable : Character
{
    [Header("Enemy Properties")]
    [SerializeField] public List<GameObject> itemDrops;
    [SerializeField] private int expDrop = 100;
    [SerializeField] private GameObject damagePopupPrefab;

    CharState state = CharState.Idle;
    GameObject target;
    PlayerController player;
    private float positionLockTimer;

    void Awake()
    {
        hp = 200;
        speed = 2f;
    }

    void Start()
    {
        player = Locator.Instance.Player;
        target = player.gameObject;
    }


    void Update()
    {

        if (hp <= 0)
        {
            // Implement drops
            Debug.Log("Killed " + gameObject.name);
            // Insert death protocol here.
            Die();
        }

        // Turn into state machine
        if (state == CharState.Chase)
        {
            if (player.hp <= 0) return;
            Follow(target);
        }
    }

    public void Die()
    {
        GetComponent<StateMachine>().enabled = false;
        GetComponent<ScriptMachine>().enabled = false;
        rb.constraints = ~RigidbodyConstraints.FreezeAll;
        player.GetEXP(expDrop);

        physicalCollider.enabled = false;
        Destroy(this);
    }

    new void Attack()
    {
        if (player.hp <= 0) return;
        player.PlayerTakeDamage(atk);
    }

    void DropItems()
    {
        foreach (GameObject item in itemDrops)
        {
            Instantiate(item, transform.position, transform.rotation);
        }
    }

    public void HandleDamage(int dmg, string dmgSource, string damagedPart)
    {
        hp -= dmg;
        CustomEvent.Trigger(gameObject, "StartChase");
        
        DamagePopUp popUp = Instantiate(damagePopupPrefab, transform.position, transform.rotation).GetComponent<DamagePopUp>();
        popUp.PopNum(dmg);
    }

    public string GetDescription()
    {
        return description;
    }

    public GameObject GetTarget()
    {
        return target;
    }

    public float DistanceToTarget()
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    public void SetPositionLocked(bool value)
    {
        positionLocked = value;
    }
}

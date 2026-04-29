using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class Character : MonoBehaviour
{
    // Default stats that should exist in all characters
    [Header("Physics Setting")]
    public Rigidbody rb;
    [SerializeField] protected Collider physicalCollider;
    [SerializeField] Collider triggerCollider;
    [SerializeField] Collider hitBox;

    [Header("Stored Info")]
    [SerializeField] List<string> dialogues;

    [Header("Stats Setting")]
    public int hp = 100;
    public int atk = 10;
    public int lvl = 1;
    public float speed = 3f;
    public virtual string description => "This is a sample character";

    [Header("Status")]
    public bool positionLocked = false;

    // Default methods that should exist in most characters
    public void Attack()
    {
        //Debug.Log(gameObject.name + "dealt " + atk + " damage!");
    }

    public enum CharState
    {
        Idle,
        Flinch,
        Chase,
    }

    public void Follow(GameObject target)
    {
        if (positionLocked || target == null)
        {
            return;
        }

        Vector3 target_pos = target.transform.position;
        transform.LookAt(target_pos);
        transform.position = Vector3.MoveTowards(
            transform.position,
            target_pos,
            speed * Time.deltaTime
            );
    }
}
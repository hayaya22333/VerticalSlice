using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] public virtual string description => "This is a sample item.";
    private float durability = 100f;

    public void Interact()
    {
        Debug.Log("Picked up" + gameObject.name);
        Destroy(gameObject);
    }

    public void TakeDamage(int dmg, string source)
    {
        durability -= dmg;
        Debug.Log(gameObject.name + "'s durability reduced.");
    }

    public string GetDescription()
    {
        return description;
    }
}

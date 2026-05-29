using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public GameObject pickUpText;
    [SerializeField] public virtual string description => "This was in a pair";
    public float durability = 100f;
    public string displayName = "ball";
    public int count;
    public int price;
    public float dropRate;
    
    public void ShowText()
    {
        pickUpText.SetActive(true);
    }

    public void HideText()
    {
        pickUpText.SetActive(false);
    }

    public void Interact()
    {
        Debug.Log("Picked up" + gameObject.name);
        Destroy(gameObject);
    }

    public void TakeDamage(int dmg, string source, RaycastHit hit)
    {
        durability -= dmg;
        Debug.Log(gameObject.name + "'s durability reduced.");
    }

    public string GetDescription()
    {
        return description;
    }
}

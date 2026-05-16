using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InventoryItem
{
    public string itemName;
    public int price;
    public int count;
}

public class Inventory : MonoBehaviour
{
    public List<InventoryItem> inventory = new List<InventoryItem>();
    public PlayerController player;

    void Start()
    {
        player = Locator.Instance.Player;

        player.GetItem += HandleGetItem;
    }

    void HandleGetItem(Item _item)
    {
        
        InventoryItem existingItem = inventory.Find(i => 
            i.itemName == _item.displayName &&
            i.price == _item.price
        );

        if (existingItem != null)
        {
            existingItem.count += _item.count;
        }
        else
        {
            inventory.Add(new InventoryItem
            {
                itemName = _item.displayName,
                price = _item.price,
                count = _item.count
            });
        }
    }
}

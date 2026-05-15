using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop", menuName = "ScriptableObj/Shop")]
public class ShopData : ScriptableObject
{
    public List<ShopItem> items = new List<ShopItem>();
}

[Serializable]
public class ShopItem
{
    public string name;
    public int price;
    public int stock;
}
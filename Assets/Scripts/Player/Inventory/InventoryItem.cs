using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Item,
    Aid,
    Weapon,
    Armour
}

public class InventoryItem : Interactible
{
    [Header("Item Properties")]
    public string itemName;
    public string description;
    public Sprite sprite;

    public ItemType itemType;

    protected override void Start()
    {
        base.Start();

        interactMessage += "to pickup " + itemName;
    }
}
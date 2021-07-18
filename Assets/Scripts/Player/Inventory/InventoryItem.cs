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
    protected string useFailure;

    public ItemType itemType;

    protected override void Start()
    {
        base.Start();

        interactMessage += "to pickup " + itemName;

        useFailure = "Could not use " + itemName;
    }

    public virtual bool Use()
    {
        return false;
    }

    public string GetUseFailure()
    {
        return useFailure;
    }
}
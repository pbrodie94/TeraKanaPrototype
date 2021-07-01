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

public class InventoryItem : MonoBehaviour
{
    [Header("Item Properties")]
    public string Name;

    public ItemType itemType;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemBox : Interactible
{
    [Header("Item Crate Properties")]
    public InventoryItem item;

    private LaserTargetingGun gun;

    protected override void Start()
    {
        base.Start();

        if (item)
            interactMessage += "to pickup " + item.itemName;
    }

    protected override void Interact()
    {
        base.Interact();

        try
        {
            Inventory inv = player.gameObject.GetComponent<Inventory>();
            inv.PickupItem(item);

            gun.equipped = true;

        } catch(NullReferenceException e)
        {
            Debug.LogWarning("OH NO! " + e);
        }
    }
}

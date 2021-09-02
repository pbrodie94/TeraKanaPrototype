using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : Interactable
{
    [Header("Item Crate Properties")]
    public InventoryItem item;

    [HideInInspector] public int itemIndex;

    protected override void Start()
    {
        base.Start();

        if (item)
            interactMessage += "to pickup " + item.itemName;
    }

    protected override void Interact()
    {
        base.Interact();

        Inventory inv = player.gameObject.GetComponent<Inventory>();

        if (inv.PickupItem(item))
        {
            audioSource.PlayOneShot(interactSound);
            itemIndex = 0;
            return;
        }

        isInteractable = true;

    }
}

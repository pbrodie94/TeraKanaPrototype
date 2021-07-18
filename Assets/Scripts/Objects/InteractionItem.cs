using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionItem : InventoryItem
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        itemType = ItemType.Item;

        
    }

    public override bool Use()
    {
        useFailure = "Cannot use " + itemName + " yet";

        return base.Use();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : InventoryItem
{
    private float amount = 20;

    private PlayerStats stats;

    protected override void Start()
    {
        if (itemName == "")
            itemName = "Health Pack";

        base.Start();

        itemType = ItemType.Aid;

        if (amount <= 0)
            amount = 20;

        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    public override bool Use()
    {
        if (!stats)
        {
            stats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        }

        if (stats.AddHealth(amount))
            return true;

        useFailure = "Health is already full";

        return false;
    }
}

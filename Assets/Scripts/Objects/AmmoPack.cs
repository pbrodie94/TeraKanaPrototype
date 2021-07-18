using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : InventoryItem
{
    [SerializeField] private int amount;

    private WeaponManager weaponManager;

    protected override void Start()
    {
        if (itemName == "")
            itemName = "Ammo Pack";

        base.Start();

        itemType = ItemType.Aid;

        if (amount <= 0)
        {
            amount = 30;
        }

        LevelController.PlayerSpawned += GetWeaponManagerReference;
    }

    public void GetWeaponManagerReference()
    {
        weaponManager = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponManager>();
    }

    public override bool Use()
    {
        if (!weaponManager)
            weaponManager = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponManager>();

        useFailure = "No weapon equipped.";

        return weaponManager.AddAmmo(amount);
    }
}

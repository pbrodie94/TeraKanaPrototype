using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    private WeaponManager wManager;

    private void Start()
    {
        wManager = GetComponent<WeaponManager>();
    }

    public bool PickupItem(InventoryItem item)
    {
        //Take in the Inventory Item class and check the item type to sort where it goes
        switch (item.itemType)
        {
            case ItemType.Weapon:

                Weapon w = item.gameObject.GetComponent<Weapon>();

                if (!wManager.HasWeapon())
                {
                    //Have an empty weapon slot, equip it to either primary or secondary

                    wManager.EquipWeapon(w, false);

                } else
                {
                    //Weapon slots full, add to inventory

                    AddWeapons(w);
                }

                break;
        }

        return true;
    }

    public void AddWeapons(Weapon w)
    {

    }
}

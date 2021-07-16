using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<InventoryItem> weapons = new List<InventoryItem>();
    private List<InventoryItem> aidItems = new List<InventoryItem>();
    private List<InventoryItem> items = new List<InventoryItem>();

    private WeaponManager wManager;

    private HUDManager hud;

    private void Start()
    {
        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUDManager>();
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

            case ItemType.Aid:

                //Add to aid inventory

                AddAidItems(item);

                break;

            case ItemType.Item:

                //Add item to items inventory

                AddItems(item);

                break;

            case ItemType.Armour:

                //Add item to armour inventory

                break;

            default:

                break;
        }

        hud.AddNotification("Picked up " + item.Name);

        return true;
    }

    private void AddWeapons(InventoryItem weapon)
    {
        weapons.Add(weapon);
    }

    private void AddAidItems(InventoryItem aid)
    {
        aidItems.Add(aid);
    }

    private void AddItems(InventoryItem item)
    {
        items.Add(item);
    }
}

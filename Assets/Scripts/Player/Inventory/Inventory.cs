using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<InventoryItem> weapons = new List<InventoryItem>();
    private List<InventoryItem> aidItems = new List<InventoryItem>();
    private List<InventoryItem> items = new List<InventoryItem>();

    private EquipmentMenu equipmentMenu;
    private InteractMenu interactMenu;

    private WeaponManager wManager;

    private HUDManager hud;

    private void Start()
    {
        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUDManager>();
        wManager = GetComponent<WeaponManager>();

        equipmentMenu = hud.equipmentMenu.gameObject.GetComponentInChildren<EquipmentMenu>();
        interactMenu = hud.interactMenu.gameObject.GetComponentInChildren<InteractMenu>();
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

                equipmentMenu.AddInventoryItem(item);

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

        hud.AddNotification("Picked up " + item.itemName);

        return true;
    }

    public bool AddItem(InventoryItem item)
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

                }
                else
                {
                    //Weapon slots full, add to inventory

                    AddWeapons(w);
                }

                equipmentMenu.AddInventoryItem(item);

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

        return true;
    }

    public bool ContainsItem(InventoryItem item)
    {
        //Take in the Inventory Item class and check the item type to sort where it goes
        switch (item.itemType)
        {
            case ItemType.Weapon:

                return weapons.Contains(item);

                break;

            case ItemType.Aid:

                //Add to aid inventory

                return aidItems.Contains(item);

                break;

            case ItemType.Item:

                //Add item to items inventory

                return items.Contains(item);

                break;

            case ItemType.Armour:

            //Add item to armour inventory

            default:

                break;

        }

        return false;
    }

    public bool RemoveItem(InventoryItem item)
    {
        //Take in the Inventory Item class and check the item type to sort where it goes
        switch (item.itemType)
        {
            case ItemType.Weapon:

                return RemoveWeapons(item);

                break;

            case ItemType.Aid:

                //Add to aid inventory

                return RemoveAidItems(item);

                break;

            case ItemType.Item:

                //Add item to items inventory

                return RemoveItems(item);

                break;

            case ItemType.Armour:

                //Add item to armour inventory

                break;

            default:

                break;
        }

        return false;
    }

    private void AddWeapons(InventoryItem weapon)
    {
        weapons.Add(weapon);
    }

    private void AddAidItems(InventoryItem aid)
    {
        aidItems.Add(aid);

        if (!equipmentMenu.AddInventoryItem(aid))
        {
            hud.AddNotification(equipmentMenu.GetError(), HUDManager.NotificationType.Warning);
        }
    }

    private void AddItems(InventoryItem item)
    {
        items.Add(item);
    }

    private bool RemoveWeapons(InventoryItem item)
    {
        if (weapons.Contains(item))
        {
            weapons.Remove(item);
            return true;
        }

        return false;
    }

    private bool RemoveAidItems(InventoryItem item)
    {
        if (aidItems.Contains(item))
        {
            aidItems.Remove(item);
            return true;
        }

        return false;
    }

    private bool RemoveItems(InventoryItem item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            return true;
        }

        return false;
    }
}

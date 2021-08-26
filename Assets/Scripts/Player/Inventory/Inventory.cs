using System;
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

    private InteractionItem currentHeldItem;

    private WeaponManager wManager;

    private void Start()
    {
        wManager = GetComponent<WeaponManager>();

        equipmentMenu = HUDManager.instance.equipmentMenu.gameObject.GetComponentInChildren<EquipmentMenu>();
        interactMenu = HUDManager.instance.interactMenu.gameObject.GetComponentInChildren<InteractMenu>();
    }

    public bool PickupItem(InventoryItem item)
    {
        //Take in the Inventory Item class and check the item type to sort where it goes
        switch (item.itemType)
        {
            case ItemType.Weapon:

                Weapon w = item.gameObject.GetComponent<Weapon>();

                if (!wManager.EquipWeapon(w, false))
                {
                    //Have an empty weapon slot, equip it to either primary or secondary

                    //AddWeapons(w);

                }

                break;

            case ItemType.Aid:

                //Add to aid inventory
                if (AddAidItems(item))
                {
                    HUDManager.instance.AddNotification("Picked up " + item.itemName);
                    return true;
                }

                return false;

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

        HUDManager.instance.AddNotification("Picked up " + item.itemName);

        return true;
    }

    public bool AddItem(InventoryItem item)
    {
        //Take in the Inventory Item class and check the item type to sort where it goes
        switch (item.itemType)
        {
            case ItemType.Weapon:

                Weapon w = item.gameObject.GetComponent<Weapon>();

                if (!wManager.EquipWeapon(w, false))
                {
                    Debug.Log("Didn't pickup");
                    //Have an empty weapon slot, equip it to either primary or secondary

                    //AddWeapons(w);

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

        return true;
    }

    public bool ContainsItem(InventoryItem item)
    {
        //Take in the Inventory Item class and check the item type to sort where it goes
        switch (item.itemType)
        {
            case ItemType.Weapon:

                return weapons.Contains(item);

            case ItemType.Aid:

                //Add to aid inventory

                return aidItems.Contains(item);

            case ItemType.Item:

                //Add item to items inventory

                return items.Contains(item);

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

            case ItemType.Aid:

                //Add to aid inventory

                return RemoveAidItems(item);

            case ItemType.Item:

                //Add item to items inventory

                return RemoveItems(item);

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

    private bool AddAidItems(InventoryItem aid)
    {

        if (!equipmentMenu.AddInventoryItem(aid))
        {
            HUDManager.instance.AddNotification(equipmentMenu.GetError(), HUDManager.NotificationType.Warning);

            return false;
        }

        aidItems.Add(aid);

        return true;
    }

    private void AddItems(InventoryItem item)
    {
        items.Add(item);
        interactMenu.AddInventoryItem(item);
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

    public void AddInteractItem(InteractionItem item)
    {
        currentHeldItem = item;
        wManager.HolsterWeapon(true);
        HUDManager.instance.AddActiveItem(item);
    }

    public void UseInterractItem(bool useSuccessful)
    {
        interactMenu.ItemUsed(useSuccessful);
        HUDManager.instance.HideActiveItem();
        wManager.HolsterWeapon(false);
        currentHeldItem = null;
    }

    public InteractionItem GetHeldItem()
    {
        return currentHeldItem;
    }

    public bool IsHoldingItem()
    {
        return currentHeldItem;
    }

    private void Update()
    {
        if (IsHoldingItem() && !HUDManager.instance.IsMenuOpen())
        {
            if (Input.GetButtonDown(InputManager.Action) || Input.GetButtonDown(InputManager.Shoot))
            {
                UseInterractItem(false);
            }
        }
    }
}

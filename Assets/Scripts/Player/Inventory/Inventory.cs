using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<InventoryItem> _weapons = new List<InventoryItem>();
    public List<InventoryItem> weapons
    {
        get { return _weapons; }
    }
    private List<InventoryItem> _aidItems = new List<InventoryItem>();
    public List<InventoryItem> aidItems
    {
        get { return _aidItems; }
    }
    private List<InventoryItem> _items = new List<InventoryItem>();
    public List<InventoryItem> items
    {
        get { return _items; }
    }

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
    
    public bool LoadItems(InventoryItem item)
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

        return true;
    }

    public bool ContainsItem(InventoryItem item)
    {
        //Take in the Inventory Item class and check the item type to sort where it goes
        switch (item.itemType)
        {
            case ItemType.Weapon:

                return _weapons.Contains(item);

            case ItemType.Aid:

                //Add to aid inventory

                return _aidItems.Contains(item);

            case ItemType.Item:

                //Add item to items inventory

                return _items.Contains(item);

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
        _weapons.Add(weapon);
    }

    private bool AddAidItems(InventoryItem aid)
    {

        if (!equipmentMenu.AddInventoryItem(aid))
        {
            HUDManager.instance.AddNotification(equipmentMenu.GetError(), HUDManager.NotificationType.Warning);

            return false;
        }

        _aidItems.Add(aid);

        return true;
    }

    private void AddItems(InventoryItem item)
    {
        _items.Add(item);
        interactMenu.AddInventoryItem(item);
    }

    private bool RemoveWeapons(InventoryItem item)
    {
        if (_weapons.Contains(item))
        {
            _weapons.Remove(item);
            return true;
        }

        return false;
    }

    private bool RemoveAidItems(InventoryItem item)
    {
        if (_aidItems.Contains(item))
        {
            _aidItems.Remove(item);
            return true;
        }

        return false;
    }

    private bool RemoveItems(InventoryItem item)
    {
        if (_items.Contains(item))
        {
            _items.Remove(item);
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

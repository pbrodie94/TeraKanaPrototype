using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractMenu : InventoryMenu
{
    private InventorySlot selectedSlot = null;
    
    public override void PrimaryItemSelect(InventorySlot slot)
    {
        if (slot.IsSelected())
        {
            Debug.Log("Deselecting cause already selected");
            inventory.UseInterractItem(false);
            HUDManager.instance.CloseMenu();
            return;
        }

        if (!inventory)
        {
            inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        }

        if (inventory.RemoveItem(slot.item))
        {
            //use the item
            if (slot.item.itemUseSound)
            {
                inventoryAudio.PlayOneShot(slot.item.itemUseSound);
            }

            for (int i = 0; i < inventorySlots.Count; ++i)
            {
                if (inventorySlots[i].SlotFilled())
                {
                    break;
                }
                
                inventorySlots[i].DeSelectSlot();
            }

            ItemHover();
            slot.SelectSlot();
            HUDManager.instance.CloseMenu();

            selectedSlot = slot;
            
            inventory.AddInteractItem((InteractionItem)slot.item);

        } else
        {
            HUDManager.instance.AddNotification("Could not use item", HUDManager.NotificationType.Warning);
        }
    }

    public void ItemUsed(bool successful)
    {
        //If using the item was successful
        if (successful)
        {
            //Remove the item from the inventory
            if (selectedSlot != null)
            {
                HUDManager.instance.AddNotification("Removed: " + selectedSlot.item.itemName);
                RemoveInventoryItem(selectedSlot);
            }
        }
        else
        {

            //Re-add the item to inventories
            InventorySlot slot = selectedSlot;
            selectedSlot.ClearSlot();
            --inventoryCapacity;
            inventory.AddItem(slot.item);
            
            //Set the selected slot to null to prevent errors
            selectedSlot = null;
        }
    }
}

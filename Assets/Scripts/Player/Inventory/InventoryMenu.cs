using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    [Header("Menu Properties")]
    [SerializeField] private int maxSlots;
    [SerializeField] protected GameObject detailsPanel;
    [SerializeField] List<InventorySlot> inventorySlots = new List<InventorySlot>();
    [SerializeField] GridLayoutGroup itemSlotGrid;

    [SerializeField] private GameObject inventorySlot;

    [SerializeField] protected float slideSpeed = 10;
    protected RectTransform detailsRect;
    protected Vector3 defaultPosition;
    [SerializeField] protected Vector3 detailDisplayPosition = new Vector3(400, 0, 0);

    private string error = "Inventory full.";

    protected AudioSource inventoryAudio;

    protected Inventory inventory;
    protected DetailsWindow detailsWindow;

    protected int inventoryCapacity = 0;

    protected virtual void Start()
    {
        detailsRect = detailsPanel.GetComponent<RectTransform>();
        defaultPosition = gameObject.GetComponent<RectTransform>().localPosition;

        detailsPanel.SetActive(false);

        detailsWindow = detailsPanel.GetComponent<DetailsWindow>();

        if (!inventorySlot)
            inventorySlot = transform.Find("SelectionSlot").gameObject;

        if (!itemSlotGrid)
            itemSlotGrid = transform.Find("ItemSlotGrid").GetComponent<GridLayoutGroup>();

        foreach (InventorySlot slot in inventorySlots)
        {
            slot.InitializeSlot(this);
        }

        inventoryAudio = HUDManager.instance.gameObject.GetComponent<AudioSource>();

        LevelController.PlayerSpawned += GetInventoryReference;
    }

    protected virtual void OnValidate()
    {
        if (!inventorySlot)
        {
            inventorySlot = transform.Find("SelectionSlot").gameObject;

            inventorySlots.Add(inventorySlot.GetComponent<InventorySlot>());
        }

        if (maxSlots <= 0)
        {
            maxSlots = 4 * 5;
        }

        if (inventorySlots.Count < maxSlots)
        {
            int baseSlots = maxSlots - inventorySlots.Count - 1;

            for (int i = 0; i <= baseSlots; i++)
            {
                GameObject slot = Instantiate(inventorySlot, itemSlotGrid.gameObject.transform);
                inventorySlots.Add(slot.GetComponent<InventorySlot>());
            }
        }

    }

    public void GetInventoryReference()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    public virtual bool AddInventoryItem(InventoryItem item)
    {
        Debug.Log("Capacity: " + inventoryCapacity + " # Slots: " + inventorySlots.Count + " Filled = " + IsFull());

        if (IsFull())
            return false;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (!inventorySlots[i].SlotFilled())
            {
                inventorySlots[i].SetSlot(item);
                ++inventoryCapacity;

                Debug.Log("Capacity: " + inventoryCapacity + " # Slots: " + inventorySlots.Count + " Filled = " + IsFull());

                return true;
            }
        }

        return false;
    }

    protected virtual void RemoveInventoryItem(InventorySlot slot)
    {
        //Remove the item
        slot.ClearSlot();
        --inventoryCapacity;

        //Rearrange items in the slots
        int index = inventorySlots.IndexOf(slot);

        for (int i = index + 1; i < inventorySlots.Count; i++)
        {
            if (!inventorySlots[i].SlotFilled())
            {
                break;
            }

            //Decrease inventory capacity, because we add in the Add function
            --inventoryCapacity;

            AddInventoryItem(inventorySlots[i].item);
            inventorySlots[i].ClearSlot();
        }
    }

    public void PrimaryItemSelect(InventorySlot slot)
    {
        if (!inventory)
        {
            inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        }

        if (inventory.RemoveItem(slot.item))
        {
            //use the item
            if (slot.item.Use())
            {
                //Remove item
                HUDManager.instance.AddNotification("Used " + slot.item.itemName);

                if (slot.item.itemUseSound)
                {
                    inventoryAudio.PlayOneShot(slot.item.itemUseSound);
                }

                RemoveInventoryItem(slot);

                ItemHover();

            } else
            {
                //Display reason why it could not be used
                HUDManager.instance.AddNotification(slot.item.GetUseFailure(), HUDManager.NotificationType.Warning);
                slot.ClearSlot();
                --inventoryCapacity;
                inventory.AddItem(slot.item);
            }
        } else
        {
            HUDManager.instance.AddNotification("Could not use item", HUDManager.NotificationType.Warning);
        }
    }

    public virtual void SecondaryItemSelect(InventorySlot slot)
    {

    }

    public virtual void ThirdItemSelect(InventorySlot slot)
    {

    }

    public virtual void ItemHover(InventoryItem item)
    {
        //Show the item's description
        detailsWindow.SetItemDetails(item);

        //Show details panel
        if (!detailsPanel.activeSelf)
        {
            detailsRect.localPosition = Vector3.zero;
            detailsPanel.SetActive(true);
        }

        //Animate the details panel outward

    }

    public virtual void ItemHover()
    {
        //Hide the item's description
        detailsPanel.SetActive(false);
    }

    private void Update()
    {
        if (detailsPanel.activeSelf)
        {
            detailsRect.localPosition = Vector3.Lerp(detailsRect.localPosition, detailDisplayPosition, slideSpeed * Time.fixedDeltaTime);
        }
    }

    public bool IsFull()
    {
        return inventoryCapacity >= inventorySlots.Count;
    }

    public string GetError()
    {
        return error;
    }
}
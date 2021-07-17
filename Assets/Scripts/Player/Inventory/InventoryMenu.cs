using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    [Header("Menu Properties")]
    [SerializeField] protected GameObject detailsPanel;
    [SerializeField] List<InventorySlot> inventorySlots = new List<InventorySlot>();
    [SerializeField] GridLayoutGroup itemSlotGrid;

    [SerializeField] private GameObject inventorySlot;

    [SerializeField] protected float slideSpeed = 10;
    protected RectTransform detailsRect;
    protected Vector3 defaultPosition;
    [SerializeField] protected Vector3 detailDisplayPosition = new Vector3(400, 0, 0);

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

        LevelController.PlayerSpawned += GetInventoryReference;
    }

    protected virtual void OnValidate()
    {
        if (!inventorySlot)
            inventorySlot = transform.Find("SelectionSlot").gameObject;

        inventorySlots.Add(inventorySlot.GetComponent<InventorySlot>());

        int baseSlots = (4 * 5) - inventorySlots.Count;

        for (int i = 0; i <= baseSlots; i++)
        {
            GameObject slot = Instantiate(inventorySlot, itemSlotGrid.gameObject.transform);
            inventorySlots.Add(slot.GetComponent<InventorySlot>());
        }
    }

    public void GetInventoryReference()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    public virtual void AddInventoryItem(InventoryItem item, int index)
    {
        if (!inventorySlots[inventoryCapacity].SlotFilled())
        {
            inventorySlots[inventoryCapacity].SetSlot(item, index);
            inventoryCapacity++;
        }
    }

    public virtual void PrimaryItemSelect(InventorySlot slot)
    {

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
}
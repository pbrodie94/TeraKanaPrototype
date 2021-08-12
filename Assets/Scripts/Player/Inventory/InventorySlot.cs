using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image slotItemImage;
    [SerializeField] private Image selectionEffect;
    public InventoryItem item;

    private bool selected = false;

    private InventoryMenu menu;

    private void Awake()
    {
        slotItemImage.enabled = false;
        selectionEffect.enabled = false;
    }

    private void OnValidate()
    {
        slotItemImage = transform.Find("ItemImage").GetComponent<Image>();
        selectionEffect = transform.Find("SelectionEffect").GetComponent<Image>();
    }

    public void InitializeSlot(InventoryMenu inventoryMenu)
    {
        menu = inventoryMenu;
    }

    public void SetSlot(InventoryItem item)
    {
        this.item = item;
        slotItemImage.sprite = item.sprite;
        slotItemImage.enabled = true;
    }

    public void SelectSlot()
    {
        selected = true;
        selectionEffect.enabled = true;
    }

    public void DeSelectSlot()
    {
        selected = false;
        selectionEffect.enabled = false;
    }

    public void ClearSlot()
    {
        DeSelectSlot();
        slotItemImage.enabled = false;
    }

    public bool SlotFilled()
    {
        return slotItemImage.enabled;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!slotItemImage.enabled)
            return;

        Color hoverColor = slotItemImage.color;
        hoverColor.a = 0.7f;
        slotItemImage.color = hoverColor;

        menu.ItemHover(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!slotItemImage.enabled)
            return;

        slotItemImage.color = Color.white;

        menu.ItemHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            menu.PrimaryItemSelect(this);
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            menu.SecondaryItemSelect(this);
        }

        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            menu.ThirdItemSelect(this);
        }
    }

    public bool IsSelected()
    {
        return selected;
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image slotItemImage;
    [SerializeField] private Image selectionEffect;

    private int _itemIndex = 0;
    public int itemIndex
    {
        get
        {
            return _itemIndex;
        }
    }
    private bool selected = false;

    private InventoryMenu menu;

    private void Start()
    {
        slotItemImage.enabled = false;
        selectionEffect.enabled = false;
    }

    private void OnValidate()
    {
        slotItemImage = transform.Find("ItemImage").GetComponent<Image>();
        selectionEffect = transform.Find("SelectionEffect").GetComponent<Image>();
    }

    public void SetSlot(InventoryMenu inventoryMenu, Image itemImage, int index)
    {
        menu = inventoryMenu;
        _itemIndex = index;
        slotItemImage = itemImage;
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
        _itemIndex = 0;
        selected = false;
        slotItemImage.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!slotItemImage.enabled)
            return;

        Color hoverColor = slotItemImage.color;
        hoverColor.a = 0.7f;
        slotItemImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!slotItemImage.enabled)
            return;

        slotItemImage.color = Color.white;
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
    }
}

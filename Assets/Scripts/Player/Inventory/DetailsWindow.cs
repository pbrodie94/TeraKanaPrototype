using UnityEngine;
using UnityEngine.UI;

public class DetailsWindow : MonoBehaviour
{
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text itemDescriptionText;

    public void SetItemDetails(InventoryItem item)
    {
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
    }
}

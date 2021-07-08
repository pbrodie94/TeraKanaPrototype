using UnityEngine;
using UnityEngine.EventSystems;

public class MenuItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public int menuItemIndex = 0;

    TitleMenu titleScreen;

    private void Start()
    {
        titleScreen = GetComponentInParent<TitleMenu>();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //select the menu item
        titleScreen.SelectItem(menuItemIndex);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Change to selected colour, and update the menu selection
        titleScreen.UpdateMenuSelection(menuItemIndex);
    }
}

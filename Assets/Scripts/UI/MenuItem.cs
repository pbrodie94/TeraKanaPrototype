using UnityEngine;
using UnityEngine.EventSystems;

public class MenuItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    private int menuItemIndex = 0;

    MenuScript menuScreen;

    public void InitializeMenuItem(MenuScript menu, int index)
    {
        menuScreen = menu;
        menuItemIndex = index;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //select the menu item
        menuScreen.SelectItem(menuItemIndex);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Change to selected colour, and update the menu selection
        menuScreen.UpdateMenuSelection(menuItemIndex);
    }
}

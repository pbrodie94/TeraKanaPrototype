using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    protected int menuSelection = 0;
    [SerializeField] protected Text[] menuItems;
    protected Color defaultColor = Color.white;
    protected Color selectedColor = Color.cyan;

    protected virtual void Start()
    {
        menuSelection = 0;

        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].color = defaultColor;

            MenuItem item = menuItems[i].GetComponent<MenuItem>();

            if (item == null)
            {
                item = menuItems[i].gameObject.AddComponent<MenuItem>();
            }

            item.InitializeMenuItem(this, i);
        }

        menuItems[menuSelection].color = selectedColor;
    }

    protected virtual void Update()
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].color = defaultColor;
        }

        menuItems[menuSelection].color = selectedColor;
    }

    public virtual void SelectItem(int selection)
    {

    }

    public void UpdateMenuSelection(int selectionIndex)
    {
        menuSelection = selectionIndex;
    }
}

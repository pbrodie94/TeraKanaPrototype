using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour
{
    private int menuSelection = 0;
    [SerializeField] private Text[] menuItems;
    private Color defaultColor = Color.white;
    private Color selectedColor = Color.cyan;

    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject optionsPanel;

    private void Start()
    {
        menuSelection = 0;

        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].color = defaultColor;
            menuItems[i].GetComponent<MenuItem>().menuItemIndex = i;
        }

        menuItems[menuSelection].color = selectedColor;
    }

    private void Update()
    {
        if (!instructionsPanel.activeSelf && !optionsPanel.activeSelf)
        {
            for (int i = 0; i < menuItems.Length; i++)
            {
                menuItems[i].color = defaultColor;
            }

            menuItems[menuSelection].color = selectedColor;
        }

        if (instructionsPanel.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                instructionsPanel.SetActive(false);
            }
        }

        if (optionsPanel.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                optionsPanel.SetActive(false);
            }
        }
    }

    public void SelectItem(int selection)
    {
        switch (selection)
        {
            case 0:
                //Load Level
                SceneManager.LoadScene(LevelManager.Level);

                break;

            case 1:
                //Show Instructions Page

                instructionsPanel.SetActive(true);

                break;

            case 2:
                //Options

                optionsPanel.SetActive(true);

                break;

            case 3:
                //Quit
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }

    public void UpdateMenuSelection(int selectionIndex)
    {
        menuSelection = selectionIndex;
    }

    public void BackButton()
    {
        //Close options menu

        //optionsPanel.SetActive(false);
    }
}

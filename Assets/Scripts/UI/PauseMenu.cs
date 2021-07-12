using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MenuScript
{
    [SerializeField] private GameObject optionsPanel;
    private HUDManager hud;

    protected override void Start()
    {
        base.Start();

        hud = GetComponentInParent<HUDManager>();
    }

    protected override void Update()
    {
        if (!optionsPanel.activeSelf)
        {
            for (int i = 0; i < menuItems.Length; i++)
            {
                menuItems[i].color = defaultColor;
            }

            menuItems[menuSelection].color = selectedColor;
        }

        if (optionsPanel.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                optionsPanel.SetActive(false);
            }
        }
    }

    public override void SelectItem(int selection)
    {
        switch(selection)
        {
            case 0:
                //Resume: Un-pause
                hud.UnPauseGame();
                break;

            case 1:
                //Options
                optionsPanel.SetActive(true);
                break;

            case 2:
                //Main Menu
                hud.UnPauseGame();
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(LevelManager.MainMenu);

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
}

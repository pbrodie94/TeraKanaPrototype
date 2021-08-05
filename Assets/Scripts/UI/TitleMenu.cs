using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MenuScript
{
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject optionsPanel;

    protected override void Update()
    {
        base.Update();

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

    public override void SelectItem(int selection)
    {
        switch (selection)
        {
            case 0:
                //Load Level
                GameManager.instance.LoadLevel(LevelManager.Level);

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
                GameManager.instance.QuitGame();
                break;
        }
    }

    

    public void BackButton()
    {
        //Close options menu

        //optionsPanel.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndLabOne : ExtractionPoint
{
    protected override void ScanComplete()
    {
        //Exit to main menu
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(LevelManager.MainMenu);
    }
}

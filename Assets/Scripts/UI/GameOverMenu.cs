using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : MenuScript
{
   public override void SelectItem(int selectionIndex)
   {
      switch(selectionIndex)
      {
         case 0:
            //Return to main menu
            HUDManager.instance.UnPauseGame();
            Cursor.lockState = CursorLockMode.Confined;
            GameManager.instance.LoadLevel(LevelManager.MainMenu);
            break;
      }
   }
}

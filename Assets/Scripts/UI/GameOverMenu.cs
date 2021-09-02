using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MenuScript
{
   [SerializeField] private Text livesText;
   
   void Start()
   {
      livesText.text = "Lives: " + GameManager.lives;
      
      if (GameManager.lives <= 0)
      {
         menuItems[1].gameObject.SetActive(false);
         
      }
      else
      {
         menuItems[1].gameObject.SetActive(true);
      }
   }

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
         case 1:
            --GameManager.lives;
            GameManager.instance.LoadGame();
            break;
      }
   }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shmoove
{
    public class MainMenu : MonoBehaviour
    {
        public void ToMainMenu()
        {
            SceneManager.LoadScene("Level 1- ashton");
        }

      public void PlayGame()
        {
            SceneManager.LoadScene("Level 1- ashton");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}

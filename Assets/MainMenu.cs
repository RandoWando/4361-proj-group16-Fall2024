using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Shmoove
{
    public class MainMenu : MonoBehaviour
    {
        public void Update()
        {
            Cursor.lockState = CursorLockMode.None;
        }

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

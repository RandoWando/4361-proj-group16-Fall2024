using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
namespace Shmoove
{
    public class lastTryPt2 : MonoBehaviour
    {
        public void Update()
        {
            // locking cursor
            Cursor.lockState = CursorLockMode.None;
        }
        public void goToStart()
        {
            SceneManager.LoadScene("Level 1- ashton");
        }

        public void goToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}

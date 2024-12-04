using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search.Providers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shmoove
{
    public class newNewMainMenu : MonoBehaviour
    {
      public void endGame()
        {
            SceneManager.LoadScene("Level 1- ashton");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shmoove
{
    public class gameOverScreen : MonoBehaviour
    {
        public void endGame()
        {
            SceneManager.LoadScene("Level ashton");
        }
        
    }
}

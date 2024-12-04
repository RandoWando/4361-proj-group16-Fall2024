using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shmoove
{
    public class MainMenuClicked : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            MainMenuPressed();
        }

        private void MainMenuPressed()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            transform.position = new Vector3(5.5f, 10f, 3.90f);
            Debug.Log("Main Menu button was pressed.");
        }
    }
}

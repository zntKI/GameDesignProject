using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        if (index == 7)
        {
            index = 0;
        }

        SceneManager.LoadScene(index + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

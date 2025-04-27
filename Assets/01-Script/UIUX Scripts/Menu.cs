using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    bool Pause = false;

    public void StartScene(string Name)
    {
        SceneManager.LoadScene(Name);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetPause()
    {
        Pause = !Pause;
    }

    private void Update()
    {
        if (Pause == true)
        {
            Time.timeScale = 0.0f;
        }
        else if (Pause == false)
        {
            Time.timeScale = 1.0f;
        }
    }
}

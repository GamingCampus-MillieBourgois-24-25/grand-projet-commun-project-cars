using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptions : MonoBehaviour
{
    [SerializeField] GameObject Camera;
    [SerializeField] GameObject Screen;
    [SerializeField] GameObject Menu;

    public void GoGarage()
    {
        Menu.SetActive(false);
        Camera.GetComponent<Animator>().SetBool("IsGarage", true);
        Screen.GetComponenet<VideoPlayer>().Play();
    }

    public void GoMenu()
    {
        Camera.GetComponent<Animator>().SetBool("IsGarage", false);
        Screen.GetComponenet<VideoPlayer>().Stop();
        Menu.SetActive(true);
    }
}

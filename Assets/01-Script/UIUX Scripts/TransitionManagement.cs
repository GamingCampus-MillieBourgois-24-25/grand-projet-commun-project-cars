using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManagement : MonoBehaviour
{
    [SerializeField] CameraControl script2;
    [SerializeField] Button Button1;
    [SerializeField] Button Button2;

    public void Wait()
    {
        script2.enabled = false;
        Button1.interactable = false;
        Button2.interactable = false;
    }

    public void Go()
    {
        script2.enabled = true;
        Button1.interactable = true;
        Button2.interactable = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManagement : MonoBehaviour
{
    [SerializeField] Button Button1;
    [SerializeField] Button Button2;

    public void Wait()
    {
        Button1.interactable = false;
        Button2.interactable = false;
    }

    public void Go()
    {
        Button1.interactable = true;
        Button2.interactable = true;
    }
}

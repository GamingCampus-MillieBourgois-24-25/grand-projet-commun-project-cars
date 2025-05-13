using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarButton : MonoBehaviour
{
    [SerializeField] GameObject CanvasCar;
    [SerializeField] MenuScript script;
    [SerializeField] CameraControl script2;

    public void OnClick()
    {
        script2.rotationSpeed = 0;
        StartCoroutine(script.WaitToGoPersonalize());
    }

    public void Return()
    {
        CanvasCar.SetActive(false);
        StartCoroutine(script.WaitToGoGarage());
        script2.rotationSpeed = 1;
    }
}

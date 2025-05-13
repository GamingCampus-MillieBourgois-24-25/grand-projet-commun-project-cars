using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite sprite1;
    [SerializeField] Sprite sprite2;

    public void ChangeImageUI1()
    {
        image.sprite = sprite1;
    }

    public void ChangeImageUI2()
    {
        image.sprite = sprite2;
    }
}

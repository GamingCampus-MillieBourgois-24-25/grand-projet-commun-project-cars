using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPage : MonoBehaviour
{
    [SerializeField] GameObject[] ButtonsPage;
    [SerializeField] Sprite InactiveSprite;

    public void SelectANew()
    {
        for (int i = 0; i < ButtonsPage.Length; i++)
        {
            if (ButtonsPage[i].GetComponent<IsActive>().GetBool())
            {
                ButtonsPage[i].GetComponent<Image>().sprite = InactiveSprite;
                ButtonsPage[i].GetComponent<IsActive>().SetBool(false);
            }
        }
    }
}

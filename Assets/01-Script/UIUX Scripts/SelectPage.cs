using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPage : MonoBehaviour
{
    [SerializeField] GameObject[] ButtonsPage;
    [SerializeField] GameObject[] ScrollView;
    [SerializeField] Sprite InactiveSprite;

    public void SelectANew()
    {
        for (int i = 0; i < ButtonsPage.Length; i++)
        {
            if (ButtonsPage[i].GetComponent<IsActive>().GetBool())
            {
                ButtonsPage[i].GetComponent<Image>().sprite = InactiveSprite;
                Color color = ButtonsPage[i].GetComponent<Image>().color;
                color.a = 0.4f;
                ButtonsPage[i].GetComponent<Image>().color = color;
                ButtonsPage[i].GetComponent<IsActive>().SetBool(false);
            }
        }
    }

    public void UnactiveScrollView()
    {
        for (int i = 0; i < ScrollView.Length; i++)
        {
            if (ScrollView[i].activeInHierarchy)
            {
                ScrollView[i].SetActive(false);
            }
        }
    }

    public void ChangePage(Image Page)
    {
        Color clr = Page.color;
        clr.a = 1f; 
        Page.color = clr;
    }
}

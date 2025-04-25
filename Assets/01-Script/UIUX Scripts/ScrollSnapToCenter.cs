using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using System;

public class ScrollSnapToCenter : MonoBehaviour
{
    [SerializeField] CustomizeCar script;
    [SerializeField] SaveData script2 ;
    [SerializeField] Button Next;
    [SerializeField] Button Previous;
    [SerializeField] string Categorie;
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform SampleListItem;
    public HorizontalLayoutGroup horizontalLayoutGroup;
    public TMP_Text Name;

    public string[] ItemNames;

    private bool IsSnapped;
    private bool CanChange = false;

    public float snapForce;
    public int ItemNumber;
    float snapSpeed;

    private void Start()
    {
        IsSnapped = false;
        scrollRect.velocity = new Vector2(1350f*script2.GetCar(Categorie), 0f);
    }

    void Update()
    {
        ItemNumber = Mathf.RoundToInt((0 - content.localPosition.x / (SampleListItem.rect.width + horizontalLayoutGroup.spacing)));

        if (scrollRect.velocity.magnitude < 200)
        {
            scrollRect.velocity = Vector2.zero;
            snapSpeed += snapForce * Time.deltaTime;
            content.localPosition = new Vector3(
                Mathf.MoveTowards(content.localPosition.x, 0 - (ItemNumber * (SampleListItem.rect.width + horizontalLayoutGroup.spacing)), snapSpeed),
                content.localPosition.y,
                content.localPosition.z);
            Name.text = ItemNames[ItemNumber];
            if (content.localPosition.x == 0 - (ItemNumber) * (SampleListItem.rect.width + horizontalLayoutGroup.spacing))
            {
                IsSnapped = true;
                CanChange = true;
            }
        }
        if (scrollRect.velocity.magnitude > 200)
        {
            Name.text = "  ";
            IsSnapped = false;
            snapSpeed = 0;
        }
        if (scrollRect.velocity.magnitude > 0)
        {
            CanChange = false;
        }
    }

    //Car
    public void NextItemCar()
    {
        if (ItemNumber < ItemNames.Length - 1)
        {
            scrollRect.velocity = new Vector2(-1350f, 0f);
            StartCoroutine(change(script.NextCar, Next));
            script2.SetCar("Car", ItemNumber+1);
        }
    }

    public void PreviousItemCar()
    {
        if (ItemNumber > 0)
        {
            scrollRect.velocity = new Vector2(1350f, 0f);
            StartCoroutine(change(script.PreviousCar, Previous));
            script2.SetCar("Car", ItemNumber+1);
        }
    }

    //Roues
    public void NextItemRoues()
    {
        if (ItemNumber < ItemNames.Length - 1)
        {
            scrollRect.velocity = new Vector2(-1350f, 0f);
            StartCoroutine(change(script.NextRouesMaterial, Next));
            script2.SetCar("Roues", ItemNumber+1);
        }
    }

    public void PreviousItemRoues()
    {
        if (ItemNumber > 0)
        {
            scrollRect.velocity = new Vector2(1350f, 0f);
            StartCoroutine(change(script.PreviousRouesMaterial, Previous));
            script2.SetCar("Roues", ItemNumber+1);
        }
    }

    //Vitres
    public void NextItemVitres()
    {
        if (ItemNumber < ItemNames.Length - 1)
        {
            scrollRect.velocity = new Vector2(-1350f, 0f);
            StartCoroutine(change(script.NextVitresMaterial, Next));
            script2.SetCar("Vitres", ItemNumber+1);
        }
    }

    public void PreviousItemVitres()
    {
        if (ItemNumber > 0)
        {
            scrollRect.velocity = new Vector2(1350f, 0f);
            StartCoroutine(change(script.PreviousVitresMaterial, Previous));
            script2.SetCar("Vitres", ItemNumber+1);
        }
    }

    //Accessory
    public void NextItemAccessory()
    {
        if (ItemNumber < ItemNames.Length - 1)
        {
            scrollRect.velocity = new Vector2(-1350f, 0f);
            StartCoroutine(change(script.NextAccessoiresMaterial, Next));
            script2.SetCar("Accessory", ItemNumber+1);
        }
    }

    public void PreviousItemAccessory()
    {
        if (ItemNumber > 0)
        {
            scrollRect.velocity = new Vector2(1350f, 0f);
            StartCoroutine(change(script.PreviousAccessoiresMaterial, Previous));
            script2.SetCar("Accessory", ItemNumber+1);
        }
    }

    //Material
    public void NextItemMaterial()
    {
        if (ItemNumber < ItemNames.Length - 1)
        {
            scrollRect.velocity = new Vector2(-1350f, 0f);
            StartCoroutine(change(script.NextCarrosserieMaterial, Next));
            script2.SetCar("Material", ItemNumber+1);
        }
    }

    public void PreviousItemMaterial()
    {
        if (ItemNumber > 0)
        {
            scrollRect.velocity = new Vector2(1350f, 0f);
            StartCoroutine(change(script.PreviousCarrosserieMaterial, Previous));
            script2.SetCar("Material", ItemNumber+1);
        }
    }

    //Phares
    public void NextItemPhares()
    {
        if (ItemNumber < ItemNames.Length - 1)
        {
            scrollRect.velocity = new Vector2(-1350f, 0f);
            StartCoroutine(change(script.NextPharesMaterial, Next));
            script2.SetCar("Phares", ItemNumber+1);
        }
    }

    public void PreviousItemPhares()
    {
        if (ItemNumber > 0)
        {
            scrollRect.velocity = new Vector2(1350f, 0f);
            StartCoroutine(change(script.PreviousPharesMaterial, Previous));
            script2.SetCar("Phares", ItemNumber+1);
        }
    }

    IEnumerator change(Action action, Button bouton)
    {
        yield return new WaitForSeconds(1);
        action();
        script.SaveCarData();
    }
}

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
    private int Number;

    public float snapForce;
    public int ItemNumber;
    float snapSpeed;

    private void Start()
    {
        IsSnapped = false;
        content.anchoredPosition = new Vector2(-(570*script2.GetCar(Categorie)), 0);
        Number = script2.GetCar(Categorie);
    }

    void Update()
    {
        ItemNumber = Mathf.RoundToInt((0 - content.localPosition.x / (SampleListItem.rect.width + horizontalLayoutGroup.spacing)));

        if (scrollRect.velocity.magnitude < 200)
        {
            scrollRect.velocity = Vector2.zero;
            snapSpeed += snapForce * Time.deltaTime;
            content.localPosition = new Vector3(
                Mathf.MoveTowards(content.localPosition.x, 0 - (Number * (SampleListItem.rect.width + horizontalLayoutGroup.spacing)), snapSpeed),
                content.localPosition.y,
                content.localPosition.z);
            Name.text = ItemNames[ItemNumber];
            if (content.localPosition.x == 0 - (Number) * (SampleListItem.rect.width + horizontalLayoutGroup.spacing))
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
        if (Number < ItemNames.Length - 1)
        {
            Number += 1;
            scrollRect.velocity = new Vector2(-1500f, 0f);
            StartCoroutine(change(script.NextCar, Number));
            script2.SetCar("Car", Number);
        }
    }

    public void PreviousItemCar()
    {
        if (Number > 0)
        {
            Number -= 1;
            scrollRect.velocity = new Vector2(1500f, 0f);
            StartCoroutine(change(script.PreviousCar, Number));
            script2.SetCar("Car", Number);
        }
    }

    //Roues
    public void NextItemRoues()
    {
        if (Number < ItemNames.Length - 1)
        {
            Number += 1;
            scrollRect.velocity = new Vector2(-1500f, 0f);
            StartCoroutine(change(script.NextRouesMaterial, Number));
            script2.SetCar("Roues", Number);
        }
    }

    public void PreviousItemRoues()
    {
        if (Number > 0)
        {
            Number -= 1;
            scrollRect.velocity = new Vector2(1500f, 0f);
            StartCoroutine(change(script.PreviousRouesMaterial, Number));
            script2.SetCar("Roues", Number);
        }
    }

    //Vitres
    public void NextItemVitres()
    {
        if (Number < ItemNames.Length - 1)
        {
            Number += 1;
            scrollRect.velocity = new Vector2(-1500f, 0f);
            StartCoroutine(change(script.NextVitresMaterial, Number));
            script2.SetCar("Vitres", Number);
        }
    }

    public void PreviousItemVitres()
    {
        if (Number > 0)
        {
            Number -= 1;
            scrollRect.velocity = new Vector2(1500f, 0f);
            StartCoroutine(change(script.PreviousVitresMaterial, Number));
            script2.SetCar("Vitres", Number);
        }
    }

    //Accessory
    public void NextItemAccessory()
    {
        if (Number < ItemNames.Length - 1)
        {
            Number += 1;
            scrollRect.velocity = new Vector2(-1500f, 0f);
            StartCoroutine(change(script.NextAccessoiresMaterial, Number));
            script2.SetCar("Accessory", Number);
        }
    }

    public void PreviousItemAccessory()
    {
        if (Number > 0)
        {
            Number -= 1;
            scrollRect.velocity = new Vector2(1500f, 0f);
            StartCoroutine(change(script.PreviousAccessoiresMaterial, Number));
            script2.SetCar("Accessory", Number);
        }
    }

    //Material
    public void NextItemMaterial()
    {
        if (Number < ItemNames.Length - 1)
        {
            Number += 1;
            scrollRect.velocity = new Vector2(-1500f, 0f);
            StartCoroutine(change(script.NextCarrosserieMaterial, Number));
            script2.SetCar("Material", Number);
        }
    }

    public void PreviousItemMaterial()
    {
        if (Number > 0)
        {
            Number -= 1;
            scrollRect.velocity = new Vector2(1500f, 0f);
            StartCoroutine(change(script.PreviousCarrosserieMaterial, Number));
            script2.SetCar("Material", Number);
        }
    }

    //Phares
    public void NextItemPhares()
    {
        if (Number < ItemNames.Length - 1)
        {
            Number += 1;
            scrollRect.velocity = new Vector2(-1500f, 0f);
            StartCoroutine(change(script.NextPharesMaterial, Number));
            script2.SetCar("Phares", Number);
        }
    }

    public void PreviousItemPhares()
    {
        if (Number > 0)
        {
            Number -= 1;
            scrollRect.velocity = new Vector2(1500f, 0f);
            StartCoroutine(change(script.PreviousPharesMaterial, Number));
            script2.SetCar("Phares", Number);
        }
    }

    IEnumerator change(Action action, float Number)
    {
        yield return new WaitForSeconds(0.5f);
        action();
        script.SaveCarData();
        //yield return new WaitForSeconds(3f);
        //content.anchoredPosition = new Vector2(-(570 * Number), 0);
    }
}

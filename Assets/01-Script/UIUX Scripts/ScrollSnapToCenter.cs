using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using System;

public class ScrollSnapToCenter : MonoBehaviour
{
    [SerializeField] CustomizeCar script;
    [SerializeField] Button Next;
    [SerializeField] Button Previous;
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
        Next.interactable = false;
        if (ItemNumber < ItemNames.Length - 1)
        {
            scrollRect.velocity = new Vector2(-1350f, 0f);
            StartCoroutine(change(script.NextCar, Next));
        }
    }

    public void PreviousItemCar()
    {
        Previous.interactable = false;
        if (ItemNumber > 0)
        {
            scrollRect.velocity = new Vector2(1350f, 0f);
            StartCoroutine(change(script.PreviousCar, Previous));
        }
    }

    //Roues
    public void NextItemRoues()
    {
        Next.interactable = false;
        if (ItemNumber < ItemNames.Length - 1)
        {
            scrollRect.velocity = new Vector2(-1350f, 0f);
            StartCoroutine(change(script.NextRouesMaterial, Next));
        }
    }

    public void PreviousItemRoues()
    {
        Previous.interactable = false;
        if (ItemNumber > 0)
        {
            scrollRect.velocity = new Vector2(1350f, 0f);
            StartCoroutine(change(script.PreviousRouesMaterial, Previous));
        }
    }

    //Vitres
    public void NextItemVitres()
    {
        Next.interactable = false;
        if (ItemNumber < ItemNames.Length - 1)
        {
            scrollRect.velocity = new Vector2(-1350f, 0f);
            StartCoroutine(change(script.NextVitresMaterial, Next));
        }
    }

    public void PreviousItemVitres()
    {
        Previous.interactable = false;
        if (ItemNumber > 0)
        {
            scrollRect.velocity = new Vector2(1350f, 0f);
            StartCoroutine(change(script.PreviousVitresMaterial, Previous));
        }
    }

    //Accessory
    public void NextItemAccessory()
    {
        Next.interactable = false;
        if (ItemNumber < ItemNames.Length - 1)
        {
            scrollRect.velocity = new Vector2(-1350f, 0f);
            StartCoroutine(change(script.NextAccessoiresMaterial, Next));
        }
    }

    public void PreviousItemAccessory()
    {
        Previous.interactable = false;
        if (ItemNumber > 0)
        {
            scrollRect.velocity = new Vector2(1350f, 0f);
            StartCoroutine(change(script.PreviousAccessoiresMaterial, Previous));
        }
    }

    //Material
    public void NextItemMaterial()
    {
        Next.interactable = false;
        if (ItemNumber < ItemNames.Length - 1)
        {
            scrollRect.velocity = new Vector2(-1350f, 0f);
            StartCoroutine(change(script.NextCarrosserieMaterial, Next));
        }
    }

    public void PreviousItemMaterial()
    {
        Previous.interactable = false;
        if (ItemNumber > 0)
        {
            scrollRect.velocity = new Vector2(1350f, 0f);
            StartCoroutine(change(script.PreviousCarrosserieMaterial, Previous));
        }
    }

    //Phares
    public void NextItemPhares()
    {
        Next.interactable = false;
        if (ItemNumber < ItemNames.Length - 1)
        {
            scrollRect.velocity = new Vector2(-1350f, 0f);
            StartCoroutine(change(script.NextPharesMaterial, Next));
        }
    }

    public void PreviousItemPhares()
    {
        Previous.interactable = false;
        if (ItemNumber > 0)
        {
            scrollRect.velocity = new Vector2(1350f, 0f);
            StartCoroutine(change(script.PreviousPharesMaterial, Previous));
        }
    }

    IEnumerator change(Action action, Button bouton)
    {
        yield return new WaitForSeconds(1);
        action();
        bouton.interactable = true;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ScrollSnapToCenter : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform SampleListItem;
    public HorizontalLayoutGroup horizontalLayoutGroup;
    public TMP_Text Name;

    public string[] ItemNames;

    private bool IsSnapped;

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
        Debug.Log(ItemNumber);

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
            }
        }
        if (scrollRect.velocity.magnitude > 200)
        {
            Name.text = "  ";
            IsSnapped = false;
            snapSpeed = 0;
        }
    }

    public void NextItem()
    {
        if (ItemNumber < ItemNames.Length - 1)
        {
            scrollRect.velocity = new Vector2(-1350f, 0f);
        }
    }

    public void PreviousItem()
    {
        if (ItemNumber > 0)
        {
            scrollRect.velocity = new Vector2(1350f, 0f);
        }
    }

}

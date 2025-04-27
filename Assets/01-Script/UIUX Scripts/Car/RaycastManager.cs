using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    [SerializeField] GameObject Menu;

    void Update()
    {
        if (!Menu.activeSelf)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var carButton = hit.collider.GetComponent<CarButton>();
                if (carButton != null && Input.GetMouseButtonDown(0))
                {
                    carButton.OnClick();
                }
            }
        }
    }
}

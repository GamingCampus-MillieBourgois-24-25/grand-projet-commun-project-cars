using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    void Update()
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField] GameObject Cars;
    [SerializeField] GameObject CarsScroll;
    [SerializeField] GameObject Stats_;

    // Update is called once per frame
    void Update()
    {
        if(Cars != null && Cars.activeSelf && CarsScroll.activeSelf)
        {
            Stats_.SetActive(true);
        }
        else { Stats_.SetActive(false); }
    }
}

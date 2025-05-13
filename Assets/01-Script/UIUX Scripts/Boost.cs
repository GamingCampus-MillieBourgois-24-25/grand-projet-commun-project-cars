using CarController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Boost : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_TextMeshPro;
    [SerializeField] Slider boost;
    [SerializeField] CarController.CarController script;

    void Update()
    {
        m_TextMeshPro.SetText(script.GetCurrentLap().ToString());
        float boostRatio = script.GetBoostAmount() / script.GetMaxBoostAmount();
        boost.value = 1 - boostRatio;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneParameter : MonoBehaviour
{
    [Header("Scene Parameter")]
    [SerializeField] private int targetFrameRate = 30;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Get the refresh rate of the display
        double maxFrameRate = Screen.currentResolution.refreshRateRatio.value;
        if ( maxFrameRate <= targetFrameRate)
        {
            Application.targetFrameRate = (int)maxFrameRate;
        }
        else
        {
            Application.targetFrameRate = targetFrameRate;
        }

    }
}

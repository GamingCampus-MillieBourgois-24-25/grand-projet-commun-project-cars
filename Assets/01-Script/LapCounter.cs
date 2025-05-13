using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapCounter : MonoBehaviour
{
    
    [Header("Lap Counter")]
    [SerializeField] private string playerTag = "Joueur";
    [SerializeField] Checkpoint script;
    private int maxLap;
    
    private void Start()
    {
        maxLap = GameManager.Instance.MaxLap;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            CarController.CarController player = other.GetComponent<CarController.CarController>();
            if (player != null & player.GetCurrentCheckpoint() == 1)
            {
                player.LapCompleted();
                player.SetCurrentCheckpoint(0);
            }
        }
    }
    
}

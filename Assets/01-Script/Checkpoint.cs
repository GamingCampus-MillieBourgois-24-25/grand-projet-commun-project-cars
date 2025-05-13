using System.Collections;
using System.Collections.Generic;
using CarController;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Lap Counter")]
    [SerializeField] private string playerTag = "Joueur";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            CarController.CarController player = other.GetComponent<CarController.CarController>();
            if (player != null)
            {
                player.CheckpointCompleted();
            }
        }
    }

}
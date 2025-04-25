using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CineBehav : MonoBehaviour
{
    [Header("Section 1")]
    [SerializeField] private GameObject[] sectionTarget;
    [SerializeField] private Vector3[] sectionTargetPos;
    [SerializeField] private CinemachineVirtualCamera[] virtualCamera;

    [Header("Paramètres de la séquence")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool loopSequence = true;
    [SerializeField] private float waitTimeBetweenSequences = 0.5f;
    
    [Header("Paramètres des caméras")]
    [SerializeField] private int defaultPriority = 0; // Priorité plus basse pour éviter les transitions
    [SerializeField] private int activePriority = 15;
    [SerializeField] private bool useInstantCuts = true; // Option pour des coupures nettes entre caméras

    private int currentIndex = 0;
    private bool isSequenceRunning = false;

    private void Start()
    {
        // Configuration initiale des caméras
        ConfigureAllCameras();
        
        // Démarrer la séquence
        StartCameraSequence();
    }
    
    private void ConfigureAllCameras()
    {
        for (int i = 0; i < virtualCamera.Length; i++)
        {
            // S'assurer que toutes les caméras sont désactivées au départ
            virtualCamera[i].Priority = defaultPriority;
            
            // Vérifier si chaque caméra a un Follow et un LookAt définis
            if (sectionTarget[i] != null)
            {
                // Configurer la cible de suivi (Follow)
                virtualCamera[i].Follow = sectionTarget[i].transform;
                
                // Configurer la cible de regard (LookAt) - optionnel
                // virtualCamera[i].LookAt = sectionTarget[i].transform;
            }
        }
    }

    public void StartCameraSequence()
    {
        if (!isSequenceRunning)
        {
            isSequenceRunning = true;
            currentIndex = 0;
            StartCoroutine(CameraSequenceCoroutine());
        }
    }

    private IEnumerator CameraSequenceCoroutine()
    {
        while (true)
        {
            // Vérifier que les tableaux ont la même taille
            if (currentIndex < sectionTarget.Length &&
                currentIndex < sectionTargetPos.Length &&
                currentIndex < virtualCamera.Length)
            {
                // Activer la caméra actuelle d'abord (coupure nette)
                virtualCamera[currentIndex].Priority = activePriority;
                
                // Déplacer l'objet cible
                yield return StartCoroutine(MoveObject(sectionTarget[currentIndex], sectionTargetPos[currentIndex]));

                // Attendre un court instant à la position finale pour une meilleure lisibilité
                yield return new WaitForSeconds(waitTimeBetweenSequences);
                
                // Désactiver la caméra actuelle avant de passer à la suivante
                virtualCamera[currentIndex].Priority = defaultPriority;
                
                // Passer à l'index suivant
                currentIndex++;

                // Si on a terminé tous les indices et qu'on boucle
                if (currentIndex >= sectionTarget.Length && loopSequence)
                {
                    currentIndex = 0;
                }
                else if (currentIndex >= sectionTarget.Length)
                {
                    // Fin de la séquence sans boucle
                    isSequenceRunning = false;
                    break;
                }
            }
            else
            {
                Debug.LogError("Les tableaux n'ont pas la même taille ou sont vides");
                isSequenceRunning = false;
                break;
            }
        }
    }

    private IEnumerator MoveObject(GameObject obj, Vector3 targetPosition)
    {
        Vector3 startPosition = obj.transform.position;
        float journeyDistance = Vector3.Distance(startPosition, targetPosition);
        float journeyTime = journeyDistance / moveSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < journeyTime)
        {
            float t = elapsedTime / journeyTime;
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Assurer que l'objet est exactement à la position cible
        obj.transform.position = targetPosition;
    }
}
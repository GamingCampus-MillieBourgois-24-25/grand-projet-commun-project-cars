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
    private CinemachineVirtualCamera playerCamera;
    private GameObject player;

    [Header("Paramètres de la séquence")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool loopSequence = true;
    [SerializeField] private float waitTimeBetweenSequences = 0.5f;
    [SerializeField] private float pauseBeforeRestart = 2f; // Pause avant de recommencer la boucle

    [Header("Paramètres des caméras")]
    [SerializeField] private int defaultPriority = 0;
    [SerializeField] private int activePriority = 15;
    [SerializeField] private bool useInstantCuts = true;

    private int currentIndex = 0;
    private bool isSequenceRunning = false;

    private void Start()
    {
        // Get the player that has been spawn by the PlayerSpawnPoint
        player = GameManager.Instance.PlayerCarInstance;
        if (player != null)
        {
            playerCamera = player.GetComponentInChildren<CinemachineVirtualCamera>();
        }
        
        // Démarrer la séquence
        StartCameraSequence();
    }

    public void StartCameraSequence()
    {
        if (!isSequenceRunning)
        {
            isSequenceRunning = true;
            currentIndex = 0;
            
            // Désactiver la caméra du joueur au début de la séquence
            if (playerCamera != null)
            {
                playerCamera.Priority = defaultPriority;
            }
            
            StartCoroutine(CameraSequenceCoroutine());
        }
    }

    public void StopCameraSequence()
    {
        isSequenceRunning = false;
        StopAllCoroutines();

        // Réinitialiser toutes les priorités de caméra
        foreach (var cam in virtualCamera)
        {
            if (cam != null)
                cam.Priority = defaultPriority;
        }
        
        // Activer la caméra du joueur lorsqu'on arrête manuellement la séquence
        if (playerCamera != null)
        {
            playerCamera.Priority = activePriority;
        }
    }

    private IEnumerator CameraSequenceCoroutine()
    {
        // Continuer tant que la séquence est en cours
        while (isSequenceRunning)
        {
            if (currentIndex < sectionTarget.Length &&
                currentIndex < sectionTargetPos.Length &&
                currentIndex < virtualCamera.Length)
            {
                // Activer la caméra actuelle
                virtualCamera[currentIndex].Priority = activePriority;

                // Déplacer l'objet cible
                yield return StartCoroutine(MoveObject(sectionTarget[currentIndex], sectionTargetPos[currentIndex]));

                // Attendre un court instant
                yield return new WaitForSeconds(waitTimeBetweenSequences);

                // Désactiver la caméra actuelle
                virtualCamera[currentIndex].Priority = defaultPriority;

                // Passer à l'index suivant
                currentIndex++;

                // Si on a terminé tous les objets
                if (currentIndex >= sectionTarget.Length)
                {
                    // Activer la caméra du joueur à la fin de la séquence
                    if (playerCamera != null)
                    {
                        playerCamera.Priority = activePriority;
                    }

                    if (loopSequence)
                    {
                        // Pause plus longue avant de recommencer
                        yield return new WaitForSeconds(pauseBeforeRestart);

                        // Désactiver la caméra du joueur avant de redémarrer la séquence
                        if (playerCamera != null)
                        {
                            playerCamera.Priority = defaultPriority;
                        }

                        currentIndex = 0;
                    }
                    else
                    {
                        // Fin de la séquence sans boucle
                        isSequenceRunning = false;
                        GameManager.Instance.StartGameSequence();
                        
                        break;
                    }
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
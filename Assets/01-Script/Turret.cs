using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    #region Variables
    
    [Header("Turret Settings")]
    [SerializeField] private float range = 10f;
    [SerializeField] private float fireCooldown = 10f;
    [SerializeField] private string targetTag = "Enemy";
    [SerializeField] private float rotationOffset = 90f; // Offset de rotation pour la tête de la tourelle
    
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    private float nextFireTime = 0f;
    
    [Header("References")]
    [SerializeField] private Transform TurretBody;
    [SerializeField] private Transform TurretHead;
    [SerializeField] private SphereCollider TurretDetector;
    [SerializeField] private Transform SpawnPoint;
    
    private Transform target;
    
    #endregion
    
    #region Unity Methods
    
    private void Start()
    {
        // Initialize the turret
        target = null;
        
        // Set the turret's range
        TurretDetector.radius = range;
        nextFireTime = Time.time + fireCooldown;
    }
    
    private void Update()
    {
        // Check if the turret has a target
        if (target == null)
        {
            target = FindClosestTarget();
        }
        
        // If a target is found, rotate and fire at it
        if (target != null)
        {
            RotateTurret(target);
            FireAtTarget(target);
            
            // Check if the target is out of range
            if (Vector3.Distance(transform.position, target.position) > range)
            {
                target = null; // Reset the target
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw a sphere to visualize the turret's range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the turret can detect the target
        if (other.CompareTag(targetTag))
        {
            target = other.transform;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // Reset the target if it leaves the turret's range
        if (other.CompareTag(targetTag))
        {
            target = null;
        }
    }
    
    #endregion

    #region Turret Logic

    // Find the closest target within range
    private Transform FindClosestTarget()
    {
        
        Collider[] targetsInRange = Physics.OverlapSphere(transform.position, range);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider target in targetsInRange)
        {
            if (target.CompareTag(targetTag))
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    closestTarget = target.transform;
                }
            }
        }

        return closestTarget;
    }
    
    // Rotate the body horizontally (the head is a child of the body) and the head vertically
    private void RotateTurret(Transform target)
    {
        Vector3 direction = (target.position - TurretBody.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = lookRotation.eulerAngles;
        
        // Rotate the body
        TurretBody.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        
        // Rotate the head
        float headRotationX = Mathf.Clamp(rotation.x, -45f, 45f);
        TurretHead.localRotation = Quaternion.Euler(0f, rotationOffset, headRotationX);
    }
    
    // Fire at the target a fake bullet for debug purposes using a raycast
    private void FireAtTarget(Transform target)
    {
        // Vérifier si le délai de recharge est écoulé
        if (Time.time >= nextFireTime)
        {
            // Mettre à jour le temps du prochain tir
            nextFireTime = Time.time + fireCooldown;
            Debug.Log("Tir sur la cible : " + target.name);

            // Vérifier que le prefab est assigné
            if (projectilePrefab == null)
            {
                Debug.LogError("Projectile prefab non assigné sur la tourelle!");
                return;
            }

            // Calculer la direction vers la cible
            Vector3 direction = (target.position - SpawnPoint.position).normalized;
        
            // Créer une rotation qui pointe vers la cible
            Quaternion targetRotation = Quaternion.LookRotation(direction);
        
            // Instancier le projectile avec la rotation correcte
            GameObject projectile = Instantiate(projectilePrefab, SpawnPoint.position, targetRotation);

            // Obtenir le Rigidbody du projectile
            Rigidbody rb = projectile.GetComponentInChildren<Rigidbody>();

            if (rb != null)
            {
                // Appliquer une force dans la direction avant du projectile
                rb.velocity = projectile.transform.forward * projectileSpeed;
            }
            else
            {
                Debug.Log("No RB");
            }
        }
    }

    #endregion
    
    
    
}

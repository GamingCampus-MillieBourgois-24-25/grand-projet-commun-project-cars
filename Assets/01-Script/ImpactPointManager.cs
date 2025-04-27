using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactPointManager : MonoBehaviour
{
    // Liste statique qui stocke les points d'impact
    public static List<Vector3> impactPoints = new List<Vector3>();
    public static float displayTime = 60f; // Durée d'affichage en secondes
    
    // Structure pour stocker les points d'impact et leur temps de création
    private static List<ImpactPoint> impacts = new List<ImpactPoint>();
    
    private struct ImpactPoint
    {
        public Vector3 position;
        public float creationTime;
    }
    
    private void Update()
    {
        // Supprimer les points d'impact trop anciens
        float currentTime = Time.time;
        impacts.RemoveAll(impact => currentTime - impact.creationTime > displayTime);
    }
    
    // Ajouter un nouveau point d'impact
    public static void AddImpactPoint(Vector3 position)
    {
        impacts.Add(new ImpactPoint { position = position, creationTime = Time.time });
    }
    
    // Dessiner les points d'impact
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var impact in impacts)
        {
            Gizmos.DrawSphere(impact.position, 0.2f);
        }
    }
}
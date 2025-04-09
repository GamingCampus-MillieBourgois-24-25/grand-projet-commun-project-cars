using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    
    // Pour faciliter le débogage
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawRay(transform.position, transform.forward * 2);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up, "Point de spawn joueur");
#endif
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    
    // Load player prefab from GameManager using the index
    private GameObject playerPrefab;
    private GameObject playerInstance;
    private int playerPrefabIndex;
    private int playerMaterialIndex;
    private CustomizeCar customizer;
    private GameObject body;
    
    private void Awake()
    {
        // Charger le préfab de joueur depuis le GameManager
        playerPrefabIndex = GameManager.Instance.PlayerPrefabIndex;
        playerPrefab = GameManager.Instance.CarPrefabs[playerPrefabIndex];
        playerMaterialIndex = GameManager.Instance.PlayerMaterialIndex;
        
        // Vérifier si le préfab est valide
        if (playerPrefab == null)
        {
            Debug.LogError("Le préfab de joueur est nul. Assurez-vous qu'il est assigné dans le GameManager.");
            return;
        }
        
        customizer = FindFirstObjectByType<CustomizeCar>();
        // Vérifier si le customizer est valide
        if (customizer == null)
        {
            Debug.LogError("Le customizer est nul. Assurez-vous qu'il est assigné dans le GameManager.");
            return;
        }
        
    }

    private void start()
    {
        body = customizer.FindFirstComponentWithTag("Body");
        // Vérifier si le corps est valide
        if (body == null)
        {
            Debug.LogError("Le corps est nul. Assurez-vous qu'il est assigné dans le GameManager.");
            return;
        }
        // Appliquer le matériau au corps
        customizer.ApplyMaterial(body, playerMaterialIndex);

        // Instancier le joueur
        playerInstance = Instantiate(playerPrefab, transform.position, Quaternion.identity);
    }
    
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
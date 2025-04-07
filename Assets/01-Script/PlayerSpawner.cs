using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Préfabs de voitures (facultatif si le GameManager est utilisé)")]
    public GameObject[] carPrefabs;

    [Header("Configuration")]
    [Tooltip("Position Y supplémentaire lors du spawn")]
    public float heightOffset = 0.1f;

    private GameObject playerCar;

    void Start()
    {
        SpawnerVoiture();
    }

    public void SpawnerVoiture()
    {
        // Destruction de la voiture précédente si elle existe
        if (playerCar != null)
        {
            Destroy(playerCar);
        }

        GameObject prefabVoiture = null;

        // Méthode 1: Utilisation du GameManager (recommandée)
        if (GameManager.Instance != null)
        {
            prefabVoiture = GameManager.Instance.ObtenirVoitureSelectionnee();
        }
        // Méthode 2: Utilisation des PlayerPrefs (fallback)
        else if (PlayerPrefs.HasKey("VoitureSelectionneeIndex") && carPrefabs != null && carPrefabs.Length > 0)
        {
            int index = PlayerPrefs.GetInt("VoitureSelectionneeIndex");
            if (index >= 0 && index < carPrefabs.Length)
            {
                prefabVoiture = carPrefabs[index];
            }
        }

        // Si aucune voiture n'est trouvée, utiliser la première disponible ou afficher une erreur
        if (prefabVoiture == null)
        {
            if (carPrefabs != null && carPrefabs.Length > 0)
            {
                prefabVoiture = carPrefabs[0];
                Debug.LogWarning("Aucune voiture sélectionnée trouvée, utilisation de la première voiture par défaut.");
            }
            else
            {
                Debug.LogError("Aucune voiture disponible pour le spawn !");
                return;
            }
        }

        // Création de la voiture du joueur
        Vector3 spawnPosition = transform.position + new Vector3(0, heightOffset, 0);
        playerCar = Instantiate(prefabVoiture, spawnPosition, transform.rotation);

        // Ajoutez ici les composants nécessaires au joueur (contrôleur, caméra, etc.)
        // Par exemple: playerCar.AddComponent<CarController>();
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

    public GameObject GetPlayerCar()
    {
        return playerCar;
    }
}
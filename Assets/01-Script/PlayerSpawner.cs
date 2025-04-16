using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Options de spawn")]
    [SerializeField] private bool spawnOnStart = true;
    
    private GameObject playerCarInstance;
    
    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnPlayerCar();
        }
    }
    
    public GameObject SpawnPlayerCar()
    {
        // Récupérer les données de la voiture du joueur via le GameManager
        CarData carData = GameManager.Instance.GetPlayerCarData();
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        
        if (carData != null && carData.CarInfo.carPrefab != null)
        {
            // Supprimer l'ancienne instance si elle existe
            if (playerCarInstance != null)
            {
                Destroy(playerCarInstance);
            }
            
            // Instancier le prefab de la voiture à la position et rotation du spawner
            playerCarInstance = Instantiate(carData.CarInfo.carPrefab, transform.position, transform.rotation);
            
            // Appliquer les personnalisations
            ApplyCarCustomizations(playerCarInstance, carData, playerData);
            
            return playerCarInstance;
        }
        else
        {
            Debug.LogError("Impossible de spawner la voiture: CarData ou prefab invalide");
            return null;
        }
    }
    
    private void ApplyCarCustomizations(GameObject carInstance, CarData carData, PlayerData playerData)
    {
        // Appliquer les matériaux de carrosserie
        ApplyMaterial(carInstance, carData, CarMaterialType.Carrosserie, playerData.PlayerCarCarrosserieIndex);
        
        // Appliquer les matériaux d'accessoires
        ApplyMaterial(carInstance, carData, CarMaterialType.Accessoires, playerData.PlayerCarAccessoireIndex);
        
        // Appliquer les matériaux de phares
        ApplyMaterial(carInstance, carData, CarMaterialType.Phares, playerData.PlayerCarPharesIndex);
        
        // Appliquer les matériaux de vitres
        ApplyMaterial(carInstance, carData, CarMaterialType.Vitres, playerData.PlayerCarVitresIndex);
        
        // Appliquer les matériaux de roues
        ApplyWheelMaterial(carInstance, carData, CarMaterialType.Roues, playerData.PlayerCarRouesIndex);
    }
    
    private void ApplyMaterial(GameObject carInstance, CarData carData, CarMaterialType materialType, int materialIndex)
    {
        // Vérifier si l'index est valide et si des matériaux sont disponibles
        List<Material> materials = carData.GetMaterials(materialType);
        if (materialIndex >= 0 && materials != null && materialIndex < materials.Count)
        {
            // Obtenir le renderer du châssis
            MeshRenderer chassisRenderer = carData.GetChassisRenderer(carInstance);
            
            if (chassisRenderer != null)
            {
                Material[] rendererMaterials = chassisRenderer.materials;
                int indexInRenderer = carData.GetIndexInRenderer(materialType);
                
                if (rendererMaterials.Length > indexInRenderer)
                {
                    rendererMaterials[indexInRenderer] = carData.GetMaterial(materialType, materialIndex);
                    chassisRenderer.materials = rendererMaterials;
                }
            }
        }
    }
    
    private void ApplyWheelMaterial(GameObject carInstance, CarData carData, CarMaterialType materialType, int materialIndex)
    {
        List<Material> materials = carData.GetMaterials(materialType);
        if (materialIndex >= 0 && materials != null && materialIndex < materials.Count)
        {
            List<MeshRenderer> rouesRenderers = carData.GetRouesRenderers(carInstance);
            
            if (rouesRenderers != null && rouesRenderers.Count > 0)
            {
                foreach (MeshRenderer renderer in rouesRenderers)
                {
                    Material[] rendererMaterials = renderer.materials;
                    int indexInRenderer = carData.GetIndexInRenderer(materialType);
                    
                    if (rendererMaterials.Length > indexInRenderer)
                    {
                        rendererMaterials[indexInRenderer] = carData.GetMaterial(materialType, materialIndex);
                        renderer.materials = rendererMaterials;
                    }
                }
            }
        }
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
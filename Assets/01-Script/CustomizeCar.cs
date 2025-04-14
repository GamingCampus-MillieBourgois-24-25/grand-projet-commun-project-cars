using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomizeCar : MonoBehaviour
{
    [Header("Scene Loader")]
    [SerializeField] private string sceneName = "Level1-DEVMAP";
    
    [Header("Spawn Point")]
    [SerializeField] private Transform spawnPoint;
    
    // Get the car data from the GameManager
    private CarData carData;
    private GameObject carInstance; // Instance of the car prefab
    private int carIndex; // Index of the car in the GameManager
    private int carrosserieIndex; // Index of the carrosserie material
    private int accessoiresIndex; // Index of the accessoires material
    
    // Spawn the car at the spawn point
    private void Start()
    {
        carData = GameManager.Instance.GetPlayerCarData();
        
        if (spawnPoint != null)
        {
            // Instancier la voiture à la position du spawn
            carInstance = Instantiate(carData.CarInfo.carPrefab, spawnPoint.position, spawnPoint.rotation);
            carInstance.transform.SetParent(spawnPoint);
        }
        else
        {
            Debug.LogError("Aucun point de spawn spécifié !");
        }
    }
    
    // Change the car
    public void ChangeCar(int index)
    {
        // Vérifier si l'index est valide
        if (index >= 0 && index < GameManager.Instance.GetCarDataCount())
        {
            // Détruire l'instance actuelle de la voiture
            if (carInstance != null)
            {
                Destroy(carInstance);
            }
            
            // Charger les nouvelles données de la voiture
            carData = GameManager.Instance.GetCarData(index);
            
            // Instancier la nouvelle voiture à la position du spawn
            carInstance = Instantiate(carData.CarInfo.carPrefab, spawnPoint.position, spawnPoint.rotation);
            carInstance.transform.SetParent(spawnPoint);
        }
        else
        {
            Debug.LogError("Index de voiture invalide : " + index);
        }
    }
    
    // Next car
    public void NextCar()
    {
        // Incrémenter l'index de la voiture
        carIndex = (carIndex + 1) % GameManager.Instance.GetCarDataCount();
        
        // Changer la voiture
        ChangeCar(carIndex);
    }
    
    // Previous car
    public void PreviousCar()
    {
        // Décrémenter l'index de la voiture
        carIndex = (carIndex - 1 + GameManager.Instance.GetCarDataCount()) % GameManager.Instance.GetCarDataCount();
        
        // Changer la voiture
        ChangeCar(carIndex);
    }
    
    // Change Les matéraiux du Renderer Chassis (Accessoires,Carrosserie,Phares,Vitres)
    
    // Change le matériau de carrosserie
    public void ChangeCarrosserieMaterial(int index)
    {
        // Vérifier si l'index est valide
        if (index >= 0 && index < carData.GetMaterials(CarMaterialType.Carrosserie).Count)
        {
            // Obtenir le renderer du châssis
            MeshRenderer chassisRenderer = carData.GetChassisRenderer(carInstance);
            
            // Changer le matériau de la carrosserie
            if (chassisRenderer != null)
            {
                Material[] materials = chassisRenderer.materials;
                if (materials.Length > carData.GetIndexInRenderer(CarMaterialType.Carrosserie))
                {
                    materials[carData.GetIndexInRenderer(CarMaterialType.Carrosserie)] = carData.GetMaterial(CarMaterialType.Carrosserie, index);
                    chassisRenderer.materials = materials;
                }
                else
                {
                    Debug.LogError("Le renderer n'a pas assez de matériaux !");
                }
            }
            else
            {
                Debug.LogError("Aucun renderer de châssis trouvé !");
            }
        }
        else
        {
            Debug.LogError("Index de matériau de carrosserie invalide : " + index);
        }
    }
    
    // Next Carrosserie Material
    public void NextCarrosserieMaterial()
    {
        // Incrémenter l'index de la carrosserie
        carrosserieIndex = (carrosserieIndex + 1) % carData.GetMaterials(CarMaterialType.Carrosserie).Count;
        
        // Changer le matériau de la carrosserie
        ChangeCarrosserieMaterial(carrosserieIndex);
    }
    
    // Previous Carrosserie Material
    public void PreviousCarrosserieMaterial()
    {
        // Décrémenter l'index de la carrosserie
        carrosserieIndex = (carrosserieIndex - 1 + carData.GetMaterials(CarMaterialType.Carrosserie).Count) % carData.GetMaterials(CarMaterialType.Carrosserie).Count;
        
        // Changer le matériau de la carrosserie
        ChangeCarrosserieMaterial(carrosserieIndex);
    }
    
    // Change le matériau d'accessoires
    public void ChangeAccessoiresMaterial(int index)
    {
        // Vérifier si l'index est valide
        if (index >= 0 && index < carData.GetMaterials(CarMaterialType.Accessoires).Count)
        {
            // Obtenir le renderer du châssis
            MeshRenderer chassisRenderer = carData.GetChassisRenderer(carInstance);
            
            // Changer le matériau d'accessoires a l'index spécifié dans le carData
            if (chassisRenderer != null)
            {
                Material[] materials = chassisRenderer.materials;
                if (materials.Length > carData.GetIndexInRenderer(CarMaterialType.Accessoires))
                {
                    materials[carData.GetIndexInRenderer(CarMaterialType.Accessoires)] = carData.GetMaterial(CarMaterialType.Accessoires, index);
                    chassisRenderer.materials = materials;
                }
                else
                {
                    Debug.LogError("Le renderer n'a pas assez de matériaux !");
                }
            }
            else
            {
                Debug.LogError("Aucun renderer de châssis trouvé !");
            }
            
        }
        else
        {
            Debug.LogError("Index de matériau d'accessoires invalide : " + index);
        }
    }
    
    // Next Accessoires Material
    public void NextAccessoiresMaterial()
    {
        // Incrémenter l'index d'accessoires
        accessoiresIndex = (accessoiresIndex + 1) % carData.GetMaterials(CarMaterialType.Accessoires).Count;
        
        // Changer le matériau d'accessoires
        ChangeAccessoiresMaterial(accessoiresIndex);
    }
    // Previous Accessoires Material
    public void PreviousAccessoiresMaterial()
    {
        // Décrémenter l'index d'accessoires
        accessoiresIndex = (accessoiresIndex - 1 + carData.GetMaterials(CarMaterialType.Accessoires).Count) % carData.GetMaterials(CarMaterialType.Accessoires).Count;
        
        // Changer le matériau d'accessoires
        ChangeAccessoiresMaterial(accessoiresIndex);
    }

    private void OnDrawGizmos()
    {
        // Dessiner un cube à la position du GameObject
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);

        // Dessiner une flèche pour l'axe Z (avant)
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 0.7f);
        DrawArrowTip(transform.position + transform.forward * 0.7f, transform.forward, Color.red);

        // Dessiner une ligne pour l'axe Y (haut)
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * 0.5f);

        // Dessiner une ligne pour l'axe X (droite)
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.right * 0.5f);

        // Étiquette pour identifier le spawn
        Gizmos.color = Color.white;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.6f, "Point de spawn");
#endif
    }

    // Dessine la pointe d'une flèche
    private void DrawArrowTip(Vector3 position, Vector3 direction, Color color)
    {
        Vector3 right = Vector3.Cross(Vector3.up, direction).normalized * 0.1f;
        if (right == Vector3.zero)
            right = Vector3.Cross(Vector3.forward, direction).normalized * 0.1f;
        Vector3 up = Vector3.Cross(direction, right).normalized * 0.1f;

        Gizmos.color = color;
        Gizmos.DrawRay(position, -direction * 0.2f + right);
        Gizmos.DrawRay(position, -direction * 0.2f - right);
        Gizmos.DrawRay(position, -direction * 0.2f + up);
        Gizmos.DrawRay(position, -direction * 0.2f - up);
    }
    
    public void LoadScene()
    {
        // Charger la scène spécifiée
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    
}


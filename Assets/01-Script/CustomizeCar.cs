using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomizeCar : MonoBehaviour
{
    [Header("Scene Loader")]
    [SerializeField] private string sceneName = "Level1-DEVMAP";

    [Header("Materials Finder Settings")] 
    [SerializeField] private string[] materialNames;
    
    // Voiture actuellement affichée
    private GameObject[] carPrefabs;
    private Material[] carMaterials;
    private GameObject currentCar;

    // Index du préfab actuel dans le tableau
    private int currentIndex = 0;
    private int currentMaterialIndex = 0;
    GameObject currentBody = null;

    private void Start()
    {

        carPrefabs = GameManager.Instance.CarPrefabs;
        // Vérifier si le tableau de préfab est vide
        if (carPrefabs == null || carPrefabs.Length == 0)
        {
            Debug.LogError("Aucun préfab de voiture n'a été assigné dans le GameManager.");
            return;
        }
        
        carMaterials = GameManager.Instance.CarMaterials;
        
        // Charger la voiture initiale
        currentIndex = GameManager.Instance.PlayerPrefabIndex;
        currentMaterialIndex = GameManager.Instance.PlayerMaterialIndex;
    
        if (carPrefabs.Length > 0)
        {
            ChargerVoiture(currentIndex);
        }
        else
        {
            Debug.LogWarning("Aucun préfab de voiture n'a été assigné.");
        }
        
        // Appliquer le matériau initial
        if (currentBody != null && carMaterials.Length > 0)
        {
            ApplyMaterial(currentBody, 0);
        }
        else
        {
            Debug.LogWarning("Aucun matériau n'a été assigné.");
        }
    }
    
    // Passer à la voiture suivante
    public void Next()
    {
        if (carPrefabs.Length == 0) return;

        currentIndex++;
        if (currentIndex >= carPrefabs.Length)
        {
            currentIndex = 0;
        }

        ChargerVoiture(currentIndex);
        
    }

    // Revenir à la voiture précédente
    public void Previous()
    {
        if (carPrefabs.Length == 0) return;

        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = carPrefabs.Length - 1; // Aller à la fin si on dépasse le début
        }

        ChargerVoiture(currentIndex);
        
    }

    // Fonction pour charger une voiture spécifique
    private void ChargerVoiture(int index)
    {
        // Détruire la voiture actuelle si elle existe
        if (currentCar != null)
        {
            Destroy(currentCar);
        }

        // Créer la nouvelle voiture à la position et rotation de ce GameObject
        if (carPrefabs[index] != null)
        {
            
            currentCar = Instantiate(
                carPrefabs[index],
                transform.position,
                transform.rotation,
                this.transform
            );
            
            currentBody = FindFirstComponentWithTag("Body");
            
        }
        else
        {
            Debug.LogError($"Le préfab à l'index {index} est null.");
        }
    }
    
    public GameObject FindFirstComponentWithTag(string tag)
    {
        GameObject foundObject = null;
        foreach (Transform child in currentCar.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag(tag))
            {
                foundObject = child.gameObject;
                break;
            }
        }
        
        return foundObject;
    }
    
    // Recherche le Material dans le tableau de matériaux du game object celui qui comporte un string dans son nom
    private int FindMaterialInArray(GameObject target,int targetMaterialIndex)
    {
        for (int i = 0; i < target.GetComponent<MeshRenderer>().materials.Length; i++)
        {
            // if (target.GetComponent<MeshRenderer>().materials[i].name.Contains(materialNames[targetMaterialIndex]))
            // {
            //     Debug.Log(target.GetComponent<MeshRenderer>().materials[i].name);
            //     return i;
            // }
            
            Debug.Log(target.GetComponent<MeshRenderer>().materials[i].name);
        }
        
        Debug.Log("Found nothing In Array");
        
        return 0;
    }

    // Retourne l'index de la voiture actuellement affichée
    public int ObtenirIndexActuel()
    {
        return currentIndex;
    }

    // Retourne le nombre total de voitures disponibles
    public int ObtenirNombreVoitures()
    {
        return carPrefabs.Length;
    }

    public void ApplyMaterial(GameObject target, int targetMaterialIndex)
    {
        if (target == null || !target.TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
            return;
        
        int materialIndexToChange = FindMaterialInArray(target, targetMaterialIndex);
    
        // Obtenir une copie du tableau de matériaux
        Material[] materials = renderer.materials;
    
        // Modifier le matériau spécifique
        materials[materialIndexToChange] = carMaterials[currentMaterialIndex];
    
        // Réaffecter le tableau complet au renderer
        renderer.materials = materials;
    
        Debug.Log("Material appliqué: " + carMaterials[currentMaterialIndex].name + " à l'index: " + materialIndexToChange);
    }
    
    public void NextBodyMaterial()
    {
        currentMaterialIndex++;
        if (currentMaterialIndex >= carMaterials.Length)
        {
            currentMaterialIndex = 0;
        }

        ApplyMaterial(currentBody, 0);
    }
    
    public void PreviousBodyMaterial()
    {
        currentMaterialIndex--;
        if (currentMaterialIndex < 0)
        {
            currentMaterialIndex = carMaterials.Length - 1; // Aller à la fin si on dépasse le début
        }

        ApplyMaterial(currentBody, 0);
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
    
    public void SauvegarderChoixVoiture()
    {
        GameManager.Instance.PlayerPrefabIndex = currentIndex;
        GameManager.Instance.PlayerMaterialIndex = currentMaterialIndex;
        GameManager.Instance.SauvegarderDonnees();
    }

    public void ChargerChoixVoiture()
    {
        
    }
    
    public void LoadScene()
    {
        // Charger la scène spécifiée
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    
}


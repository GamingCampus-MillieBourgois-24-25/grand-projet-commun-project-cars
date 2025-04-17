using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomizeCar : MonoBehaviour
{
    //Get The Data For The script add after the spawn
    [SerializeField] GameObject CanvasCar;
    [SerializeField] MenuScript script;
    [SerializeField] CameraControl script2;

    //Rest
    [Header("Scene Loader")]
    [SerializeField] private string sceneName = "Level1-DEVMAP";
    
    [Header("Spawn Point")]
    [SerializeField] private Transform spawnPoint;
    
    // Sound / Music settings
    [Header("Sound / Music")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private bool playMusicOnStart = true;
    [SerializeField] private float musicVolume = 0.5f;
    
    // Get the car data from the GameManager
    private CarData carData;
    private GameObject carInstance; // Instance of the car prefab
    private int carIndex; // Index of the car in the GameManager
    private int carrosserieIndex; // Index of the carrosserie material
    private int accessoiresIndex; // Index of the accessoires material
    private int pharesIndex; // Index of the phares material
    private int rouesIndex; // Index of the roues material
    private int vitresIndex; // Index of the vitres material
    
    // Spawn the car at the spawn point
    private void Start()
    {
        carData = GameManager.Instance.GetPlayerCarData();
        
        if (spawnPoint != null)
        {
            // Instancier la voiture à la position du spawn
            carInstance = Instantiate(carData.CarInfo.carPrefab, spawnPoint.position, spawnPoint.rotation);
            carInstance.transform.SetParent(spawnPoint);
            
            // Charger les données de la voiture
            LoadCarData();
        }
        else
        {
            Debug.LogError("Aucun point de spawn spécifié !");
        }
        
        // Configuration et démarrage de la musique de fond
        if (audioSource != null && backgroundMusic != null && playMusicOnStart)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;  // Lecture en boucle
            audioSource.volume = musicVolume;
            audioSource.Play();
        }
        else if (audioSource == null)
        {
            Debug.LogWarning("AudioSource non assigné à CustomizeCar.");
        }
    }
    
    // Démarrer la lecture de la musique
    public void PlayMusic()
    {
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.Play();
        }
    }

    // Arrêter la lecture de la musique
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    // Régler le volume de la musique
    public void SetMusicVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
            musicVolume = audioSource.volume;
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
            carInstance.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            Transform Mesh = carInstance.transform.Find("Mesh");
            if (Mesh != null)
            {
                Transform body = Mesh.Find("Body");
                if (body != null)
                {
                    CarButton ScriptButton = body.gameObject.AddComponent<CarButton>();
                    AssignCarButton(ScriptButton);
                    Outline ScriptOutline = body.gameObject.AddComponent<Outline>();
                    ScriptOutline.OutlineWidth = 10;
                }
            }
        }
        else
        {
            Debug.LogError("Index de voiture invalide : " + index);
        }
    }

    void AssignCarButton(CarButton carButton)
    {
        // Utiliser de la réflexion pour accéder aux SerializeField privés
        var type = typeof(CarButton);

        type.GetField("CanvasCar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(carButton, CanvasCar);

        type.GetField("script", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(carButton, script);

        type.GetField("script2", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(carButton, script2);
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
    
    // Change le matériau de phares
    public void ChangePharesMaterial(int index)
    {
        // Vérifier si l'index est valide
        if (index >= 0 && index < carData.GetMaterials(CarMaterialType.Phares).Count)
        {
            // Obtenir le renderer du châssis
            MeshRenderer chassisRenderer = carData.GetChassisRenderer(carInstance);
            
            // Changer le matériau de phares a l'index spécifié dans le carData
            if (chassisRenderer != null)
            {
                Material[] materials = chassisRenderer.materials;
                if (materials.Length > carData.GetIndexInRenderer(CarMaterialType.Phares))
                {
                    materials[carData.GetIndexInRenderer(CarMaterialType.Phares)] = carData.GetMaterial(CarMaterialType.Phares, index);
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
            Debug.LogError("Index de matériau de phares invalide : " + index);
        }
    }
    
    // Next Phares Material
    public void NextPharesMaterial()
    {
        // Incrémenter l'index de phares
        pharesIndex = (pharesIndex + 1) % carData.GetMaterials(CarMaterialType.Phares).Count;
        
        // Changer le matériau de phares
        ChangePharesMaterial(pharesIndex);
    }
    
    // Previous Phares Material
    public void PreviousPharesMaterial()
    {
        // Décrémenter l'index de phares
        pharesIndex = (pharesIndex - 1 + carData.GetMaterials(CarMaterialType.Phares).Count) % carData.GetMaterials(CarMaterialType.Phares).Count;
        
        // Changer le matériau de phares
        ChangePharesMaterial(pharesIndex);
    }
    
    // Change le matériau de roues
    public void ChangeRouesMaterial(int index)
    {
        // Vérifier si l'index est valide
        if (index >= 0 && index < carData.GetMaterials(CarMaterialType.Roues).Count)
        {
            // Obtenir le renderer du châssis
            List<MeshRenderer> rouesRenderer = carData.GetRouesRenderers(carInstance);
            
            // Changer le matériau de roues a l'index spécifié dans le carData
            if (rouesRenderer != null)
            {
                foreach (MeshRenderer renderer in rouesRenderer)
                {
                    Material[] materials = renderer.materials;
                    if (materials.Length > carData.GetIndexInRenderer(CarMaterialType.Roues))
                    {
                        materials[carData.GetIndexInRenderer(CarMaterialType.Roues)] = carData.GetMaterial(CarMaterialType.Roues, index);
                        renderer.materials = materials;
                    }
                    else
                    {
                        Debug.LogError("Le renderer n'a pas assez de matériaux !");
                    }
                }
            }
            else
            {
                Debug.LogError("Aucun renderer de roues trouvé !");
            }
            
        }
        else
        {
            Debug.LogError("Index de matériau de roues invalide : " + index);
        }
    }
    
    // Next Roues Material
    public void NextRouesMaterial()
    {
        // Incrémenter l'index de roues
        rouesIndex = (rouesIndex + 1) % carData.GetMaterials(CarMaterialType.Roues).Count;
        
        // Changer le matériau de roues
        ChangeRouesMaterial(rouesIndex);
    }
    
    // Previous Roues Material
    public void PreviousRouesMaterial()
    {
        // Décrémenter l'index de roues
        rouesIndex = (rouesIndex - 1 + carData.GetMaterials(CarMaterialType.Roues).Count) % carData.GetMaterials(CarMaterialType.Roues).Count;
        
        // Changer le matériau de roues
        ChangeRouesMaterial(rouesIndex);
    }
    
    // Change le matériau de vitres
    public void ChangeVitresMaterial(int index)
    {
        // Vérifier si l'index est valide
        if (index >= 0 && index < carData.GetMaterials(CarMaterialType.Vitres).Count)
        {
            // Obtenir le renderer du châssis
            MeshRenderer chassisRenderer = carData.GetChassisRenderer(carInstance);
            
            // Changer le matériau de vitres a l'index spécifié dans le carData
            if (chassisRenderer != null)
            {
                Material[] materials = chassisRenderer.materials;
                if (materials.Length > carData.GetIndexInRenderer(CarMaterialType.Vitres))
                {
                    materials[carData.GetIndexInRenderer(CarMaterialType.Vitres)] = carData.GetMaterial(CarMaterialType.Vitres, index);
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
            Debug.LogError("Index de matériau de vitres invalide : " + index);
        }
    }
    
    // Next Vitres Material
    public void NextVitresMaterial()
    {
        // Incrémenter l'index de vitres
        vitresIndex = (vitresIndex + 1) % carData.GetMaterials(CarMaterialType.Vitres).Count;
        
        // Changer le matériau de vitres
        ChangeVitresMaterial(vitresIndex);
    }
    
    // Previous Vitres Material
    public void PreviousVitresMaterial()
    {
        // Décrémenter l'index de vitres
        vitresIndex = (vitresIndex - 1 + carData.GetMaterials(CarMaterialType.Vitres).Count) % carData.GetMaterials(CarMaterialType.Vitres).Count;
        
        // Changer le matériau de vitres
        ChangeVitresMaterial(vitresIndex);
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
    
    // Save the car data using the GameManager SaveMethod
    public void SaveCarData()
    {
        PlayerData currentPlayerData = GameManager.Instance.GetPlayerData();
        currentPlayerData.PlayerCarDataIndex = carIndex;
        currentPlayerData.PlayerCarAccessoireIndex = accessoiresIndex;
        currentPlayerData.PlayerCarCarrosserieIndex = carrosserieIndex;
        currentPlayerData.PlayerCarPharesIndex = pharesIndex;
        currentPlayerData.PlayerCarVitresIndex = vitresIndex;
        currentPlayerData.PlayerCarRouesIndex = rouesIndex;
        
        GameManager.Instance.SetPlayerData(currentPlayerData);
        
        GameManager.Instance.SauvegarderDonnees();
    }
    
    // Load the car data using the GameManager LoadMethod
    public void LoadCarData()
    {
        PlayerData currentPlayerData = GameManager.Instance.GetPlayerData();
        carIndex = currentPlayerData.PlayerCarDataIndex;
        accessoiresIndex = currentPlayerData.PlayerCarAccessoireIndex;
        carrosserieIndex = currentPlayerData.PlayerCarCarrosserieIndex;
        pharesIndex = currentPlayerData.PlayerCarPharesIndex;
        vitresIndex = currentPlayerData.PlayerCarVitresIndex;
        rouesIndex = currentPlayerData.PlayerCarRouesIndex;
        
        ChangeCar(carIndex);
        
        if (carData.IsMaterialEnabled(CarMaterialType.Accessoires))
        {
            ChangeAccessoiresMaterial(accessoiresIndex);
        }
        
        if (carData.IsMaterialEnabled(CarMaterialType.Carrosserie))
        {
            ChangeCarrosserieMaterial(carrosserieIndex);
        }
        
        if (carData.IsMaterialEnabled(CarMaterialType.Phares))
        {
            ChangePharesMaterial(pharesIndex);
        }
        
        if (carData.IsMaterialEnabled(CarMaterialType.Vitres))
        {
            ChangeVitresMaterial(vitresIndex);
        }
        
        if (carData.IsMaterialEnabled(CarMaterialType.Roues))
        {
            ChangeRouesMaterial(rouesIndex);
        }
    }
    
}


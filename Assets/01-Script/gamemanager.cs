using UnityEngine;
using System.Collections;
using System.IO;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }
    
    #region Variables
    
    [Header("Player Data")]
    [SerializeField] private PlayerData playerData;
    
    [Header("Game Settings")]
    [SerializeField] private GameObject[] carPrefabs;
    [SerializeField] private Material[] carMaterials;
    
    #endregion
    
    [System.Serializable]
    private class SaveData
    {
        public int playerPrefabIndex;
        public int playerMaterialIndex;
    }

    #region Unity Methods
    
    // Game Manager Persistence
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Charger les données du joueur
        ChargerDonnees();
    } 
    
    // Reset player data scriptable object on application quit
    private void OnApplicationQuit()
    {
        playerData.PlayerPrefabIndex = 0;
        playerData.PlayerMaterialIndex = 0;
    }
    
    #endregion
    
    #region Getter And Setter
    // Getters and Setters
    public int PlayerPrefabIndex
    {
        get { return playerData.PlayerPrefabIndex; }
        set { playerData.PlayerPrefabIndex = value; }
    }
    
    public int PlayerMaterialIndex
    {
        get { return playerData.PlayerMaterialIndex; }
        set { playerData.PlayerMaterialIndex = value; }
    }
    
    public GameObject[] CarPrefabs
    {
        get { return carPrefabs; }
        set { carPrefabs = value; }
    }
    
    public Material[] CarMaterials
    {
        get { return carMaterials; }
        set { carMaterials = value; }
    }
    
    #endregion
    
    #region Save and Load
    public void SauvegarderDonnees()
    {
        SaveData saveData = new SaveData
        {
            playerPrefabIndex = playerData.PlayerPrefabIndex,
            playerMaterialIndex = playerData.PlayerMaterialIndex
        };
    
        string json = JsonUtility.ToJson(saveData, true);
        string filePath = GetSaveFilePath();
    
        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("Données sauvegardées avec succès : " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de la sauvegarde : " + e.Message);
        }
    }

    // Charger les données depuis un fichier JSON
    public void ChargerDonnees()
    {
        string filePath = GetSaveFilePath();
    
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            
                playerData.PlayerPrefabIndex = saveData.playerPrefabIndex;
                playerData.PlayerMaterialIndex = saveData.playerMaterialIndex;
                Debug.Log("Données chargées avec succès");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Erreur lors du chargement : " + e.Message);
            }
        }
        else
        {
            Debug.Log("Aucune sauvegarde trouvée. Utilisation des valeurs par défaut.");
        }
    }

    // Obtenir le chemin du fichier de sauvegarde
    private string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "player_save.json");
    }
    
    #endregion
    
}
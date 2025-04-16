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
    [SerializeField] private CarData[] carData;
    
    #endregion
    
    [System.Serializable]
    private class SaveData
    {
        public int playerCarDataIndex;
        public int playerCarAccessoireIndex;
        public int playerCarCarrosserieIndex;
        public int playerCarPharesIndex;
        public int playerCarVitresIndex;
        public int playerCarRouesIndex;
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
    
    #endregion
    
    #region Getter And Setter
    // Getters and Setters
    
    // Get the car data based on the index
    public CarData GetCarData(int index)
    {
        if (index >= 0 && index < carData.Length)
        {
            return carData[index];
        }
        else
        {
            Debug.LogError("Index de voiture invalide : " + index);
            return null;
        }
    }
    
    // Get the player car data
    public CarData GetPlayerCarData()
    {
        return GetCarData(playerData.PlayerCarDataIndex);
    }
    
    // Get the player car data index
    public int GetPlayerCarDataIndex()
    {
        return playerData.PlayerCarDataIndex;
    }
    
    // Set the player car data index
    public void SetPlayerCarDataIndex(int index)
    {
        if (index >= 0 && index < carData.Length)
        {
            playerData.PlayerCarDataIndex = index;
        }
        else
        {
            Debug.LogError("Index de voiture invalide : " + index);
        }
    }
    
    // Get the player data
    public PlayerData GetPlayerData()
    {
        return playerData;
    }
    
    // Set the player data
    public void SetPlayerData(PlayerData data)
    {
        playerData = data;
    }
    
    // Get the car data count
    public int GetCarDataCount()
    {
        return carData.Length;
    }
    
    #endregion
    
    #region Save and Load
    public void SauvegarderDonnees()
    {
        SaveData saveData = new SaveData
        {
            playerCarDataIndex = playerData.PlayerCarDataIndex,
            playerCarAccessoireIndex = playerData.PlayerCarAccessoireIndex,
            playerCarCarrosserieIndex = playerData.PlayerCarCarrosserieIndex,
            playerCarPharesIndex = playerData.PlayerCarPharesIndex,
            playerCarVitresIndex = playerData.PlayerCarVitresIndex,
            playerCarRouesIndex = playerData.PlayerCarRouesIndex
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
            
                playerData.PlayerCarDataIndex = saveData.playerCarDataIndex;
                playerData.PlayerCarAccessoireIndex = saveData.playerCarAccessoireIndex;
                playerData.PlayerCarCarrosserieIndex = saveData.playerCarCarrosserieIndex;
                playerData.PlayerCarPharesIndex = saveData.playerCarPharesIndex;
                playerData.PlayerCarVitresIndex = saveData.playerCarVitresIndex;
                playerData.PlayerCarRouesIndex = saveData.playerCarRouesIndex;
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
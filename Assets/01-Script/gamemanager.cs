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

    [Header("Gameplay Settings")] 
    [SerializeField] private int maxLap;
    
    [Header("Game Start")]
    [SerializeField] private GameObject startGame;
    [SerializeField] private bool startGameActive;
    [SerializeField] private float ligthSequenceTime = 1f;
    [SerializeField] private float startGameTime = 2f;
    [SerializeField] private Material[] startGameMaterials;
    
    private bool canMove = false;
    private int playerCurrentLap = 0;
    private GameObject playeCarInstance;
    
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
    
    private void Start()
    {
        // Initialiser le jeu
        if (startGameActive)
        {
            StartGameSequence();
        }
    }
    
    #endregion
    
    #region startGame

    public void StartGameSequence()
    {
        // Lancer la coroutine de la séquence de départ
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        // Attendre startGameTime secondes avant de commencer la séquence
        Debug.Log("Attente de " + startGameTime + " secondes avant le démarrage...");
        yield return new WaitForSeconds(startGameTime);
    
        // Commencer la séquence de lumières
        Debug.Log("Début de la séquence de lumières");
    
        // Pour chaque matériau dans le tableau
        for (int i = 0; i < startGameMaterials.Length; i++)
        {
            
            yield return new WaitForSeconds(ligthSequenceTime);
            // Changer le matériau
            startGame.GetComponent<MeshRenderer>().material = startGameMaterials[i];
            Debug.Log("Changement de matériau: " + startGameMaterials[i].name);
            
        }
    
        // Réinitialiser le matériau à celui par défaut
        // Peut être implementé si nécessaire
        
        CanMove = true;
        
        Debug.Log("Séquence de départ terminée");
    }
    
    #endregion
    
    #region Getter And Setter
    // Getters and Setters
    
    public GameObject PlayerCarInstance
    {
        get { return playeCarInstance; }
        set { playeCarInstance = value; }
    }
    
    public bool IsStartGameActive
    {
        get { return startGameActive; }
        set { startGameActive = value; }
    }
    
    public bool CanMove
    {
        get { return canMove; }
        set { canMove = value; }
    }
    
    public int MaxLap
    {
        get { return maxLap; }
        set { maxLap = value; }
    }
    
    public int PlayerCurrentLap
    {
        get { return playerCurrentLap; }
        set { playerCurrentLap = value; }
    }
    
    // Race finished
    public void RaceFinished()
    {
        canMove = false;
        Debug.Log("Course terminée !");
    }
    
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
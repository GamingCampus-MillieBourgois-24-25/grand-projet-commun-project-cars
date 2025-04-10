using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }
    
    [Header("Player Data")]
    [SerializeField] private int playerPrefabIndex = 0;
    
    [Header("Game Settings")]
    [SerializeField] private GameObject[] carPrefabs;
    
    

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
    }
    
    // Getters and Setters
    public int PlayerPrefabIndex
    {
        get { return playerPrefabIndex; }
        set { playerPrefabIndex = value; }
    }
    
    public GameObject[] CarPrefabs
    {
        get { return carPrefabs; }
        set { carPrefabs = value; }
    }
    
    // Save Manager
    
    // player custom car based on a car prefab
    
}
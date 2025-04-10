using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Jeu/Données Joueur")]
public class PlayerData : ScriptableObject
{
   
    [Header("Données Joueur")]
    [SerializeField] private int playerPrefabIndex = 0;
    [SerializeField] private int playerMaterialIndex = 0;
    
    // Getter and Setter
    public int PlayerPrefabIndex
    {
        get { return playerPrefabIndex; }
        set { playerPrefabIndex = value; }
    }
    
    public int PlayerMaterialIndex
    {
        get { return playerMaterialIndex; }
        set { playerMaterialIndex = value; }
    }
    
    
}
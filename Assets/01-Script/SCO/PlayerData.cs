using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Jeu/Données Joueur")]
public class PlayerData : ScriptableObject
{
   
    [Header("Données Joueur")]
    [SerializeField] private int playerCarDataIndex = 0;
    
    // Getter and Setter
    public int PlayerCarDataIndex
    {
        get { return playerCarDataIndex; }
        set { playerCarDataIndex = value; }
    }
    
}
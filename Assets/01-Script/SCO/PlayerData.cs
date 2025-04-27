using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Jeu/Données Joueur")]
public class PlayerData : ScriptableObject
{
   
    [Header("Données Joueur")]
    [SerializeField] private int playerCarDataIndex = 0;
    [SerializeField] private int playerCarAccessoireIndex = 0;
    [SerializeField] private int playerCarCarrosserieIndex = 0;
    [SerializeField] private int playerCarPharesIndex = 0;
    [SerializeField] private int playerCarVitresIndex = 0;
    [SerializeField] private int playerCarRouesIndex = 0;
    
    // Getter and Setter
    public int PlayerCarDataIndex
    {
        get { return playerCarDataIndex; }
        set { playerCarDataIndex = value; }
    }
    
    public int PlayerCarAccessoireIndex
    {
        get { return playerCarAccessoireIndex; }
        set { playerCarAccessoireIndex = value; }
    }
    
    public int PlayerCarCarrosserieIndex
    {
        get { return playerCarCarrosserieIndex; }
        set { playerCarCarrosserieIndex = value; }
    }
    
    public int PlayerCarPharesIndex
    {
        get { return playerCarPharesIndex; }
        set { playerCarPharesIndex = value; }
    }
    
    public int PlayerCarVitresIndex
    {
        get { return playerCarVitresIndex; }
        set { playerCarVitresIndex = value; }
    }
    
    public int PlayerCarRouesIndex
    {
        get { return playerCarRouesIndex; }
        set { playerCarRouesIndex = value; }
    }
}
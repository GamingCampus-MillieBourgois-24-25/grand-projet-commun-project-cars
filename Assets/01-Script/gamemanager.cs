using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Données persistantes
    public int VoitureSelectionneeIndex { get; private set; }
    public GameObject[] ModelesVoitures { get; private set; }

    private void Awake()
    {
        // Implémentation du pattern Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ModelesVoitures = Resources.LoadAll<GameObject>("Voitures");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Définir la voiture sélectionnée
    public void DefinirVoitureSelectionnee(int index)
    {
        VoitureSelectionneeIndex = index;
    }

    // Obtenir la voiture sélectionnée
    public GameObject ObtenirVoitureSelectionnee()
    {
        if (ModelesVoitures != null && ModelesVoitures.Length > 0 && 
            VoitureSelectionneeIndex >= 0 && VoitureSelectionneeIndex < ModelesVoitures.Length)
        {
            return ModelesVoitures[VoitureSelectionneeIndex];
        }
        return null;
    }
}
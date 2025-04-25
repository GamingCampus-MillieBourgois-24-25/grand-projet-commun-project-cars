using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public StarData starsdata = new StarData();

    private void Awake()
    {
        LoadFromJson();
    }

    public void SaveToJson()
    {
        string data = JsonUtility.ToJson(starsdata);
        string filePath = Application.persistentDataPath + "/StarsData.json";
        System.IO.File.WriteAllText(filePath, data);
    }

    public void LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "/StarsData.json";
        string data = System.IO.File.ReadAllText(filePath);

        starsdata = JsonUtility.FromJson<StarData>(data);
    }

    public void SetCar(string categorie, int number)
    {
        // Vérifie si une voiture avec la même catégorie existe déjà
        foreach (SaveCar car in starsdata.levelstar)
        {
            if (car.categorie == categorie)
            {
                car.number = number; // Mise à jour du nom
                SaveToJson();
                return;
            }
        }

        // Sinon on en ajoute une nouvelle
        SaveCar newCar = new SaveCar
        {
            categorie = categorie,
            number = number
        };
        starsdata.levelstar.Add(newCar);
        SaveToJson();
    }

    public int GetCar(string categorie)
    {
        foreach (SaveCar car in starsdata.levelstar)
        {
            if (car.categorie == categorie)
            {
                return car.number;
            }
        }

        return 0;
    }
}

[System.Serializable]
public class StarData
{
    public List<SaveCar> levelstar = new List<SaveCar>();
}

[System.Serializable]
public class SaveCar
{
    public string categorie;
    public int number;
}
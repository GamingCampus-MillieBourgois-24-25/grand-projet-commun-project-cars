using UnityEngine;
using System;
using System.Collections.Generic;

public enum CarMaterialType
{
    Accessoires,
    Carrosserie,
    Phares,
    Roues,
    Vitres
}

[Serializable]
public struct MaterialCollection
{
    public string nomCollection;
    public List<Material> materials;
}


[Serializable]
public struct Car
{
    // Car Info
    public string carName;
    public GameObject carPrefab;
    
    // Material Info
    public MaterialCollection accessoiresMaterials;
    public MaterialCollection carrosserieMaterials;
    public MaterialCollection pharesMaterials;
    public MaterialCollection rouesMaterials;
    public MaterialCollection vitresMaterials;
    
}


[CreateAssetMenu(fileName = "CarData", menuName = "Jeu/CarData")]
public class CarData : ScriptableObject
{
    [Header("Car Data")] 
    [SerializeField] private Car car;
    
    public Car CarInfo => car;
    
    // Méthode pour accéder aux matériaux par type
    public List<Material> GetMaterials(CarMaterialType type)
    {
        switch (type)
        {
            case CarMaterialType.Accessoires:
                return car.accessoiresMaterials.materials;
            case CarMaterialType.Carrosserie:
                return car.carrosserieMaterials.materials;
            case CarMaterialType.Phares:
                return car.pharesMaterials.materials;
            case CarMaterialType.Roues:
                return car.rouesMaterials.materials;
            case CarMaterialType.Vitres:
                return car.vitresMaterials.materials;
            default:
                return null;
        }
    }
    
    public Material GetMaterial(CarMaterialType type, int index)
    {
        List<Material> materials = GetMaterials(type);
        
        if (materials != null && index >= 0 && index < materials.Count)
            return materials[index];
            
        return null;
    }

}

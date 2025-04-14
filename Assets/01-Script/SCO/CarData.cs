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
    public int indexInRenderer;
}

[Serializable]
public struct MeshRendererReference
{
    public string componentTag;
}

[Serializable]
public struct Car
{
    // Car Info
    public string carName;
    public GameObject carPrefab;
    
    // Renderer Reference
    public MeshRendererReference chassisRenderer;
    public List<MeshRendererReference> rouesRenderers;
    
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
    
    // Méthode pour obtenir le renderer du châssis d'une instance de voiture
    public MeshRenderer GetChassisRenderer(GameObject carInstance)
    {
        if (carInstance == null) return null;

        if (!string.IsNullOrEmpty(car.chassisRenderer.componentTag))
        {
            // Recherche par tag dans tous les enfants
            foreach (Transform child in carInstance.GetComponentsInChildren<Transform>())
            {
                if (child.CompareTag(car.chassisRenderer.componentTag))
                {
                    return child.GetComponent<MeshRenderer>();
                }
            }
        }

        return null;
    }
    
    // Get the indexInRenderer of the material
    public int GetIndexInRenderer(CarMaterialType type)
    {
        switch (type)
        {
            case CarMaterialType.Accessoires:
                return car.accessoiresMaterials.indexInRenderer;
            case CarMaterialType.Carrosserie:
                return car.carrosserieMaterials.indexInRenderer;
            case CarMaterialType.Phares:
                return car.pharesMaterials.indexInRenderer;
            case CarMaterialType.Roues:
                return car.rouesMaterials.indexInRenderer;
            case CarMaterialType.Vitres:
                return car.vitresMaterials.indexInRenderer;
            default:
                return -1;
        }
    }
    
    // Méthode pour obtenir les renderers des roues
    public List<MeshRenderer> GetRouesRenderers(GameObject carInstance)
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        
        if (carInstance == null || car.rouesRenderers == null) 
            return renderers;
            
        foreach (var roueRef in car.rouesRenderers)
        {
            MeshRenderer renderer = null;
            
            if (!string.IsNullOrEmpty(roueRef.componentTag))
            {
                // Recherche par tag dans tous les enfants
                foreach (Transform child in carInstance.GetComponentsInChildren<Transform>())
                {
                    if (child.CompareTag(roueRef.componentTag))
                    {
                        renderer = child.GetComponent<MeshRenderer>();
                        if (renderer != null) renderers.Add(renderer);
                    }
                }
            }
        }
        
        return renderers;
    }

}

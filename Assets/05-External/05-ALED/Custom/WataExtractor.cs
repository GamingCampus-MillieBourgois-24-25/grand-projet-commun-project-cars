using UnityEngine;

[ExecuteInEditMode]
public class WaterMaterialController : MonoBehaviour
{
    public Material waterMaterial;

    void Update()
    {
        if (waterMaterial != null)
        {
            // Extraire les textures et les paramètres du matériau de l'eau
            Texture waterTexture = waterMaterial.GetTexture("_MainTex");
            Color waterColor = waterMaterial.GetColor("_Color");

            // Passer les textures et les paramètres au shader globalement
            Shader.SetGlobalTexture("_GlobalWaterTexture", waterTexture);
            Shader.SetGlobalColor("_GlobalWaterColor", waterColor);
        }
    }
}

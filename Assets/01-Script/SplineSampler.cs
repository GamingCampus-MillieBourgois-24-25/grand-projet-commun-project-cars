using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode()]
public class SplineSampler : MonoBehaviour
{
    public SplineContainer splineContainer;
    public float roadWidth = 4f;
    public int resolution = 100; // Nombre de segments pour échantillonner la spline
    public Material roadMaterial;
    
    [Header("Options de la route")]
    public bool generateCurbs = true;
    public float curbHeight = 0.2f;
    public float curbWidth = 0.5f;
    public Material curbMaterial;
    
    [Header("Options de texture")]
    public float textureStretch = 1f;
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private GameObject curbsContainer;
    
    private void OnEnable()
    {
        // S'assurer que nous avons les composants nécessaires
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        // Toujours réappliquer le matériau pour éviter le rose
        if (roadMaterial != null)
            meshRenderer.sharedMaterial = roadMaterial;
        else
            Debug.LogWarning("Aucun matériau de route assigné!");

        // Conteneur pour les bordures
        curbsContainer = transform.Find("Bordures")?.gameObject;
        if (curbsContainer == null && generateCurbs)
        {
            curbsContainer = new GameObject("Bordures");
            curbsContainer.transform.parent = transform;
            curbsContainer.transform.localPosition = Vector3.zero;
            curbsContainer.transform.localRotation = Quaternion.identity;
            curbsContainer.transform.localScale = Vector3.one;
        }
    }
    
    private void ConfigurerMateriauxDoubleFace()
    {
        // Configuration du matériau de la route pour être visible des deux côtés
        if (roadMaterial != null)
        {
            // Désactiver le culling pour voir les deux faces
            roadMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        }
    
        // Configuration du matériau des bordures pour être visible des deux côtés
        if (curbMaterial != null)
        {
            curbMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        }
    }
    
    public void GenererRoute()
    {
        if (splineContainer == null || splineContainer.Spline == null)
        {
            Debug.LogError("SplineContainer manquant!");
            return;
        }

        // Configurer les matériaux pour être visibles des deux côtés
        ConfigurerMateriauxDoubleFace();

        // Générer la route
        Mesh roadMesh = CreerMeshRoute();
        meshFilter.sharedMesh = roadMesh;

        // Générer les bordures
        if (generateCurbs)
        {
            NettoyerBordures();
            CreerBordures();
        }
    }
    
    private void CreerBordures()
    {
        if (curbsContainer == null) return;

        Spline spline = splineContainer.Spline;
    
        // Créer la bordure gauche (côté = -1)
        GameObject bordureGauche = CreerUneBordure(spline, -1);
        bordureGauche.name = "BordureGauche";
        bordureGauche.transform.SetParent(curbsContainer.transform, false);
    
        // Créer la bordure droite (côté = 1)
        GameObject bordureDroite = CreerUneBordure(spline, 1);
        bordureDroite.name = "BordureDroite";
        bordureDroite.transform.SetParent(curbsContainer.transform, false);
    }
    
    private void NettoyerBordures()
    {
        if (curbsContainer == null) return;
        
        // Supprimer les bordures existantes
        for (int i = curbsContainer.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(curbsContainer.transform.GetChild(i).gameObject);
        }
    }
    
    private GameObject CreerUneBordure(Spline spline, int cote)
    {
        GameObject bordure = new GameObject();
        MeshFilter meshFilter = bordure.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = bordure.AddComponent<MeshRenderer>();
    
        if (curbMaterial != null)
        meshRenderer.sharedMaterial = curbMaterial;
        else
        Debug.LogWarning("Aucun matériau de bordure assigné!");

        Mesh mesh = new Mesh();
    
        Vector3[] vertices = new Vector3[resolution * 4];
        Vector2[] uvs = new Vector2[resolution * 4];
        int[] triangles = new int[(resolution - 1) * 24];
    
        float roadOffset = (roadWidth * 0.5f) * cote;
        float curbOffset = curbWidth * cote;
        Transform splineTransform = splineContainer.transform;
    
        // Calcul simplifié pour éviter le problème des bordures au centre
        for (int i = 0; i < resolution; i++)
        {
        float t = i / (float)(resolution - 1);
        float3 position = spline.EvaluatePosition(t);
        position = splineTransform.TransformPoint(position);

        float3 tangent = spline.EvaluateTangent(t);
        tangent = splineTransform.TransformDirection(tangent);

        float3 up = new float3(0, 1, 0);
        float3 normal = math.normalize(math.cross(tangent, up));

        // Application directe des offsets sans ajustement de courbure
        Vector3 roadPoint = new Vector3(position.x, position.y, position.z) + (new Vector3(normal.x, normal.y, normal.z) * roadOffset);
        Vector3 curbPoint = new Vector3(position.x, position.y, position.z) + (new Vector3(normal.x, normal.y, normal.z) * (roadOffset + curbOffset));

        vertices[i * 4] = transform.InverseTransformPoint(roadPoint);
        vertices[i * 4 + 1] = transform.InverseTransformPoint(curbPoint);
        vertices[i * 4 + 2] = transform.InverseTransformPoint(new Vector3(roadPoint.x, roadPoint.y + curbHeight, roadPoint.z));
        vertices[i * 4 + 3] = transform.InverseTransformPoint(new Vector3(curbPoint.x, curbPoint.y + curbHeight, curbPoint.z));

        uvs[i * 4] = new Vector2(0, t * textureStretch);
        uvs[i * 4 + 1] = new Vector2(1, t * textureStretch);
        uvs[i * 4 + 2] = new Vector2(0, t * textureStretch + 0.5f);
        uvs[i * 4 + 3] = new Vector2(1, t * textureStretch + 0.5f);

        if (i < resolution - 1)
        {
            int baseIdx = i * 4;
            int nextBaseIdx = (i + 1) * 4;
            int backfaceOffset = (resolution - 1) * 12;

            // Face avant (selon le côté)
            if (cote == -1) {
                // Face avant normale
                triangles[i * 12] = baseIdx;
                triangles[i * 12 + 1] = nextBaseIdx;
                triangles[i * 12 + 2] = baseIdx + 2;

                triangles[i * 12 + 3] = baseIdx + 2;
                triangles[i * 12 + 4] = nextBaseIdx;
                triangles[i * 12 + 5] = nextBaseIdx + 2;

                // Face avant inversée (face arrière)
                triangles[backfaceOffset + i * 12] = nextBaseIdx;
                triangles[backfaceOffset + i * 12 + 1] = baseIdx;
                triangles[backfaceOffset + i * 12 + 2] = nextBaseIdx + 2;

                triangles[backfaceOffset + i * 12 + 3] = nextBaseIdx + 2;
                triangles[backfaceOffset + i * 12 + 4] = baseIdx;
                triangles[backfaceOffset + i * 12 + 5] = baseIdx + 2;
            } else {
                // Face avant normale
                triangles[i * 12] = baseIdx;
                triangles[i * 12 + 1] = baseIdx + 2;
                triangles[i * 12 + 2] = nextBaseIdx;

                triangles[i * 12 + 3] = nextBaseIdx;
                triangles[i * 12 + 4] = baseIdx + 2;
                triangles[i * 12 + 5] = nextBaseIdx + 2;

                // Face avant inversée (face arrière)
                triangles[backfaceOffset + i * 12] = baseIdx + 2;
                triangles[backfaceOffset + i * 12 + 1] = baseIdx;
                triangles[backfaceOffset + i * 12 + 2] = nextBaseIdx + 2;

                triangles[backfaceOffset + i * 12 + 3] = nextBaseIdx + 2;
                triangles[backfaceOffset + i * 12 + 4] = baseIdx;
                triangles[backfaceOffset + i * 12 + 5] = nextBaseIdx;
            }

            // Face du dessus normale
            triangles[i * 12 + 6] = baseIdx + 2;
            triangles[i * 12 + 7] = nextBaseIdx + 2;
            triangles[i * 12 + 8] = baseIdx + 3;

            triangles[i * 12 + 9] = baseIdx + 3;
            triangles[i * 12 + 10] = nextBaseIdx + 2;
            triangles[i * 12 + 11] = nextBaseIdx + 3;

            // Face du dessus inversée
            triangles[backfaceOffset + i * 12 + 6] = nextBaseIdx + 2;
            triangles[backfaceOffset + i * 12 + 7] = baseIdx + 2;
            triangles[backfaceOffset + i * 12 + 8] = nextBaseIdx + 3;

            triangles[backfaceOffset + i * 12 + 9] = nextBaseIdx + 3;
            triangles[backfaceOffset + i * 12 + 10] = baseIdx + 2;
            triangles[backfaceOffset + i * 12 + 11] = baseIdx + 3;
        }
        }
    
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    
        meshFilter.sharedMesh = mesh;
        return bordure;
    }

    private Mesh CreerMeshRoute()
    {
        Spline spline = splineContainer.Spline;
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[resolution * 2];
        Vector2[] uvs = new Vector2[resolution * 2];
        // Double le nombre de triangles pour avoir des faces des deux côtés
        int[] triangles = new int[(resolution - 1) * 12];

        float halfWidth = roadWidth * 0.5f;
        Transform splineTransform = splineContainer.transform;

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            float3 position = spline.EvaluatePosition(t);
            position = splineTransform.TransformPoint(position);

            float3 tangent = spline.EvaluateTangent(t);
            tangent = splineTransform.TransformDirection(tangent);

            float3 up = new float3(0, 1, 0);
            float3 normal = math.normalize(math.cross(tangent, up));

            Vector3 leftPoint = new Vector3(position.x, position.y, position.z) - (new Vector3(normal.x, normal.y, normal.z) * halfWidth);
            Vector3 rightPoint = new Vector3(position.x, position.y, position.z) + (new Vector3(normal.x, normal.y, normal.z) * halfWidth);

            vertices[i * 2] = transform.InverseTransformPoint(leftPoint);
            vertices[i * 2 + 1] = transform.InverseTransformPoint(rightPoint);

            uvs[i * 2] = new Vector2(0, t * textureStretch);
            uvs[i * 2 + 1] = new Vector2(1, t * textureStretch);

            if (i < resolution - 1)
            {
                int baseIdx = i * 2;
                int nextBaseIdx = (i + 1) * 2;

                // Face avant (visible du dessus)
                triangles[i * 12] = baseIdx;
                triangles[i * 12 + 1] = baseIdx + 1;
                triangles[i * 12 + 2] = nextBaseIdx;

                triangles[i * 12 + 3] = nextBaseIdx;
                triangles[i * 12 + 4] = baseIdx + 1;
                triangles[i * 12 + 5] = nextBaseIdx + 1;

                // Face arrière (visible du dessous) - ordre des sommets inversé
                triangles[i * 12 + 6] = baseIdx + 1;
                triangles[i * 12 + 7] = baseIdx;
                triangles[i * 12 + 8] = nextBaseIdx + 1;

                triangles[i * 12 + 9] = nextBaseIdx + 1;
                triangles[i * 12 + 10] = baseIdx;
                triangles[i * 12 + 11] = nextBaseIdx;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    // Pour permettre la mise à jour en temps réel dans l'éditeur
    private void Update()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            // Actualisation automatique (optionnelle, à décommenter si nécessaire)
            // GenererRoute();
        }
    }

    // Pour nettoyer les ressources lors de la destruction du composant
    private void OnDestroy()
    {
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            if (Application.isEditor)
                DestroyImmediate(meshFilter.sharedMesh);
            else
                Destroy(meshFilter.sharedMesh);
        }
    }
}

// Ajouter un bouton personnalisé dans l'éditeur Unity
#if UNITY_EDITOR
[CustomEditor(typeof(SplineSampler))]
public class SplineSamplerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SplineSampler sampler = (SplineSampler)target;
        if (GUILayout.Button("Générer la Route"))
        {
            sampler.GenererRoute();
        }
    }
}
#endif
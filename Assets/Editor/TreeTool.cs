using UnityEngine;
using UnityEditor;

public class TreeGroundCorrector : EditorWindow
{
    [MenuItem("Tools/Tree Ground Corrector")]
    static void Init() => GetWindow<TreeGroundCorrector>("Tree Corrector");

    Terrain terrain;
    float extraHeight = 0f;

    void OnGUI()
    {
        terrain = EditorGUILayout.ObjectField("Terrain", terrain, typeof(Terrain), true) as Terrain;
        extraHeight = EditorGUILayout.FloatField("Extra Height Y", extraHeight);
        if (GUILayout.Button("Correct Tree Positions"))
            CorrectTrees();
    }

    void CorrectTrees()
    {
        if (terrain == null) return;
        var data = terrain.terrainData;
        var instances = data.treeInstances;
        for (int i = 0; i < instances.Length; i++)
        {
            Vector3 worldPos = Vector3.Scale(instances[i].position, data.size) + terrain.transform.position;
            float terrainHeight = terrain.SampleHeight(worldPos);
            worldPos.y = terrainHeight + extraHeight;
            // Reconversion en normalized position
            Vector3 norm = (worldPos - terrain.transform.position);
            norm = new Vector3(norm.x / data.size.x, 0, norm.z / data.size.z);
            instances[i].position = new Vector3(norm.x, 0, norm.z);
        }
        data.treeInstances = instances;
        Debug.Log($"Repositioned {instances.Length} trees.");
    }
}
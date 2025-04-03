using UnityEditor;
using UnityEngine;

namespace CarController
{
    [CustomEditor(typeof(CarController))]
    public class AVC_Editor : Editor
    {
        private Texture2D headerBackground;

        private void OnEnable()
        {
            // Create a white texture for the header background
            headerBackground = new Texture2D(1, 1);
            headerBackground.SetPixel(0, 0, Color.black);
            headerBackground.Apply();
        }

        private void OnDisable()
        {
            // Destroy the texture to free up memory
            DestroyImmediate(headerBackground);
        }

        public override void OnInspectorGUI()
        {
            // Define the colors
            Color primaryColor = new Color(0, 1f, 0); // Green

            // Create a header for the script with white background
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.fontSize = 27;
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.normal.textColor = primaryColor;
            headerStyle.normal.background = headerBackground;
            headerStyle.padding = new RectOffset(1, 1, 1, 1);
            GUILayout.Space(10f);
            GUILayout.Label("Catchow", headerStyle);
            GUILayout.Space(10f);

            // Create the buttons
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.fontSize = 12;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.padding = new RectOffset(5, 5, 5, 5);
            
            GUILayout.Space(10f);

            // Display all public variables of the SimcadeVehicleController script
            DrawDefaultInspector();
        }
    }
}

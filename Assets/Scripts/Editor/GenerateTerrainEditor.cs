using UnityEngine;
using UnityEditor;

namespace CrimsonPlague {
    [CustomEditor(typeof(GenerateTerrain))]
    public class GenerateTerrainEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            if (GUILayout.Button("Generate")) {
                // Used for testing terrain generation in editor
                GenerateTerrain terrain = (GenerateTerrain)target;
                terrain.seed = (int)System.DateTime.Now.Ticks;
                terrain.Generate();
            }
        }
    }
}
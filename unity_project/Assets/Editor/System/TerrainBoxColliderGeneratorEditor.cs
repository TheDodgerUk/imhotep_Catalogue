using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainBoxColliderGenerator))]
public class TerrainBoxColliderGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TerrainBoxColliderGenerator generator = (TerrainBoxColliderGenerator)target;
        EditorGUILayout.HelpBox("Generates a surface shell. Openings smaller than one voxel may close. Start at 64-96 cells.", MessageType.Info);
        if (GUILayout.Button("Generate Box Colliders", GUILayout.Height(32)))
        {
            generator.GenerateColliders();
            EditorUtility.SetDirty(generator);
        }
        if (GUILayout.Button("Remove Box Colliders", GUILayout.Height(26)))
        {
            generator.RemoveColliders();
            EditorUtility.SetDirty(generator);
        }
    }
}

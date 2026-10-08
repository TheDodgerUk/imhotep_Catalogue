using UnityEditor.Formats.Fbx.Exporter;
using Autodesk.Fbx;
using FilePathHelper;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralTube))]
public class ProceduralTubeEditor : InspectorMethodEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("SaveAsFBX()") == true)
        {
            SaveCableAsFbx(target);
        }
    }

    public static GameObject SaveCableAsFbx(Object target)
    {
        const string FbxPath = "Assets/Artwork/Models/Cables/Custom/CableWireMesh 1.fbx";
        var filePath = FilePath.FromRelative(FbxPath);
        Utils.IO.ForceFolder(filePath.GetFolder().Absolute);

        return SaveFbx(target, AssetDatabase.GenerateUniqueAssetPath(FbxPath));
    }

    public static GameObject OverwriteFbx(Object target, GameObject clone)
    {
        var prefab = PrefabUtility.GetCorrespondingObjectFromSource(clone);
        if (prefab == null)
        {
            Debug.LogWarning($"Failed to update FBX. {clone.name} is not linked to an FBX\n");
        }

        return SaveFbx(target, AssetDatabase.GetAssetPath(prefab));
    }

    private static GameObject SaveFbx(Object target, string path)
    {
        //var options = new ExportModelSettingsSerialize()
        //{
        //    objectPosition = ExportSettings.ObjectPosition.Reset,
        //    exportFormat = ExportSettings.ExportFormat.Binary,
        //    include = ExportSettings.Include.Model,
        //};
        ModelExporter.ExportObject(path, target /*, options*/);
        AssetDatabase.Refresh();

        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        EditorGUIUtility.PingObject(fbx);
        return fbx;
    }
}

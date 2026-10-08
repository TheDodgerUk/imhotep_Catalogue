using FilePathHelper;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ScriptableObjectUtility
{
    //[MenuItem("Assets/Create/Drum Sound Bank")]
    //public static void DrumSoundBank() { CreateAsset<DrumSoundBank>(); }

    public static T CreateAsset<T>() where T : ScriptableObject
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path) == true)
        {
            path = "Assets";
        }
        else if (string.IsNullOrEmpty(Path.GetExtension(path)) == false)
        {
            path = path.Strip(Path.GetFileName(path));
        }

        return CreateAsset<T>(path);
    }

    public static T CreateAsset<T>(string folderPath) where T : ScriptableObject
    {
        var folder = FilePath.FromRelative(folderPath);
        Utils.IO.ForceFolder(folder.Absolute);

        var path = folder.Append($"/New {typeof(T).Name}.asset");

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath(path.Relative);
        var asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, uniquePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AssetCounter
{
    [MenuItem("Tools/Count Project Assets")]
    public static void CountAssets()
    {
        string[] guids = AssetDatabase.FindAssets("", new[] { "Assets" });

        Dictionary<string, int> counts = new Dictionary<string, int>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (AssetDatabase.IsValidFolder(path))
                continue;

            string extension = System.IO.Path.GetExtension(path).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension))
                extension = "(no extension)";

            if (!counts.ContainsKey(extension))
                counts[extension] = 0;

            counts[extension]++;
        }

        Debug.Log("TOTAL ASSETS: " +
            System.Linq.Enumerable.Sum(counts.Values));

        foreach (var item in counts)
        {
            Debug.Log(item.Key + ": " + item.Value);
        }
    }
}

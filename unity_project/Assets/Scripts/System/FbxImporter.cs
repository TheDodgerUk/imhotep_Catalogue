using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
public class FbxImporter : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        ModelImporter model = (ModelImporter)assetImporter;
        model.isReadable = true;
    }

}
#else
public class FbxImporter {
}
#endif
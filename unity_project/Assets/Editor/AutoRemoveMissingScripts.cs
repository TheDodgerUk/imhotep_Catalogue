using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class AutoRemoveMissingScripts
{
    static AutoRemoveMissingScripts()
    {
        PrefabStage.prefabSaving += OnPrefabSaving;
    }

    private static void OnPrefabSaving(GameObject prefabRoot)
    {
        int removed = 0;

        foreach (Transform child in
                 prefabRoot.GetComponentsInChildren<Transform>(true))
        {
            GameObject go = child.gameObject;

            // Remove missing scripts
            removed +=
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

            // Remove MiniatureBaseScaler components
            MiniatureBaseScaler[] scalers =
                go.GetComponents<MiniatureBaseScaler>();

            foreach (MiniatureBaseScaler scaler in scalers)
            {
                Object.DestroyImmediate(scaler);
                removed++;
            }
        }

        if (removed > 0)
        {
            Debug.Log(
                $"Automatically removed {removed} components " +
                $"from prefab: {prefabRoot.name}");
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class MeshPolygonCounter : MonoBehaviour
{
    [Header("Scan Settings")]
    public bool countEntireScene = true;
    public GameObject targetGameObject;
    public bool includeChildren = true;
    public bool includeInactive = true;

    [Header("Scene Statistics")]
    [SerializeField] private int sceneCount;
    [SerializeField] private long gameObjectCount;
    [SerializeField] private long activeGameObjectCount;
    [SerializeField] private long meshCount;
    [SerializeField] private long uniqueMeshCount;
    [SerializeField] private long vertexCount;
    [SerializeField] private long triangleCount;
    [SerializeField] private long polygonCount;
    [SerializeField] private long subMeshCount;
    [SerializeField] private long materialSlotCount;
    [SerializeField] private long uniqueMaterialCount;

    [Header("Additional Statistics")]
    [SerializeField] private long meshRendererCount;
    [SerializeField] private long skinnedMeshRendererCount;
    [SerializeField] private long colliderCount;
    [SerializeField] private long lightCount;
    [SerializeField] private long cameraCount;

    public void RefreshPolygonCount()
    {
        ResetStatistics();

        HashSet<Mesh> uniqueMeshes = new HashSet<Mesh>();
        HashSet<Material> uniqueMaterials =
            new HashSet<Material>();

        if (countEntireScene)
        {
            for (int s = 0; s < SceneManager.sceneCount; s++)
            {
                Scene scene = SceneManager.GetSceneAt(s);

                if (!scene.isLoaded)
                    continue;

                sceneCount++;

                GameObject[] roots =
                    scene.GetRootGameObjects();

                foreach (GameObject root in roots)
                {
                    ScanGameObject(
                        root,
                        true,
                        uniqueMeshes,
                        uniqueMaterials);
                }
            }
        }
        else
        {
            GameObject target =
                targetGameObject != null
                    ? targetGameObject
                    : gameObject;

            sceneCount = 1;

            ScanGameObject(
                target,
                includeChildren,
                uniqueMeshes,
                uniqueMaterials);
        }

        uniqueMeshCount = uniqueMeshes.Count;
        uniqueMaterialCount = uniqueMaterials.Count;
        polygonCount = triangleCount;

        Debug.Log(
            "SCENE MESH STATISTICS\n" +
            "Scenes: " + sceneCount + "\n" +
            "GameObjects: " + gameObjectCount + "\n" +
            "Active GameObjects: " +
                activeGameObjectCount + "\n" +
            "Mesh Instances: " + meshCount + "\n" +
            "Unique Meshes: " + uniqueMeshCount + "\n" +
            "Vertices: " + vertexCount + "\n" +
            "Triangles: " + triangleCount + "\n" +
            "Material Slots: " + materialSlotCount);

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    private void ScanGameObject(
        GameObject root,
        bool scanChildren,
        HashSet<Mesh> uniqueMeshes,
        HashSet<Material> uniqueMaterials)
    {
        Transform[] transforms = scanChildren
            ? root.GetComponentsInChildren<Transform>(true)
            : new Transform[] { root.transform };

        foreach (Transform current in transforms)
        {
            GameObject obj = current.gameObject;

            if (!includeInactive && !obj.activeInHierarchy)
                continue;

            gameObjectCount++;

            if (obj.activeInHierarchy)
                activeGameObjectCount++;

            MeshFilter[] filters =
                obj.GetComponents<MeshFilter>();

            foreach (MeshFilter filter in filters)
            {
                if (filter.sharedMesh == null)
                    continue;

                AddMesh(filter.sharedMesh, uniqueMeshes);
            }

            MeshRenderer[] meshRenderers =
                obj.GetComponents<MeshRenderer>();

            foreach (MeshRenderer renderer in meshRenderers)
            {
                meshRendererCount++;

                AddMaterials(
                    renderer,
                    uniqueMaterials);
            }

            SkinnedMeshRenderer[] skinned =
                obj.GetComponents<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer renderer in skinned)
            {
                skinnedMeshRendererCount++;

                if (renderer.sharedMesh != null)
                    AddMesh(renderer.sharedMesh, uniqueMeshes);

                AddMaterials(
                    renderer,
                    uniqueMaterials);
            }

            colliderCount += obj.GetComponents<Collider>().Length;
            lightCount += obj.GetComponents<Light>().Length;
            cameraCount += obj.GetComponents<Camera>().Length;
        }
    }

    private void AddMesh(
        Mesh mesh,
        HashSet<Mesh> uniqueMeshes)
    {
        meshCount++;
        uniqueMeshes.Add(mesh);

        vertexCount += mesh.vertexCount;
        subMeshCount += mesh.subMeshCount;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            long indices = (long)mesh.GetIndexCount(i);

            switch (mesh.GetTopology(i))
            {
                case MeshTopology.Triangles:
                    triangleCount += indices / 3;
                    break;

                case MeshTopology.Quads:
                    triangleCount += (indices / 4) * 2;
                    break;
            }
        }
    }

    private void AddMaterials(
        Renderer renderer,
        HashSet<Material> uniqueMaterials)
    {
        Material[] materials = renderer.sharedMaterials;

        materialSlotCount += materials.Length;

        foreach (Material material in materials)
        {
            if (material != null)
                uniqueMaterials.Add(material);
        }
    }

    private void ResetStatistics()
    {
        sceneCount = 0;
        gameObjectCount = 0;
        activeGameObjectCount = 0;
        meshCount = 0;
        uniqueMeshCount = 0;
        vertexCount = 0;
        triangleCount = 0;
        polygonCount = 0;
        subMeshCount = 0;
        materialSlotCount = 0;
        uniqueMaterialCount = 0;
        meshRendererCount = 0;
        skinnedMeshRendererCount = 0;
        colliderCount = 0;
        lightCount = 0;
        cameraCount = 0;
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(MeshPolygonCounter))]
public class MeshPolygonCounterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MeshPolygonCounter counter =
            (MeshPolygonCounter)target;

        serializedObject.Update();

        // ----------------------------------------
        // EDITABLE SCAN SETTINGS
        // ----------------------------------------

        EditorGUILayout.LabelField(
            "Scan Settings",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("countEntireScene"));

        if (!counter.countEntireScene)
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("targetGameObject"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("includeChildren"));
        }

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("includeInactive"));

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(10);

        // ----------------------------------------
        // REFRESH BUTTON
        // ----------------------------------------

        if (GUILayout.Button(
            "REFRESH SCENE STATISTICS",
            GUILayout.Height(45)))
        {
            counter.RefreshPolygonCount();
        }

        EditorGUILayout.Space(15);

        // ----------------------------------------
        // READ-ONLY STATISTICS
        // ----------------------------------------

        EditorGUILayout.LabelField(
            "Scene Statistics",
            EditorStyles.boldLabel);

        DrawReadOnlyNumber("Scene Count", "sceneCount");
        DrawReadOnlyNumber("GameObjects", "gameObjectCount");
        DrawReadOnlyNumber(
            "Active GameObjects", "activeGameObjectCount");

        EditorGUILayout.Space(5);

        DrawReadOnlyNumber("Mesh Instances", "meshCount");
        DrawReadOnlyNumber("Unique Meshes", "uniqueMeshCount");
        DrawReadOnlyNumber("Vertices", "vertexCount");
        DrawReadOnlyNumber("Triangles", "triangleCount");
        DrawReadOnlyNumber("Polygons", "polygonCount");
        DrawReadOnlyNumber("Submeshes", "subMeshCount");

        EditorGUILayout.Space(5);

        DrawReadOnlyNumber(
            "Material Slots", "materialSlotCount");

        DrawReadOnlyNumber(
            "Unique Materials", "uniqueMaterialCount");

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Additional Statistics",
            EditorStyles.boldLabel);

        DrawReadOnlyNumber(
            "Mesh Renderers", "meshRendererCount");

        DrawReadOnlyNumber(
            "Skinned Mesh Renderers", "skinnedMeshRendererCount");

        DrawReadOnlyNumber("Colliders", "colliderCount");
        DrawReadOnlyNumber("Lights", "lightCount");
        DrawReadOnlyNumber("Cameras", "cameraCount");

        EditorGUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Statistics are read-only.\n" +
            "Numbers use thousands separators.\n" +
            "Press Refresh to update the counts.",
            MessageType.Info);
    }

    // ----------------------------------------
    // READ-ONLY NUMBER WITH COMMAS
    // ----------------------------------------

    private void DrawReadOnlyNumber(
        string label,
        string propertyName)
    {
        SerializedProperty property =
            serializedObject.FindProperty(propertyName);

        if (property == null)
            return;

        long value = property.longValue;

        // N0 formats numbers with commas.
        string formatted = value.ToString("N0",
            System.Globalization.CultureInfo.GetCultureInfo("en-GB"));

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.TextField(label, formatted);
        }
    }
}

#endif


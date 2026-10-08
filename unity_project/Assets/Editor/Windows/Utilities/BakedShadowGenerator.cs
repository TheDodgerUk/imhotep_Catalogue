using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class BakedShadowGenerator : EditorWindow
{
    [MenuItem("Window/Utility/Baked Shadow Generator")]
    public static void ShowWindow() { GetWindow<BakedShadowGenerator>("Shadows"); }

    private const string ShadowSuffix = "_Shadow";

    private List<GameObject> m_SelectedObjects;
    private Vector2 m_ScrollPos = Vector2.zero;
    private bool m_Foldout = false;

    private void Awake()
    {
        m_SelectedObjects = new List<GameObject>();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Grab all LOD Groups") == true)
        {
            GrabShadowlessObjects();
        }

        m_Foldout = EditorGUILayout.Foldout(m_Foldout, $"({m_SelectedObjects.Count}) objects without shadows");

        if (m_Foldout == true)
        {
            using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos))
            {
                foreach (var go in m_SelectedObjects)
                {
                    EditorHelper.ObjectField(go);
                }
                m_ScrollPos = s.scrollPosition;
            }
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Generate Shadowcaster") == true)
        {
            foreach (var go in m_SelectedObjects)
            {
                GenerateShadowcaster(go);
                GrabShadowlessObjects();
            }
        }
    }

    private void GrabShadowlessObjects()
    {
        var lods = FindObjectsOfType<LODGroup>().ToList();
        lods.RemoveAll(l => RequiresShadowcaster(l) == false);
        m_SelectedObjects = lods.Extract(l => l.gameObject);
        m_Foldout = true;
    }

    private bool RequiresShadowcaster(LODGroup lodGroup)
    {
        foreach (var child in lodGroup.transform.GetDirectChildren())
        {
            if (child.name.Contains(ShadowSuffix) == true)
            {
                return false;
            }
        }
        foreach (var lod in lodGroup.GetLODs())
        {
            foreach (var renderer in lod.renderers)
            {
                if (renderer.shadowCastingMode == ShadowCastingMode.On)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void GenerateShadowcaster(GameObject go)
    {
        var lodGroup = go.GetComponent<LODGroup>();
        if (lodGroup != null)
        {
            foreach (var renderer in lodGroup.GetLODs()[0].renderers)
            {
                var shadowcaster = Utils.Unity.Clone(renderer.gameObject, renderer.transform.parent, true);
                shadowcaster.name = shadowcaster.name + ShadowSuffix;

                var shadowcasterRenderer = shadowcaster.GetComponent<Renderer>();
                shadowcasterRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                StaticFlagEditor.ToggleFlag(shadowcaster, StaticEditorFlags.ContributeGI, true);
            }
        }
    }
}

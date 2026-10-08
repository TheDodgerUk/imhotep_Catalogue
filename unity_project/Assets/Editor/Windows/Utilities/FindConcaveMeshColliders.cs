using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FindConcaveMeshColliders : EditorWindow
{
    [MenuItem("Window/Utility/Find Concave MeshColliders")]
    public static void ShowWindow() => GetWindow<FindConcaveMeshColliders>("Concave MeshColliders");

    [SerializeField]
    private List<MeshCollider> m_Colliders = new List<MeshCollider>();

    private Vector2 m_ScrollPos = Vector2.zero;

    private void Awake()
    {
        FindColliders();
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Find Concave MeshColliders"))
        {
            FindColliders();
        }

        using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos))
        {
            foreach (var meshCollider in m_Colliders)
            {
                EditorHelper.ObjectField(meshCollider);
            }
            m_ScrollPos = s.scrollPosition;
        }
    }

    private void FindColliders()
    {
        m_Colliders = FindObjectsOfType<MeshCollider>().ToList();
        m_Colliders.RemoveAll(c => c.convex == true);
        m_Colliders.Sort(Comparisons.ByHierarchy);
    }
}

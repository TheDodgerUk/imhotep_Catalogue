using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BoxColliderChain : EditorWindow
{
    [MenuItem("Window/Utility/Box Collider Chain")]
    public static void ShowWindow() { GetWindow<BoxColliderChain>("Collider Chain"); }

    private List<BoxCollider> m_CurrentColliders = new List<BoxCollider>();
    private List<Transform> m_CurrentNodes = new List<Transform>();

    private Transform m_Root;

    private bool m_AutoUpdate = false;

    private bool m_GenerateNavMesh = false;
    private bool m_FlipDirection = false;
    private float m_Padding = 0.05f;
    private float m_Skin = 0f;

    private void OnEnable()
    {
        SceneView.duringSceneGui -= OnSceneGui;
        SceneView.duringSceneGui += OnSceneGui;
        autoRepaintOnSceneChange = true;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGui;
    }

    private void OnGUI()
    {
        m_Padding = EditorGUILayout.FloatField("Collider Padding", m_Padding);
        m_Skin = EditorGUILayout.Slider("Skin Thickness", m_Skin, 0, 0.49f);
        m_FlipDirection = EditorGUILayout.Toggle("Flip Direction", m_FlipDirection);
        m_GenerateNavMesh = EditorGUILayout.Toggle("Block NavMesh", m_GenerateNavMesh);

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        m_Root = EditorHelper.ObjectField("Root", m_Root);
        bool rootChanged = EditorGUI.EndChangeCheck();

        EditorGUILayout.Space();

        if (m_Root == null)
        {
            m_CurrentColliders.Clear();
            m_CurrentNodes.Clear();
            m_AutoUpdate = false;
            return;
        }

        // get nodes & colliders from root
        m_CurrentNodes = m_Root.GetDirectChildren();
        m_CurrentColliders = m_Root.GetComponentsInChildren<BoxCollider>().ToList();
        m_CurrentColliders.ForEach(c => m_CurrentNodes.Remove(c.transform));

        // must have at least one node 
        if (m_CurrentNodes.Count < 2)
        {
            EditorGUILayout.HelpBox("Root must have at least 2 children to align", MessageType.Error);
            m_CurrentColliders.Clear();
            return;
        }

        if (rootChanged == true)
        {
            var a = m_CurrentNodes[0];
            var b = m_CurrentNodes[1];
            var collider = m_CurrentColliders.First();
            var delta = b.position - a.position;
            var direction = delta.normalized;
            float distance = delta.magnitude;

            int childCount = collider.transform.childCount;
            m_GenerateNavMesh = childCount > 0;
            m_Skin = collider.size.z * 0.5f - collider.center.z.Abs();
            m_FlipDirection = a.right.Dot(delta) < 0;
            m_Padding = collider.size.x - distance;

            m_Padding = m_Padding.Round(4);
            m_Skin = m_Skin.Round(4);
        }

        using (new GuiIndentScope())
        {
            DrawObjects("Nodes", m_CurrentNodes);

            EditorGUILayout.Space();

            DrawObjects("Colliders", m_CurrentColliders);
        }

        GUILayout.FlexibleSpace();

        m_AutoUpdate = EditorGUILayout.Toggle("Auto Update", m_AutoUpdate);
        using (new GuiEnabledScope(!m_AutoUpdate))
        {
            if (GUILayout.Button("Update Collider Chain") == true || m_AutoUpdate == true)
            {
                UpdateChain();
            }
        }

        SceneView.RepaintAll();
    }

    private void UpdateChain()
    {
        // previous node
        var a = m_CurrentNodes.First();
        for (int i = 1; i < m_CurrentNodes.Count; ++i)
        {
            // current node
            var b = m_CurrentNodes[i];
            PositionCollider(i, a, b);
            a = b;
        }
        a.right = m_CurrentNodes.First().right;

        // destroy excess colliders
        for (int i = m_CurrentNodes.Count - 1; i < m_CurrentColliders.Count; ++i)
        {
            m_CurrentColliders[i].DestroyGameObjectImmediate();
            m_CurrentColliders.RemoveAt(i);
            i--;
        }

        // rename organise nodes
        for (int i = 0; i < m_CurrentNodes.Count; ++i)
        {
            m_CurrentNodes[i].name = $"Node ({i + 1})";
            m_CurrentNodes[i].SetAsLastSibling();
        }
        // rename and organise colliders
        for (int i = 0; i < m_CurrentColliders.Count; ++i)
        {
            m_CurrentColliders[i].name = $"Collider ({i + 1})";
            m_CurrentColliders[i].transform.SetAsLastSibling();
        }

        // generate / destroy nav mesh blockers
        int navMeshArea = NavMesh.GetAreaFromName("Not Walkable");
        foreach (var collider in m_CurrentColliders)
        {
            // grab the existing blocker from the child
            var navMeshBlocker = collider.transform.FindComponents<Transform>().First();
            if (m_GenerateNavMesh == true)
            {
                // if there is no blocker, create one
                if (navMeshBlocker == null)
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.GetComponent<Collider>().DestroyObjectImmediate();
                    cube.transform.SetParent(collider.transform);
                    navMeshBlocker = cube.transform;
                }
                // set the flags, layer and nav mesh area
                var go = navMeshBlocker.gameObject;
                go.name = $"{m_Root.name}_{collider.name}";
                GameObjectUtility.SetNavMeshArea(go, navMeshArea);
                StaticFlagEditor.ToggleFlag(go, StaticEditorFlags.NavigationStatic, true);
                StaticFlagEditor.ToggleFlag(go, StaticEditorFlags.OffMeshLinkGeneration, true);

                // position and orient
                navMeshBlocker.transform.localRotation = Quaternion.identity;
                navMeshBlocker.transform.localPosition = collider.center;
                navMeshBlocker.transform.localScale = collider.size;
            }
            else
            {
                // destroy any existing nav mesh blocker
                if (navMeshBlocker != null)
                {
                    navMeshBlocker.DestroyGameObjectImmediate();
                }
            }
        }
    }

    private void PositionCollider(int i, Transform a, Transform b)
    {
        // get direction and distance between nodes
        var delta = b.position - a.position;
        var direction = delta.normalized;
        float distance = delta.magnitude;

        // create new collider if needed
        if (i > m_CurrentColliders.Count)
        {
            var collider = m_Root.CreateChild<BoxCollider>();
            collider.gameObject.layer = Layers.DefaultLayer;
            collider.gameObject.isStatic = true;
            m_CurrentColliders.Add(collider);
        }

        // get current collider and height
        var col = m_CurrentColliders[i - 1];
        float height = GetHeight(a, b, col);

        // set size of collider
        float depth = col.size.z * 0.5f - m_Skin;
        col.size = new Vector3(distance + m_Padding, height, col.size.z);
        col.center = new Vector3(0f, height * 0.5f, depth);

        // align and position collider 
        float directionSign = m_FlipDirection ? -1f : 1f;
        col.transform.position = (a.position + b.position) * 0.5f;
        col.transform.right = direction * directionSign;

        // point this node towards the next node
        a.right = direction * directionSign;
    }

    private float GetHeight(Transform a, Transform b, BoxCollider collider)
    {
        var aHeight = a.Find("Height")?.position.y;
        var bHeight = b.Find("Height")?.position.y;
        if (aHeight == null && bHeight == null)
        {
            return collider.size.y;
        }
        if (aHeight == null)
        {
            return bHeight.Value;
        }
        if (bHeight == null)
        {
            return aHeight.Value;
        }
        return aHeight.Value.Min(bHeight.Value);
    }

    private void DrawObjects<T>(string label, List<T> objects) where T : Object
    {
        if (objects.Count > 0)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            foreach (var obj in objects)
            {
                EditorHelper.ObjectField(obj);
            }
        }
    }

    private void OnSceneGui(SceneView sceneView)
    {
        m_CurrentNodes.RemoveAll(s => s == null);

        var previous = m_CurrentNodes.First();
        for (int i = 1; i < m_CurrentNodes.Count; ++i)
        {
            var current = m_CurrentNodes[i];

            Handles.color = Color.red;
            Handles.DrawLine(previous.position, current.position);

            previous = current;
        }

        foreach (var collider in m_CurrentColliders)
        {
            Handles.matrix = collider.transform.localToWorldMatrix;
            Handles.color = Color.green;
            Handles.DrawWireCube(collider.center, collider.size);
            Handles.color = Color.red;
            Handles.DrawWireCube(collider.center.With(z: 0), collider.size.With(z: 0));
        }
        Handles.matrix = Matrix4x4.identity;
    }
}

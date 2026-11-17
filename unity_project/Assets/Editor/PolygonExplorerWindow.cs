
/*
 * ============================================================
 * POLYGON EXPLORER - UNITY 2022.3
 * ============================================================
 *
 * PURPOSE:
 * WizTree-style polygon analyser for Unity scenes.
 * Shows which GameObjects and their children use the most
 * triangles, vertices and meshes.
 *
 * INSTALLATION:
 * 1. Save this script as PolygonExplorerWindow.cs.
 * 2. Place it inside an Editor folder
 *    (e.g. Assets/SystemCore/Editor/).
 * 3. Wait for Unity to compile.
 *
 * HOW TO USE:
 * 1. Open Tools -> Polygon Explorer.
 * 2. Click "Scan Scene" to analyse the loaded scenes.
 * 3. Expand GameObjects to inspect their children.
 * 4. Click a GameObject name to select it in the Hierarchy.
 * 5. Use the treemap to identify high-polygon objects.
 * 6. Click a treemap rectangle to select that GameObject.
 *
 * OPTIONS:
 * - Inactive      : Include inactive GameObjects.
 * - Selected Only : Scan only selected GameObjects and children.
 * - Largest First : Always sort by triangles, largest first.
 * - Treemap       : Show visual polygon usage.
 * - Hierarchy     : Show expandable GameObject hierarchy.
 * - Search        : Filter GameObjects by name.
 *
 * STATISTICS:
 * - Triangles   : Total mesh triangles, including children.
 * - Vertices    : Total mesh vertices, including children.
 * - Meshes      : Number of mesh instances.
 * - GameObjects : Number of scanned GameObjects.
 * - Percentage  : Share of total scanned triangles.
 *
 * NOTES:
 * - Works in Edit Mode and Play Mode.
 * - Does not modify scene objects or meshes.
 * - Scans only when "Scan Scene" is pressed.
 * - Parent totals include all descendants.
 * - Shared mesh instances are counted separately.
 * - Treemap displays top-level GameObjects.
 * - Counts mesh geometry, not actual rendered triangles.
 *
 * PERFORMANCE:
 * Use this tool to identify high-polygon FBX models,
 * complex prefabs and expensive scene geometry.
 *
 * ============================================================
 */

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PolygonExplorerWindow : EditorWindow
{
    private class Node
    {
        public GameObject gameObject;
        public string name;

        public readonly List<Node> children = new List<Node>();

        public long ownTriangles;
        public long totalTriangles;

        public long ownVertices;
        public long totalVertices;

        public int meshCount;
        public int totalMeshCount;

        public bool expanded;
    }

    private readonly List<Node> roots = new List<Node>();
    private readonly List<Node> allNodes = new List<Node>();

    private Vector2 scrollPosition;

    private long sceneTriangles;
    private long sceneVertices;
    private int sceneMeshes;
    private int sceneObjects;

    private bool includeInactive = true;
    private bool selectedOnly;
    private bool showTreemap = true;
    private bool showHierarchy = true;

    private string searchText = "";

    private const float RowHeight = 22f;
    private const float NameWidth = 280f;
    private const float BarWidth = 150f;

    private static readonly CultureInfo Culture =
        CultureInfo.GetCultureInfo("en-GB");

    // ============================================================
    // WINDOW
    // ============================================================

    [MenuItem("Tools/Polygon Explorer")]
    public static void Open()
    {
        GetWindow<PolygonExplorerWindow>(
            "Polygon Explorer");
    }

    private void OnGUI()
    {
        DrawToolbar();
        DrawSummary();

        if (allNodes.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "Press Scan Scene to analyse polygons.",
                MessageType.Info);

            return;
        }

        // Show hierarchy above treemap.
        if (showHierarchy)
            DrawHierarchy();

        if (showTreemap)
            DrawTreemap();
    }

    // ============================================================
    // TOOLBAR
    // ============================================================

    private void DrawToolbar()
    {
        EditorGUILayout.BeginVertical(
            EditorStyles.toolbar);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(
            "Scan Scene",
            EditorStyles.toolbarButton,
            GUILayout.Width(100)))
        {
            ScanScene();
        }

        includeInactive = GUILayout.Toggle(
            includeInactive,
            "Inactive",
            EditorStyles.toolbarButton,
            GUILayout.Width(70));

        selectedOnly = GUILayout.Toggle(
            selectedOnly,
            "Selected Only",
            EditorStyles.toolbarButton,
            GUILayout.Width(105));

        // Always largest first. No toggle needed.
        GUILayout.Label(
            "Largest First",
            EditorStyles.toolbarButton,
            GUILayout.Width(100));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        showTreemap = GUILayout.Toggle(
            showTreemap,
            "Treemap",
            EditorStyles.toolbarButton);

        showHierarchy = GUILayout.Toggle(
            showHierarchy,
            "Hierarchy",
            EditorStyles.toolbarButton);

        searchText = GUILayout.TextField(
            searchText,
            GUI.skin.FindStyle("ToolbarSeachTextField")
                ?? EditorStyles.toolbarTextField);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    // ============================================================
    // SCAN
    // ============================================================

    private void ScanScene()
    {
        roots.Clear();
        allNodes.Clear();

        sceneTriangles = 0;
        sceneVertices = 0;
        sceneMeshes = 0;
        sceneObjects = 0;

        try
        {
            if (selectedOnly)
            {
                GameObject[] selected =
                    Selection.gameObjects;

                // Avoid counting selected descendants twice.
                HashSet<GameObject> selectedSet =
                    new HashSet<GameObject>(selected);

                foreach (GameObject obj in selected)
                {
                    if (obj == null)
                        continue;

                    Transform parent = obj.transform.parent;
                    bool nested = false;

                    while (parent != null)
                    {
                        if (selectedSet.Contains(parent.gameObject))
                        {
                            nested = true;
                            break;
                        }

                        parent = parent.parent;
                    }

                    if (!nested)
                    {
                        Node node = BuildNode(obj);

                        if (node != null)
                            roots.Add(node);
                    }
                }
            }
            else
            {
                for (int s = 0;
                     s < SceneManager.sceneCount;
                     s++)
                {
                    Scene scene =
                        SceneManager.GetSceneAt(s);

                    if (!scene.isLoaded)
                        continue;

                    foreach (GameObject obj
                             in scene.GetRootGameObjects())
                    {
                        Node node = BuildNode(obj);

                        if (node != null)
                            roots.Add(node);
                    }
                }
            }

            // Calculate totals before sorting.
            CalculateTotals();

            // Sort highest triangle counts first.
            SortNodes(roots);

            // Reset hierarchy scrolling.
            scrollPosition = Vector2.zero;

            Repaint();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    // ============================================================
    // BUILD NODES
    // ============================================================

    private Node BuildNode(GameObject obj)
    {
        if (obj == null)
            return null;

        if (!includeInactive && !obj.activeInHierarchy)
            return null;

        Node node = new Node
        {
            gameObject = obj,
            name = obj.name
        };

        allNodes.Add(node);
        sceneObjects++;

        MeshFilter[] filters =
            obj.GetComponents<MeshFilter>();

        foreach (MeshFilter filter in filters)
        {
            if (filter.sharedMesh != null)
                AddMesh(node, filter.sharedMesh);
        }

        SkinnedMeshRenderer[] skinned =
            obj.GetComponents<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer renderer in skinned)
        {
            if (renderer.sharedMesh != null)
                AddMesh(node, renderer.sharedMesh);
        }

        for (int i = 0;
             i < obj.transform.childCount;
             i++)
        {
            GameObject child =
                obj.transform.GetChild(i).gameObject;

            Node childNode = BuildNode(child);

            if (childNode != null)
                node.children.Add(childNode);
        }

        return node;
    }

    // ============================================================
    // MESH STATISTICS
    // ============================================================

    private void AddMesh(Node node, Mesh mesh)
    {
        node.meshCount++;
        node.ownVertices += mesh.vertexCount;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            long indices = (long)mesh.GetIndexCount(i);

            switch (mesh.GetTopology(i))
            {
                case MeshTopology.Triangles:
                    node.ownTriangles += indices / 3;
                    break;

                case MeshTopology.Quads:
                    node.ownTriangles +=
                        (indices / 4) * 2;
                    break;
            }
        }
    }

    // ============================================================
    // TOTALS
    // ============================================================

    private void CalculateTotals()
    {
        foreach (Node root in roots)
            CalculateNodeTotals(root);

        sceneTriangles =
            roots.Sum(n => n.totalTriangles);

        sceneVertices =
            roots.Sum(n => n.totalVertices);

        sceneMeshes =
            roots.Sum(n => n.totalMeshCount);
    }

    private void CalculateNodeTotals(Node node)
    {
        node.totalTriangles = node.ownTriangles;
        node.totalVertices = node.ownVertices;
        node.totalMeshCount = node.meshCount;

        foreach (Node child in node.children)
        {
            CalculateNodeTotals(child);

            node.totalTriangles +=
                child.totalTriangles;

            node.totalVertices +=
                child.totalVertices;

            node.totalMeshCount +=
                child.totalMeshCount;
        }
    }

    // ============================================================
    // SORTING - LARGEST FIRST
    // ============================================================

    private void SortNodes(List<Node> nodes)
    {
        // Highest total triangle count first.
        nodes.Sort((a, b) =>
        {
            int result =
                b.totalTriangles.CompareTo(a.totalTriangles);

            // Alphabetical when counts are equal.
            return result != 0
                ? result
                : string.Compare(
                    a.name,
                    b.name,
                    StringComparison.OrdinalIgnoreCase);
        });

        // Recursively sort every child level.
        foreach (Node node in nodes)
            SortNodes(node.children);
    }

    // ============================================================
    // SUMMARY
    // ============================================================

    private void DrawSummary()
    {
        EditorGUILayout.Space(8);

        EditorGUILayout.BeginHorizontal(
            EditorStyles.helpBox);

        DrawStatistic(
            "Triangles",
            sceneTriangles.ToString("N0", Culture));

        DrawStatistic(
            "Vertices",
            sceneVertices.ToString("N0", Culture));

        DrawStatistic(
            "Meshes",
            sceneMeshes.ToString("N0", Culture));

        DrawStatistic(
            "GameObjects",
            sceneObjects.ToString("N0", Culture));

        EditorGUILayout.EndHorizontal();
    }

    private void DrawStatistic(
        string label,
        string value)
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Label(
            label,
            EditorStyles.miniLabel);

        GUILayout.Label(
            value,
            EditorStyles.boldLabel);

        EditorGUILayout.EndVertical();
    }

    // ============================================================
    // HIERARCHY
    // ============================================================

    private void DrawHierarchy()
    {
        // Sorting already occurred during ScanScene.
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Polygon Hierarchy",
            EditorStyles.boldLabel);

        scrollPosition =
            EditorGUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.MinHeight(180));

        foreach (Node node in roots)
            DrawNode(node, 0);

        EditorGUILayout.EndScrollView();
    }

    private void DrawNode(Node node, int depth)
    {
        if (!MatchesSearch(node))
            return;

        EditorGUILayout.BeginHorizontal(
            GUILayout.Height(RowHeight));

        GUILayout.Space(depth * 15f);

        // Fixed-width foldout slot.
        // Prevents foldouts stretching across the row.
        Rect foldoutRect = GUILayoutUtility.GetRect(
            16f,
            RowHeight,
            GUILayout.Width(16f));

        if (node.children.Count > 0)
        {
            node.expanded = EditorGUI.Foldout(
                foldoutRect,
                node.expanded,
                GUIContent.none,
                false);
        }

        // Select object by clicking its name.
        if (GUILayout.Button(
            node.name,
            EditorStyles.label,
            GUILayout.Width(
                Mathf.Max(80, NameWidth - depth * 15))))
        {
            Selection.activeGameObject =
                node.gameObject;

            EditorGUIUtility.PingObject(
                node.gameObject);
        }

        // Total triangle count.
        GUILayout.Label(
            node.totalTriangles.ToString(
                "N0", Culture),
            GUILayout.Width(100));

        // Percentage of total scene triangles.
        float percentage = sceneTriangles > 0
            ? (float)node.totalTriangles / sceneTriangles
            : 0f;

        Rect barRect =
            GUILayoutUtility.GetRect(
                BarWidth,
                15,
                GUILayout.Width(BarWidth));

        EditorGUI.ProgressBar(
            barRect,
            percentage,
            (percentage * 100f).ToString("F1") + "%");

        EditorGUILayout.EndHorizontal();

        // Draw children in sorted order.
        if (node.expanded)
        {
            foreach (Node child in node.children)
                DrawNode(child, depth + 1);
        }
    }

    // ============================================================
    // SEARCH
    // ============================================================

    private bool MatchesSearch(Node node)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return true;

        if (node.name.IndexOf(
                searchText,
                StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        foreach (Node child in node.children)
        {
            if (MatchesSearch(child))
                return true;
        }

        return false;
    }

    // ============================================================
    // TREEMAP
    // ============================================================

    private void DrawTreemap()
    {
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Polygon Treemap",
            EditorStyles.boldLabel);

        Rect rect = GUILayoutUtility.GetRect(
            100,
            280,
            GUILayout.ExpandWidth(true));

        EditorGUI.DrawRect(
            rect,
            new Color(0.13f, 0.13f, 0.13f));

        List<Node> visible = roots
            .Where(n => n.totalTriangles > 0)
            .OrderByDescending(n => n.totalTriangles)
            .ToList();

        if (visible.Count == 0)
            return;

        DrawTreemapNodes(
            visible,
            rect,
            true);
    }

    private void DrawTreemapNodes(
        List<Node> nodes,
        Rect rect,
        bool horizontal)
    {
        if (nodes.Count == 0)
            return;

        long total =
            nodes.Sum(n => n.totalTriangles);

        if (total <= 0)
            return;

        float offset = 0f;

        foreach (Node node in nodes)
        {
            float fraction =
                (float)node.totalTriangles / total;

            Rect childRect;

            if (horizontal)
            {
                float width = rect.width * fraction;

                childRect = new Rect(
                    rect.x + offset,
                    rect.y,
                    width,
                    rect.height);

                offset += width;
            }
            else
            {
                float height = rect.height * fraction;

                childRect = new Rect(
                    rect.x,
                    rect.y + offset,
                    rect.width,
                    height);

                offset += height;
            }

            childRect = new Rect(
                childRect.x + 1,
                childRect.y + 1,
                Mathf.Max(0, childRect.width - 2),
                Mathf.Max(0, childRect.height - 2));

            float intensity = Mathf.Clamp01(
                (float)node.totalTriangles /
                Mathf.Max(1, sceneTriangles));

            Color colour = Color.Lerp(
                new Color(0.15f, 0.45f, 0.65f),
                new Color(0.9f, 0.3f, 0.2f),
                intensity);

            EditorGUI.DrawRect(
                childRect,
                colour);

            if (childRect.width > 70 &&
                childRect.height > 35)
            {
                GUI.Label(
                    childRect,
                    node.name + "\n" +
                    node.totalTriangles.ToString(
                        "N0", Culture),
                    EditorStyles.whiteMiniLabel);
            }

            // Click a treemap rectangle to select its GameObject.
            if (Event.current.type ==
                EventType.MouseDown &&
                childRect.Contains(
                    Event.current.mousePosition))
            {
                Selection.activeGameObject =
                    node.gameObject;

                EditorGUIUtility.PingObject(
                    node.gameObject);

                Event.current.Use();
            }
        }
    }
}

#endif

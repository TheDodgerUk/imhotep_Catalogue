
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class MiniatureBaseScaler : MonoBehaviour
{
    [Header("Child Objects")]
    public Transform rootModel;
    public Transform placementCollision;

    [Header("Target Base")]
    [Min(1f)]
    public float targetDiameterMM = 40f;

    [Header("Base Detection")]
    [Tooltip("Height of the bottom section to scan, in mm.")]
    [Min(0.1f)]
    public float scanHeightMM = 5f;

    [Tooltip("Ignore tiny groups of vertices.")]
    [Min(3)]
    public int minimumVertices = 12;

    [Header("Options")]
    public bool alignBottomToZero = true;

    [Header("Measured Results")]
    [SerializeField] private float measuredDiameterMM;
    [SerializeField] private float lastScaleFactor;
    [SerializeField] private int sampledVertices;

    public float MeasuredDiameterMM => measuredDiameterMM;

    private void Reset()
    {
        FindChildren();
    }

    public void FindChildren()
    {
        if (rootModel == null)
            rootModel = transform.Find("RootModel");

        if (placementCollision == null)
            placementCollision =
                transform.Find("PlacementCollision");
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (rootModel == null)
            return;

        Renderer[] renderers =
            rootModel.GetComponentsInChildren<Renderer>(true);

        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        float scanY =
            bounds.min.y + scanHeightMM / 1000f;

        float lineLength =
            Mathf.Max(bounds.size.x, bounds.size.z) * 1.5f;

        float half = lineLength * 0.5f;

        Vector3 centre = new Vector3(
            bounds.center.x,
            scanY,
            bounds.center.z
        );

        // X-axis horizontal line
        Vector3 xStart =
            centre + Vector3.left * half;

        Vector3 xEnd =
            centre + Vector3.right * half;

        // Z-axis horizontal line (90 degrees to X)
        Vector3 zStart =
            centre + Vector3.back * half;

        Vector3 zEnd =
            centre + Vector3.forward * half;

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(xStart, xEnd);
        Gizmos.DrawLine(zStart, zEnd);

        Handles.Label(
            centre + Vector3.up * 0.005f,
            $"Scan Height: {scanHeightMM:F1} mm"
        );
    }

    private bool TryMeasure(
        out float diameter,
        out float bottomY,
        out int count)
    {
        diameter = 0f;
        bottomY = 0f;
        count = 0;

        FindChildren();

        if (rootModel == null)
        {
            Debug.LogError("RootModel not found.", this);
            return false;
        }

        MeshFilter[] filters =
            rootModel.GetComponentsInChildren<MeshFilter>(true);

        Matrix4x4 worldToParent =
            transform.worldToLocalMatrix;

        float minY = float.PositiveInfinity;
        bool found = false;

        // First pass: find lowest mesh vertex.
        foreach (MeshFilter filter in filters)
        {
            Mesh mesh = filter.sharedMesh;

            if (mesh == null)
                continue;

            Vector3[] vertices;

            try
            {
                vertices = mesh.vertices;
            }
            catch (System.Exception)
            {
                Debug.LogWarning(
                    $"Cannot read mesh: {mesh.name}. Enable Read/Write.",
                    filter);
                continue;
            }

            Matrix4x4 matrix =
                worldToParent *
                filter.transform.localToWorldMatrix;

            foreach (Vector3 vertex in vertices)
            {
                Vector3 p =
                    matrix.MultiplyPoint3x4(vertex);

                minY = Mathf.Min(minY, p.y);
                found = true;
            }
        }

        if (!found)
            return false;

        bottomY = minY;

        float maxScanY =
            minY + scanHeightMM / 1000f;

        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float minZ = float.PositiveInfinity;
        float maxZ = float.NegativeInfinity;

        // Second pass: sample vertices below scan height.
        foreach (MeshFilter filter in filters)
        {
            Mesh mesh = filter.sharedMesh;

            if (mesh == null)
                continue;

            Vector3[] vertices;

            try
            {
                vertices = mesh.vertices;
            }
            catch (System.Exception)
            {
                continue;
            }

            Matrix4x4 matrix =
                worldToParent *
                filter.transform.localToWorldMatrix;

            foreach (Vector3 vertex in vertices)
            {
                Vector3 p =
                    matrix.MultiplyPoint3x4(vertex);

                if (p.y > maxScanY)
                    continue;

                minX = Mathf.Min(minX, p.x);
                maxX = Mathf.Max(maxX, p.x);

                minZ = Mathf.Min(minZ, p.z);
                maxZ = Mathf.Max(maxZ, p.z);

                count++;
            }
        }

        if (count < minimumVertices)
        {
            Debug.LogWarning(
                $"Only {count} bottom vertices found. " +
                "Increase Scan Height.",
                this);
            return false;
        }

        float width = maxX - minX;
        float depth = maxZ - minZ;

        diameter = Mathf.Max(width, depth);

        return diameter > 0.000001f;
    }

    public void MeasureBase()
    {
        if (!TryMeasure(
            out float diameter,
            out float bottom,
            out int count))
        {
            Debug.LogWarning(
                "Unable to measure base.",
                this);
            return;
        }

        measuredDiameterMM = diameter * 1000f;
        sampledVertices = count;

        EditorUtility.SetDirty(this);

        Debug.Log(
            $"Base diameter: {measuredDiameterMM:F2} mm\n" +
            $"Bottom Y: {bottom:F5}\n" +
            $"Sampled vertices: {count}",
            this);
    }

    public void ScaleToTarget()
    {
        FindChildren();

        if (rootModel == null)
        {
            Debug.LogError("RootModel missing.", this);
            return;
        }

        if (transform.localScale != Vector3.one)
        {
            Debug.LogError(
                "Parent scale must be (1,1,1) before scaling.",
                this);
            return;
        }

        if (!TryMeasure(
            out float diameter,
            out float bottom,
            out int count))
        {
            Debug.LogWarning(
                "Unable to scale base.",
                this);
            return;
        }

        float target = targetDiameterMM / 1000f;
        float factor = target / diameter;

        if (float.IsNaN(factor) ||
            float.IsInfinity(factor) ||
            factor <= 0f)
            return;

        Undo.RecordObject(
            rootModel, "Scale RootModel");

        if (placementCollision != null)
        {
            Undo.RecordObject(
                placementCollision,
                "Scale PlacementCollision");
        }

        Undo.RecordObject(this, "Scale Miniature Base");

        // Parent scale remains (1,1,1).
        rootModel.localScale *= factor;
        rootModel.localPosition *= factor;

        if (placementCollision != null)
        {
            placementCollision.localScale *= factor;
            placementCollision.localPosition *= factor;
        }

        if (TryMeasure(
            out float newDiameter,
            out float newBottom,
            out int newCount))
        {
            if (alignBottomToZero)
            {
                Vector3 offset =
                    Vector3.up * -newBottom;

                rootModel.localPosition += offset;

                if (placementCollision != null)
                    placementCollision.localPosition += offset;
            }

            measuredDiameterMM = newDiameter * 1000f;
            sampledVertices = newCount;
        }

        lastScaleFactor = factor;


        this.gameObject.transform.localScale = Vector3.one;
        EditorUtility.SetDirty(this.gameObject);
        EditorUtility.SetDirty(rootModel);

        if (placementCollision != null)
            EditorUtility.SetDirty(placementCollision);

        EditorUtility.SetDirty(this);

        PrefabUtility.SavePrefabAsset(gameObject);

        Debug.Log(
            $"Scaled miniature base to {targetDiameterMM:F2} mm\n" +
            $"Scale factor: {factor:F5}\n" +
            $"Measured diameter: {measuredDiameterMM:F2} mm\n" +
            $"Parent scale: {transform.localScale}",
            this);
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(MiniatureBaseScaler))]
public class MiniatureBaseScalerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Editable fields
        DrawPropertiesExcluding(
            serializedObject,
            "m_Script",
            "measuredDiameterMM",
            "lastScaleFactor",
            "sampledVertices"
        );

        EditorGUILayout.Space(10);

        // Read-only measured results
        EditorGUILayout.LabelField(
            "Measured Results",
            EditorStyles.boldLabel
        );

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(
                    "measuredDiameterMM")
            );

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(
                    "lastScaleFactor")
            );

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(
                    "sampledVertices")
            );
        }

        serializedObject.ApplyModifiedProperties();

        MiniatureBaseScaler scaler =
            (MiniatureBaseScaler)target;

        EditorGUILayout.Space(12);

        if (GUILayout.Button(
            "Find RootModel / PlacementCollision",
            GUILayout.Height(28)))
        {
            scaler.FindChildren();
            EditorUtility.SetDirty(scaler);
        }

        if (GUILayout.Button(
            "Measure Current Base",
            GUILayout.Height(32)))
        {
            scaler.MeasureBase();
        }

        EditorGUILayout.Space(8);

        if (GUILayout.Button(
            "Scale Base To Target Size",
            GUILayout.Height(36)))
        {
            scaler.ScaleToTarget();
        }

        EditorGUILayout.Space(8);

        EditorGUILayout.HelpBox(
            "Two yellow lines cross at 90 degrees at the " +
            "scan height. Measured results are read-only. " +
            "RootModel and PlacementCollision are scaled together.",
            MessageType.Info
        );
    }
}

#endif

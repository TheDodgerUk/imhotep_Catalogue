
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;



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

        // Convert vertices to parent-local coordinates.
        Matrix4x4 worldToParent = transform.worldToLocalMatrix;

        float minY = float.PositiveInfinity;
        bool found = false;

        // First pass: find the lowest vertex.
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
                worldToParent * filter.transform.localToWorldMatrix;

            foreach (Vector3 vertex in vertices)
            {
                Vector3 p = matrix.MultiplyPoint3x4(vertex);
                minY = Mathf.Min(minY, p.y);
                found = true;
            }
        }

        if (!found)
            return false;

        bottomY = minY;

        float maxScanY = minY + scanHeightMM / 1000f;

        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float minZ = float.PositiveInfinity;
        float maxZ = float.NegativeInfinity;

        // Second pass: examine only bottom vertices.
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
                worldToParent * filter.transform.localToWorldMatrix;

            foreach (Vector3 vertex in vertices)
            {
                Vector3 p = matrix.MultiplyPoint3x4(vertex);

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
            Debug.LogWarning("Unable to measure base.", this);
            return;
        }

        measuredDiameterMM = diameter * 1000f;
        sampledVertices = count;

        UnityEditor.EditorUtility.SetDirty(this);

        Debug.Log(
            $"Base diameter: {measuredDiameterMM:F2} mm\n" +
            $"Bottom Y: {bottom:F5}\n" +
            $"Sampled vertices: {count}",
            this);
    }

    public void ScaleToTarget()
    {
        if (!TryMeasure(
            out float diameter,
            out float bottom,
            out int count))
        {
            Debug.LogWarning("Unable to scale base.", this);
            return;
        }

        float target = targetDiameterMM / 1000f;
        float factor = target / diameter;

        if (!float.IsFinite(factor) || factor <= 0f)
            return;

        UnityEditor.Undo.RecordObject(
            transform, "Scale Miniature Parent");

        UnityEditor.Undo.RecordObject(
            rootModel, "Scale RootModel");

        if (placementCollision != null)
            UnityEditor.Undo.RecordObject(
                placementCollision, "Scale PlacementCollision");

        // Keep the prefab parent at scale 1.
        // This assumes it is not already carrying a custom scale.
        transform.localScale = Vector3.one;

        rootModel.localScale *= factor;
        rootModel.localPosition *= factor;

        if (placementCollision != null)
        {
            placementCollision.localScale *= factor;
            placementCollision.localPosition *= factor;
        }

        // Measure again after scaling.
        if (TryMeasure(
            out float newDiameter,
            out float newBottom,
            out int newCount))
        {
            if (alignBottomToZero)
            {
                Vector3 offset = Vector3.up * -newBottom;

                rootModel.localPosition += offset;

                if (placementCollision != null)
                    placementCollision.localPosition += offset;
            }

            measuredDiameterMM = newDiameter * 1000f;
            sampledVertices = newCount;
        }

        lastScaleFactor = factor;

        UnityEditor.EditorUtility.SetDirty(this);

        Debug.Log(
            $"Scaled miniature base to {targetDiameterMM:F2} mm\n" +
            $"Scale factor: {factor:F5}\n" +
            $"Parent scale: {transform.localScale}",
            this);
    }

#endif
}


/////////////////////////////////////////////////////////////////
///
#if UNITY_EDITOR



[CustomEditor(typeof(MiniatureBaseScaler))]
public class MiniatureBaseScalerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

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

        EditorGUILayout.HelpBox(
            "Measures mesh vertices near the bottom of RootModel. " +
            "Supports models rotated 0 or -90 degrees on X, " +
            "provided the base is horizontal in the scene.",
            MessageType.Info);
    }
}

#endif
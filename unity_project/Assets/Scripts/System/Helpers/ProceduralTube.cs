using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer)), ExecuteInEditMode]
public class ProceduralTube : MonoBehaviour
{
    public Material Material => m_MeshRenderer.sharedMaterial;
    public Mesh Mesh => m_MeshFilter.sharedMesh;

    [SerializeField]
    private int m_SegmentCount = 8;
    [SerializeField]
    private float m_Radius = 0.05f;

    private List<Vector3> m_Positions;
    private List<Vector3> m_Ups;

    private List<Vector3> m_Vertices;
    private List<Vector3> m_Normals;
    private List<Vector2> m_Uvs;

    private List<int> m_Triangles;

    [SerializeField]
    private MeshRenderer m_MeshRenderer;
    [SerializeField]
    private MeshFilter m_MeshFilter;
    private Mesh m_Mesh;

    private void Awake()
    {
        // setup mesh renderer + filter
        m_MeshFilter = gameObject.ForceComponent<MeshFilter>();
        m_MeshRenderer = gameObject.ForceComponent<MeshRenderer>();

        ValidateMeshFilter();

        m_Triangles = new List<int>();
        m_Vertices = new List<Vector3>();
        m_Normals = new List<Vector3>();
        m_Uvs = new List<Vector2>();
    }

    public void SetMaterial(Material material) => m_MeshRenderer.sharedMaterial = material;

    public void SetPositions(List<Vector3> positions, List<Vector3> ups)
    {
        if (positions.Count >= 2)
        {
            m_Positions = positions.Extract(transform.InverseTransformPoint);
            m_Ups = ups.Extract(transform.InverseTransformDirection);
            RebuildMesh();
        }
    }

    public void SetPositions(List<Vector3> positions)
    {
        SetPositions(positions, positions.Extract(p => Vector3.up));
    }

    private void ValidateMeshFilter()
    {
        if (m_MeshFilter.sharedMesh == null)
        {
            m_MeshFilter.sharedMesh = new Mesh();
            m_MeshFilter.sharedMesh.MarkDynamic();
        }
    }

    [InspectorButton]
    private void RebuildMesh()
    {
        ValidateMeshFilter();
        m_Triangles.Clear();
        m_Vertices.Clear();
        m_Normals.Clear();
        m_Uvs.Clear();

        for (int i = 0; i < m_Positions.Count; i++)
        {
            var forward = GetTubeDirection(i);
            BuildRing(m_Positions[i], forward, m_Ups[i], i / 2f, i > 0);
        }

        var mesh = m_MeshFilter.sharedMesh;
        mesh.SetVertices(m_Vertices);
        mesh.SetNormals(m_Normals);
        mesh.SetUVs(0, m_Uvs);
        mesh.SetTriangles(m_Triangles, 0);
        mesh.RecalculateBounds();
    }

    private Vector3 GetTubeDirection(int i)
    {
        int n = m_Positions.Count - 1;
        if (i == 0)
        {
            return m_Positions[1] - m_Positions[0];
        }
        else if (i == n)
        {
            return m_Positions[n] - m_Positions[n - 1];
        }
        else
        {
            var before = m_Positions[i] - m_Positions[i - 1];
            var after = m_Positions[i + 1] - m_Positions[i];
            return (before + after) * 0.5f;
        }
    }

    private void BuildRing(Vector3 centre, Vector3 forward, Vector3 up, float v, bool addTriangle)
    {
        var rotation = Quaternion.LookRotation(forward, up);
        float angleInc = (Mathf.PI * 2.0f) / m_SegmentCount;
        for (int i = 0; i <= m_SegmentCount; i++)
        {
            float angle = angleInc * i;

            // compute normal
            var normal = Vector3.zero;
            normal.x = Mathf.Sin(angle);
            normal.y = Mathf.Cos(angle);
            normal = rotation * normal;
            m_Normals.Add(normal);

            // add vertex along normal from centre
            m_Vertices.Add(centre + normal * m_Radius);

            // compute uv to wrap at each segment
            m_Uvs.Add(new Vector2((float)i / m_SegmentCount, v));

            if (i > 0 && true == addTriangle)
            {
                int baseIndex = m_Vertices.Count - 1;
                int vertsPerRow = m_SegmentCount + 1;

                int index0 = baseIndex;
                int index1 = baseIndex - 1;
                int index2 = baseIndex - vertsPerRow;
                int index3 = baseIndex - vertsPerRow - 1;

                m_Triangles.Add(index0);
                m_Triangles.Add(index2);
                m_Triangles.Add(index1);

                m_Triangles.Add(index2);
                m_Triangles.Add(index3);
                m_Triangles.Add(index1);
            }
        }
    }

//    private void OnDrawGizmos()
//    {
//        Gizmos.matrix = transform.localToWorldMatrix;

//        Gizmos.color = Color.blue;
//        foreach (var vert in m_Vertices)
//        {
//#if UNITY_EDITOR
//            float size = UnityEditor.HandleUtility.GetHandleSize(transform.TransformPoint(vert));
//#else
//            float size = 1f;
//#endif
//            Gizmos.DrawSphere(vert, size * m_Radius * 5f);
//        }

//        Gizmos.color = Color.yellow;
//        foreach (var pos in m_Positions)
//        {
//#if UNITY_EDITOR
//            float size = UnityEditor.HandleUtility.GetHandleSize(transform.TransformPoint(pos));
//#else
//            float size = 1f;
//#endif
//            Gizmos.DrawSphere(pos, size * m_Radius * 20f);
//        }
//    }
}

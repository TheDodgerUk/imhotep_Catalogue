using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OutlineMesh : MonoBehaviour
{
    private void Awake()
    {
        gameObject.AverageMeshNormals();
    }
}

public static class OutlineMeshExtensions
{
    private static readonly Vector3 zeroVec = Vector3.zero;
    private static Dictionary<Mesh, Color[]> m_ModelToVertexColors = new Dictionary<Mesh, Color[]>();

    public struct VertInfo
    {
        public Vector3 vert;
        public int origIndex;
        public Vector3 normal;
        public Vector3 averagedNormal;
    }

    public static void AverageMeshNormals(this GameObject go)
    {
        foreach (var meshFilter in go.GetComponentsInChildren<MeshFilter>(true))
        {
            meshFilter.AverageMeshNormals();
        }
    }

    public static void AverageMeshNormals(this MeshFilter meshFilter)
    {
        Mesh mesh = meshFilter.mesh;
        if (true == m_ModelToVertexColors.ContainsKey(meshFilter.sharedMesh))
        {
            Color[] colors = m_ModelToVertexColors[meshFilter.sharedMesh];

            if (mesh.vertices.Length == colors.Length)
            {
                mesh.colors = colors;
            }
            else
            {
                Debug.LogWarning("Mis match length of verts to colors");
            }
        }
        else
        {
            Vector3[] verts = mesh.vertices;
            Vector3[] normals = mesh.normals;
            VertInfo[] vertInfo = new VertInfo[verts.Length];

            for (int i = 0; i < verts.Length; i++)
            {
                vertInfo[i] = new VertInfo()
                {
                    vert = verts[i],
                    origIndex = i,
                    normal = normals[i]
                };
            }

            var groups = vertInfo.GroupBy(x => x.vert);
            VertInfo[] processedVertInfo = new VertInfo[vertInfo.Length];
            int index = 0;
            foreach (IGrouping<Vector3, VertInfo> group in groups)
            {
                Vector3 avgNormal = zeroVec;
                foreach (VertInfo item in group)
                {
                    avgNormal += item.normal;
                }
                avgNormal = avgNormal / group.Count();
                foreach (VertInfo item in group)
                {
                    processedVertInfo[index] = new VertInfo()
                    {
                        vert = item.vert,
                        origIndex = item.origIndex,
                        normal = item.normal,
                        averagedNormal = avgNormal
                    };
                    index++;
                }
            }
            Color[] colors = new Color[verts.Length];
            for (int i = 0; i < processedVertInfo.Length; i++)
            {
                VertInfo info = processedVertInfo[i];

                int origIndex = info.origIndex;
                Vector3 normal = info.averagedNormal;
                Color normColor = new Color(normal.x, normal.y, normal.z, 1);
                colors[origIndex] = normColor;
            }
            mesh.colors = colors;
            m_ModelToVertexColors.Add(meshFilter.sharedMesh, colors);
        }
    }
}

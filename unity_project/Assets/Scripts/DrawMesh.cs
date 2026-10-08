using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMesh : MonoBehaviour
{
    public Mesh mesh;
    [SerializeField]
    private Material m_Material;

    private Matrix4x4[][] m_Matrices;

	// Use this for initialization
	void Start ()
    {
        this.WaitFor(1.0f, () =>
        {
        List<Transform> children = transform.GetDirectChildren();
        m_Matrices = new Matrix4x4[children.Count][];
        for (int i = 0; i < children.Count; i++)
        {
            m_Matrices[i] = GetMatrices(children[i]).ToArray();
            Debug.Log("Setup instancing for: " + m_Matrices[i].Length);
        }
        });

    }

// Update is called once per frame
    void Update ()
    {
        if (null != m_Matrices)
        {
            for(int x = 0; x < m_Matrices.Length; x++)
            {
                Graphics.DrawMeshInstanced(mesh, 0, m_Material, m_Matrices[x], m_Matrices[x].Length);
            }
        }
	}

    private List<Matrix4x4> GetMatrices(Transform parent)
    {
        List<Matrix4x4> matrices = new List<Matrix4x4>();
        //Get Children
        List<Transform> children = parent.GetDirectChildren();

        for(int i =0; i < children.Count; i++)
        {
            Matrix4x4 mat = new Matrix4x4();
            Vector3 position = children[i].position + (children[i].forward * 0.25f);
            mat.SetTRS(position, children[i].rotation, Vector3.one);
            matrices.Add(mat);
        }

        return matrices;
    }
}

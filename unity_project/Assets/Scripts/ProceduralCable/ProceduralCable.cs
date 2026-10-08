using System.Collections.Generic;
using UnityEngine;

public class ProceduralCable : MonoBehaviour, ISplineCallbackReceiver
{
    public ProceduralTube WireMesh => m_Wire.ProceduralMesh;
    public Material WireMaterial => m_Wire.Material;
    public GameObject BakedMesh
    {
        get { return m_BakedMesh; }
        set { m_BakedMesh = value; }
    }
    public bool IsDirtyBake
    {
        get { return m_IsDirtyBake; }
        set { m_IsDirtyBake = value; }
    }

    [SerializeField, Range(0, 256)]
    private int m_Quality = 24;

    [SerializeField, HideInInspector]
    private GameObject m_BakedMesh;
    [SerializeField, HideInInspector]
    private bool m_IsDirtyBake = false;

    [SerializeField]
    private CableSection m_Start;
    [SerializeField]
    private CableSection m_Wire;
    [SerializeField]
    private CableSection m_End;

    [InspectorButton]
    public void OnSplineChanged()
    {
        var spline = GetComponent<MonoSpline>();

        // ensure dummy creation
        m_Start.Create(transform, "Start", false);
        m_Wire.Create(transform, "Wire", true);
        m_End.Create(transform, "End", false);

        // position start dummy
        m_Start.Transform.position = spline.GetWorldPosition(0f);
        m_Start.Transform.forward = spline.GetStartDirection();

        // position end dummy
        m_End.Transform.position = spline.GetWorldPosition(1f);
        m_End.Transform.forward = spline.GetEndDirection();

        // generate list of positions and up vectors
        var positions = new List<Vector3>();
        var ups = new List<Vector3>();
        for (int i = 0; i <= m_Quality; ++i)
        {
            float t = i / (float)m_Quality;
            positions.Add(spline.GetWorldPosition(t));
            ups.Add(spline.GetUp(t));
        }

        // flip back to procedural mesh
        if (m_BakedMesh != null)
        {
            m_BakedMesh.SetActive(false);
        }
        m_Wire.ProceduralMesh.SetActive(true);

        // orient the cable transform and update mesh
        m_Wire.Transform.OrientTo(m_Start.Transform);
        m_Wire.ProceduralMesh.SetPositions(positions, ups);
        m_IsDirtyBake = true;
    }
}

[System.Serializable]
public class CableSection
{
    public ProceduralTube ProceduralMesh => m_ProceduralMesh;
    public Transform Transform => m_Transform;
    public Material Material => m_Material;

    [SerializeField, HideInInspector]
    private ProceduralTube m_ProceduralMesh;
    [SerializeField, HideInInspector]
    private MeshRenderer m_Renderer;
    [SerializeField, HideInInspector]
    private Transform m_Transform;

    [SerializeField]
    private Material m_Material;
    [SerializeField]
    private Mesh m_Mesh;

    public void Create(Transform parent, string name, bool procedural)
    {
        if (m_Transform == null)
        {
            m_Transform = parent.CreateChild(name);
        }
        if (m_Renderer == null)
        {
            if (procedural == true)
            {
                m_ProceduralMesh = m_Transform.AddComponent<ProceduralTube>();
                m_Renderer = m_Transform.GetComponent<MeshRenderer>();
            }
            else
            {
                m_Transform.AddComponent<MeshFilter>();
                m_Renderer = m_Transform.AddComponent<MeshRenderer>();
            }
        }

        if (m_Mesh != null && procedural == false)
        {
            m_Transform.GetComponent<MeshFilter>().sharedMesh = m_Mesh;
        }
        if (procedural == true)
        {
           // bool bState = m_ProceduralMesh.gameObject.activeSelf;
         //   m_ProceduralMesh.SetActive(true);
            m_Mesh = m_ProceduralMesh.Mesh;
          //  m_ProceduralMesh.SetActive(bState);
        }
        m_Renderer.sharedMaterial = m_Material;
    }
}
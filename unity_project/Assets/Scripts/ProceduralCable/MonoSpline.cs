using System.Collections.Generic;
using UnityEngine;

public interface ISplineCallbackReceiver
{
    void OnSplineChanged();
}

[ExecuteInEditMode]
public class MonoSpline : MonoBehaviour
{
    public TransformSpline Spline => m_Spline;
    public Transform Start => m_Nodes.First();
    public Transform End => m_Nodes.Last();

    [SerializeField]
    private Transform m_NodeParent;

    [SerializeField, HideInInspector]
    private List<Quaternion> m_Rotations;
    [SerializeField, HideInInspector]
    private List<Vector3> m_Positions;
    [SerializeField, HideInInspector]
    private List<Transform> m_Nodes;

    private TransformSpline m_Spline;

    private void Awake()
    {
        if (m_Nodes == null)
        {
            m_Nodes = new List<Transform>();
            m_Positions = new List<Vector3>();
            m_Rotations = new List<Quaternion>();
        }
        CreateSpline();
    }

    private void Reset()
    {
        if (m_NodeParent == null)
        {
            m_NodeParent = transform.CreateChild("Nodes");
            var start = m_NodeParent.CreateChild("Node 1");
            var end = m_NodeParent.CreateChild("Node 2");
            start.localPosition = new Vector3(0, 0, 0);
            end.localPosition = new Vector3(0, 0, 1);
        }
    }

    private void Update()
    {
        if (m_Spline == null)
        {
            CreateSpline();
        }
        if (m_NodeParent != null)
        {
            var nodes = m_NodeParent.GetDirectChildren();
            if (HaveNodesChanged(nodes) == true)
            {
                for (int i = 0; i < nodes.Count; ++i)
                {
                    nodes[i].name = $"Node ({i})";
                }
                m_Nodes = nodes;

                OnNodesChanged();

                m_Rotations = nodes.Extract(n => n.localRotation);
                m_Positions = nodes.Extract(n => n.localPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(float t) => m_NodeParent.TransformPoint(GetLocalPosition(t));
    public Vector3 GetLocalPosition(float t) => m_Spline.GetPosition(t.Clamp(0, 1));
    public Vector3 GetForward(float t) => m_Spline.GetForward(t.Clamp(0, 1));
    public Vector3 GetUp(float t) => m_Spline.GetUp(t.Clamp(0, 1));

    public Vector3 GetStartDirection() => GetDirection(0f, 0.01f);
    public Vector3 GetEndDirection() => GetDirection(1f, 0.99f);

    private Vector3 GetDirection(float f0, float f1) => GetLocalPosition(f0) - GetLocalPosition(f1);

    private void CreateSpline()
    {
        if (m_Nodes.Count >= 2)
        {
            m_Spline = new TransformSpline(m_Nodes);
        }
    }

    private void OnNodesChanged()
    {
        if (m_Nodes.Count >= 2)
        {
            CreateSpline();

            foreach (var r in GetComponents<ISplineCallbackReceiver>())
            {
                r.OnSplineChanged();
            }
        }
        else
        {
            m_Positions.Clear();
            m_Rotations.Clear();
            m_Spline = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (m_Spline == null)
        {
            return;
        }

        var pos = GetWorldPosition(0);
        for (int i = 1; i <= 100; ++i)
        {
            float t = i / 100f;
            var newPos = GetWorldPosition(t);
            Gizmos.DrawLine(pos, newPos);
            pos = newPos;
        }
    }

    private bool HaveNodesChanged(List<Transform> nodes)
    {
        if (m_Nodes.Count != nodes.Count)
        {
            return true;
        }
        if (m_Positions.Count != nodes.Count)
        {
            return true;
        }
        if (m_Rotations.Count != nodes.Count)
        {
            return true;
        }
        for (int i = 0; i < nodes.Count; ++i)
        {
            if (m_Nodes[i] != nodes[i])
            {
                return true;
            }
            if (Vector3.Distance(m_Positions[i], nodes[i].localPosition) > 0)
            {
                return true;
            }
            if (Quaternion.Angle(m_Rotations[i], nodes[i].localRotation) > 0)
            {
                return true;
            }
        }
        return false;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class TransformSpline
{
    private readonly Vector3Spline m_Position;
    private readonly Vector3Spline m_Forward;
    private readonly Vector3Spline m_Up;

    public TransformSpline(List<Transform> nodes)
    {
        if (nodes.Count < 2)
        {
            throw new ArgumentException("Cannot create a spline with less than 2 points");
        }

        var tParam = Vector3Spline.GetParameters(nodes);

        m_Position = new Vector3Spline(tParam, nodes.Extract(n => n.localPosition));
        for (int i = 0; i < nodes.Count; ++i)
        {
            float t = i / (nodes.Count - 1f);
            AlignNode(nodes[i], t);
        }

        m_Forward = new Vector3Spline(tParam, nodes.Extract(n => n.forward));
        m_Up = new Vector3Spline(tParam, nodes.Extract(n => n.up));
    }

    public Vector3 GetPosition(float t) => m_Position.Interpolate(t);
    public Vector3 GetForward(float t) => m_Forward.Interpolate(t);
    public Vector3 GetUp(float t) => m_Up.Interpolate(t);

    private void AlignNode(Transform node, float t)
    {
        const float Inc = 0.01f;
        var before = node.localPosition - GetPosition(t - Inc);
        var after = GetPosition(t + Inc) - node.localPosition;
        var direction = (before + after) * 0.5f;
        node.LookAt(node.localPosition + direction, node.up);
    }
}

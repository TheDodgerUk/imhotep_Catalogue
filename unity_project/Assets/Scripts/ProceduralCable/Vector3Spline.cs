using MathNet.Numerics.Interpolation;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Spline
{
    private readonly CubicSpline[] m_Splines;

    public Vector3Spline(List<Vector3> vectors) : this(GetParameters(vectors), vectors) { }

    public Vector3Spline(List<double> t, List<Vector3> vectors)
    {
        if (vectors.Count < 2)
        {
            throw new ArgumentException("Cannot create a spline with less than 2 elements");
        }

        m_Splines = new CubicSpline[3];
        for (int i = 0; i < m_Splines.Length; ++i)
        {
            m_Splines[i] = CubicSpline.InterpolateNatural(t, vectors.Extract(v => (double)v[i]));
        }
    }

    public Vector3 Interpolate(float t)
    {
        var vector = Vector3.zero;
        for (int i = 0; i < m_Splines.Length; ++i)
        {
            vector[i] = (float)m_Splines[i].Interpolate(t);
        }
        return vector;
    }

    public static List<double> GetParameters<T>(List<T> nodes)
    {
        int i = 0;
        double n = nodes.Count - 1.0;
        return nodes.Extract(p => (i++) / n);
    }
}

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RadialPositions : EditorWindow
{
    [MenuItem("Window/Utility/Radial Positions")]
    public static void GetWindow()
    {
        GetWindow<RadialPositions>("Radial Positions");
    }

    private List<Transform> m_Children = new List<Transform>();
    private float m_StartAngle = 0;
    private float m_Radius = 0;

    private void OnGUI()
    {
        if (GUILayout.Button("Grab Children") == true)
        {
            m_Children = Selection.activeTransform.FindComponents<Transform>();
        }

        if (m_Children.Count > 0)
        {
            if (GUILayout.Button("Reset Defaults") == true)
            {
                m_Radius = m_Children[0].localPosition.magnitude;
                m_StartAngle = Vector3.Angle(Vector3.up, m_Children[0].localPosition);
            }

            for (int i = 0; i < m_Children.Count; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.ObjectField(m_Children[i], m_Children[i].GetType(), true);

                    if (GUILayout.Button("X") == true)
                    {
                        m_Children.RemoveAt(i);
                        --i;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // fields for disc and radius
            float radius = Mathf.Max(EditorGUILayout.FloatField("Radius", m_Radius), 0);
            if (m_Radius != radius)
            {
                m_Radius = radius;
                PositionChildren();
            }

            // fields for disc and radius
            float startAngle = Mathf.Max(EditorGUILayout.FloatField("StartAngle", m_StartAngle), 0);
            if (m_StartAngle != startAngle)
            {
                m_StartAngle = startAngle;
                PositionChildren();
            }
        }
    }

    private void PositionChildren()
    {
        // compute step in angle and each position
        Vector2 centre = Vector2.zero;
        float angleStep = (-Mathf.PI * 2) / m_Children.Count;
        for (int i = 0; i < m_Children.Count; ++i)
        {
            float theta = (i * angleStep);
            //float angle = theta * Mathf.Rad2Deg;
            theta -= m_StartAngle * Mathf.Deg2Rad;
            theta += Mathf.PI * 0.5f;

            float x = centre.x + m_Radius * Mathf.Cos(theta);
            float y = centre.y + m_Radius * Mathf.Sin(theta);
            m_Children[i].localPosition = new Vector3(x, y, 0);
            m_Children[i].localRotation = Quaternion.identity;// (0, 0, angle);
        }
    }
}

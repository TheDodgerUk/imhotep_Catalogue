using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class LightProbePlacement : EditorWindow
{
    private LightProbeGroup m_LightProbeGroup;
    private float m_MergeDistance = 1;

    [MenuItem("Window/Utility/Generate Light Probes")]
    private static void Init()
    {
        GetWindow<LightProbePlacement>().Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate probes") == true)
        {
            PlaceProbes();
        }

        m_MergeDistance = EditorGUILayout.FloatField("Vector merge distance", m_MergeDistance);
        m_LightProbeGroup = EditorHelper.ObjectField("Light Probe Group", m_LightProbeGroup);
        GUI.skin.label.wordWrap = true;
        EditorGUILayout.LabelField("This script will automatically generate light probe positions based on the current navmesh.");
        EditorGUILayout.LabelField("Please make sure that you have generated a navmesh before using the script.");
    }

    private void PlaceProbes()
    {
        // make sure gameobject is set 
        if (m_LightProbeGroup == null)
        {
            m_LightProbeGroup = FindObjectOfType<LightProbeGroup>();
        }
        if (m_LightProbeGroup == null)
        {
            EditorUtility.DisplayDialog("Error", "Probe object not set", "OK");
            return;
        }

        // grab nav mesh data
        var navMeshPoints = NavMesh.CalculateTriangulation().vertices.ToList();
        var mergedPoints = new List<Vector3>(m_LightProbeGroup.probePositions.Length);

        float sqrMerge = m_MergeDistance * m_MergeDistance;
        var probeTransform = m_LightProbeGroup.transform;

        // for each point on navmesh
        for (int i = 0; i < navMeshPoints.Count; ++i)
        {
            // add this point to nearby list and 'use' it
            var nearbyPoints = new List<Vector3> { navMeshPoints[i] };

            // find all other nearby points
            for (int j = i + 1; j < navMeshPoints.Count; ++j)
            {
                // if this point is nearby, add to nearby points and remove from total list
                if ((navMeshPoints[i] - navMeshPoints[j]).sqrMagnitude <= sqrMerge)
                {
                    nearbyPoints.Add(navMeshPoints[j]);
                    navMeshPoints.RemoveAt(j);
                    --j;
                }
            }

            // average nearby points
            var average = nearbyPoints.Aggregate((v1, v2) => v1 + v2) / nearbyPoints.Count;
            mergedPoints.Add(probeTransform.InverseTransformPoint(average));
        }

        // apply merged positions to probe group
        Undo.RecordObject(m_LightProbeGroup, "Applied Light Probe Positions");
        m_LightProbeGroup.probePositions = mergedPoints.ToArray();

        Debug.Log("Generated " + mergedPoints.Count + " light probes on (\"" + m_LightProbeGroup.name + "\")\n", m_LightProbeGroup);
    }
}

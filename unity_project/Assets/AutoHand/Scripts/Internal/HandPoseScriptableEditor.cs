#if UNITY_EDITOR
using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandPoseScriptable))]
public class HandPoseScriptableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (HandPoseScriptable)target;

        GUILayout.Label("ONLY MAKE LEFT HANDS");
        GUILayout.Space(10);

        if (script.leftPose.posePositions != null && script.leftPose.posePositions.Length == 20)
        {
            if (GUILayout.Button("Fix Right Hand", GUILayout.Height(40)))
            {
                script.rightPose.posePositions = new Vector3[script.leftPose.posePositions.Length];
                for (int i = 0; i < script.leftPose.posePositions.Length; i++)
                {
                    var left = script.leftPose.posePositions[i];
                    var right = script.rightPose.posePositions[i];
                    right = new Vector3(left.x * -1, left.y * -1, left.z * -1);
                    script.rightPose.posePositions[i] = right;
                }

                // straght copy
                script.rightPose.poseRotations = new Quaternion[script.leftPose.poseRotations.Length];
                for (int i = 0; i < script.leftPose.poseRotations.Length; i++)
                {
                    var left = script.leftPose.poseRotations[i];
                    var right = script.rightPose.poseRotations[i];
                    script.rightPose.poseRotations[i] = left;
                }


                if (Mathf.Abs(script.rightPose.handOffset.x) != Mathf.Abs(script.leftPose.handOffset.x) &&
                    Mathf.Abs(script.rightPose.handOffset.y) != Mathf.Abs(script.leftPose.handOffset.y) &&
                    Mathf.Abs(script.rightPose.handOffset.z) != Mathf.Abs(script.leftPose.handOffset.z))
                {
                    script.rightPose.handOffset = script.leftPose.handOffset;
                }
                script.rightPose.rotationOffset = script.leftPose.rotationOffset;
                script.rightPose.localQuaternionOffset = script.leftPose.localQuaternionOffset;

                EditorUtility.SetDirty(script);
            }
        }
        else
        {
            GUILayout.Label($"ADD TIP, Needs to be 20 NOT {script.leftPose.posePositions.Length}", GUILayout.Height(40));
        }

    }
}
#endif
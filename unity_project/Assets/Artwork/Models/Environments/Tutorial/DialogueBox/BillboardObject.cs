using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class BillboardObject : MonoBehaviour
{
    public Transform m_FollowTarget;
    public Transform m_LookTarget;

    // Update is called once per frame
    void Update()
    {
        Vector3 lookPosition = Vector3.zero;

        if (true == Application.isPlaying)
        {
            if (null != m_LookTarget)
            {
                lookPosition = m_LookTarget.position;
            }
        }
        else
        {
#if UNITY_EDITOR
            if (null != SceneView.currentDrawingSceneView)
            {
                lookPosition = SceneView.currentDrawingSceneView.camera.transform.position;
            }
            else
            {
                lookPosition = SceneView.lastActiveSceneView.camera.transform.position;
            }
#endif
        }

        if (null != m_FollowTarget)
        {
            transform.position = m_FollowTarget.position;
        }
        lookPosition.y = 0f;
        Vector3 position = transform.position;
        position.y = 0f;

        Vector3 direction = lookPosition - position;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
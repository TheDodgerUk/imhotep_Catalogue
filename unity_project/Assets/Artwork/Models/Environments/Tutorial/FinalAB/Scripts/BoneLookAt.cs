using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoneLookAt : MonoBehaviour
{
    public Transform NeckForwardTransform;
    public Transform Bone;
    public Vector3 offset = new Vector3(-90f, 90f, 0f);
    public Transform LookAtTarget;

    [SerializeField]
    public float normalisedLerp = 0f;

    public float m_fVeritcalLimit = 15f;
    public float m_fHorizontalLimit = 35f;

    public Vector3 vector;
    public float angleX;
    public float angleY;
    public float angleDiff;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(null != Bone)
        {            
            vector = (LookAtTarget.position - Bone.position).normalized;            
            Debug.DrawRay(Bone.position, vector, Color.blue);

            Vector3 angle = transform.InverseTransformDirection(vector);
            if (angle.z > 0f)
            {
                angleY = angle.x * Mathf.Rad2Deg;
                angleX = angle.y * Mathf.Rad2Deg;

                Quaternion lookRotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(-angleX, angleY, 0f);
                Debug.DrawRay(Bone.position, lookRotation * Vector3.forward, Color.green);

                Bone.rotation = lookRotation * Quaternion.Euler(offset);
            }
        }
    }
}
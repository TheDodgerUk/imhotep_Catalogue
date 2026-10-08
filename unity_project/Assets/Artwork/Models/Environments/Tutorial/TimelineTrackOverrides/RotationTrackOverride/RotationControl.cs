using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotationControl : MonoBehaviour
{
    public bool SetStartRotation;
    public Vector3 StartRotation;
    public Vector3 Axis;
    public float Speed;

    // Update is called once per frame
    void Update()
    {
        float dt = 0.15f;
        if (true == Application.isPlaying)
        {
            dt = Time.deltaTime;
        }

        transform.Rotate(Axis, Speed * Time.deltaTime, Space.Self);
    }

    public void SetRotationControl(bool setStartRotation, Vector3 startRotation, Vector3 rotation, float speed)
    {
        SetStartRotation = setStartRotation;
        StartRotation = startRotation;
        Axis = rotation;
        Speed = speed;

        if(true == SetStartRotation)
        {
            transform.localRotation = Quaternion.Euler(StartRotation);
        }
    }
}

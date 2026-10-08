using UnityEngine;
using UnityEngine.Playables;
using System;

[Serializable]
public class RotationControlBehaviour : PlayableBehaviour
{
    [Tooltip("Should the Start Rotation be applied")]
    [SerializeField]
    private bool SetRotation;

    [Tooltip("Rotation of the object if SetRotation is true")]
    [SerializeField]
    private Vector3 StartRotation;

    [Tooltip("Local axis this object will rotate around")]
    public Vector3 Axis;

    [Tooltip("Speed in degrees per second this will rotate")]
    public float Speed;  

    private bool bFirstFrameHappened = false;
    private RotationControl RotationControlRef = null;
    
    private Vector3 m_InitialRotation;
    private Vector3 m_AxisRotation;
    private float m_Speed;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);

        RotationControlRef = playerData as RotationControl;

        if (null != RotationControlRef)
        {
            if (false == bFirstFrameHappened)
            {
                m_InitialRotation = RotationControlRef.StartRotation;
                m_AxisRotation = RotationControlRef.Axis;
                m_Speed = RotationControlRef.Speed;

                bFirstFrameHappened = true;
            }

           // RotationControlRef.SetRotationControl(SetRotation, StartRotation, Axis, Speed);
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (null != RotationControlRef)
        {
            RotationControlRef.SetRotationControl(SetRotation, m_InitialRotation, m_AxisRotation, m_Speed);
            bFirstFrameHappened = false;
        }

        base.OnBehaviourPause(playable, info);
    }
}
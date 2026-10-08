using UnityEngine;
using UnityEngine.Playables;

public class RotationControlMixer : PlayableBehaviour
{
    private RotationControl rotationControl;
    private bool m_bFirstFrameHappened;

    private bool SetRotation;
    private Vector3 oldStartRotation;
    private Vector3 oldAxis;
    private float OldSpeed;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        rotationControl = playerData as RotationControl;

        if (null == rotationControl)
        {
            return;
        }

        if (false == m_bFirstFrameHappened)
        {
            SetRotation = rotationControl.SetStartRotation;
            oldStartRotation = rotationControl.transform.localRotation.eulerAngles;
            oldAxis = rotationControl.Axis;
            OldSpeed = rotationControl.Speed;
            
            m_bFirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount();
        float blendedSpeed = 0f;
        Vector3 blendedAxis = oldAxis;
        float totalWeight = 0f;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<RotationControlBehaviour> inputPlayable = (ScriptPlayable<RotationControlBehaviour>)playable.GetInput(i);
            RotationControlBehaviour behaviour = inputPlayable.GetBehaviour();
            
            blendedSpeed += behaviour.Speed * inputWeight;
            blendedAxis = Vector3.Lerp(blendedAxis, behaviour.Axis, inputWeight);
            totalWeight += inputWeight;
        }

        rotationControl.SetRotationControl(false, Vector3.zero, blendedAxis, blendedSpeed);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        m_bFirstFrameHappened = false;

        if (null == rotationControl)
            return;

        rotationControl.SetRotationControl(true, oldStartRotation, oldAxis, OldSpeed);
    }
}

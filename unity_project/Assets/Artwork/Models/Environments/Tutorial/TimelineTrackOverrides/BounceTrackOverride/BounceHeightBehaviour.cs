using UnityEngine;
using UnityEngine.Playables;
using System;

[Serializable]
public class BounceHeightBehaviour : PlayableBehaviour
{
    public float Speed;
    public float Strength;

    private bool bFirstFrameHappened = false;
    private BounceHeight m_BounceHeightRef = null;
    private float OldSpeed;
    private float OldStrength;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_BounceHeightRef = playerData as BounceHeight;

        if (null != m_BounceHeightRef)
        {
            if (false == bFirstFrameHappened)
            {
                OldSpeed = m_BounceHeightRef.m_BounceSpeed;
                OldStrength = m_BounceHeightRef.m_BounceStrength;
                bFirstFrameHappened = true;
            }

            m_BounceHeightRef.SetBounce(Speed, Strength);
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (null != m_BounceHeightRef)
        {
            m_BounceHeightRef.SetBounce(Speed, Strength);
        }

        base.OnBehaviourPause(playable, info);
    }
}

using UnityEngine;
using UnityEngine.Playables;

public class BounceHeightMixer : PlayableBehaviour
{
    private BounceHeight bounceHeight;
    private bool m_bFirstFrameHappened;

    private float OldStrength;
    private float OldSpeed;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        bounceHeight = playerData as BounceHeight;

        if(null == bounceHeight)
        {
            return;
        }

        if(false == m_bFirstFrameHappened)
        {
            OldSpeed = bounceHeight.m_BounceSpeed;
            OldStrength = bounceHeight.m_BounceStrength;
            m_bFirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount();
        float blendedStrength = 0f;
        float blendedSpeed = 0f;
        float totalWeight = 0f;

        for(int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<BounceHeightBehaviour> inputPlayable = (ScriptPlayable<BounceHeightBehaviour>)playable.GetInput(i);
            BounceHeightBehaviour behaviour = inputPlayable.GetBehaviour();

            blendedStrength += behaviour.Strength * inputWeight;
            blendedSpeed += behaviour.Speed * inputWeight;

            totalWeight += inputWeight;
        }

        bounceHeight.SetBounce(blendedSpeed, blendedStrength);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        m_bFirstFrameHappened = false;

        if (null == bounceHeight)
            return;

        bounceHeight.SetBounce(OldSpeed, OldStrength);
    }
}

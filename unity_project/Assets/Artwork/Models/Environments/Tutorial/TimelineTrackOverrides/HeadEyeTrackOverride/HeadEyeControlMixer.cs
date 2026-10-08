using UnityEngine;
using UnityEngine.Playables;

namespace AB
{
    public class HeadEyeControlMixer : PlayableBehaviour
    {
        private FaceUI faceUI;
        private bool m_bFirstFrameHappened;

        private float m_OldBlend;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            faceUI = playerData as FaceUI;

            if (null == faceUI)
            {
                return;
            }

            if (false == m_bFirstFrameHappened)
            {
                m_OldBlend = faceUI.m_EyeLookBlend;
                m_bFirstFrameHappened = true;
            }

            int inputCount = playable.GetInputCount();
            float blendedLook = 0f;
            float totalWeight = 0f;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<HeadEyeControlBehaviour> inputPlayable = (ScriptPlayable<HeadEyeControlBehaviour>)playable.GetInput(i);
                HeadEyeControlBehaviour behaviour = inputPlayable.GetBehaviour();

                blendedLook += behaviour.Blend * inputWeight;
                totalWeight += inputWeight;
            }

            faceUI.m_EyeLookBlend = blendedLook;
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            m_bFirstFrameHappened = false;

            if (null == faceUI)
                return;

            faceUI.m_EyeLookBlend = m_OldBlend;
        }
    }
}
using UnityEngine;
using UnityEngine.Playables;
using System;

namespace AB
{
    [Serializable]
    public class EmotionControlBehaviour : PlayableBehaviour
    {
        [SerializeField]
        private ABEmotion Emotion;

        private bool bFirstFrameHappened = false;
        private EyeEmotion m_EyeEmotionRef = null;
        private ABEmotion m_OldEmotion;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            m_EyeEmotionRef = playerData as EyeEmotion;

            if (null != m_EyeEmotionRef)
            {
                if (false == bFirstFrameHappened)
                {
                    m_OldEmotion = m_EyeEmotionRef.m_CurrentEyeEmotion;
                    bFirstFrameHappened = true;
                }

                m_EyeEmotionRef.SetEmotion(Emotion);
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (null != m_EyeEmotionRef)
            {
                m_EyeEmotionRef.SetEmotion(m_OldEmotion);
            }

            base.OnBehaviourPause(playable, info);
        }
    }
}
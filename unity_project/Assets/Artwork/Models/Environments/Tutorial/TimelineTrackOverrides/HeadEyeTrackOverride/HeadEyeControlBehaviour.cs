using UnityEngine;
using UnityEngine.Playables;
using System;

namespace AB
{
    [Serializable]
    public class HeadEyeControlBehaviour : PlayableBehaviour
    {
        [Range(0,1)]
        public float Blend;

        private bool bFirstFrameHappened = false;
        private FaceUI m_FaceUI = null;

        private float m_OldEyeBlend;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);

            m_FaceUI = playerData as FaceUI;

            if (null != m_FaceUI)
            {
                if (false == bFirstFrameHappened)
                {
                    m_OldEyeBlend = m_FaceUI.m_EyeLookBlend;

                    bFirstFrameHappened = true;
                }
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (null != m_FaceUI)
            {
                m_FaceUI.m_EyeLookBlend = m_OldEyeBlend;
                bFirstFrameHappened = false;
            }

            base.OnBehaviourPause(playable, info);
        }
    }
}

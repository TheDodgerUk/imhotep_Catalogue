using UnityEngine;
using UnityEngine.Playables;
using System;

namespace UI
{
    [Serializable]
    public class DialogueBoxBehaviour : PlayableBehaviour
    {
        [SerializeField]
        [TextArea(15, 20)]
        private string Text;

        private bool bFirstFrameHappened = false;
        private DialogueBoxControl m_DialogueBoxRef = null;
        private string m_OldText;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            m_DialogueBoxRef = playerData as DialogueBoxControl;

            if (null != m_DialogueBoxRef)
            {
                if (false == bFirstFrameHappened)
                {
                    m_OldText = m_DialogueBoxRef.m_Text;
                    bFirstFrameHappened = true;
                }

                m_DialogueBoxRef.SetText(Text);
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (null != m_DialogueBoxRef)
            {
                m_DialogueBoxRef.SetText(m_OldText);
            }

            base.OnBehaviourPause(playable, info);
        }
    }
}
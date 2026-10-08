using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentAnimation)]
public class ContentAnimationMetaData : MetaData
{
    public class StringItemData
    {
        [System.NonSerialized]
        public GameObject AnimationGameObject;

        [System.NonSerialized]
        public Animator AnimationAnimator;

        [System.NonSerialized]
        public Collider AnimationCollider;

        public string AnimationGameObjectName;
        public List<string> AnimationName = new List<string>();
    }
    public enum AnimationType
    {
        Trigger,
        Animation,
    }


    [SerializeField]

    public List<StringItemData> m_AnimationData;
    public AnimationType m_AnimationType = AnimationType.Animation;

    public ContentAnimationMetaData()
    {
        m_AnimationData = new List<StringItemData>();
    }
}
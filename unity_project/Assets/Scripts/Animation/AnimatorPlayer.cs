using System;
using System.Collections.Generic;
using UnityEngine;

public enum TransitionType { Enter, Exit };

[RequireComponent(typeof(Animator))]
public class AnimatorPlayer : MonoBehaviour
{
    private bool m_bInitialised;
    public bool IsEmpty => (m_Animator.runtimeAnimatorController == null);
    public Animator Animator => m_Animator;

    public string CurrentAnim { get; private set; }
    public bool IsPlaying { get; private set; }
    public bool m_IsLocked { get; private set; }
    public bool AllowRepeats { get; set; }

    [SerializeField]
    private bool m_DisableOnStart = true;

    private Dictionary<string, AnimationClip> m_Clips = new Dictionary<string, AnimationClip>();
    private Animator m_Animator;

    private void Awake()
    {
        if (false == m_bInitialised)
        {
            Init();
        }
    }

    private void Init()
    {
        m_Animator = GetComponent<Animator>();

        if (IsEmpty == true)
        {
            m_Clips.Clear();
        }
        else
        {
            var clips = m_Animator.runtimeAnimatorController.animationClips;
            var animationList = clips.ToList();
            for(int i = 0; i < animationList.Count; i++)
            {
                if(false == m_Clips.ContainsKey(animationList[i].name))
                {
                    m_Clips.Add(animationList[i].name, animationList[i]);
                }
            }
        }
        ToggleLocked(false);
        
        m_bInitialised = true;
        if (m_DisableOnStart == true)
        {
            StopAnimations();
        }
    }

    private void OnDisable()
    {
        StopAnimations();
    }

    public void ToggleEnabled(bool state)
    {
        if(false == m_bInitialised)
        {
            Init();
        }

        m_Animator.enabled = state;
        this.enabled = state;
    }

    public void ToggleLocked(bool state) => m_IsLocked = state;

    public void SnapToStart(TransitionType type) => SnapToStart(type.ToString());
    public void SnapToStart(string animation = null) => SnapTo(animation, 0f);

    public void SnapToEnd(TransitionType type) => SnapToEnd(type.ToString());
    public void SnapToEnd(string animation = null) => SnapTo(animation, 1f);

    public bool HasAnimation(TransitionType type) => HasAnimation(type.ToString());
    public bool HasAnimation(string animName) => m_Clips.ContainsKey(animName);

    public void Play(TransitionType type, Action callback = null) => Play(type.ToString(), callback);

    public void Play(string name, Action callback = null)
    {
        if (m_IsLocked == true)
        {
            return;
        }

        if (gameObject.activeInHierarchy == true && HasAnimation(name) == true)
        {
            if (IsPlaying == true && AllowRepeats == false && CurrentAnim == name)
            {
                return;
            }

            IsPlaying = true;
            CurrentAnim = name;
            ToggleEnabled(IsPlaying);

            m_Animator.CrossFade(name, 0.25f);
            m_Animator.Update(0f);

            var clip = m_Clips[name];
            this.StopAllCoroutines();
            this.WaitFor(clip.length, () =>
            {
                SnapToEnd();
                callback?.Invoke();
            });
        }
        else
        {
            StopAnimations();
            callback?.Invoke();
        }
    }

    public void StopAnimations()
    {
        this.StopAllCoroutines();

        IsPlaying = false;
        ToggleEnabled(IsPlaying);
    }

    private void SnapTo(string animation, float time)
    {
        if (gameObject.activeInHierarchy == true)
        {
            if (animation != null || IsPlaying == true)
            {
                animation = animation ?? CurrentAnim;

                if (HasAnimation(animation) == true)
                {
                    ToggleEnabled(true);
                    m_Animator.Play(animation, 0, time);
                    m_Animator.Update(0f);
                    StopAnimations();
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BounceHeight : MonoBehaviour
{
    public float m_BounceSpeed = 1.0f;
    [SerializeField]
    private float m_BounceHeight = 0.15f;
    public float m_BounceStrength = 1.0f;

    [SerializeField]
    private float m_Time = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (true == Application.isPlaying)
        {
            m_Time += Time.deltaTime * m_BounceSpeed;
        }
        else
        {
            m_Time += 0.15f * m_BounceSpeed;
        }

        float direction = Mathf.Sin(m_Time);

        Vector3 position = (Vector3.up * direction) * m_BounceHeight * m_BounceStrength;
        transform.localPosition = position;
    }

    public void SetBounce(float Speed, float Strength)
    {
        m_BounceSpeed = Speed;
        m_BounceStrength = Strength;
    }
}

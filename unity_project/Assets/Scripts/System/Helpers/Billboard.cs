using UnityEngine;

public class Billboard : ObjectTracker
{
    [SerializeField]
    private bool m_FlipFace = true;

    protected override void OnUpdate()
    {
        m_Transform.LookAt(m_Target, Vector3.up);

        if (m_FlipFace == true)
        {
            m_Transform.Rotate(0, 180, 0, Space.Self);
        }
    }
}

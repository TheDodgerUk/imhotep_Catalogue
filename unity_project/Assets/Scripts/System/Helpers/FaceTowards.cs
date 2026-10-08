using UnityEngine;

[ExecuteInEditMode]
public class FaceTowards : MonoBehaviour
{
    private enum TrackingMode
    {
        Target,
        Camera,
    };

    [SerializeField]
    private TrackingMode m_TrackingMode = TrackingMode.Target;
    [SerializeField]
    private Vector3 m_Offset = Vector3.zero;
    [SerializeField]
    private Transform m_Target = null;

    [SerializeField]
    private bool m_RuntimeOnly = true;

    private Transform m_Transform;

    private void Awake()
    {
        m_Transform = transform;
    }

    private void OnEnable()
    {
        if (m_TrackingMode == TrackingMode.Camera)
        {
            m_Target = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (m_RuntimeOnly == true && Application.isPlaying == false)
        {
            return;
        }

        if (m_Target != null && m_Transform.parent != null)
        {
            var up = m_Transform.parent.up;
            var distanceToPlane = Vector3.Dot(up, m_Target.position - m_Transform.position);
            var planePoint = m_Target.position - up * distanceToPlane;
            m_Transform.LookAt(planePoint, up);

            //m_Transform.LookAt(m_Target, m_Transform.parent.up);
            m_Transform.Rotate(m_Offset, Space.Self);
        }
    }
}

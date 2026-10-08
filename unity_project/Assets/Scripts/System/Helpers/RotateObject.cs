using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_Speed = Vector3.zero;
    [SerializeField]
    private Vector3 m_Lock = Vector3.zero;

    private Quaternion m_StartingRotation;
    private Transform m_Transform;
    private Vector3 m_Current = Vector3.zero;

    private void Awake()
    {
        m_Transform = transform;
        m_StartingRotation = m_Transform.localRotation;
    }

    private void OnEnable()
    {
        ResetRotation();
    }

    private void Update()
    {
        m_Current += (m_Speed * Time.deltaTime);
        var delta = Quaternion.Euler(m_Current);
        var rotation = m_StartingRotation * delta;
        m_Transform.localRotation = LockAngles(rotation);
    }

    public void ResetRotation() => m_Transform.localRotation = m_StartingRotation;

    public void SetSpeed(float x, float y, float z) => m_Speed = new Vector3(x, y, z);

    private Quaternion LockAngles(Quaternion rotation)
    {
        var euler = rotation.eulerAngles;
        if (m_Lock.x != 0)
        {
            euler.x = euler.x.RoundToNearest(m_Lock.x);
        }
        if (m_Lock.y != 0)
        {
            euler.y = euler.y.RoundToNearest(m_Lock.y);
        }
        if (m_Lock.z != 0)
        {
            euler.z = euler.z.RoundToNearest(m_Lock.z);
        }
        return Quaternion.Euler(euler);
    }
}

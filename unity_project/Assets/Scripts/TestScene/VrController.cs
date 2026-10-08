using UnityEngine;
using UnityEngine.XR;

public class VrController : MonoBehaviour
{
    private const int MaxLod = 4;

    [SerializeField]
    private XRNode m_Node = XRNode.LeftHand;

    private Transform m_SpawnPoint;
    private Transform m_Transform;
    private TextMesh m_TxtLod;

    private int m_LodIndex = 0;

    private void Awake()
    {
        m_Transform = transform;

        m_SpawnPoint = GameObject.Find("SpawnPoint")?.transform;

        m_TxtLod = m_Transform.FindComponent<TextMesh>("TxtLod");
        var txt = m_Transform.FindComponent<TextMesh>("Txt");
        txt.text = m_Node.ToString()[0].ToString();
    }

    private void Update()
    {
        m_Transform.localPosition = InputTracking.GetLocalPosition(m_Node);
        m_Transform.localRotation = InputTracking.GetLocalRotation(m_Node);

        if (m_Node == XRNode.LeftHand)
        {
            if (Input.GetButtonDown("LeftTrackPad") == true)
            {
                float h = Input.GetAxis("LeftTrackPadHorizontal");
                if (h.Abs() > 0.6f)
                {
                    ActivateLodModel((int)h.Sign());
                }
            }
        }
        else if (m_Node == XRNode.RightHand)
        {
            if (Input.GetButtonDown("RightTrackPad") == true)
            {
                float h = Input.GetAxis("RightTrackPadHorizontal");
                if (h.Abs() > 0.6f)
                {
                    if (h.Sign() > 0)
                    {
                        var rotates = m_SpawnPoint.GetComponentsInChildren<RotateObject>();
                        rotates.ToList().ForEach(r => r.enabled = !r.enabled);
                    }
                    else
                    {
                        ToggleHolograms();
                    }
                }
            }
        }
    }

    private void ToggleHolograms()
    {
        foreach (Transform child in m_SpawnPoint)
        {
            var hologram = child.Find("Hologram")?.gameObject;
            if (hologram != null)
            {
                hologram.SetActive(!hologram.activeSelf);
                ForceLodToCurrent(hologram.GetComponentInChildren<LODGroup>());
            }
        }
    }

    private void ActivateLodModel(int delta)
    {
        m_LodIndex = (m_LodIndex + delta).Wrap(MaxLod);
        m_TxtLod.text = $"LOD\n{m_LodIndex}";

        ForceLodToCurrent(m_SpawnPoint.GetComponentsInChildren<LODGroup>(true));
    }

    private void ForceLodToCurrent(params LODGroup[] lodGroups)
    {
        foreach (var group in lodGroups)
        {
            if (m_LodIndex < group.lodCount)
            {
                group.ForceLOD(m_LodIndex);
            }
        }
    }
}

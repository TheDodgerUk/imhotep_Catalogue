using UnityEngine;

public class ScaleBounds : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_Size = Vector3.zero;

    private Transform m_Transform;

    private void Awake()
    {
        m_Transform = transform;
    }

    private void OnDrawGizmosSelected()
    {
        var matrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, m_Size);
        Gizmos.matrix = matrix;
    }

    public void FitInside(Transform target, bool uniformScaling = true)
    {
        FitInside(target, m_Transform, m_Size, uniformScaling);
    }

    public static void FitInside(Transform target, Transform destination, Vector3 size, bool uniformScaling = true)
    {
        // reset to position
        var parent = target.parent;
        target.SetParent(null, true);
        target.localScale = Vector3.one;
        target.rotation = destination.rotation;
        target.position = destination.position;

        var combinedBounds = GetBounds(target);
        var totalSize = combinedBounds.size;
        var deltaSize = totalSize - size;

        float scale = 1f;
        float max = Mathf.Max(deltaSize.x, deltaSize.y, deltaSize.z);
        for (int i = 0; i < 3; ++i)
        {
            if (max == deltaSize[i])
            {
                scale = size[i] / totalSize[i];
                break;
            }
        }

        var deltaPos = destination.position - combinedBounds.center;
        target.Translate(deltaPos * scale);
        target.localScale = (uniformScaling ? (Vector3.one * scale) : size);
        target.SetParent(parent, true);
    }

    public static Bounds GetBounds(Transform parent)
    {
        if (parent != null)
        {
            var renderers = parent.GetComponentsInChildren<Renderer>().ToList();
            renderers.RemoveAll(r => r.CompareTag("IgnoreBounds"));

            if (renderers.Count > 0)
            {
                var combinedBounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Count; i++)
                {
                    combinedBounds.Encapsulate(renderers[i].bounds);
                }
                return combinedBounds;
            }
            else
            {
                Debug.Log("Requested bounds on model with no renderers: " + parent.name);
            }
        }
        return new Bounds();
    }
}

using UnityEngine;

[ExecuteAlways]
public class ShowObjectGizmos : MonoBehaviour
{
    [Header("Gizmo Settings")]
    public bool showGizmos = true;
    public bool showBounds = true;
    public bool showPosition = true;
    public bool showForward = true;
    public bool showName = true;

    [Header("Appearance")]
    public Color gizmoColor = Color.green;
    public float positionRadius = 0.1f;
    public float forwardLength = 1f;

    [Header("Bounds")]
    public bool includeChildren = false;

    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        Gizmos.color = gizmoColor;

        if (showPosition)
        {
            Gizmos.DrawWireSphere(
                transform.position,
                positionRadius
            );
        }

        if (showForward)
        {
            Gizmos.DrawLine(
                transform.position,
                transform.position +
                transform.forward * forwardLength
            );
        }

        if (showBounds)
        {
            DrawBounds();
        }

#if UNITY_EDITOR
        if (showName)
        {
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 0.2f,
                gameObject.name
            );
        }
#endif
    }

    private void DrawBounds()
    {
        Collider[] colliders = includeChildren
            ? GetComponentsInChildren<Collider>(true)
            : GetComponents<Collider>();

        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                if (collider == null)
                    continue;

                Gizmos.DrawWireCube(
                    collider.bounds.center,
                    collider.bounds.size
                );
            }

            return;
        }

        Renderer[] renderers = includeChildren
            ? GetComponentsInChildren<Renderer>(true)
            : GetComponents<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
                continue;

            Gizmos.DrawWireCube(
                renderer.bounds.center,
                renderer.bounds.size
            );
        }
    }
}
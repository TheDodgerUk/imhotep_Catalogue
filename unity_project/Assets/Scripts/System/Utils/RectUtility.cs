using UnityEngine;

public static class RectUtility
{
    private const float ColliderOffset = 0f;
    private const float ColliderDepth = 32f;

    public static void AlignTransformToRect(Transform transform)
    {
        var rect = GetRect(transform.parent.ToRect());
        transform.SetScale(rect.width, rect.height);
        transform.SetLocalPos(rect.x, rect.y);
    }

    public static void AlignColliderToRect(BoxCollider box, bool resizeDepth = true)
    {
        if (box == null)
        {
            return;
        }

        var transform = box.GetRect();
        if (transform == null)
        {
            return;
        }

        var rect = GetRect(transform);

        float centerZ = resizeDepth ? ColliderOffset : box.center.z;
        float sizeZ = resizeDepth ? ColliderDepth : box.size.z;
        box.size = new Vector3(rect.width, rect.height, sizeZ);
        box.center = new Vector3(rect.x, rect.y, centerZ);
    }

    private static Rect GetRect(RectTransform transform)
    {
        var corners = new Vector3[4];
        transform.GetLocalCorners(corners);

        float x = 0, y = 0;
        for (int i = 0; i < corners.Length; ++i)
        {
            x += corners[i].x;
            y += corners[i].y;
        }
        x *= (1f / corners.Length);
        y *= (1f / corners.Length);

        float w = transform.rect.size.x;
        float h = transform.rect.size.y;
        return new Rect(x, y, w, h);
    }

    public static void CentreTransformToCollider(BoxCollider boxCollider)
    {
#if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(boxCollider.transform, "Centre BoxCollider");
        UnityEditor.Undo.RecordObject(boxCollider, "Centre BoxCollider");
#endif
        boxCollider.transform.localPosition += boxCollider.center;
        boxCollider.center = Vector3.zero;
    }
}
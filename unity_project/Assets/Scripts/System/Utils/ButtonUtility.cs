using UnityEngine;

public static class ButtonUtility
{
    private const float ColliderOffset = ColliderDepth * 0.5f;
    private const float ColliderDepth = 100f;

    public static void AlignColliderToButton(Transform transform)
    {
        var rect = transform.GetComponent<RectTransform>();
        var box = transform.GetComponent<BoxCollider>();

        if (rect == null || box == null)
        {
            return;
        }

        var corners = new Vector3[4];
        rect.GetLocalCorners(corners);

        float x = 0, y = 0;
        for (int i = 0; i < corners.Length; ++i)
        {
            x += corners[i].x;
            y += corners[i].y;
        }
        x *= (1f / corners.Length);
        y *= (1f / corners.Length);

        float w = rect.rect.size.x;
        float h = rect.rect.size.y;

        box.center = new Vector3(x, y, ColliderOffset);
        box.size = new Vector3(w, h, ColliderDepth);
    }
}
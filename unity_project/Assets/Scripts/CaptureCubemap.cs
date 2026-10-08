#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CaptureCubemap : MonoBehaviour
{
    [SerializeField]
    private RenderTexture m_Cubemap;

    [InspectorButton]
    private void Capture()
    {
#if UNITY_EDITOR
        var camera = GetComponent<Camera>();
        camera.RenderToCubemap(m_Cubemap);

        var equirect = new RenderTexture(m_Cubemap);
        equirect.Create();

        m_Cubemap.ConvertToEquirect(equirect);

        SaveAs(equirect, "_EXR.exr", TextureFormat.RGBAFloat, t => t.EncodeToEXR());
    }

    private void SaveAs(RenderTexture rt, string extension, TextureFormat format, System.Func<Texture2D, byte[]> encoder)
    {
        RenderTexture.active = rt;
        var texture = new Texture2D(rt.width, rt.height, format, false);
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

        RenderTexture.active = null;

        var bytes = encoder(texture);

        string path = AssetDatabase.GetAssetPath(m_Cubemap).Strip(".asset");
        System.IO.File.WriteAllBytes(path + extension, bytes);
#endif
    }
}

using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GLRender : MonoBehaviour
{
    private static List<GLRender> m_Instances = new List<GLRender>();

    [SerializeField, HideInInspector]
    private Material m_MatDepthOn;
    [SerializeField, HideInInspector]
    private Material m_MatDepthOff;

    [System.NonSerialized]
    private GLRenderQueue m_Lines = new GLRenderQueue(GL.LINES);
    [System.NonSerialized]
    private GLRenderQueue m_Quads = new GLRenderQueue(GL.QUADS);
    [System.NonSerialized]
    private List<int> m_Ids = new List<int>();

    private void Awake()
    {
        m_MatDepthOn = new Material(Shader.Find("Unlit/DebugLine")) { hideFlags = HideFlags.HideAndDontSave };
        m_MatDepthOff = new Material(Shader.Find("Unlit/DebugLine")) { hideFlags = HideFlags.HideAndDontSave };
        m_MatDepthOn.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
        m_MatDepthOff.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
    }

    private void OnEnable()
    {
        m_Instances.Add(this);
    }

    private void OnDisable()
    {
        m_Instances.Remove(this);
    }

    private void Update()
    {
        m_Lines.ManualUpdate();
        m_Quads.ManualUpdate();
    }

    private void OnPostRender()
    {
        m_MatDepthOn.SetPass(0);
        m_Quads.DrawDepthOn();
        m_Lines.DrawDepthOn();

        m_MatDepthOff.SetPass(0);
        m_Quads.DrawDepthOff();
        m_Lines.DrawDepthOff();
    }

    private int RequestIdInternal(int id)
    {
        const int MaxId = 9999999;
        for (int i = 0; i < MaxId; ++i)
        {
            if (m_Ids.Contains(i) == false)
            {
                return i;
            }
        }
        return id;
    }

    public static int RequestId(int id)
    {
        if (m_Instances.Count > 0)
        {
            id = m_Instances.First().RequestIdInternal(id);
            m_Instances.ForEach(gl => gl.m_Ids.Add(id));
        }
        return id;
    }

    public static bool HasId(int id)
    {
        if (m_Instances.Count == 0)
        {
            return true;
        }
        return m_Instances.First().m_Ids.Contains(id);
    }

    public static void ClearLines(int id)
    {
        foreach (var instance in m_Instances)
        {
            instance.m_Lines.ClearId(id);
            instance.m_Quads.ClearId(id);
            instance.m_Ids.Remove(id);
        }
    }

    public static void DrawLine(GLLine line, bool depthTest) => m_Instances.ForEach(gl => gl.m_Lines.AddShape(line, depthTest));

    public static void DrawQuads(GLQuad quad, bool depthTest) => m_Instances.ForEach(gl => gl.m_Quads.AddShape(quad, depthTest));
}
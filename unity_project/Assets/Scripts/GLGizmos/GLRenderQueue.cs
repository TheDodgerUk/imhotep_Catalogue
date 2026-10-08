using System.Collections.Generic;
using UnityEngine;

public class GLRenderQueue
{
    private List<GLShape> m_DepthOn = new List<GLShape>();
    private List<GLShape> m_DepthOff = new List<GLShape>();

    private int m_GlMode = 0;

    public GLRenderQueue(int glMode)
    {
        m_GlMode = glMode;
    }

    public void AddShape(GLShape shape, bool depthTest)
    {
        if (depthTest == true)
        {
            m_DepthOn.Add(shape);
        }
        else
        {
            m_DepthOff.Add(shape);
        }
    }

    public void ClearId(int id)
    {
        m_DepthOn.RemoveAll(s => s.Id == id);
        m_DepthOff.RemoveAll(s => s.Id == id);
    }

    public void ManualUpdate()
    {
        m_DepthOn.RemoveAll(l => l.HasElapsed);
        m_DepthOff.RemoveAll(l => l.HasElapsed);
    }

    public void DrawDepthOn() => Draw(m_DepthOn);
    public void DrawDepthOff() => Draw(m_DepthOff);

    private void Draw(List<GLShape> shapes)
    {
        GL.Begin(m_GlMode);
        foreach (var shape in shapes)
        {
            shape.Draw();
        }
        GL.End();
    }
}
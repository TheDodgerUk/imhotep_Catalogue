using UnityEngine;

[System.Serializable]
public class GLShape
{
    public bool HasElapsed => Time.time >= m_EndTime;
    public int Id { get; private set; }

    private float m_EndTime;
    private Color m_Color;

    public GLShape(int id, Color color, float duration)
    {
        Id = id;
        m_Color = color;
        m_EndTime = float.MaxValue;
    }

    public virtual void Draw()
    {
        GL.Color(m_Color);
    }
}

[System.Serializable]
public class GLLine : GLShape
{
    private Vector3 m_Start;
    private Vector3 m_End;

    public GLLine(int id, Vector3 start, Vector3 end, Color color, float duration) : base(id, color, duration)
    {
        m_Start = start;
        m_End = end;
    }

    public override void Draw()
    {
        base.Draw();

        GL.Vertex(m_Start);
        GL.Vertex(m_End);
    }
}

[System.Serializable]
public class GLQuad : GLShape
{
    private readonly Vector3[] m_Vertices;

    public GLQuad(int id, Vector3[] vertices, Color color, float duration) : base(id, color, duration)
    {
        m_Vertices = vertices;
    }

    public override void Draw()
    {
        base.Draw();

        foreach (var v in m_Vertices)
        {
            GL.Vertex(v);
        }
    }
}
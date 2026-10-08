using System.Collections.Generic;
using UnityEngine;

public static class GLDebug
{
    public static Matrix4x4 Matrix = Matrix4x4.identity;

    public class Scope : System.IDisposable
    {
        private Matrix4x4 m_Matrix;

        public Scope(ref int id, Matrix4x4 matrix)
        {
            m_Matrix = Matrix;
            Matrix = matrix;
            id = Begin(id);
        }

        public void Dispose()
        {
            Matrix = m_Matrix;
        }
    }

    public static int Begin(int id)
    {
        GLRender.ClearLines(id);
        return GLRender.RequestId(id);
    }

    public static bool HasId(int id) => GLRender.HasId(id);

    public static void DrawLine(int id, Vector3 start, Vector3 end, Color? color = null, float duration = 0, bool depthTest = false)
    {
        if (start == end)
        {
            return;
        }
        end = Matrix.MultiplyPoint3x4(end);
        start = Matrix.MultiplyPoint3x4(start);
        GLRender.DrawLine(new GLLine(id, start, end, color ?? Color.white, duration), depthTest);
    }

    public static void DrawQuads(int id, Vector3[] verts, Color? color = null, float duration = 0, bool depthTest = false)
    {
        if ((verts.Length % 4) != 0)
        {
            return;
        }

        for (int i = 0; i < verts.Length; ++i)
        {
            verts[i] = Matrix.MultiplyPoint3x4(verts[i]);
        }
        GLRender.DrawQuads(new GLQuad(id, verts, color ?? Color.white, duration), depthTest);
    }

    public static void DrawRay(int id, Vector3 start, Vector3 dir, Color? color = null, float duration = 0, bool depthTest = false)
    {
        if (dir != Vector3.zero)
        {
            DrawLine(id, start, start + dir, color, duration, depthTest);
        }
    }

    public static void DrawLineArrow(int id, Vector3 start, Vector3 end, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20, Color? color = null, float duration = 0, bool depthTest = false)
    {
        DrawArrow(id, start, end - start, arrowHeadLength, arrowHeadAngle, color, duration, depthTest);
    }

    public static void DrawArrow(int id, Vector3 start, Vector3 dir, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20, Color? color = null, float duration = 0, bool depthTest = false)
    {
        if (dir != Vector3.zero)
        {
            DrawRay(id, start, dir, color, duration, depthTest);
            Vector3 right = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
            Vector3 left = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
            DrawRay(id, start + dir, right * arrowHeadLength, color, duration, depthTest);
            DrawRay(id, start + dir, left * arrowHeadLength, color, duration, depthTest);
        }
    }

    public static void DrawSquare(int id, Vector3 position, Quaternion? rotation = null, Vector3? size = null, Color? color = null, float duration = 0, bool depthTest = false)
    {
        DrawSquare(id, Matrix4x4.TRS(position, rotation ?? Quaternion.identity, size ?? Vector3.one), color, duration, depthTest);
    }
    public static void DrawSquare(int id, Matrix4x4 matrix, Color? color = null, float duration = 0, bool depthTest = false)
    {
        var p_1 = matrix.MultiplyPoint3x4(new Vector3(0.5f, 0, 0.5f));
        var p_2 = matrix.MultiplyPoint3x4(new Vector3(0.5f, 0, -0.5f));
        var p_3 = matrix.MultiplyPoint3x4(new Vector3(-0.5f, 0, -0.5f));
        var p_4 = matrix.MultiplyPoint3x4(new Vector3(-0.5f, 0, 0.5f));

        DrawLine(id, p_1, p_2, color, duration, depthTest);
        DrawLine(id, p_2, p_3, color, duration, depthTest);
        DrawLine(id, p_3, p_4, color, duration, depthTest);
        DrawLine(id, p_4, p_1, color, duration, depthTest);
    }

    public static void DrawWireSphere(int id, Vector3 origin, float radius, Color? color = null, float duration = 0, bool depthTest = false)
    {
        const int theta = 16;
        const int phi = 16;

        for (int y = -1; y < theta; ++y)
        {
            for (int x = 0; x < phi; ++x)
            {
                var a = GetPoint(x, y + 0, theta, phi, radius, origin);
                var b = GetPoint(x, y + 1, theta, phi, radius, origin);
                DrawLine(id, a, b, color, duration, depthTest);
            }
        }
        for (int x = 0; x < phi; ++x)
        {
            for (int y = 0; y < theta - 1; ++y)
            {
                var a = GetPoint(x + 0, y, theta, phi, radius, origin);
                var b = GetPoint(x + 1, y, theta, phi, radius, origin);
                DrawLine(id, a, b, color, duration, depthTest);
            }
        }
    }

    public static void DrawSphere(int id, Vector3 origin, float radius, Color? color = null, float duration = 0, bool depthTest = false)
    {
        const int theta = 20;
        const int phi = 20;

        var northPole = new Vector3(0, radius, 0) + origin;
        var southPole = new Vector3(0, -radius, 0) + origin;

        var verts = new List<Vector3>();

        for (int x = 0; x < phi; ++x)
        {
            verts.Add(northPole);
            verts.Add(northPole);
            verts.Add(GetPoint(x + 0, 0, theta, phi, radius, origin));
            verts.Add(GetPoint(x + 1, 0, theta, phi, radius, origin));
        }
        for (int y = 1; y < theta; ++y)
        {
            for (int x = 0; x < phi; ++x)
            {
                verts.Add(GetPoint(x + 1, y - 1, theta, phi, radius, origin));
                verts.Add(GetPoint(x + 0, y - 1, theta, phi, radius, origin));
                verts.Add(GetPoint(x + 0, y + 0, theta, phi, radius, origin));
                verts.Add(GetPoint(x + 1, y + 0, theta, phi, radius, origin));
            }
        }
        for (int x = 0; x < phi; ++x)
        {
            verts.Add(GetPoint(x + 1, theta - 1, theta, phi, radius, origin));
            verts.Add(GetPoint(x + 0, theta - 1, theta, phi, radius, origin));
            verts.Add(southPole);
            verts.Add(southPole);
        }

        DrawQuads(id, verts.ToArray(), color, duration, depthTest);
    }

    private static Vector3 GetPoint(int x, int y, int theta, int phi, float radius, Vector3 origin)
    {
        var v = new Vector3(0, radius, 0);
        v = Quaternion.AngleAxis(180f / (theta + 1) * (y + 1), Vector3.forward) * v;
        v = Quaternion.AngleAxis(360f / phi * x, Vector3.up) * v;
        return v + origin;
    }

    public static void DrawCube(int id, Vector3 position, Quaternion? rotation = null, Vector3? size = null, Color? color = null, float duration = 0, bool depthTest = false)
    {
        DrawCube(id, Matrix4x4.TRS(position, rotation ?? Quaternion.identity, size ?? Vector3.one), color, duration, depthTest);
    }
    public static void DrawCube(int id, Matrix4x4 matrix, Color? color = null, float duration = 0, bool depthTest = false)
    {
        var verts = new[]
        {
            // top
            matrix.MultiplyPoint3x4(new Vector3( 0.5f, 0.5f,-0.5f)),
            matrix.MultiplyPoint3x4(new Vector3(-0.5f, 0.5f,-0.5f)),
            matrix.MultiplyPoint3x4(new Vector3(-0.5f, 0.5f, 0.5f)),
            matrix.MultiplyPoint3x4(new Vector3( 0.5f, 0.5f, 0.5f)),
            // bottom
            matrix.MultiplyPoint3x4(new Vector3( 0.5f,-0.5f, 0.5f)),
            matrix.MultiplyPoint3x4(new Vector3(-0.5f,-0.5f, 0.5f)),
            matrix.MultiplyPoint3x4(new Vector3(-0.5f,-0.5f,-0.5f)),
            matrix.MultiplyPoint3x4(new Vector3( 0.5f,-0.5f,-0.5f)),
            // front
            matrix.MultiplyPoint3x4(new Vector3( 0.5f, 0.5f, 0.5f)),
            matrix.MultiplyPoint3x4(new Vector3(-0.5f, 0.5f, 0.5f)),
            matrix.MultiplyPoint3x4(new Vector3(-0.5f,-0.5f, 0.5f)),
            matrix.MultiplyPoint3x4(new Vector3( 0.5f,-0.5f, 0.5f)),
            // back
            matrix.MultiplyPoint3x4(new Vector3( 0.5f,-0.5f,-0.5f)),
            matrix.MultiplyPoint3x4(new Vector3(-0.5f,-0.5f,-0.5f)),
            matrix.MultiplyPoint3x4(new Vector3(-0.5f, 0.5f,-0.5f)),
            matrix.MultiplyPoint3x4(new Vector3( 0.5f, 0.5f,-0.5f)),
            // left
            matrix.MultiplyPoint3x4(new Vector3(-0.5f, 0.5f, 0.5f)),
            matrix.MultiplyPoint3x4(new Vector3(-0.5f, 0.5f,-0.5f)),
            matrix.MultiplyPoint3x4(new Vector3(-0.5f,-0.5f,-0.5f)),
            matrix.MultiplyPoint3x4(new Vector3(-0.5f,-0.5f, 0.5f)),
            // right
            matrix.MultiplyPoint3x4(new Vector3( 0.5f, 0.5f,-0.5f)),
            matrix.MultiplyPoint3x4(new Vector3( 0.5f, 0.5f, 0.5f)),
            matrix.MultiplyPoint3x4(new Vector3( 0.5f,-0.5f, 0.5f)),
            matrix.MultiplyPoint3x4(new Vector3( 0.5f,-0.5f,-0.5f)),
        };

        DrawQuads(id, verts, color, duration, depthTest);
    }

    public static void DrawWireCube(int id, Vector3 position, Quaternion? rotation = null, Vector3? size = null, Color? color = null, float duration = 0, bool depthTest = false)
    {
        DrawWireCube(id, Matrix4x4.TRS(position, rotation ?? Quaternion.identity, size ?? Vector3.one), color, duration, depthTest);
    }
    public static void DrawWireCube(int id, Matrix4x4 matrix, Color? color = null, float duration = 0, bool depthTest = false)
    {
        var down_1 = matrix.MultiplyPoint3x4(new Vector3(0.5f, -0.5f, 0.5f));
        var down_2 = matrix.MultiplyPoint3x4(new Vector3(0.5f, -0.5f, -0.5f));
        var down_3 = matrix.MultiplyPoint3x4(new Vector3(-0.5f, -0.5f, -0.5f));
        var down_4 = matrix.MultiplyPoint3x4(new Vector3(-0.5f, -0.5f, 0.5f));
        var up_1 = matrix.MultiplyPoint3x4(new Vector3(0.5f, 0.5f, 0.5f));
        var up_2 = matrix.MultiplyPoint3x4(new Vector3(0.5f, 0.5f, -0.5f));
        var up_3 = matrix.MultiplyPoint3x4(new Vector3(-0.5f, 0.5f, -0.5f));
        var up_4 = matrix.MultiplyPoint3x4(new Vector3(-0.5f, 0.5f, 0.5f));

        DrawLine(id, down_1, down_2, color, duration, depthTest);
        DrawLine(id, down_2, down_3, color, duration, depthTest);
        DrawLine(id, down_3, down_4, color, duration, depthTest);
        DrawLine(id, down_4, down_1, color, duration, depthTest);

        DrawLine(id, down_1, up_1, color, duration, depthTest);
        DrawLine(id, down_2, up_2, color, duration, depthTest);
        DrawLine(id, down_3, up_3, color, duration, depthTest);
        DrawLine(id, down_4, up_4, color, duration, depthTest);

        DrawLine(id, up_1, up_2, color, duration, depthTest);
        DrawLine(id, up_2, up_3, color, duration, depthTest);
        DrawLine(id, up_3, up_4, color, duration, depthTest);
        DrawLine(id, up_4, up_1, color, duration, depthTest);
    }
}
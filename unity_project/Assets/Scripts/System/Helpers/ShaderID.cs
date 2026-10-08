using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShaderID
{
    public static readonly int EMISSION_COLOR = Shader.PropertyToID("_EmissionColor");
    public static readonly int MAIN_TEXTURE = Shader.PropertyToID("_MainTex");
    public static readonly int COLOR = Shader.PropertyToID("_Color");
    public static readonly int OFFSET = Shader.PropertyToID("_Offset");
}

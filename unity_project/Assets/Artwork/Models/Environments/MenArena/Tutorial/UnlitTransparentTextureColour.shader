
Shader "Unlit/Transparent Texture Colour"
{
    Properties 
	{
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _MainTex ("MainTex", 2D) = "white" {}

        _SrcBlend ("SrcBlend", Int) = 5 // SrcAlpha
        _DstBlend ("DstBlend", Int) = 10 // OneMinusSrcAlpha
        _ZWrite ("ZWrite", Int) = 0 // Off
        _ZTest ("ZTest", Int) = 4 // LEqual
        _Cull ("Cull", Int) = 2 // Back

		_Factor ("Offset Factor", Float) = 0
		_Units ("Offset Units", Float) = 0
    }
    SubShader
	{
        Tags
		{
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass
		{
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Cull [_Cull]
			Offset [_Factor], [_Units]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 2.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            struct VertexInput
			{
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput
			{
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v)
			{
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR
			{
                float4 mainTex = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 col = _Color.rgb * mainTex.rgb;
                return fixed4(col, _Color.a * mainTex.a);
            }
            ENDCG
        }
    }
    CustomEditor "UnlitTransparentTextureColourEditor"
}

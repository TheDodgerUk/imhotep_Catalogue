Shader "Unlit/Fog"
{
	Properties
	{
		_Distance ("Distance", Range(0, 100)) = 1.0
		_Color ("Colour", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Geometry"
		}
		LOD 100
		
        Blend SrcAlpha OneMinusSrcAlpha
		Cull Front
		ZTest LEqual
		ZWrite Off
		
        GrabPass { }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
                float4 projPos : TEXCOORD0;
			};

            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _GrabTexture;
            uniform float _Distance;
			uniform float4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.projPos = ComputeScreenPos(o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);
				return o;
			}
			
			
			float3 rgb2hsv(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = c.g < c.b ? float4(c.bg, K.wz) : float4(c.gb, K.xy);
				float4 q = c.r < p.x ? float4(p.xyw, c.r) : float4(c.r, p.yzx);

				float e = 1.0e-10;
				float d = q.x - min(q.w, q.y);
				return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}
			
			float3 hsv2rgb(float3 c)
			{
				float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
				return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float texDepth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos));
				float eyeDepth = LinearEyeDepth(UNITY_SAMPLE_DEPTH(texDepth));
                float sceneZ = max(0, eyeDepth - _ProjectionParams.g);

                float objectZ = max(0, i.projPos.z - _ProjectionParams.g);
				float alpha = saturate((sceneZ - objectZ) / _Distance);

				// get scene colour
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 source = tex2D(_GrabTexture, sceneUVs.rg);

				return lerp(source, _Color, alpha);
			}
			ENDCG
		}
	}
}

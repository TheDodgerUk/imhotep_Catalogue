Shader "Unlit/AudioVis01"
{
	Properties
	{
		_Color("Main Color", Color) = (0.066,.1,.3,1)
		_MainTex ("Texture", 2D) = "white" {}
		[HDR]_Emission("Emission", Range(1.0, 10)) = 1.5
		_Waves("Wave Count", Range(0.5, 10)) = 1.5
		_Scale("Scale", Range(0.1, 200)) = 6.0
		_Speed("Speed", Range(0.1, 10)) = 6.0
		_Offset("Line Offset", Range(1, 40)) = 20.0
		_FXBlend01("Blend01", Range(0.0, 1)) = 0
		_FX01Strength("FX01 Strength", Range(0.01, 1)) = .1
		_FXBlend02("Blend02", Range(0.0, 1)) = 0
		_FXBlend03("Blend03", Range(0.0, 1)) = 0
			_Spectrum("Spectrum", Vector) = (10,0,0,0)
	} 
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			uniform float4 _Spectrum;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			fixed _Emission;
			fixed _Scale;
			fixed _Speed;
			fixed _FX01Strength;
			fixed _FXBlend01;
			fixed _FXBlend02;
			fixed _FXBlend03;
			fixed _Waves;
			fixed _Offset;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed2 uv = fixed2(i.uv.x - 0.25, i.uv.y - 0.5) * 10;
				fixed2 uv2 = 1.0 - abs(uv) * 1.0;
				fixed2 finalUv = lerp(uv, uv2, _FXBlend03);
				float3 col = float3(0.0, 0.0, 0.0);
				for (float j = 0; j < 13.0; j++)
				{
					finalUv.y += sin(j * _Offset + 5.0 + _Time.y * _Speed + finalUv.x * _Waves) * _Spectrum.y * _Scale;
					finalUv.y = lerp(finalUv.y, finalUv.y = cos(finalUv.y), _FXBlend01);
					col.r += abs(_Color.r / finalUv.y) * _Spectrum.y;
					col.g += abs(_Color.g / finalUv.y) * _Spectrum.y;
					col.b += abs(_Color.b / finalUv.y) * _Spectrum.y;
				}

				float y = (finalUv.y * 0.1) + 0.5;
				col = lerp(col, 1.0 - col, _FXBlend02);
				col = col * y;
				col = col * (1 - y);
				return float4(col.r, col.g, col.b, 1) * _Emission;
			}
			ENDCG
		}
	}
}

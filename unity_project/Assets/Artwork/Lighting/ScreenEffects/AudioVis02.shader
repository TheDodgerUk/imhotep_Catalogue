Shader "Unlit/AudioVis02"
{
	Properties
	{
		_Color("Main Color", Color) = (0.066,.1,.3,1)
		_MainTex ("Texture", 2D) = "white" {}
		[HDR]_Emission("Emission", Range(1.0, 10)) = 1.5
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
				fixed2 uv = fixed2(i.uv.x, i.uv.y);
				float fft = tex2D(_MainTex, uv).x;
				float3 col = float3(_Color.r, _Color.g, _Color.b) / abs(fft - uv.x);
				col = col * uv.y;
				col = col * (1.0f - uv.y);
				return float4(col.r, col.g, col.b, 1) * _Emission;
			}
			ENDCG
		}
	}
}

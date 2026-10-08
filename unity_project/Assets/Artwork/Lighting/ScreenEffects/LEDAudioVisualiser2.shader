Shader "Custom/LEDAudioVisualiser2" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Brightness("Brightness", Range(0,4)) = 1
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Unlit

		#pragma target 2.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};
		half _Brightness;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		float mod(float x, float y)
		{
			return x - y * floor(x / y);
		}

		const float bands = 20.0;
		const float leds = 25.0;
		const float colorRange = 0.3; // >0. (=1. full color range, >1. repeat colors)

		float3 hsv2rgb(float3 c) {
			float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
			float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
			return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex + 0.5) * _Brightness;
			float2 uv = IN.uv_MainTex;
			float dis = distance(float2(0.0,0.0), uv - 0.5);

			float disA = floor(dis * leds) / leds;

			float deg = acos(uv.y / dis) / 3.14;

			float degA = floor(deg*bands) / bands;

			float3 color = hsv2rgb(float3(degA*colorRange + _Time.w * 0.07, 1.0, smoothstep(0.0, 0.6, disA)));

			float bandBrightness = c.r;

			color *= bandBrightness*bandBrightness;
			/*color *= 4;

			float deltaDeg = frac((deg - degA)*bands) - 0.5;
			float deltaDis = frac((dis - disA)*leds) - 0.5;

			float shape = smoothstep(0.5, 0.35, abs(deltaDeg)) *
				smoothstep(0.5, 0.35, abs(deltaDis));*/

			//color *= shape;

			o.Albedo = color;
			o.Emission = color;
			o.Alpha = 1.0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

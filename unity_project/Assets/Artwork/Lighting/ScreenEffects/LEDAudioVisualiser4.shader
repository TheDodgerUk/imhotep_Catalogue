Shader "Custom/LEDAudioVisualiser4" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Gradient("Gradient", 2D) = "white" {}
		_Height("Height", Range(0.25,4)) = 1
		_GridX("GridX", Range(10,80)) = 1
		_GridY("GridY", Range(10,80)) = 1
		_RShift("VPos", Range(0.01,1)) = 0.01
		_WaveNum("WaveNumber", Range(1,4)) = 1
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#pragma surface surf Unlit

#pragma target 2.0

		sampler2D _MainTex;
		uniform sampler2D _Gradient;

		struct Input
		{
			float2 uv_MainTex;
		};

		half _Height;
		half _GridX;
		half _GridY;
		half _RShift;
		half _WaveNum;
		fixed4 _Color;

		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
		{
			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		float mod(float x, float y)
		{
			return x - y * floor(x / y);
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex + 0.5) * _Brightness;
			half2 uv = IN.uv_MainTex;
			uv.y -= _RShift;

			// read frequency data from first row of texture
			float fft = pow(tex2D(_MainTex, float2(uv.x * _WaveNum, 0.0)).x * _Height, float3(1.0 - _RShift,1.0 - _RShift,1.0 - _RShift));

			// led color		
			float3 color = tex2D(_Gradient, uv);

			float mask = (abs(uv.y) < fft) ? 1.0 : 0.015;

			float3 ledColor = color * mask;

			o.Albedo = ledColor;
			o.Emission = ledColor;
			o.Alpha = 1.0;
		}
	ENDCG
	}
		FallBack "Diffuse"
}

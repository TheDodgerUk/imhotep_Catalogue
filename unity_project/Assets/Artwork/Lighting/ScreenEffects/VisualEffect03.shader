Shader "Custom/LEDAudioVisualiser3" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (1,1,1,1)
		_TopColor("Top Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Height("Height", Range(0.25,4)) = 1
		_GridX("GridX", Range(10,80)) = 1
		_GridY("GridY", Range(10,80)) = 1
		_RShift("GridY", Range(0.01,1)) = 0.01
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#pragma surface surf Unlit

#pragma target 2.0

		sampler2D _MainTex;

	struct Input
	{
		float2 uv_MainTex;
	};

	half _Height;
	half _GridX;
	half _GridY;
	half _RShift;
	fixed4 _Color;
	fixed4 _BottomColor;
	fixed4 _TopColor;

	half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
	{
		half4 c;
		c.rgb = s.Albedo;
		c.a = s.Alpha;
		return c;
	}

	void surf(Input IN, inout SurfaceOutput o)
	{
		// Albedo comes from a texture tinted by color
		//fixed4 c = tex2D (_MainTex, IN.uv_MainTex + 0.5) * _Brightness;
		// quantize coordinates
		//const float bands = _GridX;
		//const float segs = _GridY;
		half2 p = IN.uv_MainTex;
		p.y -= _RShift;
		//p.x = floor(IN.uv_MainTex.x*bands) / bands;
		//p.y = floor(IN.uv_MainTex.y*segs) / segs;

		// read frequency data from first row of texture
		float fft = pow(tex2D(_MainTex, float2(p.x, 0.0)).x * _Height, float3(1.0 - _RShift,1.0 - _RShift,1.0 - _RShift));

		// led color
		float3 color = lerp(_BottomColor, _TopColor, sqrt(IN.uv_MainTex.x));

		float mask = (p.y < fft) ? 1.0 : 0.05;

		// led shape
		float2 d = frac((IN.uv_MainTex - p) *p) - 0.5;
		float led = smoothstep(0.5, 0.35, abs(d.x)) *
			smoothstep(0.5, 0.35, abs(d.y));
		float3 ledColor = color * mask;
		
		o.Albedo = ledColor;
		o.Emission = ledColor;
		o.Alpha = 1.0;
	}
	ENDCG
	}
		FallBack "Diffuse"
}

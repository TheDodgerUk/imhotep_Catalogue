Shader "Custom/LEDAudioVisualiser1" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_WrapAmount("Wrap Number", Range(1,4)) = 2
		_Brightness("Strength", Range(0,4)) = 1
		_Saturation("Shadow Brightness", Range(0,1)) = 1
		_RShift("RShift", Range(0,1)) = 1
		_GShift("GShift", Range(0,1)) = 1
		_BShift("BShift", Range(0,1)) = 1
		_Height("Height", Range(0,4)) = 1
		_EmissionPower ("Emission", Range(0,4)) = 0.5
		_GridX("GridX", Range(10,80)) = 1
		_GridY("GridY", Range(10,80)) = 1
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
		half _WrapAmount;
		half _Brightness;
		half _Saturation;
		half _RShift;
		half _GShift;
		half _BShift;
		half _Height;
		half _EmissionPower;
		half _GridX;
		half _GridY;
		fixed4 _Color;

		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		float3 B2_spline(float3 x) { // returns 3 B-spline functions of degree 2
			float3 t = 3.0 * x;
			float3 b0 = step(0.0, t)     * step(0.0, 1.0 - t);
			float3 b1 = step(0.0, t - 1.0) * step(0.0, 2.0 - t);
			float3 b2 = step(0.0, t - 2.0) * step(0.0, 3.0 - t);
			return 0.5 * (
				b0 * pow(t, float3(2.0, 2.0, 2.0)) +
				b1 * (-2.0*pow(t, float3(2.0, 2.0, 2.0)) + 6.0*t - 3.0) +
				b2 * pow(3.0 - t, float3(2.0, 2.0, 2.0))
				);
		}

		float mod(float x, float y)
		{
			return x - y * floor(x / y);
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float2 uv = IN.uv_MainTex;
			// quantize coordinates
			float fVBars = _GridY / _WrapAmount;

			float x = floor(uv.x * fVBars) / fVBars;
			float fSample = tex2D(_MainTex, float2(abs(_WrapAmount * x - 1.0), 0.25)).x * _Brightness;
			float squarewave = sign(mod(uv.x, 1.0 / fVBars) - 0.008);
			float fft = squarewave * fSample * _Height;
			
			float fHBars = _GridX;
			float fVSpacing = 0.25;
			float fVFreq = (uv.y * 3.14);
			fVFreq = sign(sin(fVFreq * fHBars) + 1.0 - fVSpacing);

			float2 centered = float2(1.0, 1.0) * uv - float2(1.0, 1.0);
			float t = _Time.x;
			float polychrome = 1.0*uv.x - t;
			float3 spline_args = frac(float3(polychrome, polychrome, polychrome) + float3(0.0, -1.0 / 3.0, -2.0 / 3.0));
			float3 spline = B2_spline(spline_args);

			float f = abs(centered.y);
			float3 base_color = (float3(1.0, 1.0, 1.0) * _Saturation) - f*spline;
			float3 flame_color = pow(base_color, float3(3.0, 3.0, 3.0));

			float tt = 0.3 - uv.y;
			float df = sign(tt);
			df = (df + 1.0) / 0.5;
			float3 outCol1 = float3(1.0 - step(fft, abs(_RShift - uv.y)), 1.0 - step(fft, abs(_GShift - uv.y)), 1.0 - step(fft, abs(_BShift - uv.y)));
			float3 col = flame_color * outCol1 * float3(fVFreq, fVFreq, fVFreq);
			col -= col * df * 0.180;

			o.Albedo = col;
			o.Emission = col * _EmissionPower;
			o.Alpha = 1.0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

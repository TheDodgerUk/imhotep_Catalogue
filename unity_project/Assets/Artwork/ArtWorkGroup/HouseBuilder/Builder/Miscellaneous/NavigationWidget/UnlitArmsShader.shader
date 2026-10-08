// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/UnlitArmsShader"
{

    Properties
    {
        _FirstTex ("Texture", 2D) = "white" {}
		_SecondTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Angle("Angle", Range(-5.0,  5.0)) = 0.0
	}
    SubShader
    {
        //Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            struct v2f
            {
				float2 uv1 : TEXCOORD1;
                float2 uv : TEXCOORD0;			
				float4 vertex : SV_POSITION;
			};

            sampler2D _FirstTex;
			float4 _FirstTex_ST;
            sampler2D _SecondTex;
			float4 _SecondTex_ST;
			
			float4 _Color;
			float4 _BlackColor= float4(0, 0, 0, 1);

			float _Angle;

            v2f vert (appdata v)
            {
                v2f o;
				
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = TRANSFORM_TEX(v.uv, _FirstTex);
				o.uv = TRANSFORM_TEX(v.uv, _SecondTex);

                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				float f = sin(_Time * 20);

				fixed4 col = (f >=0 ) ?   tex2D(_FirstTex, i.uv): tex2D(_SecondTex, i.uv);
				if (col.a == 0)
				{
					col = _Color;
				}
				else 
				{
					col = lerp(_Color, _BlackColor,abs(f));
				}
                return col;
            }
            ENDCG
        }
    }
}

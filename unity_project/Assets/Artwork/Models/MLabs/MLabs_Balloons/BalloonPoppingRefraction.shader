Shader "Unlit/BalloonPoppingRefraction"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Radius("Radius", Range(-1,1)) = 0.2
		_Amplitude("Amplitude", Range(-10,10)) = 0.05
		_Wavesize("WaveSize", Range(0, 1.0)) = 0.2
    }
    SubShader
    {
		// Queue is important! this object must be rendered after
		// Opaque objects.
        Tags {"RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100
		
		// Grab the screen behind the object into _BackgroundTexture
		GrabPass{ }
        
		Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

			float _Radius;
			float _Amplitude;
			float _Wavesize;

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float4 screenUV : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
				o.screenUV = ComputeGrabScreenPos(o.vertex);
                return o;
            }

			sampler2D _GrabTexture;

            fixed4 frag (v2f i) : SV_Target
            {

                //fixed4 col = tex2D(_MainTex, i.uv);
				//fixed4 grab = tex2Dproj(_GrabTexture, i.screenUV * float4(0.8, 0.8, 1, 1));
				
				fixed4 grab = tex2Dproj(_GrabTexture, i.screenUV + float4(sin((_Time.x * 10) + i.screenUV.x * 32.0) * 0.1, 0, 0, 0));
				
				
				//fixed4 grab = tex2Dproj(_GrabTexture, i.screenUV + float4(sin(i.screenUV.x * 32.0) * 0.1, 0, 0, 0));
				//fixed4 grab = tex2Dproj(_GrabTexture, i.screenUV + float4(sin(i.screenUV.x * 16.0) * 0.1, 0, 0, 0));
				
                
				//return frac(i.screenUV * 16.0);
                return grab;
            }
            ENDCG
        }
    }
}

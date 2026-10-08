Shader "Unlit/Chaperone"
{
    Properties {
        _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NoiseTex("Dissolve Noise", 2D) = "white"{} 
        _NScale ("Noise Scale", Range(0, 10)) = 1 
        _DisAmount("Noise Texture Opacity", Range(0.01, 1)) = 0 
        _Radius("Radius", Range(0, 20)) = 0          
        [Toggle(ALPHA)] _ALPHA("No Shadows on Transparent", Float) = 0
          
    }
 
        SubShader{
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 200
			Cull Off
          
           
        Blend SrcAlpha OneMinusSrcAlpha // transparency
CGPROGRAM


 
#pragma shader_feature LIGHTMAP
#pragma surface surf Unlit alpha addshadow// transparency
 
sampler2D _Ramp;
 
// custom lighting function that uses a texture ramp based
// on angle between light direction and normal
#pragma lighting Unlit exclude_path:prepass
inline half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten)
{   
    half4 c;
    c.rgb = s.Albedo;
    //c.a = 0; we don't want the alpha
    c.a = s.Alpha; // use the alpha of the surface output
    return c;
}

uniform float3 _Position;
uniform float _ChaperoneArrayLength;
uniform float3 _ChaperoneInfluencers[100];
float _Radius;
 
sampler2D _MainTex;
float4 _Color;
sampler2D _NoiseTex;//
float _DisAmount, _NScale;//

 
struct Input {
    float2 uv_MainTex : TEXCOORD0;
    float3 worldPos;// built in value to use the world space position
	float3 worldNormal;
   
};
 
void surf (Input IN, inout SurfaceOutput o) {
    half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

// triplanar noise
	 float3 blendNormal = saturate(pow(IN.worldNormal * 1.4,4));
    half4 nSide1 = tex2D(_NoiseTex, (IN.worldPos.xy + _Time.x) * _NScale); 
	half4 nSide2 = tex2D(_NoiseTex, (IN.worldPos.xz + _Time.x) * _NScale);
	half4 nTop = tex2D(_NoiseTex, (IN.worldPos.yz + _Time.x) * _NScale);

	float3 noisetexture = nSide1;
    noisetexture = lerp(noisetexture, nTop, blendNormal.x);
    noisetexture = lerp(noisetexture, nSide2, blendNormal.y);

	// distance influencer position to world position and sphere radius
	float3 sphereRNoise;
	for  (int i = 0; i < _ChaperoneArrayLength; i++){
		float3 dis =  distance(_ChaperoneInfluencers[i], IN.worldPos);
		float3 sphereR = 1.0 - saturate(dis / _Radius);
		sphereR *= noisetexture;
		sphereRNoise += (sphereR);
	}
	
    float alpha =  (1.0 - (step(sphereRNoise, _DisAmount))) * c.a * _Color.a;
    o.Albedo = _Color;
    o.Alpha = alpha;
}
ENDCG
 
    }
 
    Fallback "Diffuse"
}
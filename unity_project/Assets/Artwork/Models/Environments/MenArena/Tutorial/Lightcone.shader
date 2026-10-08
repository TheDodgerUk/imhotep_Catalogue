// Shader created with Shader Forge v1.41 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.41;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32921,y:32713,varname:node_3138,prsc:2|emission-4704-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32186,y:32811,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_ViewVector,id:7047,x:31554,y:33129,varname:node_7047,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:4049,x:31554,y:32971,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:1168,x:31787,y:33109,varname:node_1168,prsc:2,dt:0|A-4049-OUT,B-7047-OUT;n:type:ShaderForge.SFN_Multiply,id:2459,x:32488,y:32977,varname:node_2459,prsc:2|A-7241-A,B-1971-V,C-3486-OUT,D-7508-OUT;n:type:ShaderForge.SFN_TexCoord,id:1971,x:32186,y:32957,varname:node_1971,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Power,id:3486,x:32186,y:33109,varname:node_3486,prsc:2|VAL-3432-OUT,EXP-8569-OUT;n:type:ShaderForge.SFN_Slider,id:8569,x:31819,y:33284,ptovrint:False,ptlb:Falloff,ptin:_Falloff,varname:node_8569,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:12;n:type:ShaderForge.SFN_Fresnel,id:7582,x:31976,y:33393,varname:node_7582,prsc:2|NRM-2469-OUT,EXP-2281-OUT;n:type:ShaderForge.SFN_NormalVector,id:2469,x:31779,y:33393,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:2281,x:31622,y:33577,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:node_2281,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:8;n:type:ShaderForge.SFN_OneMinus,id:7508,x:32181,y:33393,varname:node_7508,prsc:2|IN-7582-OUT;n:type:ShaderForge.SFN_Multiply,id:4704,x:32666,y:32813,varname:node_4704,prsc:2|A-7241-RGB,B-2459-OUT,C-2684-OUT;n:type:ShaderForge.SFN_Clamp01,id:3432,x:31976,y:33109,varname:node_3432,prsc:2|IN-1168-OUT;n:type:ShaderForge.SFN_Slider,id:2684,x:32373,y:33245,ptovrint:False,ptlb:Intensity,ptin:_Intensity,varname:node_2684,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;proporder:7241-8569-2281-2684;pass:END;sub:END;*/

Shader "Shader Forge/LightCone" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Falloff ("Falloff", Range(1, 12)) = 1
        _Fresnel ("Fresnel", Range(0, 8)) = 1
        _Intensity ("Intensity", Range(0, 10)) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #ifndef UNITY_PASS_FORWARDBASE
            #define UNITY_PASS_FORWARDBASE
            #endif //UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _Falloff;
            uniform float _Fresnel;
            uniform float _Intensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float3 emissive = (_Color.rgb*(_Color.a*i.uv0.g*pow(saturate(dot(i.normalDir,viewDirection)),_Falloff)*(1.0 - pow(1.0-max(0,dot(i.normalDir, viewDirection)),_Fresnel)))*_Intensity);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

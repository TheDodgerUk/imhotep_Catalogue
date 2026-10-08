// Shader created with Shader Forge v1.41 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.41;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.015,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32989,y:32698,varname:node_3138,prsc:2|emission-7968-OUT,alpha-4166-OUT;n:type:ShaderForge.SFN_Tex2d,id:4114,x:32041,y:32830,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_4114,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:883,x:32446,y:33012,ptovrint:False,ptlb:Ramp,ptin:_Ramp,varname:node_883,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-19-OUT;n:type:ShaderForge.SFN_TexCoord,id:7024,x:32041,y:33019,varname:node_7024,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Append,id:19,x:32226,y:33029,varname:node_19,prsc:2|A-4114-R,B-7024-V;n:type:ShaderForge.SFN_RemapRange,id:2423,x:32288,y:32764,varname:node_2423,prsc:2,frmn:0,frmx:1,tomn:-0.15,tomx:4|IN-4114-R;n:type:ShaderForge.SFN_Clamp01,id:4166,x:32473,y:32764,varname:node_4166,prsc:2|IN-2423-OUT;n:type:ShaderForge.SFN_Slider,id:5789,x:32419,y:33235,ptovrint:False,ptlb:Brightness,ptin:_Brightness,varname:node_5789,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.25,cur:1.4,max:1.4;n:type:ShaderForge.SFN_Multiply,id:7968,x:32671,y:32996,varname:node_7968,prsc:2|A-883-RGB,B-5789-OUT;proporder:4114-883-5789;pass:END;sub:END;*/

Shader "Shader Forge/WeightDistribution" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Ramp ("Ramp", 2D) = "white" {}
        _Brightness ("Brightness", Range(0.25, 1.4)) = 1.4
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            Blend SrcAlpha OneMinusSrcAlpha
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
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Ramp; uniform float4 _Ramp_ST;
            uniform float _Brightness;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float2 node_19 = float2(_MainTex_var.r,i.uv0.g);
                float4 _Ramp_var = tex2D(_Ramp,TRANSFORM_TEX(node_19, _Ramp));
                float3 emissive = (_Ramp_var.rgb*_Brightness);
                float3 finalColor = emissive;
                return fixed4(finalColor,saturate((_MainTex_var.r*4.15+-0.15)));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

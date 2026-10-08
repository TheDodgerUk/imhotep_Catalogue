// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:33974,y:32484,varname:node_3138,prsc:2|emission-7677-OUT;n:type:ShaderForge.SFN_Tex2d,id:1627,x:32916,y:32594,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_1627,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:181ec605fb3b4314bbee61db18ab41c0,ntxv:0,isnm:False|UVIN-530-OUT;n:type:ShaderForge.SFN_Multiply,id:7417,x:33352,y:32777,varname:node_7417,prsc:2|A-5346-OUT,B-2346-RGB;n:type:ShaderForge.SFN_TexCoord,id:7107,x:31932,y:32803,varname:node_7107,prsc:2,uv:0;n:type:ShaderForge.SFN_ArcTan2,id:7623,x:32695,y:32880,varname:node_7623,prsc:2,attp:2|A-8439-G,B-8439-R;n:type:ShaderForge.SFN_ComponentMask,id:8439,x:32398,y:32897,varname:node_8439,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-3832-OUT;n:type:ShaderForge.SFN_RemapRange,id:3832,x:32232,y:32932,varname:node_3832,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-7107-UVOUT;n:type:ShaderForge.SFN_Append,id:530,x:32682,y:32594,varname:node_530,prsc:2|A-7623-OUT,B-5333-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5333,x:32398,y:32685,ptovrint:False,ptlb:V,ptin:_V,varname:node_5333,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:7677,x:33550,y:32642,varname:node_7677,prsc:2|A-4400-RGB,B-7417-OUT;n:type:ShaderForge.SFN_Multiply,id:5346,x:33104,y:32843,varname:node_5346,prsc:2|A-1627-R,B-5918-OUT;n:type:ShaderForge.SFN_Slider,id:5918,x:32184,y:32795,ptovrint:False,ptlb:Brightness,ptin:_Brightness,varname:node_5918,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3.711897,max:10;n:type:ShaderForge.SFN_Tex2d,id:2346,x:33095,y:33082,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_2346,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6a985dfcd1d3ca94e9753748ed65c697,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4400,x:32963,y:32238,ptovrint:False,ptlb:Gradient,ptin:_Gradient,varname:node_4400,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:271f5ee3273dd7f4fae6e204d4f8c4bf,ntxv:0,isnm:False|UVIN-530-OUT;proporder:1627-5333-5918-2346-4400;pass:END;sub:END;*/

Shader "Shader Forge/VisualEffect01" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _V ("V", Float ) = 0.5
        _Brightness ("Brightness", Range(0, 10)) = 3.711897
        _Mask ("Mask", 2D) = "white" {}
        _Gradient ("Gradient", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _V;
            uniform float _Brightness;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform sampler2D _Gradient; uniform float4 _Gradient_ST;
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
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_8439 = (i.uv0*2.0+-1.0).rg;
                float2 node_530 = float2(((atan2(node_8439.g,node_8439.r)/6.28318530718)+0.5),_V);
                float4 _Gradient_var = tex2D(_Gradient,TRANSFORM_TEX(node_530, _Gradient));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_530, _MainTex));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 emissive = (_Gradient_var.rgb*((_MainTex_var.r*_Brightness)*_Mask_var.rgb));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33190,y:32777,varname:node_3138,prsc:2|emission-2979-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32414,y:32633,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4288,x:32414,y:32791,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_4288,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2979,x:32732,y:32837,varname:node_2979,prsc:2|A-7241-RGB,B-4288-RGB,C-8019-OUT,D-1224-OUT,E-4982-OUT;n:type:ShaderForge.SFN_Slider,id:916,x:31928,y:32973,ptovrint:False,ptlb:Distance,ptin:_Distance,varname:node_916,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.04645741,max:5;n:type:ShaderForge.SFN_Clamp01,id:8019,x:32414,y:32946,varname:node_8019,prsc:2|IN-1275-OUT;n:type:ShaderForge.SFN_DepthBlend,id:1275,x:32244,y:32946,varname:node_1275,prsc:2|DIST-916-OUT;n:type:ShaderForge.SFN_Time,id:5639,x:31759,y:33600,varname:node_5639,prsc:2;n:type:ShaderForge.SFN_Slider,id:1063,x:31602,y:33506,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_1063,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Multiply,id:6779,x:31989,y:33559,varname:node_6779,prsc:2|A-1063-OUT,B-5639-TSL;n:type:ShaderForge.SFN_FragmentPosition,id:3395,x:31989,y:33739,varname:node_3395,prsc:2;n:type:ShaderForge.SFN_Add,id:8324,x:32241,y:33644,varname:node_8324,prsc:2|A-6779-OUT,B-3395-XYZ;n:type:ShaderForge.SFN_Slider,id:6192,x:32115,y:33481,ptovrint:False,ptlb:Scale,ptin:_Scale,varname:node_6192,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:8349,x:32445,y:33603,varname:node_8349,prsc:2|A-6192-OUT,B-8324-OUT;n:type:ShaderForge.SFN_ComponentMask,id:4831,x:32676,y:33616,varname:node_4831,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-8349-OUT;n:type:ShaderForge.SFN_Add,id:3234,x:32935,y:33556,varname:node_3234,prsc:2|A-4831-R,B-4831-G;n:type:ShaderForge.SFN_Add,id:2659,x:32935,y:33722,varname:node_2659,prsc:2|A-4831-B,B-4831-G;n:type:ShaderForge.SFN_Append,id:7049,x:33125,y:33688,varname:node_7049,prsc:2|A-3234-OUT,B-2659-OUT;n:type:ShaderForge.SFN_Tex2d,id:1514,x:33339,y:33688,ptovrint:False,ptlb:NoiseTex,ptin:_NoiseTex,varname:node_1514,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-7049-OUT;n:type:ShaderForge.SFN_Set,id:704,x:33549,y:33688,varname:Volumetric,prsc:2|IN-1514-R;n:type:ShaderForge.SFN_Get,id:1224,x:32414,y:33090,varname:node_1224,prsc:2|IN-704-OUT;n:type:ShaderForge.SFN_Slider,id:4982,x:32278,y:33165,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_4982,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:3;proporder:7241-4288-916-1063-6192-1514-4982;pass:END;sub:END;*/

Shader "Shader Forge/StageLaser" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Distance ("Distance", Range(0, 5)) = 0.04645741
        _Speed ("Speed", Range(0, 5)) = 0
        _Scale ("Scale", Range(0, 1)) = 0
        _NoiseTex ("NoiseTex", 2D) = "white" {}
        _Emission ("Emission", Range(0, 3)) = 1
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Distance;
            uniform float _Speed;
            uniform float _Scale;
            uniform sampler2D _NoiseTex; uniform float4 _NoiseTex_ST;
            uniform float _Emission;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 projPos : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_5639 = _Time;
                float3 node_4831 = (_Scale*((_Speed*node_5639.r)+i.posWorld.rgb)).rgb;
                float2 node_7049 = float2((node_4831.r+node_4831.g),(node_4831.b+node_4831.g));
                float4 _NoiseTex_var = tex2D(_NoiseTex,TRANSFORM_TEX(node_7049, _NoiseTex));
                float Volumetric = _NoiseTex_var.r;
                float3 emissive = (_Color.rgb*_MainTex_var.rgb*saturate(saturate((sceneZ-partZ)/_Distance))*Volumetric*_Emission);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

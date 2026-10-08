// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.015,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33286,y:32727,varname:node_3138,prsc:2|emission-3025-OUT;n:type:ShaderForge.SFN_TexCoord,id:1367,x:30151,y:33398,varname:node_1367,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Length,id:9739,x:31449,y:33258,varname:node_9739,prsc:2|IN-5516-OUT;n:type:ShaderForge.SFN_OneMinus,id:6088,x:32083,y:33333,varname:node_6088,prsc:2|IN-6382-OUT;n:type:ShaderForge.SFN_Clamp01,id:9472,x:32264,y:33333,varname:node_9472,prsc:2|IN-6088-OUT;n:type:ShaderForge.SFN_Multiply,id:1713,x:30555,y:33281,varname:node_1713,prsc:2|A-402-X,B-1367-U;n:type:ShaderForge.SFN_Frac,id:7853,x:31045,y:33165,varname:node_7853,prsc:2|IN-9985-OUT;n:type:ShaderForge.SFN_Add,id:5516,x:31269,y:33258,varname:node_5516,prsc:2|A-7853-OUT,B-5480-OUT;n:type:ShaderForge.SFN_Vector1,id:5480,x:31031,y:33319,varname:node_5480,prsc:2,v1:-0.5;n:type:ShaderForge.SFN_Multiply,id:6382,x:31877,y:33333,varname:node_6382,prsc:2|A-9739-OUT,B-6822-OUT;n:type:ShaderForge.SFN_Vector1,id:6822,x:31621,y:33408,varname:node_6822,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Slider,id:2317,x:32477,y:32686,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_2317,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.589744,max:3;n:type:ShaderForge.SFN_Multiply,id:3025,x:32935,y:32820,varname:node_3025,prsc:2|A-2317-OUT,B-2787-OUT;n:type:ShaderForge.SFN_Append,id:9985,x:30772,y:33359,varname:node_9985,prsc:2|A-1713-OUT,B-9820-OUT;n:type:ShaderForge.SFN_Multiply,id:9820,x:30555,y:33426,varname:node_9820,prsc:2|A-402-Y,B-1367-V;n:type:ShaderForge.SFN_Vector4Property,id:402,x:30040,y:33307,ptovrint:False,ptlb:Tiling,ptin:_Tiling,varname:node_402,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:25,v2:10,v3:0,v4:0;n:type:ShaderForge.SFN_ViewPosition,id:628,x:32048,y:33546,varname:node_628,prsc:2;n:type:ShaderForge.SFN_Distance,id:1384,x:32290,y:33630,varname:node_1384,prsc:2|A-628-XYZ,B-1132-XYZ;n:type:ShaderForge.SFN_FragmentPosition,id:1132,x:32045,y:33690,varname:node_1132,prsc:2;n:type:ShaderForge.SFN_Slider,id:4766,x:32133,y:33851,ptovrint:False,ptlb:DistanceFade,ptin:_DistanceFade,varname:node_4766,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:50,max:50;n:type:ShaderForge.SFN_Divide,id:6341,x:32498,y:33661,varname:node_6341,prsc:2|A-1384-OUT,B-4766-OUT;n:type:ShaderForge.SFN_Clamp01,id:3711,x:32663,y:33661,varname:node_3711,prsc:2|IN-6341-OUT;n:type:ShaderForge.SFN_OneMinus,id:441,x:32850,y:33682,varname:node_441,prsc:2|IN-3711-OUT;n:type:ShaderForge.SFN_Lerp,id:2787,x:32660,y:33036,varname:node_2787,prsc:2|A-4746-OUT,B-2731-OUT,T-441-OUT;n:type:ShaderForge.SFN_Multiply,id:2731,x:32354,y:33032,varname:node_2731,prsc:2|A-4746-OUT,B-9472-OUT;n:type:ShaderForge.SFN_TexCoord,id:3354,x:28891,y:31694,varname:node_3354,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_RemapRange,id:1442,x:29533,y:31689,varname:node_1442,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-3354-UVOUT;n:type:ShaderForge.SFN_Length,id:5690,x:29717,y:31689,varname:node_5690,prsc:2|IN-1442-OUT;n:type:ShaderForge.SFN_Slider,id:3191,x:29560,y:31578,ptovrint:False,ptlb:LEDs,ptin:_LEDs,varname:node_3191,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:25,max:50;n:type:ShaderForge.SFN_Multiply,id:857,x:29902,y:31641,varname:node_857,prsc:2|A-3191-OUT,B-5690-OUT;n:type:ShaderForge.SFN_Floor,id:3484,x:30071,y:31641,varname:node_3484,prsc:2|IN-857-OUT;n:type:ShaderForge.SFN_Divide,id:9312,x:30247,y:31641,varname:node_9312,prsc:2|A-3484-OUT,B-3191-OUT;n:type:ShaderForge.SFN_ArcCos,id:3371,x:31172,y:31848,varname:node_3371,prsc:2|IN-6914-OUT;n:type:ShaderForge.SFN_Divide,id:8551,x:30790,y:31848,varname:node_8551,prsc:2|A-4792-OUT,B-8850-OUT;n:type:ShaderForge.SFN_Slider,id:7745,x:30950,y:32308,ptovrint:False,ptlb:Bands,ptin:_Bands,varname:node_7745,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Divide,id:718,x:31366,y:31883,varname:node_718,prsc:2|A-3371-OUT,B-7745-OUT;n:type:ShaderForge.SFN_Multiply,id:4746,x:32112,y:32821,varname:node_4746,prsc:2|A-8347-OUT,B-7657-OUT;n:type:ShaderForge.SFN_HsvToRgb,id:8347,x:31890,y:32821,varname:node_8347,prsc:2|H-7657-OUT,S-6593-OUT,V-8471-OUT;n:type:ShaderForge.SFN_Set,id:2177,x:31557,y:31919,varname:DegA,prsc:2|IN-718-OUT;n:type:ShaderForge.SFN_Get,id:7657,x:31649,y:33036,varname:node_7657,prsc:2|IN-2177-OUT;n:type:ShaderForge.SFN_Vector1,id:6593,x:31649,y:32821,varname:node_6593,prsc:2,v1:1;n:type:ShaderForge.SFN_Smoothstep,id:8471,x:31649,y:32895,varname:node_8471,prsc:2|A-5099-OUT,B-8656-OUT,V-5362-OUT;n:type:ShaderForge.SFN_Vector1,id:5099,x:31454,y:32862,varname:node_5099,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:8656,x:31454,y:32915,varname:node_8656,prsc:2,v1:0.6;n:type:ShaderForge.SFN_Set,id:489,x:30432,y:31641,varname:DistA,prsc:2|IN-9312-OUT;n:type:ShaderForge.SFN_Set,id:9947,x:29533,y:31854,varname:UVV,prsc:2|IN-3354-V;n:type:ShaderForge.SFN_Get,id:8850,x:30581,y:31904,varname:node_8850,prsc:2|IN-9947-OUT;n:type:ShaderForge.SFN_Get,id:4792,x:30581,y:31848,varname:node_4792,prsc:2|IN-489-OUT;n:type:ShaderForge.SFN_Get,id:5362,x:31433,y:32978,varname:node_5362,prsc:2|IN-489-OUT;n:type:ShaderForge.SFN_Time,id:8537,x:30790,y:31976,varname:node_8537,prsc:2;n:type:ShaderForge.SFN_Add,id:6914,x:30972,y:32004,varname:node_6914,prsc:2|A-8551-OUT,B-8537-T;proporder:2317-402-4766-3191-7745;pass:END;sub:END;*/

Shader "Shader Forge/LightingPanel06" {
    Properties {
        _Emission ("Emission", Range(0, 3)) = 1.589744
        _Tiling ("Tiling", Vector) = (25,10,0,0)
        _DistanceFade ("DistanceFade", Range(0, 50)) = 50
        _LEDs ("LEDs", Range(0, 50)) = 25
        _Bands ("Bands", Range(0, 10)) = 1
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 2.0
            uniform float _Emission;
            uniform float4 _Tiling;
            uniform float _DistanceFade;
            uniform float _LEDs;
            uniform float _Bands;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float DistA = (floor((_LEDs*length((i.uv0*2.0+-1.0))))/_LEDs);
                float UVV = i.uv0.g;
                float node_8551 = (DistA/UVV);
                float4 node_8537 = _Time;
                float node_718 = (acos((node_8551+node_8537.g))/_Bands);
                float DegA = node_718;
                float node_7657 = DegA;
                float node_6088 = (1.0 - (length((frac(float2((_Tiling.r*i.uv0.r),(_Tiling.g*i.uv0.g)))+(-0.5)))*1.5));
                float3 emissive = (_Emission*lerp(((lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac(node_7657+float3(0.0,-1.0/3.0,1.0/3.0)))-1),1.0)*smoothstep( 0.0, 0.6, DistA ))*node_7657),(((lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac(node_7657+float3(0.0,-1.0/3.0,1.0/3.0)))-1),1.0)*smoothstep( 0.0, 0.6, DistA ))*node_7657)*saturate(node_6088)),(1.0 - saturate((distance(_WorldSpaceCameraPos,i.posWorld.rgb)/_DistanceFade)))));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}

// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.015,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33286,y:32727,varname:node_3138,prsc:2|emission-3025-OUT;n:type:ShaderForge.SFN_TexCoord,id:1367,x:30151,y:33398,varname:node_1367,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Length,id:9739,x:31449,y:33258,varname:node_9739,prsc:2|IN-5516-OUT;n:type:ShaderForge.SFN_OneMinus,id:6088,x:32083,y:33333,varname:node_6088,prsc:2|IN-6382-OUT;n:type:ShaderForge.SFN_Clamp01,id:9472,x:32264,y:33333,varname:node_9472,prsc:2|IN-6088-OUT;n:type:ShaderForge.SFN_Multiply,id:1713,x:30555,y:33281,varname:node_1713,prsc:2|A-402-X,B-1367-U;n:type:ShaderForge.SFN_Frac,id:7853,x:31045,y:33165,varname:node_7853,prsc:2|IN-9985-OUT;n:type:ShaderForge.SFN_Add,id:5516,x:31269,y:33258,varname:node_5516,prsc:2|A-7853-OUT,B-5480-OUT;n:type:ShaderForge.SFN_Vector1,id:5480,x:31031,y:33319,varname:node_5480,prsc:2,v1:-0.5;n:type:ShaderForge.SFN_Multiply,id:6382,x:31877,y:33333,varname:node_6382,prsc:2|A-9739-OUT,B-6822-OUT;n:type:ShaderForge.SFN_Vector1,id:6822,x:31621,y:33408,varname:node_6822,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Slider,id:2317,x:32216,y:32688,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_2317,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.589744,max:3;n:type:ShaderForge.SFN_Multiply,id:3025,x:32851,y:32837,varname:node_3025,prsc:2|A-2317-OUT,B-5352-OUT;n:type:ShaderForge.SFN_Append,id:9985,x:30772,y:33359,varname:node_9985,prsc:2|A-1713-OUT,B-9820-OUT;n:type:ShaderForge.SFN_Multiply,id:9820,x:30555,y:33426,varname:node_9820,prsc:2|A-402-Y,B-1367-V;n:type:ShaderForge.SFN_Vector4Property,id:402,x:30151,y:33247,ptovrint:False,ptlb:Tiling,ptin:_Tiling,varname:node_402,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:50,v2:50,v3:0,v4:0;n:type:ShaderForge.SFN_ViewPosition,id:628,x:32048,y:33546,varname:node_628,prsc:2;n:type:ShaderForge.SFN_Distance,id:1384,x:32290,y:33630,varname:node_1384,prsc:2|A-628-XYZ,B-1132-XYZ;n:type:ShaderForge.SFN_FragmentPosition,id:1132,x:32045,y:33690,varname:node_1132,prsc:2;n:type:ShaderForge.SFN_Slider,id:4766,x:32133,y:33851,ptovrint:False,ptlb:DistanceFade,ptin:_DistanceFade,varname:node_4766,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:25.82121,max:50;n:type:ShaderForge.SFN_Divide,id:6341,x:32498,y:33661,varname:node_6341,prsc:2|A-1384-OUT,B-4766-OUT;n:type:ShaderForge.SFN_Clamp01,id:3711,x:32663,y:33661,varname:node_3711,prsc:2|IN-6341-OUT;n:type:ShaderForge.SFN_OneMinus,id:441,x:32850,y:33682,varname:node_441,prsc:2|IN-3711-OUT;n:type:ShaderForge.SFN_Tex2d,id:2453,x:27718,y:31455,varname:node_2453,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-9732-OUT,TEX-3106-TEX;n:type:ShaderForge.SFN_Set,id:557,x:30996,y:33511,varname:UV,prsc:2|IN-9985-OUT;n:type:ShaderForge.SFN_Get,id:3677,x:27696,y:32516,varname:node_3677,prsc:2|IN-557-OUT;n:type:ShaderForge.SFN_Fmod,id:4240,x:27913,y:32516,varname:node_4240,prsc:2|A-3677-OUT,B-6143-OUT;n:type:ShaderForge.SFN_Vector1,id:6143,x:27717,y:32577,varname:node_6143,prsc:2,v1:1;n:type:ShaderForge.SFN_Power,id:2216,x:28723,y:32490,varname:node_2216,prsc:2|VAL-3761-OUT,EXP-7094-OUT;n:type:ShaderForge.SFN_ComponentMask,id:1453,x:28148,y:32490,varname:node_1453,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-4240-OUT;n:type:ShaderForge.SFN_Vector1,id:7094,x:28326,y:32584,varname:node_7094,prsc:2,v1:2;n:type:ShaderForge.SFN_Subtract,id:3761,x:28523,y:32421,varname:node_3761,prsc:2|A-1453-R,B-1117-OUT;n:type:ShaderForge.SFN_Vector1,id:1117,x:28326,y:32522,varname:node_1117,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Power,id:430,x:28723,y:32689,varname:node_430,prsc:2|VAL-7830-OUT,EXP-7094-OUT;n:type:ShaderForge.SFN_Subtract,id:7830,x:28537,y:32689,varname:node_7830,prsc:2|A-1453-G,B-1117-OUT;n:type:ShaderForge.SFN_Dot,id:8618,x:28929,y:32596,varname:node_8618,prsc:2,dt:0|A-2216-OUT,B-430-OUT;n:type:ShaderForge.SFN_Set,id:7145,x:29105,y:32606,varname:dotvalue,prsc:2|IN-8618-OUT;n:type:ShaderForge.SFN_Fmod,id:2624,x:28717,y:31589,varname:node_2624,prsc:2|A-8700-OUT,B-7324-OUT;n:type:ShaderForge.SFN_Vector1,id:3841,x:27967,y:31645,varname:node_3841,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:8700,x:28150,y:31570,varname:node_8700,prsc:2|A-2453-R,B-3841-OUT,C-4401-R;n:type:ShaderForge.SFN_Vector1,id:7324,x:28150,y:31707,varname:node_7324,prsc:2,v1:1;n:type:ShaderForge.SFN_OneMinus,id:3619,x:28898,y:31589,varname:node_3619,prsc:2|IN-2624-OUT;n:type:ShaderForge.SFN_Set,id:7359,x:29847,y:31829,varname:p,prsc:2|IN-5359-OUT;n:type:ShaderForge.SFN_Min,id:5359,x:29687,y:31829,varname:node_5359,prsc:2|A-2565-OUT,B-4153-OUT;n:type:ShaderForge.SFN_Max,id:2565,x:29489,y:31798,varname:node_2565,prsc:2|A-7791-OUT,B-2940-OUT;n:type:ShaderForge.SFN_Multiply,id:52,x:29104,y:31681,varname:node_52,prsc:2|A-3619-OUT,B-8792-OUT;n:type:ShaderForge.SFN_Vector1,id:8792,x:28898,y:31734,varname:node_8792,prsc:2,v1:3;n:type:ShaderForge.SFN_Subtract,id:7791,x:29289,y:31781,varname:node_7791,prsc:2|A-52-OUT,B-226-OUT;n:type:ShaderForge.SFN_Vector1,id:226,x:29104,y:31837,varname:node_226,prsc:2,v1:1.8;n:type:ShaderForge.SFN_Vector1,id:2940,x:29289,y:31916,varname:node_2940,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Vector1,id:4153,x:29489,y:31928,varname:node_4153,prsc:2,v1:2;n:type:ShaderForge.SFN_Get,id:4671,x:31189,y:32745,varname:node_4671,prsc:2|IN-7359-OUT;n:type:ShaderForge.SFN_Multiply,id:4687,x:31830,y:32754,varname:node_4687,prsc:2|A-7033-OUT,B-4671-OUT;n:type:ShaderForge.SFN_Get,id:4484,x:26283,y:31309,varname:node_4484,prsc:2|IN-557-OUT;n:type:ShaderForge.SFN_ComponentMask,id:9757,x:26455,y:31309,varname:node_9757,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-4484-OUT;n:type:ShaderForge.SFN_Divide,id:8814,x:27049,y:31270,varname:node_8814,prsc:2|A-865-OUT,B-6671-OUT;n:type:ShaderForge.SFN_Vector1,id:5405,x:26453,y:31460,varname:node_5405,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Add,id:5661,x:26660,y:31309,varname:node_5661,prsc:2|A-9757-R,B-5405-OUT;n:type:ShaderForge.SFN_Append,id:1481,x:27259,y:31326,varname:node_1481,prsc:2|A-8814-OUT,B-3660-OUT;n:type:ShaderForge.SFN_Round,id:865,x:26858,y:31268,varname:node_865,prsc:2|IN-5661-OUT;n:type:ShaderForge.SFN_Add,id:4831,x:26660,y:31442,varname:node_4831,prsc:2|A-9757-G,B-5405-OUT;n:type:ShaderForge.SFN_Round,id:4444,x:26858,y:31465,varname:node_4444,prsc:2|IN-4831-OUT;n:type:ShaderForge.SFN_Blend,id:957,x:32213,y:33154,varname:node_957,prsc:2,blmd:1,clmp:True|SRC-4687-OUT,DST-9472-OUT;n:type:ShaderForge.SFN_Set,id:6646,x:30332,y:33154,varname:XTiling,prsc:2|IN-402-X;n:type:ShaderForge.SFN_Set,id:3422,x:30332,y:33221,varname:YTiling,prsc:2|IN-402-Y;n:type:ShaderForge.SFN_Get,id:6671,x:26837,y:31406,varname:node_6671,prsc:2|IN-6646-OUT;n:type:ShaderForge.SFN_Divide,id:3660,x:27049,y:31465,varname:node_3660,prsc:2|A-4444-OUT,B-3380-OUT;n:type:ShaderForge.SFN_Get,id:3380,x:26795,y:31701,varname:node_3380,prsc:2|IN-3422-OUT;n:type:ShaderForge.SFN_Add,id:9732,x:27452,y:31439,varname:node_9732,prsc:2|A-1481-OUT,B-534-TSL;n:type:ShaderForge.SFN_Time,id:534,x:27259,y:31544,varname:node_534,prsc:2;n:type:ShaderForge.SFN_Tex2dAsset,id:3106,x:27452,y:31634,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_3106,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Subtract,id:8542,x:27466,y:31246,varname:node_8542,prsc:2|A-1481-OUT,B-534-TSL;n:type:ShaderForge.SFN_Tex2d,id:4401,x:27713,y:31254,varname:node_4401,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-8542-OUT,TEX-3106-TEX;n:type:ShaderForge.SFN_Vector3,id:9024,x:30918,y:32549,varname:node_9024,prsc:2,v1:0.7,v2:1.6,v3:2.8;n:type:ShaderForge.SFN_Lerp,id:5352,x:32760,y:33203,varname:node_5352,prsc:2|A-4687-OUT,B-957-OUT,T-441-OUT;n:type:ShaderForge.SFN_HsvToRgb,id:7033,x:31649,y:32616,varname:node_7033,prsc:2|H-8844-OUT,S-8283-OUT,V-7240-OUT;n:type:ShaderForge.SFN_Vector1,id:8283,x:31210,y:32699,varname:node_8283,prsc:2,v1:1;n:type:ShaderForge.SFN_TexCoord,id:3998,x:30970,y:32699,varname:node_3998,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:7884,x:30970,y:32859,varname:node_7884,prsc:2;n:type:ShaderForge.SFN_Add,id:915,x:31210,y:32859,varname:node_915,prsc:2|A-3998-V,B-7884-T;n:type:ShaderForge.SFN_Sin,id:2663,x:31367,y:32864,varname:node_2663,prsc:2|IN-915-OUT;n:type:ShaderForge.SFN_RemapRange,id:7240,x:31529,y:32864,varname:node_7240,prsc:2,frmn:-1,frmx:1,tomn:0.5,tomx:1|IN-2663-OUT;n:type:ShaderForge.SFN_Cos,id:8844,x:31402,y:32709,varname:node_8844,prsc:2|IN-1633-OUT;n:type:ShaderForge.SFN_Add,id:1633,x:31334,y:32506,varname:node_1633,prsc:2|A-7884-TSL,B-4671-OUT;proporder:2317-402-4766-3106;pass:END;sub:END;*/

Shader "Shader Forge/LightingPanel02" {
    Properties {
        _Emission ("Emission", Range(0, 3)) = 1.589744
        _Tiling ("Tiling", Vector) = (50,50,0,0)
        _DistanceFade ("DistanceFade", Range(0, 50)) = 25.82121
        _Noise ("Noise", 2D) = "white" {}
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
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
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
                float4 node_7884 = _Time;
                float2 node_9985 = float2((_Tiling.r*i.uv0.r),(_Tiling.g*i.uv0.g));
                float2 UV = node_9985;
                float2 node_9757 = UV.rg;
                float node_5405 = 0.5;
                float XTiling = _Tiling.r;
                float YTiling = _Tiling.g;
                float2 node_1481 = float2((round((node_9757.r+node_5405))/XTiling),(round((node_9757.g+node_5405))/YTiling));
                float4 node_534 = _Time;
                float2 node_9732 = (node_1481+node_534.r);
                float4 node_2453 = tex2D(_Noise,TRANSFORM_TEX(node_9732, _Noise));
                float2 node_8542 = (node_1481-node_534.r);
                float4 node_4401 = tex2D(_Noise,TRANSFORM_TEX(node_8542, _Noise));
                float p = min(max((((1.0 - fmod((node_2453.r*1.0*node_4401.r),1.0))*3.0)-1.8),0.1),2.0);
                float node_4671 = p;
                float3 node_4687 = ((lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac(cos((node_7884.r+node_4671))+float3(0.0,-1.0/3.0,1.0/3.0)))-1),1.0)*(sin((i.uv0.g+node_7884.g))*0.25+0.75))*node_4671);
                float3 emissive = (_Emission*lerp(node_4687,saturate((node_4687*saturate((1.0 - (length((frac(node_9985)+(-0.5)))*1.5))))),(1.0 - saturate((distance(_WorldSpaceCameraPos,i.posWorld.rgb)/_DistanceFade)))));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}

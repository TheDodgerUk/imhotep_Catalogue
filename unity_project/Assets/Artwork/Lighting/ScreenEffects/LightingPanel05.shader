// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33286,y:32727,varname:node_3138,prsc:2|emission-3025-OUT;n:type:ShaderForge.SFN_TexCoord,id:1367,x:30151,y:33398,varname:node_1367,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Length,id:9739,x:31449,y:33258,varname:node_9739,prsc:2|IN-5516-OUT;n:type:ShaderForge.SFN_OneMinus,id:6088,x:32083,y:33333,varname:node_6088,prsc:2|IN-6382-OUT;n:type:ShaderForge.SFN_Clamp01,id:9472,x:32264,y:33333,varname:node_9472,prsc:2|IN-6088-OUT;n:type:ShaderForge.SFN_Multiply,id:1713,x:30555,y:33281,varname:node_1713,prsc:2|A-402-X,B-1367-U;n:type:ShaderForge.SFN_Frac,id:7853,x:31045,y:33165,varname:node_7853,prsc:2|IN-9985-OUT;n:type:ShaderForge.SFN_Add,id:5516,x:31269,y:33258,varname:node_5516,prsc:2|A-7853-OUT,B-5480-OUT;n:type:ShaderForge.SFN_Vector1,id:5480,x:31031,y:33319,varname:node_5480,prsc:2,v1:-0.5;n:type:ShaderForge.SFN_Multiply,id:6382,x:31877,y:33333,varname:node_6382,prsc:2|A-9739-OUT,B-6822-OUT;n:type:ShaderForge.SFN_Vector1,id:6822,x:31621,y:33408,varname:node_6822,prsc:2,v1:1.5;n:type:ShaderForge.SFN_TexCoord,id:9615,x:29122,y:33140,varname:node_9615,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:1164,x:29954,y:33015,varname:node_1164,prsc:2;n:type:ShaderForge.SFN_Slider,id:2317,x:32477,y:32686,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_2317,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.589744,max:3;n:type:ShaderForge.SFN_Multiply,id:3025,x:32935,y:32820,varname:node_3025,prsc:2|A-2317-OUT,B-2731-OUT;n:type:ShaderForge.SFN_Length,id:9688,x:29737,y:32899,varname:node_9688,prsc:2|IN-3746-OUT;n:type:ShaderForge.SFN_Slider,id:5415,x:29009,y:32823,ptovrint:False,ptlb:node_5415,ptin:_node_5415,varname:node_5415,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:10,max:10;n:type:ShaderForge.SFN_RemapRange,id:3746,x:29319,y:33140,varname:node_3746,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-9615-UVOUT;n:type:ShaderForge.SFN_Cos,id:4383,x:30350,y:32874,varname:node_4383,prsc:2|IN-8155-OUT;n:type:ShaderForge.SFN_Multiply,id:1464,x:29954,y:32874,varname:node_1464,prsc:2|A-5415-OUT,B-9688-OUT;n:type:ShaderForge.SFN_Add,id:8155,x:30166,y:32874,varname:node_8155,prsc:2|A-1464-OUT,B-1164-TDB;n:type:ShaderForge.SFN_Append,id:9985,x:30772,y:33359,varname:node_9985,prsc:2|A-1713-OUT,B-9820-OUT;n:type:ShaderForge.SFN_Multiply,id:9820,x:30555,y:33426,varname:node_9820,prsc:2|A-402-Y,B-1367-V;n:type:ShaderForge.SFN_Vector4Property,id:402,x:30151,y:33246,ptovrint:False,ptlb:Tiling,ptin:_Tiling,varname:node_402,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:25,v2:10,v3:0,v4:0;n:type:ShaderForge.SFN_ViewPosition,id:628,x:32048,y:33546,varname:node_628,prsc:2;n:type:ShaderForge.SFN_Distance,id:1384,x:32290,y:33630,varname:node_1384,prsc:2|A-628-XYZ,B-1132-XYZ;n:type:ShaderForge.SFN_FragmentPosition,id:1132,x:32045,y:33690,varname:node_1132,prsc:2;n:type:ShaderForge.SFN_Slider,id:4766,x:32133,y:33851,ptovrint:False,ptlb:DistanceFade,ptin:_DistanceFade,varname:node_4766,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:50,max:50;n:type:ShaderForge.SFN_Divide,id:6341,x:32498,y:33661,varname:node_6341,prsc:2|A-1384-OUT,B-4766-OUT;n:type:ShaderForge.SFN_Clamp01,id:3711,x:32663,y:33661,varname:node_3711,prsc:2|IN-6341-OUT;n:type:ShaderForge.SFN_OneMinus,id:441,x:32850,y:33682,varname:node_441,prsc:2|IN-3711-OUT;n:type:ShaderForge.SFN_Multiply,id:2731,x:32583,y:32949,varname:node_2731,prsc:2|A-6460-OUT,B-9472-OUT;n:type:ShaderForge.SFN_Color,id:9341,x:30681,y:32427,ptovrint:False,ptlb:ColorA,ptin:_ColorA,varname:node_9341,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:5394,x:30681,y:32625,ptovrint:False,ptlb:ColorB,ptin:_ColorB,varname:_ColorA_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:6460,x:31093,y:32781,varname:node_6460,prsc:2|A-9341-RGB,B-5394-RGB,T-4383-OUT;proporder:2317-5415-402-4766-9341-5394;pass:END;sub:END;*/

Shader "Shader Forge/LightingPanel05" {
    Properties {
        _Emission ("Emission", Range(0, 3)) = 1.589744
        _node_5415 ("node_5415", Range(0, 10)) = 10
        _Tiling ("Tiling", Vector) = (25,10,0,0)
        _DistanceFade ("DistanceFade", Range(0, 50)) = 50
        _ColorA ("ColorA", Color) = (0.5,0.5,0.5,1)
        _ColorB ("ColorB", Color) = (0.5,0.5,0.5,1)
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
            uniform float _node_5415;
            uniform float4 _Tiling;
            uniform float4 _ColorA;
            uniform float4 _ColorB;
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
                float4 node_1164 = _Time;
                float3 emissive = (_Emission*(lerp(_ColorA.rgb,_ColorB.rgb,cos(((_node_5415*length((i.uv0*2.0+-1.0)))+node_1164.b)))*saturate((1.0 - (length((frac(float2((_Tiling.r*i.uv0.r),(_Tiling.g*i.uv0.g)))+(-0.5)))*1.5)))));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}

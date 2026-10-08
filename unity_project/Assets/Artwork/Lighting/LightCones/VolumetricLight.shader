// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:6,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.015,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33361,y:32714,varname:node_3138,prsc:2|emission-2419-OUT,voffset-8503-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:30750,y:31949,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2dAsset,id:3858,x:31741,y:34032,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_3858,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7721,x:31961,y:33930,varname:node_7721,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-6981-OUT,TEX-3858-TEX;n:type:ShaderForge.SFN_FragmentPosition,id:136,x:30641,y:34045,varname:node_136,prsc:2;n:type:ShaderForge.SFN_Time,id:5465,x:30410,y:33982,varname:node_5465,prsc:2;n:type:ShaderForge.SFN_Add,id:3354,x:30857,y:33967,varname:node_3354,prsc:2|A-4684-OUT,B-136-XYZ;n:type:ShaderForge.SFN_ComponentMask,id:9749,x:31222,y:33839,varname:node_9749,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-1210-OUT;n:type:ShaderForge.SFN_Append,id:6981,x:31741,y:33868,varname:node_6981,prsc:2|A-6499-OUT,B-3056-OUT;n:type:ShaderForge.SFN_Add,id:6499,x:31527,y:33812,varname:node_6499,prsc:2|A-9749-R,B-9749-G;n:type:ShaderForge.SFN_Add,id:3056,x:31527,y:33942,varname:node_3056,prsc:2|A-9749-R,B-9749-B;n:type:ShaderForge.SFN_Slider,id:7847,x:30641,y:33819,ptovrint:False,ptlb:Scale,ptin:_Scale,varname:node_7847,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:1210,x:31036,y:33839,varname:node_1210,prsc:2|A-7847-OUT,B-3354-OUT;n:type:ShaderForge.SFN_Slider,id:7183,x:30593,y:31849,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_7183,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3,max:3;n:type:ShaderForge.SFN_Multiply,id:2975,x:31019,y:31976,varname:node_2975,prsc:2|A-7183-OUT,B-7241-RGB,C-6077-OUT,D-4972-OUT;n:type:ShaderForge.SFN_Multiply,id:7269,x:32192,y:33990,varname:node_7269,prsc:2|A-7721-R,B-9026-OUT;n:type:ShaderForge.SFN_Slider,id:9026,x:31741,y:34223,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_9026,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Fresnel,id:7066,x:31493,y:31565,varname:node_7066,prsc:2|NRM-4403-OUT,EXP-2244-OUT;n:type:ShaderForge.SFN_NormalVector,id:4403,x:31245,y:31517,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:2244,x:31096,y:31685,ptovrint:False,ptlb:OuterFresnel,ptin:_OuterFresnel,varname:node_2244,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.050126,max:1;n:type:ShaderForge.SFN_OneMinus,id:7491,x:31695,y:31565,varname:node_7491,prsc:2|IN-7066-OUT;n:type:ShaderForge.SFN_Slider,id:759,x:30286,y:33863,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_759,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Multiply,id:4684,x:30641,y:33906,varname:node_4684,prsc:2|A-759-OUT,B-5465-TSL;n:type:ShaderForge.SFN_TexCoord,id:1505,x:32059,y:31997,varname:node_1505,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:7872,x:32194,y:32581,varname:node_7872,prsc:2|A-2609-OUT,B-8750-OUT,T-4990-OUT;n:type:ShaderForge.SFN_Vector3,id:2609,x:31931,y:32514,varname:node_2609,prsc:2,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_Slider,id:1939,x:32081,y:32167,ptovrint:False,ptlb:Falloff,ptin:_Falloff,varname:node_1939,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.1,cur:1,max:3;n:type:ShaderForge.SFN_Multiply,id:9811,x:32470,y:32110,varname:node_9811,prsc:2|A-5176-OUT,B-1939-OUT;n:type:ShaderForge.SFN_Set,id:7104,x:32826,y:32052,varname:Falloff,prsc:2|IN-8729-OUT;n:type:ShaderForge.SFN_Get,id:4990,x:31931,y:32666,varname:node_4990,prsc:2|IN-7104-OUT;n:type:ShaderForge.SFN_Set,id:3262,x:31408,y:31439,varname:Normal,prsc:2|IN-4403-OUT;n:type:ShaderForge.SFN_Set,id:5121,x:32171,y:33927,varname:Fog,prsc:2|IN-7721-R;n:type:ShaderForge.SFN_Get,id:4972,x:30600,y:32509,varname:node_4972,prsc:2|IN-5121-OUT;n:type:ShaderForge.SFN_Set,id:5386,x:31884,y:31565,varname:OuterFres,prsc:2|IN-7491-OUT;n:type:ShaderForge.SFN_Get,id:6077,x:30750,y:32090,varname:node_6077,prsc:2|IN-5386-OUT;n:type:ShaderForge.SFN_Set,id:4279,x:32369,y:33990,varname:Alpha,prsc:2|IN-7269-OUT;n:type:ShaderForge.SFN_Set,id:8192,x:31223,y:31976,varname:ConeOutter,prsc:2|IN-2975-OUT;n:type:ShaderForge.SFN_Get,id:8750,x:31931,y:32601,varname:node_8750,prsc:2|IN-8192-OUT;n:type:ShaderForge.SFN_Clamp01,id:8729,x:32666,y:32052,varname:node_8729,prsc:2|IN-9811-OUT;n:type:ShaderForge.SFN_Slider,id:5661,x:30480,y:32285,ptovrint:False,ptlb:DirectViewMul,ptin:_DirectViewMul,varname:node_5661,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1.802349,max:10;n:type:ShaderForge.SFN_Set,id:8211,x:31223,y:32217,varname:ConeInner,prsc:2|IN-6849-OUT;n:type:ShaderForge.SFN_Get,id:6444,x:30801,y:32212,varname:node_6444,prsc:2|IN-8192-OUT;n:type:ShaderForge.SFN_Multiply,id:6849,x:31041,y:32217,varname:node_6849,prsc:2|A-7241-RGB,B-5661-OUT,C-3499-OUT,D-5216-OUT,E-5448-OUT;n:type:ShaderForge.SFN_TexCoord,id:9719,x:33181,y:33840,varname:node_9719,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:7713,x:33454,y:33742,varname:node_7713,prsc:2|A-2626-OUT,B-6109-OUT,T-9719-V;n:type:ShaderForge.SFN_ValueProperty,id:2626,x:33181,y:33684,ptovrint:False,ptlb:ConeStartSize,ptin:_ConeStartSize,varname:node_2626,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_ValueProperty,id:6109,x:33181,y:33766,ptovrint:False,ptlb:ConeEndSize,ptin:_ConeEndSize,varname:node_6109,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_NormalVector,id:8505,x:33181,y:33505,prsc:2,pt:True;n:type:ShaderForge.SFN_Multiply,id:8503,x:33846,y:33569,varname:node_8503,prsc:2|A-8505-OUT,B-7713-OUT;n:type:ShaderForge.SFN_OneMinus,id:5176,x:32269,y:32026,varname:node_5176,prsc:2|IN-1505-V;n:type:ShaderForge.SFN_Vector1,id:4015,x:29763,y:33461,varname:node_4015,prsc:2,v1:2;n:type:ShaderForge.SFN_Pi,id:5387,x:29796,y:33351,varname:node_5387,prsc:2;n:type:ShaderForge.SFN_Multiply,id:911,x:29934,y:33389,varname:node_911,prsc:2|A-5387-OUT,B-4015-OUT;n:type:ShaderForge.SFN_Divide,id:2914,x:30116,y:33410,varname:node_2914,prsc:2|A-911-OUT,B-2650-OUT;n:type:ShaderForge.SFN_Vector1,id:2650,x:29934,y:33536,varname:node_2650,prsc:2,v1:360;n:type:ShaderForge.SFN_Set,id:3044,x:30281,y:33410,varname:DegToRad,prsc:2|IN-2914-OUT;n:type:ShaderForge.SFN_Multiply,id:8430,x:32162,y:32881,varname:node_8430,prsc:2|A-4706-OUT,B-4605-OUT;n:type:ShaderForge.SFN_Get,id:8592,x:31085,y:32872,varname:node_8592,prsc:2|IN-8211-OUT;n:type:ShaderForge.SFN_Get,id:3499,x:30801,y:32425,varname:node_3499,prsc:2|IN-7104-OUT;n:type:ShaderForge.SFN_Add,id:4757,x:32866,y:32683,varname:node_4757,prsc:2|A-6662-OUT,B-8430-OUT;n:type:ShaderForge.SFN_Multiply,id:5216,x:30822,y:32473,varname:node_5216,prsc:2|A-5661-OUT,B-4972-OUT;n:type:ShaderForge.SFN_Multiply,id:6662,x:32661,y:32599,varname:node_6662,prsc:2|A-7872-OUT,B-2117-OUT;n:type:ShaderForge.SFN_Slider,id:7701,x:31820,y:32374,ptovrint:False,ptlb:Distance,ptin:_Distance,varname:node_7701,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_DepthBlend,id:959,x:32226,y:32387,varname:node_959,prsc:2|DIST-7701-OUT;n:type:ShaderForge.SFN_Clamp01,id:2117,x:32403,y:32387,varname:node_2117,prsc:2|IN-959-OUT;n:type:ShaderForge.SFN_ViewPosition,id:2488,x:28865,y:33192,varname:node_2488,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:5359,x:28865,y:33320,varname:node_5359,prsc:2;n:type:ShaderForge.SFN_Subtract,id:9196,x:29247,y:33237,varname:node_9196,prsc:2|A-2488-XYZ,B-5359-XYZ;n:type:ShaderForge.SFN_Slider,id:4605,x:31788,y:32979,ptovrint:False,ptlb:InnerEffectFadeIn,ptin:_InnerEffectFadeIn,varname:node_4605,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:4706,x:31444,y:32938,varname:node_4706,prsc:2|A-8592-OUT,B-1031-OUT;n:type:ShaderForge.SFN_TexCoord,id:85,x:30465,y:32860,varname:node_85,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_OneMinus,id:1031,x:31188,y:32984,varname:node_1031,prsc:2|IN-523-OUT;n:type:ShaderForge.SFN_Multiply,id:750,x:30791,y:32984,varname:node_750,prsc:2|A-85-V,B-1529-OUT;n:type:ShaderForge.SFN_Slider,id:1529,x:30339,y:33066,ptovrint:False,ptlb:InnerFalloff,ptin:_InnerFalloff,varname:node_1529,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Clamp01,id:523,x:30986,y:32984,varname:node_523,prsc:2|IN-750-OUT;n:type:ShaderForge.SFN_Get,id:5448,x:31070,y:32490,varname:node_5448,prsc:2|IN-2621-OUT;n:type:ShaderForge.SFN_Slider,id:9358,x:31155,y:31860,ptovrint:False,ptlb:InnerFresnel,ptin:_InnerFresnel,varname:node_9358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Fresnel,id:3143,x:31521,y:31793,varname:node_3143,prsc:2|NRM-4403-OUT,EXP-9358-OUT;n:type:ShaderForge.SFN_OneMinus,id:4231,x:31702,y:31760,varname:node_4231,prsc:2|IN-3143-OUT;n:type:ShaderForge.SFN_Set,id:2621,x:31854,y:31796,varname:InnerFres,prsc:2|IN-4231-OUT;n:type:ShaderForge.SFN_Clamp01,id:2419,x:33065,y:32683,varname:node_2419,prsc:2|IN-4757-OUT;proporder:7241-3858-7847-7183-9026-2244-759-1939-5661-2626-6109-7701-4605-1529-9358;pass:END;sub:END;*/

Shader "Shader Forge/VolumetricLight" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Noise ("Noise", 2D) = "white" {}
        _Scale ("Scale", Range(0, 1)) = 1
        _Emission ("Emission", Range(0, 3)) = 3
        _Alpha ("Alpha", Range(0, 1)) = 1
        _OuterFresnel ("OuterFresnel", Range(0, 1)) = 0.050126
        _Speed ("Speed", Range(0, 10)) = 1
        _Falloff ("Falloff", Range(0.1, 3)) = 1
        _DirectViewMul ("DirectViewMul", Range(1, 10)) = 1.802349
        _ConeStartSize ("ConeStartSize", Float ) = 0.1
        _ConeEndSize ("ConeEndSize", Float ) = 5
        _Distance ("Distance", Range(0, 5)) = 0
        _InnerEffectFadeIn ("InnerEffectFadeIn", Range(0, 1)) = 0
        _InnerFalloff ("InnerFalloff", Range(0, 5)) = 1
        _InnerFresnel ("InnerFresnel", Range(0, 2)) = 0
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
            Blend One OneMinusSrcColor
            Cull Off
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
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Scale;
            uniform float _Emission;
            uniform float _OuterFresnel;
            uniform float _Speed;
            uniform float _Falloff;
            uniform float _DirectViewMul;
            uniform float _ConeStartSize;
            uniform float _ConeEndSize;
            uniform float _Distance;
            uniform float _InnerEffectFadeIn;
            uniform float _InnerFalloff;
            uniform float _InnerFresnel;
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
                float4 projPos : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                v.vertex.xyz += (v.normal*lerp(_ConeStartSize,_ConeEndSize,o.uv0.g));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float OuterFres = (1.0 - pow(1.0-max(0,dot(i.normalDir, viewDirection)),_OuterFresnel));
                float4 node_5465 = _Time;
                float3 node_9749 = (_Scale*((_Speed*node_5465.r)+i.posWorld.rgb)).rgb;
                float2 node_6981 = float2((node_9749.r+node_9749.g),(node_9749.r+node_9749.b));
                float4 node_7721 = tex2D(_Noise,TRANSFORM_TEX(node_6981, _Noise));
                float Fog = node_7721.r;
                float node_4972 = Fog;
                float3 ConeOutter = (_Emission*_Color.rgb*OuterFres*node_4972);
                float Falloff = saturate(((1.0 - i.uv0.g)*_Falloff));
                float InnerFres = (1.0 - pow(1.0-max(0,dot(i.normalDir, viewDirection)),_InnerFresnel));
                float3 ConeInner = (_Color.rgb*_DirectViewMul*Falloff*(_DirectViewMul*node_4972)*InnerFres);
                float3 emissive = saturate(((lerp(float3(0,0,0),ConeOutter,Falloff)*saturate(saturate((sceneZ-partZ)/_Distance)))+((ConeInner*(1.0 - saturate((i.uv0.g*_InnerFalloff))))*_InnerEffectFadeIn)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}

// Shader created with Shader Forge v1.41 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.41;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33064,y:32635,varname:node_3138,prsc:2|emission-1362-OUT,alpha-8708-OUT;n:type:ShaderForge.SFN_Add,id:1362,x:32687,y:32578,varname:node_1362,prsc:2|A-2680-OUT,B-4670-OUT;n:type:ShaderForge.SFN_Multiply,id:4670,x:32314,y:32716,varname:node_4670,prsc:2|A-1604-OUT,B-3619-OUT;n:type:ShaderForge.SFN_Color,id:574,x:31776,y:32890,ptovrint:False,ptlb:Effect Colour,ptin:_EffectColour,varname:node_574,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.5,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:759,x:30642,y:32428,ptovrint:False,ptlb:Scanline,ptin:_Scanline,varname:node_759,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:False|UVIN-2359-UVOUT;n:type:ShaderForge.SFN_UVTile,id:2359,x:30469,y:32428,varname:node_2359,prsc:2|UVIN-3385-UVOUT,WDT-5382-OUT,HGT-6077-OUT,TILE-8101-OUT;n:type:ShaderForge.SFN_Vector1,id:6077,x:30290,y:32478,varname:node_6077,prsc:2,v1:0.02;n:type:ShaderForge.SFN_Vector1,id:5382,x:30290,y:32428,varname:node_5382,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:8101,x:30290,y:32525,varname:node_8101,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:6388,x:30850,y:32365,varname:node_6388,prsc:2|A-6300-OUT,B-759-RGB;n:type:ShaderForge.SFN_Slider,id:6300,x:30484,y:32306,ptovrint:False,ptlb:Transparency,ptin:_Transparency,varname:node_6300,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4891394,max:1;n:type:ShaderForge.SFN_TexCoord,id:2776,x:30104,y:32246,varname:node_2776,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:3385,x:30289,y:32246,varname:node_3385,prsc:2,spu:1,spv:0.05|UVIN-2776-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:5598,x:31110,y:32150,varname:node_5598,prsc:2,uv:3,uaff:False;n:type:ShaderForge.SFN_Multiply,id:1604,x:31849,y:32451,varname:node_1604,prsc:2|A-1951-RGB,B-1879-OUT;n:type:ShaderForge.SFN_UVTile,id:1264,x:31295,y:32279,varname:node_1264,prsc:2|UVIN-5598-UVOUT,WDT-7546-OUT,HGT-7546-OUT,TILE-7546-OUT;n:type:ShaderForge.SFN_Vector1,id:7546,x:31110,y:32326,varname:node_7546,prsc:2,v1:1;n:type:ShaderForge.SFN_Tex2d,id:1951,x:31622,y:32285,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_1951,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1264-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:1931,x:31182,y:32927,ptovrint:False,ptlb:Image,ptin:_Image,varname:_node_8739_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ea3e9a0c4309d80469b3ff447dafc9db,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1879,x:31622,y:32510,varname:node_1879,prsc:2|A-9845-OUT,B-2680-OUT;n:type:ShaderForge.SFN_Tex2d,id:9800,x:30640,y:32785,ptovrint:False,ptlb:Scanline2,ptin:_Scanline2,varname:_node_759_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8252-UVOUT;n:type:ShaderForge.SFN_UVTile,id:8252,x:30467,y:32785,varname:node_8252,prsc:2|UVIN-998-UVOUT,WDT-8216-OUT,HGT-5615-OUT,TILE-7301-OUT;n:type:ShaderForge.SFN_Vector1,id:5615,x:30288,y:32835,varname:node_5615,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Vector1,id:8216,x:30288,y:32785,varname:node_8216,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:7301,x:30288,y:32883,varname:node_7301,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:8736,x:30849,y:32722,varname:node_8736,prsc:2|A-4017-OUT,B-9800-RGB;n:type:ShaderForge.SFN_Slider,id:4017,x:30482,y:32663,ptovrint:False,ptlb:Transparency_copy,ptin:_Transparency_copy,varname:_Transparency_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1497537,max:1;n:type:ShaderForge.SFN_TexCoord,id:5194,x:30103,y:32603,varname:node_5194,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:998,x:30287,y:32603,varname:node_998,prsc:2,spu:1,spv:0.03|UVIN-5194-UVOUT;n:type:ShaderForge.SFN_Add,id:9845,x:31110,y:32512,varname:node_9845,prsc:2|A-6388-OUT,B-8736-OUT,C-3469-OUT;n:type:ShaderForge.SFN_Tex2d,id:6516,x:30641,y:33147,ptovrint:False,ptlb:Scanline3,ptin:_Scanline3,varname:_node_759_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3014-UVOUT;n:type:ShaderForge.SFN_UVTile,id:3014,x:30468,y:33147,varname:node_3014,prsc:2|UVIN-7160-UVOUT,WDT-6554-OUT,HGT-8041-OUT,TILE-414-OUT;n:type:ShaderForge.SFN_Vector1,id:8041,x:30289,y:33197,varname:node_8041,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:6554,x:30289,y:33147,varname:node_6554,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:414,x:30289,y:33245,varname:node_414,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:3469,x:30850,y:33084,varname:node_3469,prsc:2|A-6755-OUT,B-6516-RGB;n:type:ShaderForge.SFN_Slider,id:6755,x:30484,y:33025,ptovrint:False,ptlb:Transparency_copy_copy,ptin:_Transparency_copy_copy,varname:_Transparency_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3487934,max:1;n:type:ShaderForge.SFN_TexCoord,id:240,x:30104,y:32965,varname:node_240,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:7160,x:30288,y:32965,varname:node_7160,prsc:2,spu:1,spv:0.02|UVIN-240-UVOUT;n:type:ShaderForge.SFN_Clamp,id:3619,x:32105,y:32990,varname:node_3619,prsc:2|IN-574-RGB,MIN-6667-OUT,MAX-4147-OUT;n:type:ShaderForge.SFN_Vector1,id:6667,x:31776,y:33046,varname:node_6667,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:4552,x:30544,y:33527,varname:node_4552,prsc:2,v1:5;n:type:ShaderForge.SFN_Sin,id:2630,x:31700,y:33139,varname:node_2630,prsc:2|IN-5884-OUT;n:type:ShaderForge.SFN_Multiply,id:5884,x:31509,y:33331,varname:node_5884,prsc:2|A-6584-T,B-7231-OUT;n:type:ShaderForge.SFN_Time,id:6584,x:31262,y:33148,varname:node_6584,prsc:2;n:type:ShaderForge.SFN_Time,id:3563,x:30579,y:33394,varname:node_3563,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8044,x:30782,y:33448,varname:node_8044,prsc:2|A-3563-TSL,B-4552-OUT;n:type:ShaderForge.SFN_Sin,id:3157,x:31033,y:33428,varname:node_3157,prsc:2|IN-8044-OUT;n:type:ShaderForge.SFN_Multiply,id:7231,x:31288,y:33408,varname:node_7231,prsc:2|A-283-OUT,B-3157-OUT;n:type:ShaderForge.SFN_Vector1,id:283,x:31018,y:33334,varname:node_283,prsc:2,v1:8;n:type:ShaderForge.SFN_ConstantClamp,id:4147,x:31920,y:33177,varname:node_4147,prsc:2,min:0.3,max:1|IN-2630-OUT;n:type:ShaderForge.SFN_Color,id:2421,x:31195,y:32714,ptovrint:False,ptlb:ImageColour,ptin:_ImageColour,varname:node_2421,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:2680,x:31451,y:32688,varname:node_2680,prsc:2|A-2421-RGB,B-1931-RGB;n:type:ShaderForge.SFN_Slider,id:552,x:32454,y:32909,ptovrint:False,ptlb:FinalAlpha,ptin:_FinalAlpha,varname:node_552,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:8708,x:32561,y:33020,varname:node_8708,prsc:2|A-552-OUT,B-1931-A;proporder:574-759-6300-1951-1931-9800-4017-6516-6755-2421-552;pass:END;sub:END;*/

Shader "Shader Forge/UIShader" {
    Properties {
        _EffectColour ("Effect Colour", Color) = (0,0.5,1,1)
        _Scanline ("Scanline", 2D) = "bump" {}
        _Transparency ("Transparency", Range(0, 1)) = 0.4891394
        _Noise ("Noise", 2D) = "white" {}
        _Image ("Image", 2D) = "white" {}
        _Scanline2 ("Scanline2", 2D) = "white" {}
        _Transparency_copy ("Transparency_copy", Range(0, 1)) = 0.1497537
        _Scanline3 ("Scanline3", 2D) = "white" {}
        _Transparency_copy_copy ("Transparency_copy_copy", Range(0, 1)) = 0.3487934
        _ImageColour ("ImageColour", Color) = (1,0,0,1)
        _FinalAlpha ("FinalAlpha", Range(0, 1)) = 1
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
            #pragma only_renderers d3d9 d3d11 glcore gles vulkan 
            #pragma target 3.0
            uniform float4 _EffectColour;
            uniform sampler2D _Scanline; uniform float4 _Scanline_ST;
            uniform float _Transparency;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform sampler2D _Image; uniform float4 _Image_ST;
            uniform sampler2D _Scanline2; uniform float4 _Scanline2_ST;
            uniform float _Transparency_copy;
            uniform sampler2D _Scanline3; uniform float4 _Scanline3_ST;
            uniform float _Transparency_copy_copy;
            uniform float4 _ImageColour;
            uniform float _FinalAlpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord3 : TEXCOORD3;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv3 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv3 = v.texcoord3;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _Image_var = tex2D(_Image,TRANSFORM_TEX(i.uv0, _Image));
                float3 node_2680 = (_ImageColour.rgb*_Image_var.rgb);
                float node_7546 = 1.0;
                float2 node_1264_tc_rcp = float2(1.0,1.0)/float2( node_7546, node_7546 );
                float node_1264_ty = floor(node_7546 * node_1264_tc_rcp.x);
                float node_1264_tx = node_7546 - node_7546 * node_1264_ty;
                float2 node_1264 = (i.uv3 + float2(node_1264_tx, node_1264_ty)) * node_1264_tc_rcp;
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_1264, _Noise));
                float node_5382 = 1.0;
                float node_8101 = 1.0;
                float2 node_2359_tc_rcp = float2(1.0,1.0)/float2( node_5382, 0.02 );
                float node_2359_ty = floor(node_8101 * node_2359_tc_rcp.x);
                float node_2359_tx = node_8101 - node_5382 * node_2359_ty;
                float4 node_9929 = _Time;
                float2 node_2359 = ((i.uv0+node_9929.g*float2(1,0.05)) + float2(node_2359_tx, node_2359_ty)) * node_2359_tc_rcp;
                float4 _Scanline_var = tex2D(_Scanline,TRANSFORM_TEX(node_2359, _Scanline));
                float node_8216 = 1.0;
                float node_7301 = 1.0;
                float2 node_8252_tc_rcp = float2(1.0,1.0)/float2( node_8216, 0.1 );
                float node_8252_ty = floor(node_7301 * node_8252_tc_rcp.x);
                float node_8252_tx = node_7301 - node_8216 * node_8252_ty;
                float2 node_8252 = ((i.uv0+node_9929.g*float2(1,0.03)) + float2(node_8252_tx, node_8252_ty)) * node_8252_tc_rcp;
                float4 _Scanline2_var = tex2D(_Scanline2,TRANSFORM_TEX(node_8252, _Scanline2));
                float node_6554 = 1.0;
                float node_414 = 1.0;
                float2 node_3014_tc_rcp = float2(1.0,1.0)/float2( node_6554, 1.0 );
                float node_3014_ty = floor(node_414 * node_3014_tc_rcp.x);
                float node_3014_tx = node_414 - node_6554 * node_3014_ty;
                float2 node_3014 = ((i.uv0+node_9929.g*float2(1,0.02)) + float2(node_3014_tx, node_3014_ty)) * node_3014_tc_rcp;
                float4 _Scanline3_var = tex2D(_Scanline3,TRANSFORM_TEX(node_3014, _Scanline3));
                float4 node_6584 = _Time;
                float4 node_3563 = _Time;
                float3 emissive = (node_2680+((_Noise_var.rgb*(((_Transparency*_Scanline_var.rgb)+(_Transparency_copy*_Scanline2_var.rgb)+(_Transparency_copy_copy*_Scanline3_var.rgb))*node_2680))*clamp(_EffectColour.rgb,0.5,clamp(sin((node_6584.g*(8.0*sin((node_3563.r*5.0))))),0.3,1))));
                float3 finalColor = emissive;
                return fixed4(finalColor,(_FinalAlpha*_Image_var.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33286,y:32892,varname:node_3138,prsc:2|diff-3619-RGB,alpha-4309-OUT,refract-3511-OUT;n:type:ShaderForge.SFN_Tex2d,id:8516,x:32137,y:33091,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_8516,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:1635,x:31980,y:33281,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:node_1635,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:8091,x:32428,y:33183,varname:node_8091,prsc:2|A-8516-RGB,B-1635-OUT;n:type:ShaderForge.SFN_ComponentMask,id:3009,x:32597,y:33183,varname:node_3009,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-8091-OUT;n:type:ShaderForge.SFN_Slider,id:2249,x:32693,y:32972,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_2249,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Color,id:3619,x:32264,y:32768,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_3619,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:1421,x:32597,y:33356,ptovrint:False,ptlb:RefractionMask,ptin:_RefractionMask,varname:node_1421,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3a5a96df060a5cf4a9cc0c59e13486b7,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3511,x:32797,y:33233,varname:node_3511,prsc:2|A-3009-OUT,B-1421-R;n:type:ShaderForge.SFN_OneMinus,id:7979,x:32413,y:33023,varname:node_7979,prsc:2|IN-8381-OUT;n:type:ShaderForge.SFN_Lerp,id:4309,x:33049,y:33001,varname:node_4309,prsc:2|A-2249-OUT,B-9573-OUT,T-6257-OUT;n:type:ShaderForge.SFN_Slider,id:6257,x:32693,y:33077,ptovrint:False,ptlb:MaskMix,ptin:_MaskMix,varname:node_6257,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Set,id:1379,x:32776,y:33398,varname:RefractionMask,prsc:2|IN-1421-R;n:type:ShaderForge.SFN_Get,id:8381,x:32231,y:33023,varname:node_8381,prsc:2|IN-1379-OUT;n:type:ShaderForge.SFN_Multiply,id:9573,x:32569,y:33003,varname:node_9573,prsc:2|A-3619-A,B-7979-OUT;proporder:8516-1635-2249-3619-1421-6257;pass:END;sub:END;*/

Shader "Shader Forge/Glass" {
    Properties {
        _Normal ("Normal", 2D) = "bump" {}
        _Refraction ("Refraction", Range(0, 1)) = 0
        _Opacity ("Opacity", Range(0, 1)) = 0
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _RefractionMask ("RefractionMask", 2D) = "white" {}
        _MaskMix ("MaskMix", Range(0, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ "Refraction" }
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
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 2.0
            uniform float4 _LightColor0;
            uniform sampler2D Refraction;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Refraction;
            uniform float _Opacity;
            uniform float4 _Color;
            uniform sampler2D _RefractionMask; uniform float4 _RefractionMask_ST;
            uniform float _MaskMix;
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
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float4 _RefractionMask_var = tex2D(_RefractionMask,TRANSFORM_TEX(i.uv0, _RefractionMask));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + ((_Normal_var.rgb*_Refraction).rg*_RefractionMask_var.r);
                float4 sceneColor = tex2D(Refraction, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuseColor = _Color.rgb;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                float RefractionMask = _RefractionMask_var.r;
                return fixed4(lerp(sceneColor.rgb, finalColor,lerp(_Opacity,(_Color.a*(1.0 - RefractionMask)),_MaskMix)),1);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 2.0
            uniform float4 _LightColor0;
            uniform sampler2D Refraction;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Refraction;
            uniform float _Opacity;
            uniform float4 _Color;
            uniform sampler2D _RefractionMask; uniform float4 _RefractionMask_ST;
            uniform float _MaskMix;
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
                LIGHTING_COORDS(4,5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float4 _RefractionMask_var = tex2D(_RefractionMask,TRANSFORM_TEX(i.uv0, _RefractionMask));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + ((_Normal_var.rgb*_Refraction).rg*_RefractionMask_var.r);
                float4 sceneColor = tex2D(Refraction, sceneUVs);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 diffuseColor = _Color.rgb;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                float RefractionMask = _RefractionMask_var.r;
                return fixed4(finalColor * lerp(_Opacity,(_Color.a*(1.0 - RefractionMask)),_MaskMix),0);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}

// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33424,y:32788,varname:node_3138,prsc:2|diff-5423-OUT,emission-6792-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32594,y:32713,ptovrint:False,ptlb:EmissiveColor,ptin:_EmissiveColor,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:1493,x:32594,y:32881,ptovrint:False,ptlb:Emissive,ptin:_Emissive,varname:node_1493,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:846,x:32834,y:32821,varname:node_846,prsc:2|A-7241-RGB,B-1493-RGB,C-6265-OUT;n:type:ShaderForge.SFN_Time,id:1542,x:32207,y:33142,varname:node_1542,prsc:2;n:type:ShaderForge.SFN_Slider,id:3662,x:32050,y:33297,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_3662,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:10,max:10;n:type:ShaderForge.SFN_Multiply,id:1654,x:32433,y:33272,varname:node_1654,prsc:2|A-1542-T,B-3662-OUT;n:type:ShaderForge.SFN_RemapRange,id:2636,x:32781,y:33272,varname:node_2636,prsc:2,frmn:0,frmx:1,tomn:0,tomx:0.1|IN-3340-OUT;n:type:ShaderForge.SFN_Add,id:6792,x:33084,y:32885,varname:node_6792,prsc:2|A-846-OUT,B-7422-OUT;n:type:ShaderForge.SFN_Slider,id:6265,x:32433,y:33064,ptovrint:False,ptlb:EmissionPower,ptin:_EmissionPower,varname:node_6265,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1.837607,max:3;n:type:ShaderForge.SFN_Noise,id:3340,x:32524,y:33449,varname:node_3340,prsc:2|XY-3830-OUT;n:type:ShaderForge.SFN_Append,id:3830,x:32196,y:33483,varname:node_3830,prsc:2|A-1654-OUT,B-1654-OUT;n:type:ShaderForge.SFN_Slider,id:1491,x:32781,y:33516,ptovrint:False,ptlb:Flicker,ptin:_Flicker,varname:node_1491,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:7422,x:33047,y:33296,varname:node_7422,prsc:2|A-2636-OUT,B-1491-OUT;n:type:ShaderForge.SFN_Tex2d,id:7177,x:32834,y:32645,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_7177,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:9699,x:32834,y:32472,ptovrint:False,ptlb:MainColor,ptin:_MainColor,varname:node_9699,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:5423,x:33067,y:32617,varname:node_5423,prsc:2|A-9699-RGB,B-7177-RGB;proporder:9699-7177-1493-7241-3662-6265-1491;pass:END;sub:END;*/

Shader "Shader Forge/BacklitSign" {
    Properties {
        _MainColor ("MainColor", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Emissive ("Emissive", 2D) = "white" {}
        _EmissiveColor ("EmissiveColor", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Speed ("Speed", Range(0, 10)) = 10
        _EmissionPower ("EmissionPower", Range(1, 3)) = 1.837607
        _Flicker ("Flicker", Range(0, 1)) = 0
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
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 2.0
            uniform float4 _LightColor0;
            uniform float4 _EmissiveColor;
            uniform sampler2D _Emissive; uniform float4 _Emissive_ST;
            uniform float _Speed;
            uniform float _EmissionPower;
            uniform float _Flicker;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _MainColor;
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
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = (_MainColor.rgb*_MainTex_var.rgb);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float4 _Emissive_var = tex2D(_Emissive,TRANSFORM_TEX(i.uv0, _Emissive));
                float4 node_1542 = _Time;
                float node_1654 = (node_1542.g*_Speed);
                float2 node_3830 = float2(node_1654,node_1654);
                float2 node_3340_skew = node_3830 + 0.2127+node_3830.x*0.3713*node_3830.y;
                float2 node_3340_rnd = 4.789*sin(489.123*(node_3340_skew));
                float node_3340 = frac(node_3340_rnd.x*node_3340_rnd.y*(1+node_3340_skew.x));
                float3 emissive = ((_EmissiveColor.rgb*_Emissive_var.rgb*_EmissionPower)+((node_3340*0.1+0.0)*_Flicker));
/// Final Color:
                float3 finalColor = diffuse + emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 2.0
            uniform float4 _LightColor0;
            uniform float4 _EmissiveColor;
            uniform sampler2D _Emissive; uniform float4 _Emissive_ST;
            uniform float _Speed;
            uniform float _EmissionPower;
            uniform float _Flicker;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _MainColor;
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
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = (_MainColor.rgb*_MainTex_var.rgb);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}

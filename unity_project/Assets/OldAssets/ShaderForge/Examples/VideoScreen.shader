// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:32840,y:33123,varname:node_2865,prsc:2|diff-6343-OUT,spec-358-OUT,gloss-1813-OUT,emission-1744-OUT;n:type:ShaderForge.SFN_Multiply,id:6343,x:32512,y:32958,varname:node_6343,prsc:2|A-8070-OUT,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:32278,y:33029,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31877,y:32519,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6130-OUT;n:type:ShaderForge.SFN_Slider,id:358,x:32132,y:33186,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1813,x:32132,y:33288,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_Slider,id:3328,x:32131,y:33444,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_3328,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:1744,x:32482,y:33368,varname:node_1744,prsc:2|A-2990-OUT,B-3328-OUT;n:type:ShaderForge.SFN_Slider,id:1292,x:30169,y:32560,ptovrint:False,ptlb:Pixelation,ptin:_Pixelation,varname:node_1292,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:32,max:32;n:type:ShaderForge.SFN_Append,id:3371,x:30723,y:32559,varname:node_3371,prsc:2|A-5081-OUT,B-5081-OUT;n:type:ShaderForge.SFN_TexCoord,id:6740,x:30723,y:32398,varname:node_6740,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:6864,x:30927,y:32398,varname:node_6864,prsc:2|A-6740-UVOUT,B-3371-OUT;n:type:ShaderForge.SFN_Add,id:8820,x:31114,y:32379,varname:node_8820,prsc:2|A-8017-OUT,B-6864-OUT;n:type:ShaderForge.SFN_Vector2,id:8017,x:30927,y:32305,varname:node_8017,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Divide,id:1006,x:31473,y:32539,varname:node_1006,prsc:2|A-2790-OUT,B-3371-OUT;n:type:ShaderForge.SFN_Round,id:2790,x:31297,y:32379,varname:node_2790,prsc:2|IN-8820-OUT;n:type:ShaderForge.SFN_RgbToHsv,id:979,x:32066,y:32519,varname:node_979,prsc:2|IN-7736-RGB;n:type:ShaderForge.SFN_Slider,id:8207,x:31909,y:32711,ptovrint:False,ptlb:Saturation,ptin:_Saturation,varname:node_8207,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:6892,x:32287,y:32629,varname:node_6892,prsc:2|A-979-SOUT,B-8207-OUT;n:type:ShaderForge.SFN_HsvToRgb,id:3442,x:32468,y:32518,varname:node_3442,prsc:2|H-979-HOUT,S-6892-OUT,V-979-VOUT;n:type:ShaderForge.SFN_Set,id:9114,x:33060,y:32518,varname:videoColour,prsc:2|IN-2481-OUT;n:type:ShaderForge.SFN_Get,id:8070,x:32257,y:32958,varname:node_8070,prsc:2|IN-9114-OUT;n:type:ShaderForge.SFN_Get,id:2990,x:32267,y:33368,varname:node_2990,prsc:2|IN-9114-OUT;n:type:ShaderForge.SFN_Tex2d,id:4662,x:32686,y:32664,ptovrint:False,ptlb:SecondaryOverlay,ptin:_SecondaryOverlay,varname:node_4662,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Lerp,id:2481,x:32880,y:32518,varname:node_2481,prsc:2|A-5187-OUT,B-4662-RGB,T-4662-A;n:type:ShaderForge.SFN_Tex2d,id:9717,x:32468,y:32664,ptovrint:False,ptlb:PrimaryOverlay,ptin:_PrimaryOverlay,varname:node_9717,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Lerp,id:5187,x:32686,y:32518,varname:node_5187,prsc:2|A-3442-OUT,B-9717-RGB,T-9717-A;n:type:ShaderForge.SFN_SwitchProperty,id:6130,x:31675,y:32519,ptovrint:False,ptlb:EnablePixelation,ptin:_EnablePixelation,varname:node_6130,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-5031-UVOUT,B-1006-OUT;n:type:ShaderForge.SFN_TexCoord,id:5031,x:31473,y:32379,varname:node_5031,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Power,id:5081,x:30533,y:32559,varname:node_5081,prsc:2|VAL-1292-OUT,EXP-7971-OUT;n:type:ShaderForge.SFN_Vector1,id:7971,x:30326,y:32638,varname:node_7971,prsc:2,v1:2;proporder:6665-7736-358-1813-3328-6130-1292-8207-9717-4662;pass:END;sub:END;*/

Shader "Shader Forge/VideoScreen" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Base Color", 2D) = "white" {}
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0.8
        _Emission ("Emission", Range(0, 1)) = 0
        [MaterialToggle] _EnablePixelation ("EnablePixelation", Float ) = 0
        _Pixelation ("Pixelation", Range(0.5, 32)) = 32
        _Saturation ("Saturation", Range(0, 1)) = 1
        _PrimaryOverlay ("PrimaryOverlay", 2D) = "black" {}
        _SecondaryOverlay ("SecondaryOverlay", 2D) = "black" {}
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _Emission;
            uniform float _Pixelation;
            uniform float _Saturation;
            uniform sampler2D _SecondaryOverlay; uniform float4 _SecondaryOverlay_ST;
            uniform sampler2D _PrimaryOverlay; uniform float4 _PrimaryOverlay_ST;
            uniform fixed _EnablePixelation;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float node_5081 = pow(_Pixelation,2.0);
                float2 node_3371 = float2(node_5081,node_5081);
                float2 _EnablePixelation_var = lerp( i.uv0, (round((float2(0.5,0.5)+(i.uv0*node_3371)))/node_3371), _EnablePixelation );
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(_EnablePixelation_var, _MainTex));
                float4 node_979_k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 node_979_p = lerp(float4(float4(_MainTex_var.rgb,0.0).zy, node_979_k.wz), float4(float4(_MainTex_var.rgb,0.0).yz, node_979_k.xy), step(float4(_MainTex_var.rgb,0.0).z, float4(_MainTex_var.rgb,0.0).y));
                float4 node_979_q = lerp(float4(node_979_p.xyw, float4(_MainTex_var.rgb,0.0).x), float4(float4(_MainTex_var.rgb,0.0).x, node_979_p.yzx), step(node_979_p.x, float4(_MainTex_var.rgb,0.0).x));
                float node_979_d = node_979_q.x - min(node_979_q.w, node_979_q.y);
                float node_979_e = 1.0e-10;
                float3 node_979 = float3(abs(node_979_q.z + (node_979_q.w - node_979_q.y) / (6.0 * node_979_d + node_979_e)), node_979_d / (node_979_q.x + node_979_e), node_979_q.x);;
                float4 _PrimaryOverlay_var = tex2D(_PrimaryOverlay,TRANSFORM_TEX(i.uv0, _PrimaryOverlay));
                float4 _SecondaryOverlay_var = tex2D(_SecondaryOverlay,TRANSFORM_TEX(i.uv0, _SecondaryOverlay));
                float3 videoColour = lerp(lerp((lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac(node_979.r+float3(0.0,-1.0/3.0,1.0/3.0)))-1),(node_979.g*_Saturation))*node_979.b),_PrimaryOverlay_var.rgb,_PrimaryOverlay_var.a),_SecondaryOverlay_var.rgb,_SecondaryOverlay_var.a);
                float3 diffuseColor = (videoColour*_Color.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = (videoColour*_Emission);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _Emission;
            uniform float _Pixelation;
            uniform float _Saturation;
            uniform sampler2D _SecondaryOverlay; uniform float4 _SecondaryOverlay_ST;
            uniform sampler2D _PrimaryOverlay; uniform float4 _PrimaryOverlay_ST;
            uniform fixed _EnablePixelation;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float node_5081 = pow(_Pixelation,2.0);
                float2 node_3371 = float2(node_5081,node_5081);
                float2 _EnablePixelation_var = lerp( i.uv0, (round((float2(0.5,0.5)+(i.uv0*node_3371)))/node_3371), _EnablePixelation );
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(_EnablePixelation_var, _MainTex));
                float4 node_979_k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 node_979_p = lerp(float4(float4(_MainTex_var.rgb,0.0).zy, node_979_k.wz), float4(float4(_MainTex_var.rgb,0.0).yz, node_979_k.xy), step(float4(_MainTex_var.rgb,0.0).z, float4(_MainTex_var.rgb,0.0).y));
                float4 node_979_q = lerp(float4(node_979_p.xyw, float4(_MainTex_var.rgb,0.0).x), float4(float4(_MainTex_var.rgb,0.0).x, node_979_p.yzx), step(node_979_p.x, float4(_MainTex_var.rgb,0.0).x));
                float node_979_d = node_979_q.x - min(node_979_q.w, node_979_q.y);
                float node_979_e = 1.0e-10;
                float3 node_979 = float3(abs(node_979_q.z + (node_979_q.w - node_979_q.y) / (6.0 * node_979_d + node_979_e)), node_979_d / (node_979_q.x + node_979_e), node_979_q.x);;
                float4 _PrimaryOverlay_var = tex2D(_PrimaryOverlay,TRANSFORM_TEX(i.uv0, _PrimaryOverlay));
                float4 _SecondaryOverlay_var = tex2D(_SecondaryOverlay,TRANSFORM_TEX(i.uv0, _SecondaryOverlay));
                float3 videoColour = lerp(lerp((lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac(node_979.r+float3(0.0,-1.0/3.0,1.0/3.0)))-1),(node_979.g*_Saturation))*node_979.b),_PrimaryOverlay_var.rgb,_PrimaryOverlay_var.a),_SecondaryOverlay_var.rgb,_SecondaryOverlay_var.a);
                float3 diffuseColor = (videoColour*_Color.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _Emission;
            uniform float _Pixelation;
            uniform float _Saturation;
            uniform sampler2D _SecondaryOverlay; uniform float4 _SecondaryOverlay_ST;
            uniform sampler2D _PrimaryOverlay; uniform float4 _PrimaryOverlay_ST;
            uniform fixed _EnablePixelation;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float node_5081 = pow(_Pixelation,2.0);
                float2 node_3371 = float2(node_5081,node_5081);
                float2 _EnablePixelation_var = lerp( i.uv0, (round((float2(0.5,0.5)+(i.uv0*node_3371)))/node_3371), _EnablePixelation );
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(_EnablePixelation_var, _MainTex));
                float4 node_979_k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 node_979_p = lerp(float4(float4(_MainTex_var.rgb,0.0).zy, node_979_k.wz), float4(float4(_MainTex_var.rgb,0.0).yz, node_979_k.xy), step(float4(_MainTex_var.rgb,0.0).z, float4(_MainTex_var.rgb,0.0).y));
                float4 node_979_q = lerp(float4(node_979_p.xyw, float4(_MainTex_var.rgb,0.0).x), float4(float4(_MainTex_var.rgb,0.0).x, node_979_p.yzx), step(node_979_p.x, float4(_MainTex_var.rgb,0.0).x));
                float node_979_d = node_979_q.x - min(node_979_q.w, node_979_q.y);
                float node_979_e = 1.0e-10;
                float3 node_979 = float3(abs(node_979_q.z + (node_979_q.w - node_979_q.y) / (6.0 * node_979_d + node_979_e)), node_979_d / (node_979_q.x + node_979_e), node_979_q.x);;
                float4 _PrimaryOverlay_var = tex2D(_PrimaryOverlay,TRANSFORM_TEX(i.uv0, _PrimaryOverlay));
                float4 _SecondaryOverlay_var = tex2D(_SecondaryOverlay,TRANSFORM_TEX(i.uv0, _SecondaryOverlay));
                float3 videoColour = lerp(lerp((lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac(node_979.r+float3(0.0,-1.0/3.0,1.0/3.0)))-1),(node_979.g*_Saturation))*node_979.b),_PrimaryOverlay_var.rgb,_PrimaryOverlay_var.a),_SecondaryOverlay_var.rgb,_SecondaryOverlay_var.a);
                o.Emission = (videoColour*_Emission);
                
                float3 diffColor = (videoColour*_Color.rgb);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metallic, specColor, specularMonochrome );
                float roughness = 1.0 - _Gloss;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:32719,y:32712,varname:node_2865,prsc:2|diff-853-OUT,spec-5035-G,gloss-1813-OUT,normal-5964-RGB,difocc-5035-G,spcocc-5035-G;n:type:ShaderForge.SFN_Tex2d,id:5964,x:32344,y:33017,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:1813,x:31995,y:32979,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_Set,id:98,x:31944,y:33326,varname:ScanLines,prsc:2|IN-5839-R;n:type:ShaderForge.SFN_Get,id:7720,x:31746,y:32595,varname:node_7720,prsc:2|IN-98-OUT;n:type:ShaderForge.SFN_TexCoord,id:2112,x:30387,y:32103,varname:node_2112,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:1527,x:30967,y:32236,ptovrint:False,ptlb:StatusTex,ptin:_StatusTex,varname:node_1527,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7818-UVOUT;n:type:ShaderForge.SFN_Parallax,id:7818,x:30726,y:32268,varname:node_7818,prsc:2|UVIN-2112-UVOUT,HEI-1919-OUT,DEP-7458-OUT,REF-1786-OUT;n:type:ShaderForge.SFN_Slider,id:1919,x:30230,y:32275,ptovrint:False,ptlb:StatusHeight,ptin:_StatusHeight,varname:node_1919,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:7458,x:30230,y:32366,ptovrint:False,ptlb:StatusDepth,ptin:_StatusDepth,varname:_StatusHeight_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Vector1,id:1786,x:30387,y:32447,varname:node_1786,prsc:2,v1:1;n:type:ShaderForge.SFN_Set,id:4289,x:31155,y:32236,varname:StatusMask,prsc:2|IN-1527-RGB;n:type:ShaderForge.SFN_Get,id:841,x:31467,y:32224,varname:node_841,prsc:2|IN-4289-OUT;n:type:ShaderForge.SFN_Multiply,id:180,x:31788,y:32153,varname:node_180,prsc:2|A-2558-RGB,B-841-OUT,C-9077-OUT;n:type:ShaderForge.SFN_Color,id:2558,x:31488,y:32066,ptovrint:False,ptlb:StatusColor,ptin:_StatusColor,varname:node_2558,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:9077,x:31331,y:31956,ptovrint:False,ptlb:StatusEmission,ptin:_StatusEmission,varname:node_9077,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Tex2d,id:5839,x:31647,y:33396,ptovrint:False,ptlb:ScreenMask,ptin:_ScreenMask,varname:node_5839,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:4730,x:32188,y:32444,varname:node_4730,prsc:2|A-180-OUT,B-7720-OUT;n:type:ShaderForge.SFN_Tex2d,id:5035,x:32120,y:32794,ptovrint:False,ptlb:MetallicTex,ptin:_MetallicTex,varname:node_5035,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:6629,x:29644,y:33148,varname:node_6629,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Sin,id:210,x:30413,y:32973,varname:node_210,prsc:2|IN-1488-OUT;n:type:ShaderForge.SFN_RemapRange,id:1522,x:30571,y:32973,varname:node_1522,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-210-OUT;n:type:ShaderForge.SFN_Multiply,id:1488,x:30253,y:32973,varname:node_1488,prsc:2|A-3234-OUT,B-7344-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7344,x:30049,y:33132,ptovrint:False,ptlb:FuzzSize,ptin:_FuzzSize,varname:node_7344,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Multiply,id:8365,x:30828,y:33042,varname:node_8365,prsc:2|A-1522-OUT,B-468-OUT,C-3066-OUT;n:type:ShaderForge.SFN_Noise,id:468,x:30571,y:33151,varname:node_468,prsc:2|XY-209-OUT;n:type:ShaderForge.SFN_Set,id:5095,x:31042,y:33042,varname:Noise,prsc:2|IN-8365-OUT;n:type:ShaderForge.SFN_Slider,id:3066,x:30414,y:33297,ptovrint:False,ptlb:NoiseStrength,ptin:_NoiseStrength,varname:node_3066,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2078988,max:1;n:type:ShaderForge.SFN_Add,id:853,x:32535,y:32573,varname:node_853,prsc:2|A-4730-OUT,B-6428-OUT;n:type:ShaderForge.SFN_Get,id:6428,x:32321,y:32675,varname:node_6428,prsc:2|IN-5095-OUT;n:type:ShaderForge.SFN_Add,id:209,x:29850,y:33171,varname:node_209,prsc:2|A-6629-UVOUT,B-4870-TSL;n:type:ShaderForge.SFN_Time,id:4870,x:29644,y:33299,varname:node_4870,prsc:2;n:type:ShaderForge.SFN_ComponentMask,id:3234,x:30049,y:32963,varname:node_3234,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-209-OUT;proporder:5839-5964-1527-9077-1813-1919-7458-2558-5035-7344-3066;pass:END;sub:END;*/

Shader "Shader Forge/StatusScreen" {
    Properties {
        _ScreenMask ("ScreenMask", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _StatusTex ("StatusTex", 2D) = "white" {}
        _StatusEmission ("StatusEmission", Range(0, 3)) = 1
        _Gloss ("Gloss", Range(0, 1)) = 0.8
        _StatusHeight ("StatusHeight", Range(0, 1)) = 0
        _StatusDepth ("StatusDepth", Range(0, 1)) = 0
        _StatusColor ("StatusColor", Color) = (0.5,0.5,0.5,1)
        _MetallicTex ("MetallicTex", 2D) = "white" {}
        _FuzzSize ("FuzzSize", Float ) = 10
        _NoiseStrength ("NoiseStrength", Range(0, 1)) = 0.2078988
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
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Gloss;
            uniform sampler2D _StatusTex; uniform float4 _StatusTex_ST;
            uniform float _StatusHeight;
            uniform float _StatusDepth;
            uniform float4 _StatusColor;
            uniform float _StatusEmission;
            uniform sampler2D _ScreenMask; uniform float4 _ScreenMask_ST;
            uniform sampler2D _MetallicTex; uniform float4 _MetallicTex_ST;
            uniform float _FuzzSize;
            uniform float _NoiseStrength;
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
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
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
                float4 _MetallicTex_var = tex2D(_MetallicTex,TRANSFORM_TEX(i.uv0, _MetallicTex));
                float3 specularAO = _MetallicTex_var.g;
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _MetallicTex_var.g;
                float specularMonochrome;
                float2 node_7818 = (_StatusDepth*(_StatusHeight - 1.0)*mul(tangentTransform, viewDirection).xy + i.uv0);
                float4 _StatusTex_var = tex2D(_StatusTex,TRANSFORM_TEX(node_7818.rg, _StatusTex));
                float3 StatusMask = _StatusTex_var.rgb;
                float3 node_180 = (_StatusColor.rgb*StatusMask*_StatusEmission);
                float4 _ScreenMask_var = tex2D(_ScreenMask,TRANSFORM_TEX(i.uv0, _ScreenMask));
                float ScanLines = _ScreenMask_var.r;
                float4 node_4870 = _Time;
                float2 node_209 = (i.uv0+node_4870.r);
                float2 node_468_skew = node_209 + 0.2127+node_209.x*0.3713*node_209.y;
                float2 node_468_rnd = 4.789*sin(489.123*(node_468_skew));
                float node_468 = frac(node_468_rnd.x*node_468_rnd.y*(1+node_468_skew.x));
                float Noise = ((sin((node_209.g*_FuzzSize))*0.5+0.5)*node_468*_NoiseStrength);
                float3 diffuseColor = ((node_180*ScanLines)+Noise); // Need this for specular when using metallic
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
                float3 indirectSpecular = (gi.indirect.specular) * specularAO;
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
                indirectDiffuse *= _MetallicTex_var.g; // Diffuse AO
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
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
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Gloss;
            uniform sampler2D _StatusTex; uniform float4 _StatusTex_ST;
            uniform float _StatusHeight;
            uniform float _StatusDepth;
            uniform float4 _StatusColor;
            uniform float _StatusEmission;
            uniform sampler2D _ScreenMask; uniform float4 _ScreenMask_ST;
            uniform sampler2D _MetallicTex; uniform float4 _MetallicTex_ST;
            uniform float _FuzzSize;
            uniform float _NoiseStrength;
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
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
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
                float4 _MetallicTex_var = tex2D(_MetallicTex,TRANSFORM_TEX(i.uv0, _MetallicTex));
                float3 specularColor = _MetallicTex_var.g;
                float specularMonochrome;
                float2 node_7818 = (_StatusDepth*(_StatusHeight - 1.0)*mul(tangentTransform, viewDirection).xy + i.uv0);
                float4 _StatusTex_var = tex2D(_StatusTex,TRANSFORM_TEX(node_7818.rg, _StatusTex));
                float3 StatusMask = _StatusTex_var.rgb;
                float3 node_180 = (_StatusColor.rgb*StatusMask*_StatusEmission);
                float4 _ScreenMask_var = tex2D(_ScreenMask,TRANSFORM_TEX(i.uv0, _ScreenMask));
                float ScanLines = _ScreenMask_var.r;
                float4 node_4870 = _Time;
                float2 node_209 = (i.uv0+node_4870.r);
                float2 node_468_skew = node_209 + 0.2127+node_209.x*0.3713*node_209.y;
                float2 node_468_rnd = 4.789*sin(489.123*(node_468_skew));
                float node_468 = frac(node_468_rnd.x*node_468_rnd.y*(1+node_468_skew.x));
                float Noise = ((sin((node_209.g*_FuzzSize))*0.5+0.5)*node_468*_NoiseStrength);
                float3 diffuseColor = ((node_180*ScanLines)+Noise); // Need this for specular when using metallic
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
            uniform float _Gloss;
            uniform sampler2D _StatusTex; uniform float4 _StatusTex_ST;
            uniform float _StatusHeight;
            uniform float _StatusDepth;
            uniform float4 _StatusColor;
            uniform float _StatusEmission;
            uniform sampler2D _ScreenMask; uniform float4 _ScreenMask_ST;
            uniform sampler2D _MetallicTex; uniform float4 _MetallicTex_ST;
            uniform float _FuzzSize;
            uniform float _NoiseStrength;
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
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float2 node_7818 = (_StatusDepth*(_StatusHeight - 1.0)*mul(tangentTransform, viewDirection).xy + i.uv0);
                float4 _StatusTex_var = tex2D(_StatusTex,TRANSFORM_TEX(node_7818.rg, _StatusTex));
                float3 StatusMask = _StatusTex_var.rgb;
                float3 node_180 = (_StatusColor.rgb*StatusMask*_StatusEmission);
                float4 _ScreenMask_var = tex2D(_ScreenMask,TRANSFORM_TEX(i.uv0, _ScreenMask));
                float ScanLines = _ScreenMask_var.r;
                float4 node_4870 = _Time;
                float2 node_209 = (i.uv0+node_4870.r);
                float2 node_468_skew = node_209 + 0.2127+node_209.x*0.3713*node_209.y;
                float2 node_468_rnd = 4.789*sin(489.123*(node_468_skew));
                float node_468 = frac(node_468_rnd.x*node_468_rnd.y*(1+node_468_skew.x));
                float Noise = ((sin((node_209.g*_FuzzSize))*0.5+0.5)*node_468*_NoiseStrength);
                float3 diffColor = ((node_180*ScanLines)+Noise);
                float specularMonochrome;
                float3 specColor;
                float4 _MetallicTex_var = tex2D(_MetallicTex,TRANSFORM_TEX(i.uv0, _MetallicTex));
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _MetallicTex_var.g, specColor, specularMonochrome );
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

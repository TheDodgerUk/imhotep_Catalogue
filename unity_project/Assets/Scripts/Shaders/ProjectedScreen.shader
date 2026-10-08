// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.015,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-9287-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32023,y:32659,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:9880,x:32023,y:32849,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_9880,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1d07d97b9e45dc049b19fb8ada691cf8,ntxv:0,isnm:False|UVIN-7724-UVOUT;n:type:ShaderForge.SFN_Multiply,id:9287,x:32353,y:32781,varname:node_9287,prsc:2|A-7241-RGB,B-9880-RGB,C-4045-OUT;n:type:ShaderForge.SFN_Slider,id:4045,x:32023,y:33033,ptovrint:False,ptlb:Emissive,ptin:_Emissive,varname:node_4045,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3,max:5;n:type:ShaderForge.SFN_TexCoord,id:5978,x:31350,y:32869,varname:node_5978,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_UVTile,id:7724,x:31651,y:33031,varname:node_7724,prsc:2|UVIN-5978-UVOUT,WDT-7755-OUT,HGT-550-OUT,TILE-8596-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7755,x:31358,y:33088,ptovrint:False,ptlb:Width,ptin:_Width,varname:node_7755,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:550,x:31358,y:33162,ptovrint:False,ptlb:Height,ptin:_Height,varname:_Width_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:8596,x:31358,y:33238,ptovrint:False,ptlb:Tile,ptin:_Tile,varname:_Height_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;proporder:7241-9880-4045-7755-550-8596;pass:END;sub:END;*/

Shader "Shader Forge/ProjectedScreen" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Emissive ("Emissive", Range(0, 5)) = 3
        _Width ("Width", Float ) = 1
        _Height ("Height", Float ) = 1
        _Tile ("Tile", Float ) = 0
		_ZBias ("Z Bias", Float) = -1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Cull Off
			Offset [_ZBias], [_ZBias]
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;

			UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _Emissive)
                UNITY_DEFINE_INSTANCED_PROP(float, _Width)
				UNITY_DEFINE_INSTANCED_PROP(float, _Height)
				UNITY_DEFINE_INSTANCED_PROP(float, _Tile)
            UNITY_INSTANCING_BUFFER_END(Props)
			
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
				UNITY_SETUP_INSTANCE_ID(i);
				float tile = UNITY_ACCESS_INSTANCED_PROP(Props, _Tile);
				float width = UNITY_ACCESS_INSTANCED_PROP(Props, _Width);
				float height = UNITY_ACCESS_INSTANCED_PROP(Props, _Height);
				float emissiveValue = UNITY_ACCESS_INSTANCED_PROP(Props, _Emissive);

                float2 node_7724_tc_rcp = float2(1.0,1.0)/float2( width, height );
                float node_7724_ty = floor(tile * node_7724_tc_rcp.x);
                float node_7724_tx = tile - width * node_7724_ty;
                float2 node_7724 = (i.uv0 + float2(node_7724_tx, node_7724_ty)) * node_7724_tc_rcp;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_7724, _MainTex));
                float3 emissive = (_Color.rgb*_MainTex_var.rgb*emissiveValue);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

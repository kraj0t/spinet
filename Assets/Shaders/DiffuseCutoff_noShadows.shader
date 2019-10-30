#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Custom/DiffuseCutoff_noShadows" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color1 ("Color 1", Color) = (1,1,1,1)
		_Color2 ("Color 2", Color) = (0.5,0.5,0.5,1)
		_Cutoff ("Cutoff", Range(0.0,1.0)) = 0.1
		_CutoffSmoothness ("Cutoff Smoothness", Range(0.0, 1.0)) = 0.0
		_LightColorTint ("Light Tint", Range(0.0,1.0)) = 0.5
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100
		
		Stencil
		{
			Ref 128
			Comp Always
			WriteMask 128
			Pass Replace
		}		
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			
			float4 _shadowVolumeSource;
			float _shadowVolumeSourceRange;
			float4 _shadowVolumeSourceLightColor;
			
			sampler2D _MainTex;
			half4 _Color1;
			half4 _Color2;
			half _Cutoff;		
			half _CutoffSmoothness;
			half _LightColorTint;
			
			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD;
			};

			struct VertexOutput
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD;
				float3 normal;
				float3 lightDirInv;
				float lightDistFalloff;
			};


			VertexOutput vert(VertexInput input)
			{
				VertexOutput output;				
				
				output.position = mul( UNITY_MATRIX_MVP, float4( input.vertex.xyz, 1.0 ) );
				output.uv = input.uv;
				output.normal = input.normal;
				
				float3 shadowSourceLS = mul(_World2Object, _shadowVolumeSource).xyz * 1.0;
				float3 vertexToLight = shadowSourceLS - input.vertex.xyz;
				float distToLight = length( vertexToLight );
				output.lightDirInv = vertexToLight / distToLight;								
				output.lightDistFalloff = 1.0 - min( 1.0, distToLight / _shadowVolumeSourceRange );
				
				return output;
			}

			float4 frag( VertexOutput input ) : COLOR
			{
				half4 DIFF = tex2D (_MainTex, input.uv );
				
				float3 normal = normalize( input.normal );
				float3 lightDirInv = normalize( input.lightDirInv );
				
				half NdotL = dot (normal, lightDirInv);
				half clampedNdotL = max( 0.0, NdotL );

				half dotAndFalloff = input.lightDistFalloff * NdotL;
				
				half hardCutoff = step(_Cutoff, dotAndFalloff);
				half actualSmoothedCutoff = hardCutoff * lerp(1.0, dotAndFalloff, _CutoffSmoothness);
								
				half4 ramp = lerp( _Color2, _Color1, actualSmoothedCutoff );

				half3 actualLightColor = lerp( half3(1.0f), _shadowVolumeSourceLightColor.rgb, _LightColorTint * hardCutoff );
				
				float4 c;				
				c.rgb = actualLightColor.rgb * DIFF.rgb * ramp.rgb;
				c.a = DIFF.a;
				
				return c;
			}
			ENDCG
		}
	}
			

	FallBack "Diffuse"
}


Shader "Custom/DiffuseCutoff" {
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
		
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _USING_DIRECTIONAL_LIGHT _USING_POINT_LIGHT
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			
			float4 _shadowVolumeSource;
			float _shadowVolumeSourceRange;
			float4 _shadowVolumeSourceLightColor;
			float3 _shadowVolumeSourceDirection;
			
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
				float3 normal : TEXCOORD2;
				float3 lightDirInv : TEXCOORD3;
				float lightDistFalloff : TEXCOORD4;
			};


			VertexOutput vert(VertexInput input)
			{
				VertexOutput output;				
				
				output.position = mul( UNITY_MATRIX_MVP, float4( input.vertex.xyz, 1.0 ) );
				output.uv = input.uv;
				output.normal = input.normal;
				
				float3 shadowSourceLS = mul(_World2Object, _shadowVolumeSource).xyz;
				
				#ifdef _USING_DIRECTIONAL_LIGHT
					output.lightDirInv = -mul(_World2Object, float4(_shadowVolumeSourceDirection, 0.0)).xyz;
					output.lightDistFalloff = 1.0;
				#endif
				
				#ifdef _USING_POINT_LIGHT
					float3 vertexToLight = shadowSourceLS - input.vertex.xyz;
					float distToLight = length( vertexToLight );
					output.lightDirInv = vertexToLight / distToLight;
					output.lightDistFalloff = 1.0 - min( 1.0, distToLight / _shadowVolumeSourceRange );
				#endif
				
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
dotAndFalloff = input.lightDistFalloff < _Cutoff ? 0.0 : NdotL;

				half hardCutoff = step(_Cutoff, dotAndFalloff);
				half actualSmoothedCutoff = hardCutoff * lerp(1.0, dotAndFalloff, _CutoffSmoothness);
								
				half4 ramp = lerp( _Color2, _Color1, actualSmoothedCutoff );

				half3 actualLightColor = lerp( half3(1,1,1), _shadowVolumeSourceLightColor.rgb, _LightColorTint * hardCutoff );
				
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

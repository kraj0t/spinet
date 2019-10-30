
Shader "Custom/DiffuseCutoff_bump" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
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
			Cull Back 
            //Lighting On
            Lighting Off
            
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
//			#include "Lighting.cginc"
uniform float4 _LightColor0;
			
			float4 _shadowVolumeSource;
			float _shadowVolumeSourceRange;
			float4 _shadowVolumeSourceLightColor;
			float3 _shadowVolumeSourceDirection;
			
			sampler2D _MainTex;
			float4 _MainTex_ST; // Required for TRANSFORM_TEX
			sampler2D _BumpMap;			
			float4 _BumpMap_ST; // Required for TRANSFORM_TEX
			half4 _Color1;
			half4 _Color2;
			half _Cutoff;		
			half _CutoffSmoothness;
			half _LightColorTint;
			
			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 uv : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float lightDistFalloff : TEXCOORD2;
				float3 lightDirectionTS : TEXCOORD3;
			};


			VertexOutput vert(VertexInput v)
			{
				VertexOutput output;				
				
				TANGENT_SPACE_ROTATION; // Creates the TBN float3x3 matrix called 'rotation'.
				
				output.position = mul( UNITY_MATRIX_MVP, float4( v.vertex.xyz, 1.0 ) );
				output.uv = TRANSFORM_TEX( v.uv, _MainTex );  
				output.uv2 = TRANSFORM_TEX( v.uv, _BumpMap );
				
				float3 shadowSourceLS = mul(_World2Object, _shadowVolumeSource).xyz;
				
				#ifdef _IS_DIRECTIONAL_LIGHT
					output.lightDirectionTS = mul(rotation, _);
					output.lightDistFalloff = 1.0;
				#else
					float3 vertexToLight = shadowSourceLS - v.vertex.xyz;
					float distToLight = length( vertexToLight );
					float3 lightDirectionLS = vertexToLight / distToLight;
					output.lightDirectionTS = mul(rotation, lightDirectionLS);
					output.lightDistFalloff = 1.0 - min( 1.0, distToLight / _shadowVolumeSourceRange );
				#endif
				
				return output;
			}

			float4 frag( VertexOutput input ) : COLOR
			{
				half4 DIFF = tex2D (_MainTex, input.uv );
				half4 NM = tex2D(_BumpMap, input.uv2);
				
				float3 normal = UnpackNormal( NM );
				float3 lightDirectionTS = normalize( input.lightDirectionTS );
				
				half NdotL = dot (normal, lightDirectionTS);
				half clampedNdotL = max( 0.0, NdotL );

				half dotAndFalloff = input.lightDistFalloff * NdotL;
				
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

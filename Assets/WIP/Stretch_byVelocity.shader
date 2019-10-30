Shader "Custom/Stretch by Velocity" {
	Properties {		
		_StretchAmount ("Stretch Amount", Float) = 1.0		
		_StretchExponent ("Stretch Exponent", Range(1.01, 2.0) ) = 1.1
		_SquashAmount ("Squash Amount", Float) = 0.2
		_MaxSquash01 ("Maximum Squash Normalized", Range(0.0,1.0)) = 0.4
		_AngularStretch ("Angular Stretch", Float) = 1.0
		_AngularStretchExponent ("Angular Stretch Exponent", Range(1.0, 8.0)) = 2.0
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			CGPROGRAM 
 
			#pragma vertex vert 
			#pragma fragment frag
			#include "UnityCG.cginc"
 

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct VertexOutput
			{
				float4 position : POSITION;
				float3 normal	: TEXCOORD2;
			};
						
			uniform float _StretchAmount;
			uniform float _StretchExponent;
			uniform float _SquashAmount;
			uniform float _MaxSquash01;
			uniform float _AngularStretch;
			uniform float _AngularStretchExponent;
			
			float3 _velocityOS;
			float3 _angularVelocityOS;

			VertexOutput vert( VertexInput input )
			{
				VertexOutput output;
				
				float3 finalPos = input.vertex.xyz;
				
				float3 toVertexDir;
				
				
				// ANGULAR STRETCH
				float3 angVelDir = normalize( _angularVelocityOS );
				toVertexDir = normalize( finalPos );
				float3 centrifugalDir = -cross( angVelDir, toVertexDir );
				float angVelMagnitude = length( _angularVelocityOS );
				float distToVertex = length( finalPos );
				float vertDotVel = dot( angVelDir, toVertexDir );
				float3 angularStretchVector = centrifugalDir;
				angularStretchVector *= _AngularStretch;
				angularStretchVector *= angVelMagnitude;
				angularStretchVector *= pow(distToVertex, _AngularStretchExponent);
				angularStretchVector *= vertDotVel * 0.5 + 0.5;
				float3 tempStretchedPos = finalPos + angularStretchVector;
				
				float3 newStretchDir = normalize( tempStretchedPos );
				finalPos = newStretchDir * distToVertex;				
				
				
				// VELOCITY STRETCH
				float3 stretchDir = normalize( -_velocityOS );
				float velMagnitude = pow( _StretchExponent, length( _velocityOS ) ) - 1.0f;				
				toVertexDir = normalize( finalPos );
				float NdotVel = dot( stretchDir, toVertexDir );
				float NdotVel_normalized = NdotVel * 0.5 + 0.5;
				float stretchAmount = _StretchAmount * velMagnitude * NdotVel_normalized;
//				float stretchAmount = _StretchAmount * velMagnitude * max(0.0, NdotVel);
//				float stretchAmount = _StretchAmount * velMagnitude * max(0.0, NdotVel) * NdotVel_normalized;
				float3 stretchVector = stretchDir * stretchAmount;				
//				finalPos += stretchVector;
				
				
				// VELOCITY SQUASH
toVertexDir = normalize( finalPos );
				float3 squashDir = -toVertexDir;
squashDir = cross( cross(toVertexDir,stretchDir), stretchDir );
squashDir=normalize(squashDir);
//float3 squashDir = -toVertexDir;
//				float squashAmount = _SquashAmount * velMagnitude * NdotVel_normalized;
float squashAmount = _SquashAmount * velMagnitude * (abs(dot(toVertexDir,squashDir)));
//float squashAmount = 0.1*_SquashAmount * _StretchAmount * velMagnitude * (abs(dot(toVertexDir,squashDir)));
//float squashAmount = 0.1*_SquashAmount * stretchAmount * (abs(dot(toVertexDir,squashDir)));
//				squashAmount = min( dot(input.vertex.xyz, toVertexDir) * squashAmount, _MaxSquash01 );
distToVertex = length( finalPos );
squashAmount = min( squashAmount, _MaxSquash01 * distToVertex * (abs(dot(toVertexDir,squashDir))));
				float3 squashVector = squashDir * squashAmount;
				
				
				finalPos += stretchVector;
				finalPos += squashVector;
				
				
				// TODO: maximum stretch
				
				// TODO: maximum angular stretch
				
				// TODO: the origin point should be a uniform
				
				// TODO: use vertex color for masking weights
				
				// TODO: use an array
				
				// TODO: leave each effect (stretch/squash/angular) as optional
												
				float4 finalPosPS = mul( UNITY_MATRIX_MVP, float4( finalPos, 1.0 ) );
								
				output.position = finalPosPS;
				output.normal = input.normal;
				return output;
			}
			
			float4 frag( VertexOutput input ) : COLOR
			{
				return float4( input.normal.xyz * 0.5 + 0.5, 1.0 );
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}


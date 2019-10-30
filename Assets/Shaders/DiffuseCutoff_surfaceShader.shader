Shader "Custom/DiffuseCutoff_surfaceShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color1 ("Color 1", Color) = (1,1,1,1)
		_Color2 ("Color 2", Color) = (0.5,0.5,0.5,1)
		_Cutoff ("Cutoff", Range(0.0,1.0)) = 0.5
		_LightFalloffAmount ("Light Falloff Amount", Range(0.0, 1.0)) = 1.0
		_AmbientCutoff ("Ambient Cutoff", Range(0.0,1.0)) = 0.0
		_LightColorTint ("Light Tint", Range(0.0,1.0)) = 0.5
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100
		
		/*
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			struct VertexInput
			{
				float4 vertex : POSITION;
			};

			struct VertexOutput
			{
				float4 position : POSITION;
			};


			VertexOutput vert(VertexInput input)
			{
				VertexOutput output;				
				output.position = mul( UNITY_MATRIX_MVP, float4( input.vertex.xyz, 1.0 ) );				
				return output;
			}

			float4 frag() : COLOR
			{
				return float4(1.0f, 1.0f, 1.0f, 1.0f);
			}
			ENDCG
		}
		*/
		
//		Pass
//		{
			CGPROGRAM		
			//#pragma target 3.0
			//#pragma profileoption MaxTexIndirections=256
			//#pragma surface surf Ramp fullforwardshadows
			//#pragma surface surf Ramp 
			//#pragma surface surf Ramp noambient novertexlights nolightmap nodirlightmap approxview halfasview 
			#pragma surface surf Ramp noambient 

			half4 _Color1;
			half4 _Color2;
			half _Cutoff;		
			half _LightFalloffAmount;
			half _AmbientCutoff;
			half _LightColorTint;

			half4 LightingRamp (SurfaceOutput s, half3 lightDir, fixed3 viewDir, half atten) 
			{
				half NdotL = dot (s.Normal, lightDir);
				half diff = NdotL * 0.5 + 0.5;
				half clampedNdotL = max( 0.0, NdotL );

				half dotAndAtten = atten * NdotL * 2.0;
				//half hardLight = step(_Cutoff, atten);
	half hardLight = step(_Cutoff, dotAndAtten);
				half softLight = dotAndAtten;
				half finalLight = lerp( 1.0, lerp( hardLight, softLight, _LightFalloffAmount ), _AmbientCutoff );

				//half4 ramp = lerp( _Color2, _Color1, step( _Cutoff, softLight ) );			
	half4 ramp = lerp( _Color2, _Color1, hardLight );
	
	//half4 ramp = _Color2 * atten;	
				
				float4 c;
//half3 actualLightColor = lerp( half3(1.0f), _LightColor0.rgb * 1.0, _LightColorTint );
half3 actualLightColor = half3( 0.5f );

//half actualAtten = lerp( 1.0, atten * 2.0, _LightFalloffAmount );
half actualAtten = 1.0;
				
				//c.rgb = (actualLightColor.rgb * s.Albedo * ramp.rgb) * (finalLight * 2.0);				
				//c.rgb = (actualLightColor.rgb * s.Albedo * ramp.rgb) * 2.0;
				
				
	c.rgb = ramp.rgb;
	//c.rgb = (actualLightColor.rgb * s.Albedo * ramp.rgb) * (atten * 2.0);
//	c.rgb = (actualLightColor.rgb * s.Albedo * ramp.rgb);
c.rgb = (actualLightColor.rgb * s.Albedo * ramp.rgb) * actualAtten;
//c.rgb = (actualLightColor.rgb * ramp.rgb) * actualAtten;
				//c.a = s.Alpha * ramp.a;
				c.a = s.Alpha;
//	c.a = ramp.a;			
	//c.a = 1.0;
//	if ( atten == 0.0f ) { 
//		//c.rgb = _Color2;
//		//c.rgb = UNITY_LIGHTMODEL_AMBIENT.rgb;
//		//c.rgb = float3( 0.0f, 0.0f, 1.0f );
//		c.rgb = (actualLightColor.rgb * s.Albedo * ramp.rgb) * actualAtten; 
//	} else {
//		c.rgb = _Color2.rgb;
//	}
	//c.rgb = float3(atten);
//c.rgb = float3( 0.01f );
				return c;		
			}

			struct Input {
				float2 uv_MainTex;
			};
			
			sampler2D _MainTex;		
			
			void surf (Input IN, inout SurfaceOutput o) {
				o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
//				o.Albedo = float3( 0.0f );
			}
			ENDCG
//		}
	} 
	

	FallBack "Diffuse"
}

Shader "Custom/DiffuseRamp" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (0,0,0,1)
		_Ramp ("Ramp (RGB)", 2D) = "white" {}
		_Cutoff ("Cutoff", Range(0.0,1.0)) = 0.001
		_SoftLightAmount ("Light Falloff Amount", Range(0.0, 1.0)) = 1.0
		_AmbientCutoff ("Ambient Cutoff", Range(0.0,1.0)) = 0.0
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 500
		
		CGPROGRAM
		#pragma target 3.0
		#pragma profileoption MaxTexIndirections=256
		#pragma surface surf Ramp fullforwardshadows

		sampler2D _Ramp;
		half4 _Color;
		half _Cutoff;		
		half _SoftLightAmount;
		half _AmbientCutoff;

		half4 LightingRamp (SurfaceOutput s, half3 lightDir, fixed3 viewDir, half atten) 
		{
			half NdotL = dot (s.Normal, lightDir);
			half diff = NdotL * 0.5 + 0.5;
			half clampedNdotL = max( 0.0, NdotL );

			half dotAndAtten = atten * NdotL * 2.0;
			//half hardLight = step(_Cutoff, atten);
half hardLight = step(_Cutoff, dotAndAtten);
			half softLight = dotAndAtten;
			half finalLight = lerp( 1.0, lerp( hardLight, softLight, _SoftLightAmount ), _AmbientCutoff );

			half4 ramp = tex2D ( _Ramp, float2(softLight, 0.5) );

			float4 c;
			c.rgb = (_LightColor0.rgb * s.Albedo * ramp.rgb) * (finalLight * 2.0);
			//c.rgb = (_LightColor0.rgb * s.Albedo * ramp.rgb) * 2.0;
			c.a = s.Alpha * ramp.a;
c.rgb = float3( 0.0f );

			return c;		
		}

		struct Input {
			float2 uv_MainTex;
		};
		
		sampler2D _MainTex;		
		
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb * tex2D (_MainTex, IN.uv_MainTex).rgb;
		}
		ENDCG
	} 
	

	FallBack "Diffuse"
}

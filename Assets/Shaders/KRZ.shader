Shader "KRZ" {
	Properties {
		_Color ("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex ("Color (RGBA)", 2D) = "white" {}
		_LightCutoff ("Light Cutoff", Range(0.0, 1.0)) = 0.2
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf KRZ fullforwardshadows

		sampler2D _MainTex;
		float4 _Color;
		float _LightCutoff;

		struct SurfaceOutputKRZ {
			fixed3 Albedo;
			fixed Alpha;
			fixed3 Emission;
			fixed3 Normal;
			fixed Specular;
		};

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutputKRZ o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = tex.rgb;
			o.Alpha = tex.a;
			o.Emission = fixed3(0.0,0.0,0.0); // Stop DX11 complaining.
		}

		inline fixed4 LightingKRZ (SurfaceOutputKRZ s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{
			atten = step(_LightCutoff, atten) * atten;

			float4 c;
			c.rgb = (_LightColor0.rgb * s.Albedo) * (atten * 2);
			c.a = s.Alpha;
			return c;
		}

		ENDCG
	}
	FallBack "VertexLit"
}
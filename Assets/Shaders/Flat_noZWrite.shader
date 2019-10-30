Shader "Custom/Flat_noZWrite" 
{

    Properties {
        _Color ("Color", Color) = (0,0,0,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader {

        Tags { "RenderType"="Opaque" }

        LOD 100

        Pass {
			Lighting Off
            ZWrite Off
            Cull Back
			
			Fog
			{
				Mode Off
			}			
			
            SetTexture [_MainTex] {
                constantColor [_Color]
                Combine texture * constant
            }
        }
    }
    
}
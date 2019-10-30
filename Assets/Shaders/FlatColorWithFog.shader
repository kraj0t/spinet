Shader "Custom/Flat Color with Fog" 
{

    Properties {
        _Color ("Color", Color) = (0,0,0,1)
    }

    SubShader {

        Tags { "RenderType"="Opaque" }

        LOD 100

        Pass {
			/*
            Stencil
			{
				CompBack Always
				PassBack IncrWrap
				CompFront Always
				PassFront IncrWrap
			}
			
			//Cull Off
			ZTest Always
			//ZWrite Off
			//Blend Zero One
			*/
			
			Lighting Off
            ZWrite On
            Cull Back
			
			Fog
			{
				Mode Global
			}
			
            SetTexture[_] {
                constantColor [_Color]
                Combine constant
            }
        }
    }
    
}
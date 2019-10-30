// Draws flat color, unaffected by fog or lighting or anything else.
// Writes 128 to the stencil buffer. Check stencil in latter drawcalls to avoid messing with the foreground.
Shader "Custom/Flat Foreground_stencil 128"
{

    Properties {
        _Color ("Color", Color) = (0,0,1,1)
    }

    SubShader {

        Tags { 
        	"RenderType"="Foreground" 
        	"Queue" = "Geometry-100"
        	"IgnoreProjector" = "True"
        	"ForceNoShadowCasting" = "True"
        }

		Stencil
		{
			//Ref 128
			Ref 192
			Comp Always
			//WriteMask 128
			WriteMask 192
			Pass Replace			
		}
		
		Color [_Color]

        LOD 100

        Pass {
			Lighting Off
            ZWrite On
            Cull Back
			
			Fog
			{
				Mode Off
			}
						
            //SetTexture [_] {
            //    constantColor [_Color]
            //}
        }
    }
    
}
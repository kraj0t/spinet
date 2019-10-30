Shader "Custom/Additive Stencil Volume" {
	Properties {
		_Color ("Color", Color) = (0,0,0,1)
	}

	SubShader {
		Tags { 
			//"RenderType"="Opaque" 
			//"Queue" = "Transparent+550"
			"Queue" = "AlphaTest+401"
			"IgnoreProjector" = "True"
		}
		
		LOD 200
				
		Lighting Off				
		//Offset 0,-1
		ZWrite Off		
			
		Fog
		{
			Mode Off
		}


		
		Pass {
			ColorMask 0
			Stencil {
				Ref 0
				ReadMask 128
				WriteMask 32
				Comp Equal
				Pass Invert
				Fail Zero
			}
			ZTest LEqual
			Cull Off
		}

		
		Pass {
			Stencil {
				Ref 32
				ReadMask 160
				Comp Equal
			}
			Cull Front
			ZTest GEqual
			Blend SrcAlpha One
			//Blend One Zero
			Color [_Color]			
		}
		
		
		Pass {
			Stencil {
				Ref 128
				//ReadMask 160 // 32 + 128
				ReadMask 128
				//WriteMask 32
				WriteMask 160
				Comp NotEqual
				//Comp Always
				Pass Replace
				//Fail Zero
			}
			Cull Back
			ZTest LEqual
			Blend SrcAlpha One
			Color [_Color]
		}
		
	} 
	
	FallBack Off
}

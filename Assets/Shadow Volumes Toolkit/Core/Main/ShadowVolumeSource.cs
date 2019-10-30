// Shadow Volumes Toolkit
// Copyright 2013 Gustav Olsson
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class ShadowVolumeSource : MonoBehaviour
{
	private static string colorPropertyName = "_shadowVolumeColor";
	private static string sourcePropertyName = "_shadowVolumeSource";
	private static string extrudeBiasPropertyName = "_shadowVolumeExtrudeBias";
	private static string extrudeAmountPropertyName = "_shadowVolumeExtrudeAmount";
	private static string rangePropertyName = "_shadowVolumeSourceRange";
	private static string lightColorPropertyName = "_shadowVolumeSourceLightColor";
	private static string falloffPropertyName = "_shadowVolumeFalloff";
	private static string lightDirectionPropertyName = "_shadowVolumeSourceDirection";
	private static string directionalLightDefineName = "_USING_DIRECTIONAL_LIGHT";
	private static string pointLightDefineName = "_USING_POINT_LIGHT";
	private static string shadow2dExtrudeDefineName = "_USING_2D_SHADOW_EXTRUDE";

	// The shadow color used to determine the color and strength of the shadow volumes controlled
	// by this light. 
	// The RGB channels decide the color while the A channel controls the shadow strength. 
	// The final screen color will be linearly interpolated from the original scene color to 
	// the shadow color using the shadow strength value.
	public Color shadowColor = new Color(0.0f, 0.0f, 0.0f, 0.5f);

	// The extrude bias used to offset the shadow volumes in the light direction. 
	// When no extrude bias is used the shadow volume is rendered at the same location as 
	// the shadow caster and z-fighting occurs.
	public float extrudeBias = 0.03f;

	// Used to decide how far the shadow volumes will reach from the original mesh
	public float extrudeDistance = 1.0f;

	public float falloff = 1.0f;
	
	public void Update()
	{
		// Set light properties
		Vector4 source;
		
		if (GetComponent<Light>().type == LightType.Directional)
		{
			Vector3 direction = -GetComponent<Light>().transform.forward;
			
			source = new Vector4(direction.x, direction.y, direction.z, 0.0f);
		}
		else
		{
			Vector3 position = GetComponent<Light>().transform.position;
			
			source = new Vector4(position.x, position.y, position.z, 1.0f);
		}
		
		// Shadow Volume properties
		Shader.SetGlobalVector(sourcePropertyName, source);
		Shader.SetGlobalFloat(extrudeBiasPropertyName, extrudeBias);
		Shader.SetGlobalFloat(extrudeAmountPropertyName, extrudeDistance);
		
		// Renderer properties
		Shader.SetGlobalColor(colorPropertyName, shadowColor);

		Shader.SetGlobalFloat(rangePropertyName, GetComponent<Light>().type == LightType.Directional ? float.PositiveInfinity : GetComponent<Light>().range);
		if ( GetComponent<Light>().type == LightType.Directional ) {
			Shader.SetGlobalVector( lightDirectionPropertyName, GetComponent<Light>().transform.forward );
			Shader.EnableKeyword( directionalLightDefineName );
			Shader.DisableKeyword( pointLightDefineName );
			Shader.DisableKeyword( shadow2dExtrudeDefineName );
		} else if ( GetComponent<Light>().type == LightType.Point ) {
			Shader.DisableKeyword( directionalLightDefineName );
			Shader.EnableKeyword( pointLightDefineName );
			Shader.DisableKeyword( shadow2dExtrudeDefineName );
		} else if ( GetComponent<Light>().type == LightType.Spot ) {
			Shader.DisableKeyword( directionalLightDefineName );
			Shader.EnableKeyword( pointLightDefineName );
			Shader.EnableKeyword( shadow2dExtrudeDefineName );
		}
		Shader.SetGlobalColor(lightColorPropertyName, GetComponent<Light>().color);
		Shader.SetGlobalFloat(falloffPropertyName, falloff);
	}
}
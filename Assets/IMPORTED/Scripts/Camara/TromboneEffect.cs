using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class TromboneEffect : MonoBehaviour {
	
	public Transform target;
	public bool useTrombone = false;
	
	private float _initHeightAtDist;
	private bool _wasUsingTrombone;

	
	// Calculate the frustum height at a given distance from a camera.
	public static float FrustumHeightAtDistance( float distance, Camera cam ) 
	{
		return 2f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
	}
	
	// Calculate the FOV needed to get a given frustum height at a given distance.
	public static float FOVForHeightAndDistance( float height, float distance ) 
	{
		return 2f * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg;
	}
			
	
	void Update () 
	{
		// Should the effect start?
		if ( !_wasUsingTrombone && useTrombone )
		{
			float distance = Vector3.Distance( transform.position, target.position );
			_initHeightAtDist = FrustumHeightAtDistance( distance, GetComponent<Camera>() );
		}
		
		if (useTrombone) 
		{
			// Measure the new distance and readjust the FOV accordingly.
			float currDistance = Vector3.Distance( transform.position, target.position );
			GetComponent<Camera>().fieldOfView = FOVForHeightAndDistance( _initHeightAtDist, currDistance );
		}
		
		_wasUsingTrombone = useTrombone;
	}
	
}


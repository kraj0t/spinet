using UnityEngine;
using System.Collections;

public class AcelerometroSuavizado : MonoBehaviour {
	
	public float accelerometerUpdateInterval = 1f / 60f;
	public float lowPassKernelWidthInSeconds = 1f;

	private float _lowPassFilterFactor;
	private Vector3 _lowPassValue;
	
	// Use this for initialization
	void Start () {
		_lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
		_lowPassValue = Input.acceleration;
	}
	
	// Update is called once per frame
	void Update () {
		_lowPassValue = Vector3.Lerp( _lowPassValue, Input.acceleration, _lowPassFilterFactor );
		Debug.Log( "INCLINACION: " + _lowPassValue.ToString() );
	}
	
	
	public Vector3 inclinacion
	{
		get { return _lowPassValue; }
	}
}



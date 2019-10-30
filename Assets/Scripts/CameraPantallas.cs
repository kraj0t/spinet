using UnityEngine;
using System.Collections;
using System;

// Camara simple estatica. 
// Cambia de posicion al llegar al borde de la pantalla.
[ExecuteInEditMode()]
public class CameraPantallas : MonoBehaviour {
	
	[Serializable]
	public class ZoomParametros
	{
		public bool enabled = false;
		public Transform target;
		public float distancia = 10f;
		public float fov = 50f;
		
		
		public float tiempoTransicion = 1f;
		
		internal float _timeStarted = -999f;		
	}
	
	
	public Transform target;
	
	public float pantallaAncho = 150f;
	public float pantallaAlto = 100f;
	
	public float distancia = 35.0f;

	public Vector3 offsetPos = Vector3.zero;
	public Vector3 offsetRot = Vector3.zero;
	
	public float fov = 20f;
	
	// El tiempo que tarda la camara en cambiar entre pantalla.
	public float tiempoCambioPantalla = 0.5f;
	
	public float tiempoCambioFov = 1f;
	
	public ZoomParametros zoom;
	
	public float trackingY = 0.5f;
	public float trackingZ = 0.5f;
	
	
	// Use this for initialization
	void Start () {
		Vector3 _centroPos = getCameraPositionByPos( target.position );
		transform.position = _centroPos;
		Vector3 _lookAtTgt = _centroPos;
		_lookAtTgt.x = target.transform.position.x;
		_lookY = _lookAtTgt.y;
		_lookZ = _lookAtTgt.z;
		transform.LookAt( _lookAtTgt );
		_y = target.transform.position.y;
		_z = target.transform.position.z;
		_camPos = transform.position;
	}
	protected float _y;
	protected float _z;

	protected Vector3 _camPos;

	protected float _lookY;
	protected float _lookZ;

	// Update is called once per frame
	void Update () {
		LimitarAspectRatio();
		Cursor.visible = false;
		
		Vector3 _pos = getCameraPositionByPos( target.position );

		_y = _pos.y + ( ( target.transform.position.y - _pos.y ) * trackingY );
		_z = _pos.z + ( ( target.transform.position.z - _pos.z ) * trackingZ );
		if ( !zoom.enabled )
		{
#if UNITY_EDITOR
			GetComponent<Camera>().fieldOfView = fov;
#else
			GetComponent<Camera>().fieldOfView = Mathf.Lerp( GetComponent<Camera>().fieldOfView, fov, tiempoCambioFov );
#endif
			
			float _centroY = _pos.y;
			float _centroZ = _pos.z;
			_pos.y = _y;
			_pos.z = _z;
			_pos += offsetPos;
#if UNITY_EDITOR
			transform.position = _pos;
#else
			transform.position = Vector3.Lerp( transform.position, _pos, tiempoCambioPantalla );
#endif
			Vector3 _lookAtTgt = transform.position;
			_lookAtTgt.x = target.position.x;
			_lookY = _centroY;
			_lookZ = _centroZ;
			_lookAtTgt.y = _lookY;
			_lookAtTgt.z = _lookZ;
			transform.LookAt( _lookAtTgt );
			transform.rotation *= Quaternion.Euler( offsetRot );
		}
		else
		{
			Vector3 _posZoom = zoom.target.position;
			_posZoom.x += zoom.distancia;
			transform.position = Vector3.Lerp( transform.position, _posZoom, zoom.tiempoTransicion );
			GetComponent<Camera>().fieldOfView = Mathf.Lerp( GetComponent<Camera>().fieldOfView, zoom.fov, zoom.tiempoTransicion );
		}
	}
	
	
	public Vector3 getCameraPositionByPos ( Vector3 pos )
	{
		return new Vector3
		(
			pos.x + distancia,
			Mathf.Floor( pos.y / pantallaAlto ) * pantallaAlto + ( pantallaAlto / 2 ),
			Mathf.Floor( pos.z / pantallaAncho ) * pantallaAncho + ( pantallaAncho / 2 )
		);
	}
	
	
	protected void LimitarAspectRatio ()
	{
		// set the desired aspect ratio (the values in this example are
	    // hard-coded for 16:9, but you could make them into public
	    // variables instead so you can set them at design time)
	    float targetaspect = pantallaAncho / pantallaAlto;
			
	    // determine the game window's current aspect ratio
	    float windowaspect = (float)Screen.width / (float)Screen.height;
	
	    // current viewport height should be scaled by this amount
	    float scaleheight = windowaspect / targetaspect;
	
	    // if scaled height is less than current height, add letterbox
	    if (scaleheight < 1.0f)
	    {  
	        Rect rect = GetComponent<Camera>().rect;
	
	        rect.width = 1.0f;
	        rect.height = scaleheight;
	        rect.x = 0;
	        rect.y = (1.0f - scaleheight) / 2.0f;
	
	        GetComponent<Camera>().rect = rect;
	    }
	    else // add pillarbox
	    {
	        float scalewidth = 1.0f / scaleheight;
	
	        Rect rect = GetComponent<Camera>().rect;
	
	        rect.width = scalewidth;
	        rect.height = 1.0f;
	        rect.x = (1.0f - scalewidth) / 2.0f;
	        rect.y = 0;
	
	        GetComponent<Camera>().rect = rect;
	    }
	}
}


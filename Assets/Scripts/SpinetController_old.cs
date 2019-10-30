using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class SpinetController_old : MonoBehaviour {
	[Serializable]
	public class RuedasVariables
	{
		public bool inputEnabled = true;
		
		// Velocidad angular maxima para las ruedas.
		// Este es el valor maximo para la fisica de las ruedas, no para el control.
		public float RPMMax = 100f;
		
		// Velocidad angular mas alla de la cual el motor no sera capaz de acelerar.
		// Si las ruedas giran muy rapido, no aceleraremos.
		public float RPMMaxMotor = 35f;
		
		public float potencia = 1500f;
		public float freno = 3000f;
		public float perdidaDeVelocidad = 200f;
		
	//	public float potenciaAgachado = 1200.0f;
		// RPM maximas cuando se va agachado. Si se va a mas de esto, se frena.
	//	public float maxRPMAgachado = 100f;
		
	//	public float idleFreno = 200.0f;
	//	public float idleFrenoAgachado = 800.0f;
		
	//	public float delayMaxSaltoAlPulsar = 0.3f;
		
	//	public float suspensionNormal = 1.0f;
	//	public float suspensionAgachado = 0.3f;
	//	public float suspensionSalto = 2.0f;
		
	//	public float delayMinAgachadoSalto = 0.25f;
	}
	
	[Serializable]
	public class PinchosVariables
	{
		public bool inputEnabled = true;
		
		// En que superficies nos podemos pinchar.
		public LayerMask colisionesPinchables;
		
		// Distancia hasta la que se hace el raycast para buscar el terreno.
		public float distanciaRayoTerreno = 3f;
		
		public float gravedadFake = 500f;
		
		// Velocidad angular maxima para las ruedas.
		// Este es el valor maximo para la fisica de las ruedas, no para el control.
		public float RPMMax = 100f;
		
		// Velocidad angular mas alla de la cual el motor no sera capaz de acelerar.
		// Si las ruedas giran muy rapido, no aceleraremos.
		public float RPMMaxMotor = 35f;
		
		public float potencia = 1500.0f;
		public float freno = 9999f;
	
		internal bool _pinchosFuera = false;
		internal bool _clavados = false;
	}
	
	[Serializable]
	public class SaltoVariables
	{
		public bool inputEnabled = true;
		
		// Sobre que colisiones se puede saltar.
		public LayerMask colisionesLayer;
		
		// Distancia hasta la que se hace el raycast para la inclinacion del terreno
		public float distanciaRayoTerreno = 3f;
			
		// Velocidad del salto cuando se pulsa. Es el salto mínimo.
		public float velInicial = 50.0f;
		
		// Velocidad del hover del salto por segundo.
		public float hoverVel = 200f;
		
		public float hoverDuracion = 0.25f;
		
		// Tiempo en segundos que el juego recuerda si se ha pulsado el salto
		public float delayPulsacion = 0.15f;
		
		// Si la pendiente del terreno tiene mas inclinacion, entonces no se puede saltar.
		public float pendienteMax = 46f;
		
		internal bool _saltando = false;
		internal bool _pulsadoAnteriorFrame = false;
		internal float _timeLastPulsado = float.NegativeInfinity;
		internal float _timeLastHecho = float.NegativeInfinity;
	}
	
	[Serializable]
	public class GiroVariables 
	{
		public bool inputEnabled = true;
		
		public float acel = 1000.0f;
		public float decel = 100f;
		
		// Controla cuan rapido vuelve el coche a orientarse con la vertical.
		public float muelleVueltaVertical = 1.25f;
	}
	
	[Serializable]
	public class JetpackVariables 
	{
		public bool inputEnabled = true;
		
		public float acel = 250f;
	
		// El jetpack no puede permitir alcanzar velocidades mayores que esta.
		public float velMax = 50f;
	}
	
	[Serializable]
	public class TraslacionAireVariables
	{
		public bool inputEnabled = true;
		
		// Velocidad en horizontal maxima a la que se puede llegar gracias al control en el aire.
		public float velMax = 20f;
		
		// Se usa este valor cuando se acelera en el sentido actual del desplazamiento.
		public float acel = 40f;
		
		// Se usa este valor cuando se acelera en sentido contrario del desplazamiento.
		public float freno = 70f;
	}

	[Serializable]
	public class NitroVariables
	{
		public bool inputEnabled = true;
		
		public float acel = 250f;
		
		// El nitro no puede permitir alcanzar velocidades mayores que esta.
		public float velMax = 50f;
	}

	[Serializable]
	public class ReverseJetpackVariables 
	{
		public bool inputEnabled = true;
		
		public float acel = 250f;
		
		// El jetpack no puede permitir alcanzar velocidades mayores que esta.
		public float velMax = 50f;
	}
		
	
	
	public Checkpoint checkpointActual;
	
	public CollisionRecorder chasis;
	public CollisionRecorder ejeDelantero;
	public CollisionRecorder ejeTrasero;	
	
	// Bajarlo hará que el coche vuelque menos.
	public float centroDeMasaOffset = -0.75f;
	
	// Velocidad angular maxima para el chasis
	public float chasisVelMaxGiro;
	
	// La velocidad maxima a la que se desplazara el coche, en cualquier direccion.
	// Esto le pone las cosas faciles a la fisica e impide velocidades locas.
	public float velMaxFisica = 100f;	
	
	public RuedasVariables ruedas;
	
	public PinchosVariables pinchos;
	
	public SaltoVariables salto;
	
	public GiroVariables giro;
	
	public JetpackVariables jetpack;
	
	public TraslacionAireVariables traslacionAire;


	public JetpackController nitroJetpackLeft;
	public JetpackController nitroJetpackRight;
	
	
	
	
	private Rigidbody _chasisRB;
	private Rigidbody _ejeDelanteroRB;
	private Rigidbody _ejeTraseroRB;
	
		
	
	public bool IsAlgunaRuedaEnSuelo ()
	{
		return ( ejeDelantero.GetColisiones().Length != 0 || ejeTrasero.GetColisiones().Length != 0 );
	}
	
	public bool IsChasisChocando ()
	{
		return ( chasis.GetColisiones().Length != 0);
	}
	
	public bool IsAmbasRuedasEnSuelo ()
	{
		return ( ejeDelantero.GetColisiones().Length != 0 && ejeTrasero.GetColisiones().Length != 0 );
	}
	
	// Mueve al coche al checkpoint.
	public void ResetToCheckpoint()
	{
		_chasisRB.velocity = _chasisRB.angularVelocity = 
			_ejeDelanteroRB.velocity = _ejeDelanteroRB.angularVelocity = 
			_ejeTraseroRB.velocity = _ejeTraseroRB.angularVelocity = Vector3.zero;
		
		_chasisRB.isKinematic = _ejeDelanteroRB.isKinematic = _ejeTraseroRB.isKinematic = true;
		
		
		_chasisRB.GetComponent<Collider>().enabled = false;
		_ejeDelanteroRB.GetComponent<Collider>().enabled = false;
		_ejeTraseroRB.GetComponent<Collider>().enabled = false;
		
		_chasisRB.detectCollisions = false;
		_ejeDelanteroRB.detectCollisions = false;
		_ejeTraseroRB.detectCollisions = false;
		
		Vector3 _offset = checkpointActual.transform.position - _chasisRB.position;
		transform.position += _offset;
		transform.rotation.SetFromToRotation( chasis.transform.forward, checkpointActual.transform.forward );
		
		_chasisRB.isKinematic = _ejeDelanteroRB.isKinematic = _ejeTraseroRB.isKinematic = false;
		
		_chasisRB.GetComponent<Collider>().enabled = true;
		_ejeDelanteroRB.GetComponent<Collider>().enabled = true;
		_ejeTraseroRB.GetComponent<Collider>().enabled = true;
		
		_chasisRB.detectCollisions = true;
		_ejeDelanteroRB.detectCollisions = true;
		_ejeTraseroRB.detectCollisions = true;
		
//		_chasisRB.position = checkpointActual.transform.position;
		
//		_chasisRB.MovePosition( _chasisRB.position + _offset );
//		_ejeDelanteroRB.MovePosition( _chasisRB.position + _offset );
//		_ejeTraseroRB.MovePosition( _chasisRB.position + _offset );
//		
//		_chasisRB.MoveRotation( checkpointActual.transform.rotation );
		
//		_chasisRB.rotation = checkpointActual.transform.rotation;
//		
//		_chasisRB.position += _offset;
//		_ejeDelanteroRB.position += _offset;
//		_ejeTraseroRB.position += _offset;
		
	}
	
	
	
	// Use this for initialization
	void Start () 
	{
		_chasisRB = chasis.GetComponent<Rigidbody>();
		_ejeDelanteroRB = ejeDelantero.GetComponent<Rigidbody>();
		_ejeTraseroRB = ejeTrasero.GetComponent<Rigidbody>();
		
		Physics.IgnoreCollision( _chasisRB.GetComponent<Collider>(), _ejeDelanteroRB.GetComponent<Collider>(), true );
		Physics.IgnoreCollision( _chasisRB.GetComponent<Collider>(), _ejeTraseroRB.GetComponent<Collider>(), true );
		
		// NO TENGO CLARO QUE ESTO MEJORE EL COMPORTAMIENTO FISICO...
		// Y HAY GENTE QUE SE QUEJA POR INTERNET :-S
		//ejeDelantero.useConeFriction = true;
		//ejeTrasero.useConeFriction = true;
	}
	
	void OnDrawGizmos ()
	{
		if ( pinchos._pinchosFuera )
		{
			float _____eseRayo = 2f;
			Gizmos.DrawRay( ejeDelantero.transform.position, ejeDelantero.transform.up * _____eseRayo );
			Gizmos.DrawRay( ejeDelantero.transform.position, ejeDelantero.transform.forward * _____eseRayo );
			Gizmos.DrawRay( ejeDelantero.transform.position, -ejeDelantero.transform.forward * _____eseRayo );
			Gizmos.DrawRay( ejeDelantero.transform.position, -ejeDelantero.transform.up * _____eseRayo );
			Gizmos.DrawRay( ejeDelantero.transform.position, ( ejeDelantero.transform.up + ejeDelantero.transform.forward ).normalized * _____eseRayo );
			Gizmos.DrawRay( ejeDelantero.transform.position, ( ejeDelantero.transform.up - ejeDelantero.transform.forward ).normalized * _____eseRayo );
			Gizmos.DrawRay( ejeDelantero.transform.position, ( - ejeDelantero.transform.up - ejeDelantero.transform.forward ).normalized * _____eseRayo );
			Gizmos.DrawRay( ejeDelantero.transform.position, ( - ejeDelantero.transform.up + ejeDelantero.transform.forward ).normalized * _____eseRayo );
			Gizmos.DrawRay( ejeTrasero.transform.position, ejeTrasero.transform.up * _____eseRayo );
			Gizmos.DrawRay( ejeTrasero.transform.position, ejeTrasero.transform.forward * _____eseRayo );
			Gizmos.DrawRay( ejeTrasero.transform.position, -ejeTrasero.transform.forward * _____eseRayo );
			Gizmos.DrawRay( ejeTrasero.transform.position, -ejeTrasero.transform.up * _____eseRayo );
			Gizmos.DrawRay( ejeTrasero.transform.position, ( ejeTrasero.transform.up + ejeTrasero.transform.forward ).normalized * _____eseRayo );
			Gizmos.DrawRay( ejeTrasero.transform.position, ( ejeTrasero.transform.up - ejeTrasero.transform.forward ).normalized * _____eseRayo );
			Gizmos.DrawRay( ejeTrasero.transform.position, ( - ejeTrasero.transform.up - ejeTrasero.transform.forward ).normalized * _____eseRayo );
			Gizmos.DrawRay( ejeTrasero.transform.position, ( - ejeTrasero.transform.up + ejeTrasero.transform.forward ).normalized * _____eseRayo );
		}
	}
	
	void OnGUI()
	{
		GUILayout.Label("Vel = " + _chasisRB.GetComponent<Rigidbody>().velocity.ToString() );
		GUILayout.Label("Vel magnitud = " + _chasisRB.GetComponent<Rigidbody>().velocity.magnitude.ToString() );
		GUILayout.Label("eje del av = " + _ejeDelanteroRB.angularVelocity.ToString() );
		GUILayout.Label("chasis av = " + _chasisRB.angularVelocity.ToString() );
		
		RaycastHit _hitInfo;
		if ( Physics.Raycast( _chasisRB.position, -_chasisRB.transform.up, out _hitInfo, salto.distanciaRayoTerreno, salto.colisionesLayer.value ) )
			GUILayout.Label( "Normal del terreno: " + Vector3.Angle( _hitInfo.normal, Vector3.up ).ToString() );
		else
			GUILayout.Label( "Normal del terreno: " );
		
		GUILayout.Label("HA DE FRENAR DEL TODO INCLUSO EN CUESTAS POCO EMPINADAS");		
		GUILayout.Label("¿SALTAR SEGUN LA NORMAL DEL TERRENO? ¿O SIEMPRE PARRIBA?");	
		GUILayout.Label("¿SEGUN LA INCLINACION SALTAS MAS O MENOS? ¿HAY UN MAXIMO DE INCLINACION PARA SALTAR?");	
		GUILayout.Label("PONER PROYECTO JUNKGINE EN linkedIn");	
		GUILayout.Label("USAR MUELLE PARA LAS MALLAS GRAFICAS.");
		GUILayout.Label("ESTUDIA PATRONES DE MOVIMIENTO ALEATORIO Y TAL");
		GUILayout.Label("MIRA A VER LAS ESCENAS QUE HAY DE EJEMPLO DE ITWEEN Y TAL");	
		GUILayout.Label("LAS RUEDAS DEJAN DE GIRAR MUY RAPIDO EN EL AIRE. HAS DE USAR TORQUE Y NO ANGULAR VELOCITY");	
		GUILayout.Label("EL COCHE PEGA UN BOTECITO CUANDO CAE AL SUELO. EVITALO. Y SE INCRUSTA SI VA MU RAPIDO :-/");	
	
		
		
//		GUILayout.Label("KEYS:");	
//		GUILayout.Label("  A and D -> accelerate");	
//		GUILayout.Label("  W -> jump");	
//		GUILayout.Label("  S -> wheel spikes");	
//		GUILayout.Label("  Left and Right -> turn");	
//		GUILayout.Label("  Up -> Jetpack");	
	}


	void Update ()
	{
		float moverHorizInput = Input.GetAxis("moverHorizontal");
		float nitroInput = Input.GetAxis( "nitro" );
		nitroJetpackLeft.desiredForce = Mathf.Max( 0f, moverHorizInput * nitroInput );
		nitroJetpackRight.desiredForce = Mathf.Max( 0f, -moverHorizInput * nitroInput );
	}

	
	void FixedUpdate ()
	{
		_chasisRB.centerOfMass = new Vector3(0, centroDeMasaOffset, 0 );
		_chasisRB.maxAngularVelocity = chasisVelMaxGiro;
		_ejeDelanteroRB.maxAngularVelocity = ruedas.RPMMax;
		_ejeTraseroRB.maxAngularVelocity = ruedas.RPMMax;
		
		
		actualizarPinchos();
		
		if ( !pinchos._clavados )
		{
			actualizarGravedad();
			
			actualizarRuedas();
			
			actualizarSalto();
		}
			
		actualizarGiro();
		
		actualizarJetpack();
		
		actualizarTraslacionAire();	
		
		_chasisRB.velocity = Vector3.ClampMagnitude(_chasisRB.velocity, velMaxFisica);
		_ejeDelanteroRB.velocity = Vector3.ClampMagnitude(_ejeDelanteroRB.velocity, velMaxFisica);
		_ejeTraseroRB.velocity = Vector3.ClampMagnitude(_ejeTraseroRB.velocity, velMaxFisica);
	}
	
	

	
	
	
	
	private void actualizarPinchos ()
	{
		bool pinchosInput = pinchos.inputEnabled && Input.GetButton("pinchos");
		
		if ( !pinchosInput )
		{
			if ( pinchos._clavados )
			{
				// TODO: DESCLAVARSE DE LA PARED!!!!
				pinchos._clavados = false;
			}
		}
		else
		{
//			if ( pinchos._clavados )
//			{
//				RaycastHit _hitInfo;
//				if ( Physics.Raycast( _chasisRB.position, -_chasisRB.transform.up, out _hitInfo, pinchos.distanciaRayoTerreno, pinchos.colisionesPinchables.value ) )
//				{
//					Vector3 _normal = _hitInfo.normal;
//					
////					// HAZ QUE EL COCHE SE PEGUE AL TERRENO
////					_chasisRB.rotation.SetFromToRotation( _chasisRB.transform.up, _normal );
////					
////					_chasisRB.velocity = _ejeDelanteroRB.velocity = _ejeTraseroRB.velocity = 
////						_chasisRB.angularVelocity = _ejeDelanteroRB.angularVelocity = _ejeTraseroRB.angularVelocity =
////							Vector3.zero;
//					
//					_chasisRB.AddForce( -_normal * pinchos.gravedadFake, ForceMode.Acceleration );
//					
//					Debug.DrawRay( _chasisRB.position, -_normal * 10f, Color.black, 0f, false );
//					
//					// MOVIMIENTO DE RUEDAS POR LA SUPERFICIE					
//					float _moverHorizInput = Input.GetAxis("moverHorizontal");
//					
//					Vector3 _ejeDelLocalAV = _ejeDelanteroRB.transform.InverseTransformDirection( _ejeDelanteroRB.angularVelocity );;
//					Vector3 _ejeTrasLocalAV = _ejeTraseroRB.transform.InverseTransformDirection( _ejeTraseroRB.angularVelocity );;
//					if ( _moverHorizInput == 0 || !pinchos.inputEnabled )
//					{
//						_ejeDelLocalAV.x = Mathf.Sign( _ejeDelLocalAV.x ) * Mathf.Max( 0f, Mathf.Abs( _ejeDelLocalAV.x ) - ( pinchos.freno * Time.fixedDeltaTime ) );
//						_ejeTrasLocalAV.x = Mathf.Sign( _ejeTrasLocalAV.x ) * Mathf.Max( 0f, Mathf.Abs( _ejeTrasLocalAV.x ) - ( pinchos.freno * Time.fixedDeltaTime ) );
//					}
//					else
//					{
//						if ( ( _moverHorizInput < 0 && _ejeDelLocalAV.x > -pinchos.RPMMaxMotor ) || ( _moverHorizInput > 0 && _ejeDelLocalAV.x < pinchos.RPMMaxMotor ) ) 
//							_ejeDelLocalAV.x += pinchos.potencia * _moverHorizInput * Time.fixedDeltaTime;
//						if ( ( _moverHorizInput < 0 && _ejeTrasLocalAV.x > -pinchos.RPMMaxMotor ) || ( _moverHorizInput > 0 && _ejeTrasLocalAV.x < pinchos.RPMMaxMotor ) ) 
//							_ejeTrasLocalAV.x += pinchos.potencia * _moverHorizInput * Time.fixedDeltaTime;
//					}
//					
//					_ejeDelanteroRB.angularVelocity = _ejeDelanteroRB.transform.TransformDirection( _ejeDelLocalAV.x, 0f, 0f );
//					_ejeTraseroRB.angularVelocity = _ejeTraseroRB.transform.TransformDirection( _ejeTrasLocalAV.x, 0f, 0f );
//				}
//			}
			if ( IsAlgunaRuedaEnSuelo() )
			{
				/*
//				Vector3 _normalDel = HAZLO( ejeDelantero );
//				Vector3 _normalTras = HAZLO( ejeTrasero );
//				Vector3 _normal = _normalDel + _normalTras;
//				_normal.Normalize();
				*/
				
				Vector3 _normal = Vector3.zero;
				List<Vector3> _contacts = new List<Vector3>();
				
				foreach ( Collision col in ejeDelantero.GetColisiones() )
					if ( ( 1 << col.gameObject.layer & pinchos.colisionesPinchables.value ) != 0 )
						foreach ( ContactPoint cont in col.contacts )
//							_normal += cont.normal;
							_contacts.Add( cont.point );
				foreach ( Collision col in ejeTrasero.GetColisiones() )
					if ( ( 1 << col.gameObject.layer & pinchos.colisionesPinchables.value ) != 0 )
						foreach ( ContactPoint cont in col.contacts )
//							_normal += cont.normal;
							_contacts.Add( cont.point );
				
//				if ( _normal != Vector3.zero )
				if ( _contacts.Count != 0 )
				{
					if ( !pinchos._clavados )
					{
						_chasisRB.velocity = _ejeDelanteroRB.velocity = _ejeTraseroRB.velocity = 
							_chasisRB.angularVelocity = _ejeDelanteroRB.angularVelocity = _ejeTraseroRB.angularVelocity =
							Vector3.zero;
					}
					pinchos._clavados = true;
					
					Vector3 _midPoint = Vector3.zero;
					foreach ( Vector3 v3 in _contacts )
						_midPoint += v3;
					_midPoint /= _contacts.Count;
					
					_normal = _chasisRB.position - _midPoint;
					_normal.Normalize();
					
					_chasisRB.AddForce( -_normal * pinchos.gravedadFake, ForceMode.Acceleration );
					
					
//					// NO FUNCIONA BIEN ESTO DE ROTAR
					_chasisRB.rotation.SetFromToRotation( _chasisRB.transform.up, -_normal );
//					_chasisRB.MoveRotation( Quaternion.LookRotation( -_normal, -chasis.transform.forward ) );
					
					
					
					Debug.DrawRay( _chasisRB.position, -_normal * 10f, Color.black, 0f, false );
					
					// MOVIMIENTO DE RUEDAS POR LA SUPERFICIE					
					float _moverHorizInput = Input.GetAxis("moverHorizontal");
					
					Vector3 _ejeDelLocalAV = _ejeDelanteroRB.transform.InverseTransformDirection( _ejeDelanteroRB.angularVelocity );;
					Vector3 _ejeTrasLocalAV = _ejeTraseroRB.transform.InverseTransformDirection( _ejeTraseroRB.angularVelocity );;
					if ( _moverHorizInput == 0 || !pinchos.inputEnabled )
					{
						_ejeDelLocalAV.x = Mathf.Sign( _ejeDelLocalAV.x ) * Mathf.Max( 0f, Mathf.Abs( _ejeDelLocalAV.x ) - ( pinchos.freno  * Time.fixedDeltaTime ) );
						_ejeTrasLocalAV.x = Mathf.Sign( _ejeTrasLocalAV.x ) * Mathf.Max( 0f, Mathf.Abs( _ejeTrasLocalAV.x ) - ( pinchos.freno * Time.fixedDeltaTime ) );
					}
					else
					{
						if ( ( _moverHorizInput < 0 && _ejeDelLocalAV.x > -pinchos.RPMMaxMotor ) || ( _moverHorizInput > 0 && _ejeDelLocalAV.x < pinchos.RPMMaxMotor ) ) 
							_ejeDelLocalAV.x += pinchos.potencia * _moverHorizInput * Time.fixedDeltaTime;
						if ( ( _moverHorizInput < 0 && _ejeTrasLocalAV.x > -pinchos.RPMMaxMotor ) || ( _moverHorizInput > 0 && _ejeTrasLocalAV.x < pinchos.RPMMaxMotor ) ) 
							_ejeTrasLocalAV.x += pinchos.potencia * _moverHorizInput * Time.fixedDeltaTime;
					}
					
					_ejeDelanteroRB.angularVelocity = _ejeDelanteroRB.transform.TransformDirection( _ejeDelLocalAV.x, 0f, 0f );
					_ejeTraseroRB.angularVelocity = _ejeTraseroRB.transform.TransformDirection( _ejeTrasLocalAV.x, 0f, 0f );
				}
			}
		}
		
		pinchos._pinchosFuera = pinchosInput;
	}
	
//
//private Vector3 HAZLO( Rigidbody eje )
//{
//	Vector3 _normal = Vector3.zero;
//	List<Vector3> _contacts = new List<Vector3>();
//	
//	foreach ( Collision col in ejeDelantero.GetColisiones() )
//		if ( ( 1 << col.gameObject.layer & pinchos.colisionesPinchables.value ) != 0 )
//			foreach ( ContactPoint cont in col.contacts )
////							_normal += cont.normal;
//				_contacts.Add( cont.point );
//	foreach ( Collision col in ejeTrasero.GetColisiones() )
//		if ( ( 1 << col.gameObject.layer & pinchos.colisionesPinchables.value ) != 0 )
//			foreach ( ContactPoint cont in col.contacts )
////							_normal += cont.normal;
//				_contacts.Add( cont.point );
//	
////				if ( _normal != Vector3.zero )
//	if ( _contacts.Count != 0 )
//	{
//		if ( !pinchos._clavados )
//		{
//			_chasisRB.velocity = _ejeDelanteroRB.velocity = _ejeTraseroRB.velocity = 
//				_chasisRB.angularVelocity = _ejeDelanteroRB.angularVelocity = _ejeTraseroRB.angularVelocity =
//				Vector3.zero;
//		}
//		pinchos._clavados = true;
//		
//		Vector3 _midPoint = Vector3.zero;
//		foreach ( Vector3 v3 in _contacts )
//			_midPoint += v3;
//		_midPoint /= _contacts.Count;
//		
//		_normal = _chasisRB.position - _midPoint;
//		_normal.Normalize();
//		
//		_chasisRB.AddForce( -_normal * pinchos.gravedadFake, ForceMode.Acceleration );
//}



	private void actualizarRuedas ()
	{
		float _moverHorizInput = Input.GetAxis("moverHorizontal");
		
		Vector3 _ejeDelLocalAV = _ejeDelanteroRB.transform.InverseTransformDirection( _ejeDelanteroRB.angularVelocity );;
		Vector3 _ejeTrasLocalAV = _ejeTraseroRB.transform.InverseTransformDirection( _ejeTraseroRB.angularVelocity );;
		if ( _moverHorizInput == 0 || !ruedas.inputEnabled )
		{
			_ejeDelLocalAV.x = Mathf.Sign( _ejeDelLocalAV.x ) * Mathf.Max( 0f, Mathf.Abs( _ejeDelLocalAV.x ) - ( ruedas.freno * Time.fixedDeltaTime ) );
			_ejeTrasLocalAV.x = Mathf.Sign( _ejeTrasLocalAV.x ) * Mathf.Max( 0f, Mathf.Abs( _ejeTrasLocalAV.x ) - ( ruedas.freno * Time.fixedDeltaTime ) );
		}
		else
		{
			if ( ( _moverHorizInput < 0 && _ejeDelLocalAV.x > -ruedas.RPMMaxMotor ) || ( _moverHorizInput > 0 && _ejeDelLocalAV.x < ruedas.RPMMaxMotor ) ) 
				_ejeDelLocalAV.x += ruedas.potencia * _moverHorizInput * Time.fixedDeltaTime;
			else
				_ejeDelLocalAV.x -= ruedas.perdidaDeVelocidad * _moverHorizInput * Time.fixedDeltaTime;
				
			if ( ( _moverHorizInput < 0 && _ejeTrasLocalAV.x > -ruedas.RPMMaxMotor ) || ( _moverHorizInput > 0 && _ejeTrasLocalAV.x < ruedas.RPMMaxMotor ) ) 
				_ejeTrasLocalAV.x += ruedas.potencia * _moverHorizInput * Time.fixedDeltaTime;
			else
				_ejeTrasLocalAV.x -= ruedas.perdidaDeVelocidad * _moverHorizInput * Time.fixedDeltaTime;
		}
		
		_ejeDelanteroRB.angularVelocity = _ejeDelanteroRB.transform.TransformDirection( _ejeDelLocalAV.x, 0f, 0f );
		_ejeTraseroRB.angularVelocity = _ejeTraseroRB.transform.TransformDirection( _ejeTrasLocalAV.x, 0f, 0f );
	}
	
	private void actualizarGravedad ()
	{
		_chasisRB.AddForce( Physics.gravity, ForceMode.Acceleration );
		_ejeDelanteroRB.AddForce( Physics.gravity, ForceMode.Acceleration );
		_ejeTraseroRB.AddForce( Physics.gravity, ForceMode.Acceleration );
	}
	
	private void actualizarSalto ()
	{
		bool _saltoInput = salto.inputEnabled && Input.GetButton("salto");
		
		Debug.DrawLine(_chasisRB.position, _chasisRB.position + ( -_chasisRB.transform.up * salto.distanciaRayoTerreno ), Color.white, 0f, false ); 
		if ( !salto._pulsadoAnteriorFrame && _saltoInput )
			salto._timeLastPulsado = Time.fixedTime;
// AÑADIR UN DELAY PARA FACILITAR EL SALTO CUANDO SE ABANDONA UNA PLATAFORMA.
		
		if ( !_saltoInput || pinchos._clavados )
			salto._saltando = false;
		
		if ( !pinchos._clavados )
		{
			if ( salto._saltando )
			{
				// HOVER
				if ( Time.fixedTime <= salto._timeLastHecho + salto.hoverDuracion )
				{
					Vector3 _vel = _chasisRB.velocity;
					_vel.y += Time.fixedDeltaTime * salto.hoverVel;
					_chasisRB.velocity = _vel;
				}
			}
			else if ( _saltoInput || ( salto._timeLastPulsado + salto.delayPulsacion >= Time.fixedTime ) )
			{
				RaycastHit _hitInfo;
				if ( IsAlgunaRuedaEnSuelo() && Physics.Raycast( _chasisRB.position, -_chasisRB.transform.up, out _hitInfo, salto.distanciaRayoTerreno, salto.colisionesLayer.value ) )
				{
					Vector3 _normal = _hitInfo.normal;
					if ( Vector3.Angle( _normal, Vector3.up ) <= salto.pendienteMax )
					{
						// NUEVO SALTO
	//					Vector3 _carRight = chasis.transform.right;
	//					Vector3 _forward = Vector3.Cross( _normal, _carRight );
					
						// ESTO SERIA PARA HACER QUE EL COCHE SE ORIENTE A LA SUPERFICIE AL SALTAR
	//					_chasisRB.rotation.SetFromToRotation( _chasisRB.transform.up, _normal );
						
	//					 ESTE CODIGO HACE QUE EL SALTO SE HAGA EN LA DIRECCION DE LA NORMAL, PERO NO FUNCIONA PERFECTO
	//					Debug.Log( "no está saltando segun la normal del terreno, sino segun la rotacion del coche!!!!" );
	//					Vector3 _vel = chasis.transform.InverseTransformDirection( _chasisRB.velocity );
	//					_vel.y = salto.velInicial;
	//					_chasisRB.velocity = chasis.transform.TransformDirection( _vel );
					
						Vector3 _vel = _chasisRB.velocity;
						_vel.y = salto.velInicial;
						_chasisRB.velocity = _vel;
						
						salto._timeLastHecho = Time.fixedTime;
						salto._timeLastPulsado = float.NegativeInfinity;
						salto._saltando = true;
						
						Debug.DrawLine(_hitInfo.point, _hitInfo.point + ( _hitInfo.normal * 5f ), Color.green, .5f, false ); 				
					}
				}
			}
			else
			{
				Debug.DrawLine(_chasisRB.position, _chasisRB.position + ( _chasisRB.transform.up * 5f ), Color.yellow, 0f, false ); 				
			}
		}
		
		salto._pulsadoAnteriorFrame = _saltoInput;
	}
	
	private void actualizarGiro ()
	{
		float _girarHoriz = Input.GetAxis("girarHorizontal");
		
		Vector3 _localAV = _chasisRB.transform.InverseTransformDirection( _chasisRB.angularVelocity );
		_localAV.y = _localAV.z = 0;
		if ( _girarHoriz == 0 || !giro.inputEnabled )
		{
			_localAV.x = Mathf.Sign( _localAV.x ) * Mathf.Max( 0f, Mathf.Abs( _localAV.x ) - ( giro.decel * Time.fixedDeltaTime ) );
			
			if ( !IsAlgunaRuedaEnSuelo() && !IsChasisChocando() && !pinchos._clavados )
				_chasisRB.gameObject.transform.rotation = Quaternion.Lerp( _chasisRB.gameObject.transform.rotation, Quaternion.identity, giro.muelleVueltaVertical );
//				iTween.RotateUpdate( _chasisRB.gameObject, Vector3.zero, giro.muelleVueltaVertical );
		}
		else
		{			
			_localAV.x += giro.acel * _girarHoriz * Time.fixedDeltaTime;
		}		
		_chasisRB.angularVelocity = _chasisRB.transform.TransformDirection( _localAV );		
	}

//	TUTORIAL
//wall riding
//
//los bloques del comienzo son un buen tutorial
//	- el 2º bloque, la cuña a 45º, no debes saltar a ella. Debes tirarte hacia ella rodando, sin saltar.
//	- el cuarto de circulo, tienes que acelerar hasta encallar la rueda.

//empujar bloques

//agarrar pesos fisicos
	
//la manera de caminar de espineto sin ruedas es como si solo pulsas la tecla de flecha, y vas rodando.


	private void actualizarJetpack ()
	{
		if ( jetpack.inputEnabled )
		{
			float _jetpackInput = Input.GetAxis( "jetpack" );
			Vector3 _vel = _chasisRB.velocity;
			
			// Cuan rapido vamos en la direccion del jetpack.
			// Si ya vamos rapido, el jetpack no acelerara mas.
			Vector3 _proj = Vector3.Project( _chasisRB.velocity, _chasisRB.transform.up );
			float _fuerzaAplicada = Mathf.Min(
				_jetpackInput * jetpack.acel * Time.fixedDeltaTime,
				Mathf.Max( jetpack.velMax - _proj.magnitude, 0f )
				);
			// Esto de arriba esta mal, porque tambien se esta anulando el jetpack cuando caes muy rapido.
//float _fuerzaAplicada = _jetpackInput * jetpack.acel * Time.fixedDeltaTime;
			
			_vel += _chasisRB.transform.up * _fuerzaAplicada;
			_chasisRB.velocity = _vel;
		}
	}

	private void actualizarTraslacionAire ()
	{
		if ( traslacionAire.inputEnabled )
		{
			float _moverHorizInput = Input.GetAxis("moverHorizontal");
			
			if ( !IsAlgunaRuedaEnSuelo() && !IsChasisChocando() )
			{
				Vector3 _vel = _chasisRB.velocity;
				
				
				// Cuan rapido vamos en el eje horizontal.
				// Si ya vamos rapido, no se acelerara mas.
				if ( _moverHorizInput > 0 )
				{
					if ( _vel.z > 0 )
					{
						_vel.z += Mathf.Min(
							_moverHorizInput * traslacionAire.acel * Time.fixedDeltaTime,
							Mathf.Max( traslacionAire.velMax - _vel.z, 0f )
							);
					}
					else
					{
						_vel.z += Mathf.Min(
							_moverHorizInput * traslacionAire.freno * Time.fixedDeltaTime,
							Mathf.Max( traslacionAire.velMax - _vel.z, 0f )
							);
					}
				}
				else
				{
					if ( _vel.z < 0 )
					{
						_vel.z += Mathf.Max(
							_moverHorizInput * traslacionAire.acel * Time.fixedDeltaTime,
							Mathf.Min( -traslacionAire.velMax - _vel.z, 0f )
							);
					}
					else
					{
						_vel.z += Mathf.Max(
							_moverHorizInput * traslacionAire.freno * Time.fixedDeltaTime,
							Mathf.Min( -traslacionAire.velMax - _vel.z, 0f )
							);
					}
				}
				
				
//				if ( ( _moverHorizInput < 0 && _vel.z >= -traslacionAire.velMax ) || ( _moverHorizInput > 0 && _vel.z <= traslacionAire.velMax ) )
//					_vel.z += _moverHorizInput * traslacionAire.acel * Time.fixedDeltaTime;
				
				_chasisRB.velocity = _vel;
			}
		}
	}
}

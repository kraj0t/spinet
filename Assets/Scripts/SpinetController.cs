using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class SpinetController : MonoBehaviour {

	public Checkpoint checkpointActual;
	
	public CollisionRecorder chasis;
	public CollisionRecorder ejeDelantero;
	public CollisionRecorder ejeTrasero;	
	
	// Bajarlo hará que el coche vuelque menos.
	public float centroDeMasaOffset = -0.75f;

	
	// La velocidad maxima a la que se desplazara el coche, en cualquier direccion.
	// Esto le pone las cosas faciles a la fisica e impide velocidades locas.
	public float velMaxFisica = 100f;	

	// Tiempo en segundos que el juego recuerda si se ha pulsado el salto
	public float jumpPressDelay = 0.15f;

	public float maxDepenetrationVelocity = 5f;

	public WheelsController wheels;
	public JumpController jump;
	public AirSteerController airSteer;
	public RollController roll;
	public SpikesController spikes;
	public JetpackController jetpack;
	public JetpackController nitroJetpackLeft;
	public JetpackController nitroJetpackRight;
	
	
	
	
	private Rigidbody _chasisRB;
	private Rigidbody _ejeDelanteroRB;
	private Rigidbody _ejeTraseroRB;




	internal bool _pulsadoAnteriorFrame = false;
	internal float _timeLastPulsado = float.NegativeInfinity;

	[SerializeField]
	[HideInInspector]
	private bool _wasJumpingLastFrame = false;

	
			
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
		
		//Physics.IgnoreCollision( _chasisRB.GetComponent<Collider>(), _ejeDelanteroRB.GetComponent<Collider>(), true );
		//Physics.IgnoreCollision( _chasisRB.GetComponent<Collider>(), _ejeTraseroRB.GetComponent<Collider>(), true );
		
		// NO TENGO CLARO QUE ESTO MEJORE EL COMPORTAMIENTO FISICO...
		// Y HAY GENTE QUE SE QUEJA POR INTERNET :-S
		//ejeDelantero.useConeFriction = true;
		//ejeTrasero.useConeFriction = true;
	}
	
	void OnDrawGizmos ()
	{
		//Gizmos.DrawRay(...............
	}
	
	void OnGUI()
	{
		GUILayout.Label("KEYS:");
		GUILayout.Label("  A and D -> accelerate");	
		GUILayout.Label("  W -> jump");	
		GUILayout.Label("  S -> wheel spikes");	
		GUILayout.Label("  Left and Right -> turn");	
		GUILayout.Label("  Up -> Jetpack");	
		GUILayout.Label("  Z -> Zoom");	
		GUILayout.Label("  Y -> Load next level");	

//		GUILayout.Label("Vel = " + _chasisRB.velocity.ToString() );
//		GUILayout.Label("Vel magnitud = " + _chasisRB.velocity.magnitude.ToString() );
//		GUILayout.Label("eje del av = " + _ejeDelanteroRB.angularVelocity.ToString() );
//		GUILayout.Label("chasis av = " + _chasisRB.angularVelocity.ToString() );
//		GUILayout.Label("slope = " + wheels.lastSlopeAngle.ToString() );
//
//		GUILayout.Label("LAS LUCES Spotlight DEBERIAN SER 2D, Y LAS DE PUNTO DEBERIAN SER 3D. MOLA, NO?");
//		GUILayout.Label("AL MOMENTO DE ESTAR GIRANDO EL COCHE TERMINA ROTADO EN OTROS EJES.");
//		GUILayout.Label("AHORA SE QUEDA QUIETO EN SLOPES, PERO DESPUES DE USAR LOS PINCHOS POR 1a VEZ, SE FRENA MENOS...");
////		GUILayout.Label("USAR MUELLE PARA LAS MALLAS GRAFICAS.");
//		GUILayout.Label("LAS RUEDAS DEJAN DE GIRAR MUY RAPIDO EN EL AIRE. HAS DE USAR TORQUE Y NO ANGULAR VELOCITY");
//		GUILayout.Label("ALCANZAS MUCHA VELOCIDAD HORIZONTAL SI HACES BUNNY JUMPING EN CUESTA ABAJO.");	
//		GUILayout.Label("ESTARIA BIEN PODER SUMAR LA VELOCIDAD DEL SALTO, PERO NO ES FIABLE POR LOS REBOTES. ALGUNA IDEA...?");	
//		GUILayout.Label("LOS RIGIDBODIES TIENEN METODO SWEEPTEST, QUIZA ES MEJOR PARA PINCHOS Y SALTO.");
//		GUILayout.Label("BUG: LAS RUEDAS PUEDEN LLEGAR A SOLAPARSE. REPRO 1/10: CON LOS PINCHOS FUERA, CAMINA HACIA UN BLOQUE NO PINCHABLE.");
//		GUILayout.Label("CREO QUE CONVIENE QUE CADA RUEDA TENGA SU PROPIO COMPONENTE. EN WheelsController YA SE INTUYE ESTA NECESIDAD. PARA LOS PINCHOS TAMBIEN.");
//		GUILayout.Label("CUANDO UNA SOMBRA PROYECTADA ES EXTRUIDA HACIA EL FONDO, EL FALLOFF DEL CAST NO FUNCIONA BIEN. EL BORDE LEJANO SE VE DURO.");
//	

		
		//	TUTORIAL
		//wall riding
		//
		//los bloques del comienzo son un buen tutorial
		//	- el 2º bloque, la cuña a 45º, no debes saltar a ella. Debes tirarte hacia ella rodando, sin saltar.
		//	- el cuarto de circulo, tienes que acelerar hasta encallar la rueda.
		//  - para tomar esquinas con los pinchos, a veces tienes que ayudarte girando el cuerpo.
		//  - puedes saltar en una pared vertical.
		//  - el salto en el aire permite reorientar la direccion instantaneamente.
		//  - saltar y girar para alcanzar esquinas 0.25 metros mas altas. [Con los valores actuales, una esquina en angulo agudo a 1.5m de altura es (creo) imposible de alcanzar sin girar el cuerpo, pero girandolo se hace facil]
		//  - hacer wallride para llegar a 1.75m. Tienes que girar el cuerpo solo al final, para tomar la esquina, y asi aprovechas las ruedas mas rato.
		
		//empujar bloques
		
		//agarrar pesos fisicos
		
		//la manera de caminar de espineto sin ruedas es como si solo pulsas la tecla de flecha, y vas rodando.

		// slip = speed difference between the rubber and the road. Usa esto para soltar particulas y ejecutar sonido.

		// IDEAS: 
		// escena en la que lo que ves son las sombras proyectadas sobre una pared de fondo. Y, jugando con esa pared, la cosa se complica: huecos en la pared, irregularidades, protuberancias...
		// enemigos que son totalmente invisibles en las zonas de sombra.
		// escenas de oscuridad en las que el shadowVolumeSource es Spinetto. La sombra, por tanto, es la zona oculta.
	}


	private bool CheckJumpInput () 
	{
		if ( Input.GetButtonDown( "salto" ) ) {
			_timeLastPulsado = Time.unscaledTime;
		}

		if ( !_wasJumpingLastFrame && jump._saltando ) {
			_timeLastPulsado = float.NegativeInfinity;
		}
		_wasJumpingLastFrame = jump._saltando;

		return (_timeLastPulsado + jumpPressDelay > Time.unscaledTime);
	}


	void Update ()
	{
		float moverHorizInput = Input.GetAxis("moverHorizontal");
		bool saltoInput = CheckJumpInput();
		bool frenoInput = Input.GetButton( "freno" );
		bool hoverInput = Input.GetButton("salto");
		float girarHorizInput = Input.GetAxis("girarHorizontal");
		bool pinchosInput = Input.GetButton( "pinchos" );
		float jetpackInput = Input.GetAxis("jetpack");
		float nitroInput = Input.GetAxis( "nitro" );

		wheels.desiredForce = moverHorizInput;
		wheels.desiredBrake = frenoInput;

		jump.desiredJumpState = saltoInput;
		jump.desiredHoverState = hoverInput;

		airSteer.desiredForce = moverHorizInput;

		roll.desiredForce = girarHorizInput;

		spikes.desiredState = pinchosInput; 

		jetpack.desiredForce = jetpackInput;

		nitroJetpackLeft.desiredForce = Mathf.Max( 0f, moverHorizInput * nitroInput );
		nitroJetpackRight.desiredForce = Mathf.Max( 0f, -moverHorizInput * nitroInput );
	}

	public float sleepVelocity = 0.1f;
	public float angularSleepVelocity = 1f;
	public float sleepThreshold = 0.1f;
	public float fullBrakeSlopeAngle = 0.9f;
	void FixedUpdate ()
	{
		_chasisRB.centerOfMass = new Vector3(0, centroDeMasaOffset, 0 );

		if ( !spikes._clavados ) {
			/// TEST NO APLICAR GRAVEDAD SI SE ESTA QUIETITO
			if ( wheels.IsAllWheelsGrounded() && wheels.lastSlopeAngle >= fullBrakeSlopeAngle && wheels.braking &&
			    Mathf.Abs( _chasisRB.angularVelocity.x ) < angularSleepVelocity && _chasisRB.velocity.magnitude < sleepVelocity && 
			    Mathf.Abs( _ejeDelanteroRB.angularVelocity.x ) < angularSleepVelocity && _ejeDelanteroRB.velocity.magnitude < sleepVelocity && 
			    Mathf.Abs( _ejeTraseroRB.angularVelocity.x ) < angularSleepVelocity && _ejeTraseroRB.velocity.magnitude < sleepVelocity ) {
				_chasisRB.Sleep();
				_ejeDelanteroRB.Sleep();
				_ejeTraseroRB.Sleep();
			} else {
				actualizarGravedad();
			}
			/// end TEST NO APLICAR GRAVEDAD SI SE ESTA QUIETITO
//			actualizarGravedad();
		}

		_chasisRB.velocity = Vector3.ClampMagnitude(_chasisRB.velocity, velMaxFisica);
		_ejeDelanteroRB.velocity = Vector3.ClampMagnitude(_ejeDelanteroRB.velocity, velMaxFisica);
		_ejeTraseroRB.velocity = Vector3.ClampMagnitude(_ejeTraseroRB.velocity, velMaxFisica);

		_chasisRB.sleepThreshold = sleepThreshold;
		_ejeDelanteroRB.sleepThreshold = sleepThreshold;
		_ejeTraseroRB.sleepThreshold = sleepThreshold;
		
		//_chasisRB.rotation = Quaternion.Euler( _chasisRB.rotation.eulerAngles.x, 0f, 0f );
		_chasisRB.angularVelocity = new Vector3( _chasisRB.angularVelocity.x, 0f, 0f );
		_ejeDelanteroRB.angularVelocity = new Vector3( _ejeDelanteroRB.angularVelocity.x, 0f, 0f );
		_ejeTraseroRB.angularVelocity = new Vector3( _ejeTraseroRB.angularVelocity.x, 0f, 0f );

		_chasisRB.maxDepenetrationVelocity = maxDepenetrationVelocity;
		_ejeDelanteroRB.maxDepenetrationVelocity = maxDepenetrationVelocity;
		_ejeTraseroRB.maxDepenetrationVelocity = maxDepenetrationVelocity;

//		_chasisRB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
//		_ejeDelanteroRB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
//		_ejeTraseroRB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
//		_chasisRB.freezeRotation = true;
//		_ejeDelanteroRB.freezeRotation = true;
//		_ejeTraseroRB.freezeRotation = true;

//		_chasisRB.rotation = Quaternion.AngleAxis( _chasisRB.rotation.eulerAngles.x, Vector3.right );
//		_ejeDelanteroRB.rotation = Quaternion.AngleAxis( _ejeDelanteroRB.rotation.eulerAngles.x , Vector3.right );
//		_ejeTraseroRB.rotation = Quaternion.AngleAxis( _ejeTraseroRB.rotation.eulerAngles.x, Vector3.right );
	}


	private void actualizarGravedad ()
	{
		Debug.DrawRay( _chasisRB.position, Physics.gravity, Color.red, 0f, false );
		_chasisRB.AddForce( Physics.gravity, ForceMode.Acceleration );
		_ejeDelanteroRB.AddForce( Physics.gravity, ForceMode.Acceleration );
		_ejeTraseroRB.AddForce( Physics.gravity, ForceMode.Acceleration );
	}
	




}

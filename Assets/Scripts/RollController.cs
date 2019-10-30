using UnityEngine;
using System.Collections;

public class RollController : MonoBehaviour {

	// 0 for disabled, 1 for full power.
	// The input code should set this.
	public float desiredForce = 0f;

	public CollisionRecorder targetBody;

	public WheelsController wheelsController;

	// Velocidad angular maxima para el chasis.
	public float chasisVelMaxGiro = 8f;

	public float acel = 500f;
	public float decel = 1000f;

	// Controla cuan rapido vuelve el coche a orientarse con la vertical.
	public float muelleVueltaVertical = 0.06f;


	void FixedUpdate () 
	{		
		Rigidbody targetRigidbody = targetBody.GetComponent<Rigidbody>();

		targetRigidbody.maxAngularVelocity = chasisVelMaxGiro;

		//Vector3 _localAV = targetRigidbody.transform.InverseTransformDirection( targetRigidbody.angularVelocity );
		Vector3 _localAV = targetRigidbody.angularVelocity;
		_localAV.y = _localAV.z = 0;

		if ( desiredForce == 0 )
		{
			_localAV.x = Mathf.Sign( _localAV.x ) * Mathf.Max( 0f, Mathf.Abs( _localAV.x ) - ( decel * Time.fixedDeltaTime ) );

			// Auto-stabilize the body, only if the body and the wheels are not touching anything.
			if ( !wheelsController.IsAnyWheelGrounded() && targetBody.GetColisiones().Length == 0 ) {
//				targetBody.gameObject.transform.rotation = Quaternion.Lerp( targetBody.gameObject.transform.rotation, Quaternion.identity, muelleVueltaVertical );
				targetRigidbody.rotation = Quaternion.Lerp( targetRigidbody.rotation, Quaternion.identity, muelleVueltaVertical );
			}
		}
		else
		{			
			_localAV.x += acel * desiredForce * Time.fixedDeltaTime;
		}		

//		targetRigidbody.angularVelocity = targetRigidbody.transform.TransformDirection( _localAV );
		targetRigidbody.angularVelocity = _localAV;
	}
}

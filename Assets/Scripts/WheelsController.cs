using UnityEngine;
using System.Collections;

public class WheelsController : MonoBehaviour {

	// 0 for disabled, 1 for full power, -1 for full power in the other direction.
	// The input code should set this.
	public float desiredForce = 0f;
	public bool desiredBrake = false;

	public bool brakeWhenIdle = true;

	public CollisionRecorder[] wheels;

	// Velocidad angular maxima para las ruedas.
	// Este es el valor maximo para la fisica de las ruedas, no para el control.
	public float wheelsMaxAngularVelocity = 60f;
	
	// Velocidad angular mas alla de la cual el motor no sera capaz de acelerar.
	// Si las ruedas giran muy rapido, no aceleraremos.
	public float engineMaxAngularVelocity = 30f;
	
	public float potencia = 3000f;
	public float freno = 3000f;

	// When the wheels are rotating faster than the engine can go, they will gradually go back
	// to the maxAngularVelocity according to this value.
	public float angularVelocityLossWhenAboveEngineMax = 30;
	
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

	public float sleepVelocity = 0.1f;


	[SerializeField()]
	[HideInInspector()]
	internal float lastSlopeAngle = 0f;


	[SerializeField()]
	[HideInInspector()]
	internal bool braking = false;

	private bool IsWheelGrounded ( CollisionRecorder c )
	{
		return (c.GetColisiones().Length != 0);
	}
	
	public bool IsAnyWheelGrounded ()
	{
		foreach ( CollisionRecorder c in wheels ) {
			if ( IsWheelGrounded( c ) ) {
				return true;
			}
		}
		return false;
	}

	public bool IsAllWheelsGrounded ()
	{
		foreach ( CollisionRecorder c in wheels ) {
			if ( !IsWheelGrounded( c ) ) {
				return false;
			}
		}
		return true;
	}



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		foreach ( CollisionRecorder c in wheels ) {
			Rigidbody rb = c.GetComponent<Rigidbody>();

			rb.maxAngularVelocity = wheelsMaxAngularVelocity;

//			Vector3 newAngularVelocity = rb.transform.InverseTransformDirection( rb.angularVelocity );
			Vector3 newAngularVelocity = rb.angularVelocity;

			if ( desiredBrake || desiredForce == 0f ) {
				if ( desiredBrake || brakeWhenIdle ) {					
					braking = true;
//					newAngularVelocity.x = Mathf.Sign( newAngularVelocity.x ) * Mathf.Max( 0f, Mathf.Abs( newAngularVelocity.x ) - ( freno * Time.fixedDeltaTime ) );
					newAngularVelocity.x = Mathf.Sign( newAngularVelocity.x ) * Mathf.Max( newAngularVelocity.x * 0.1f, Mathf.Abs( newAngularVelocity.x ) - ( freno * Time.fixedDeltaTime ) );
//					newAngularVelocity.x *= freno * Time.fixedDeltaTime;
//					rb.AddRelativeTorque( -rb.angularVelocity.x * freno * Time.fixedDeltaTime, 0f, 0f, ForceMode.Force );
//					rb.angularVelocity = -rb.angularVelocity;
					//rb.angularDrag = freno;
					if ( Mathf.Abs( newAngularVelocity.x ) < sleepVelocity && rb.velocity.magnitude < sleepVelocity ) {
//						rb.freezeRotation = true;
//						rb.angularVelocity = Vector3.zero;
//						newAngularVelocity.x = 0f;
//						rb.Sleep();
					} else {
//						rb.freezeRotation = false;
//						rb.AddRelativeTorque( -Mathf.Sign(rb.angularVelocity.x) * freno * Time.fixedDeltaTime, 0f, 0f, ForceMode.Force );
//						newAngularVelocity.x = Mathf.Sign( newAngularVelocity.x ) * Mathf.Max( 0f, Mathf.Abs( newAngularVelocity.x ) - ( freno * Time.fixedDeltaTime ) );
					}
				}
			} else {
				braking = false;
//				rb.freezeRotation = false;
//				rb.angularDrag = 0.05f;
				if ( ( desiredForce < 0 && newAngularVelocity.x > -engineMaxAngularVelocity ) || ( desiredForce > 0 && newAngularVelocity.x < engineMaxAngularVelocity ) ) {
					newAngularVelocity.x += potencia * desiredForce * Time.fixedDeltaTime;
//					rb.AddRelativeTorque( potencia * desiredForce * Time.fixedDeltaTime, 0f, 0f, ForceMode.Force );
				} else {
					newAngularVelocity.x -= angularVelocityLossWhenAboveEngineMax * desiredForce * Time.fixedDeltaTime;
//					rb.AddRelativeTorque( -angularVelocityLossWhenAboveEngineMax * desiredForce * Time.fixedDeltaTime, 0f, 0f, ForceMode.Force );
				}
			}

//			rb.angularVelocity = rb.transform.TransformDirection( newAngularVelocity.x, 0f, 0f );
			rb.angularVelocity = new Vector3( newAngularVelocity.x, 0f, 0f );
		}

		// Calculate the slope of the terrain.
		if ( IsAllWheelsGrounded() ) {
			Vector3 sumNormal = Vector3.zero;
			foreach ( CollisionRecorder colRecord in wheels ) {
				foreach ( Collision col in colRecord.GetColisiones() ) {
					foreach ( ContactPoint cont in col.contacts ) {
						sumNormal += cont.normal;
					}
				}
			}
			Vector3 constrainedNormal = sumNormal;
			constrainedNormal.x = 0f;
			constrainedNormal.Normalize();
			lastSlopeAngle = constrainedNormal.y;
		}
	}
}



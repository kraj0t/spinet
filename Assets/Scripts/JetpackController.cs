using UnityEngine;
using System.Collections;

public class JetpackController : MonoBehaviour {

	public Rigidbody target;

	// 0 for disabled, 1 for full power.
	// The input code should set this.
	public float desiredForce = 0f;
	
	public float acel = 25f;
	
	// El jetpack no puede permitir alcanzar velocidades verticales mayores que esta.
	public float velMaxVertical = 6f;

	// El jetpack no puede permitir alcanzar velocidades horizontales mayores que esta.
	public float velMaxHorizontal = 8f;

	public Vector3 localDirection = Vector3.up;

	void FixedUpdate () {
		Vector3 currentJetpackDirection = target.transform.rotation * localDirection;
		currentJetpackDirection.Normalize();

		Vector3 fuerzaAplicada = currentJetpackDirection * (desiredForce * acel * Time.fixedDeltaTime);
		if ( fuerzaAplicada.z > 0f && target.velocity.z >= velMaxHorizontal ) {
			fuerzaAplicada.z = 0f;
		} else if ( fuerzaAplicada.z < 0f && target.velocity.z <= -velMaxHorizontal ) {
			fuerzaAplicada.z = 0f;
		}
		if ( fuerzaAplicada.y > 0f && target.velocity.y >= velMaxVertical ) {
			fuerzaAplicada.y = 0f;
		} else if ( fuerzaAplicada.y < 0f && target.velocity.y <= -velMaxVertical ) {
			fuerzaAplicada.y = 0f;
		}
		
		target.velocity = target.velocity + fuerzaAplicada;


//		// Cuan rapido vamos en la direccion del jetpack.
//		// Si ya vamos rapido, el jetpack no acelerara mas.
//		float currentSpeedInJetpackDirection = Vector3.Dot( currentJetpackDirection, target.velocity );
//
//		if ( currentSpeedInJetpackDirection < velMax ) {
//			float fuerzaAplicada = desiredForce * acel * Time.fixedDeltaTime;
//
//			target.velocity = target.velocity + (currentJetpackDirection * fuerzaAplicada);
//		}
	}
}

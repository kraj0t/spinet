using UnityEngine;
using System.Collections;

public class AirSteerController : MonoBehaviour {

	// 0 for disabled, 1 for full power.
	// The input code should set this.
	public float desiredForce = 0f;

	public Rigidbody targetBody;

	public CollisionRecorder [] bodiesThatMustBeOnAir;
	
	// Velocidad en horizontal maxima a la que se puede llegar gracias al control en el aire.
	public float velMax = 2f;
	
	// Se usa este valor cuando se acelera en el sentido actual del desplazamiento.
	public float acel = 4f;
	
	// Se usa este valor cuando se acelera en sentido contrario del desplazamiento.
	public float freno = 7f;


	void FixedUpdate ()
	{
		bool isAnyBodyColliding = false;
		foreach ( CollisionRecorder cr in bodiesThatMustBeOnAir ) {
			isAnyBodyColliding |= ( cr.GetColisiones().Length != 0 );
		}

		if ( !isAnyBodyColliding )
		{
			Vector3 newVelocity = targetBody.velocity;

			// Cuan rapido vamos en el eje horizontal.
			// Si ya vamos rapido, no se acelerara mas.
			if ( desiredForce > 0 )	{
				if ( newVelocity.z > 0 ) {
					newVelocity.z += Mathf.Min(	desiredForce * acel * Time.fixedDeltaTime, Mathf.Max( velMax - newVelocity.z, 0f ) );
				} else {
					newVelocity.z += Mathf.Min( desiredForce * freno * Time.fixedDeltaTime, Mathf.Max( velMax - newVelocity.z, 0f ) );
				}
			} else {
				if ( newVelocity.z < 0 ) {
					newVelocity.z += Mathf.Max( desiredForce * acel * Time.fixedDeltaTime, Mathf.Min( -velMax - newVelocity.z, 0f ) );
				} else {
					newVelocity.z += Mathf.Max( desiredForce * freno * Time.fixedDeltaTime, Mathf.Min( -velMax - newVelocity.z, 0f ) );
				}
			}

			targetBody.velocity = newVelocity;
		}
	}

}

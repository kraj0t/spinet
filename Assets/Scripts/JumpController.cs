using UnityEngine;
using System.Collections;

public class JumpController : MonoBehaviour {

	// true if the body should try to jump.
	// The input code should set this.
	public bool desiredJumpState = false;

	// true if the body should try to hover.
	// The input code should set this.
	public bool desiredHoverState = false;


	public Rigidbody targetBody;

	public WheelsController wheelsController;

	// Sobre que colisiones se puede saltar.
	public LayerMask colisionesLayer;
	
	// Distancia hasta la que se hace el raycast para la inclinacion del terreno
	public float distanciaRayoTerreno = 0.3f;
	
	// Velocity that is applied immediately when ground-jumping.
	public float groundJumpStartVelocity = 5.0f;
	
	// Velocidad del hover del salto por segundo.
	public float hoverAccel = 22.5f;
	
	public float hoverMaxTime = 0.15f;

	// Velocity that is applied immediately when air-jumping.
	public float airJumpStartVelocity = 5.0f;

	// Time in seconds that the jump will be allowed AFTER the target stops being grounded.
	public float ghostJumpDelay = 0.1f;
	
	// Los saltos en pendientes menos inclinadas que este angulo se orientaran totalmente en la vertical.
	public float maxSlopeForFullUpwardsJump = 30f;

	// Los saltos en pendientes mas inclinadas que este angulo se orientaran totalmente en la normal de la superficie.
	public float maxSlopeForPartialUpwardsJump = 60f;

	// Number of air jumps that can be performed before landing.
	public int maxAirJumpsCount = 1;

//	// A jump will never increase the vertical speed beyond this value in case the body is already travelling fast.
//	public float maxVelVertical = 8f;

//	// A jump will never increase the horizontal speed beyond this value in case the body is already travelling fast.
//	public float maxVelHorizontal = 5f;


	[SerializeField]
	[HideInInspector]
	internal bool _saltando = false;

	internal float _timeLastJumpStarted = float.NegativeInfinity;

	[SerializeField]
	[HideInInspector]
	private int m_currentAirJumpCount;

	internal Vector3 m_lastGroundedNormal;
	internal float m_lastGroundedTime = float.NegativeInfinity;


	// Checks the state of the wheels, if there's ground beneath them.
	private void CheckIfGrounded ()
	{
		//if ( wheelsController.IsAnyWheelGrounded() ) {
		if ( wheelsController.IsAllWheelsGrounded() ) {
			RaycastHit hitInfo; 
			bool raycastResult = Physics.Raycast( targetBody.position, -targetBody.transform.up, out hitInfo, distanciaRayoTerreno, colisionesLayer.value );
			if ( raycastResult ) {
				m_currentAirJumpCount = 0;
				m_lastGroundedNormal = hitInfo.normal;
				m_lastGroundedTime = Time.unscaledTime;
			}
		}
	}

	public bool CanGroundJump () 
	{
		return (m_lastGroundedTime + ghostJumpDelay > Time.unscaledTime);
	}

	public bool CanAirJump ()
	{
		return ( !CanGroundJump() && m_currentAirJumpCount < maxAirJumpsCount );
	}


	private void DoStartGroundJump ()
	{
		Vector3 newVelocity = targetBody.velocity;

		float normalAngleFromUp = Vector3.Angle( m_lastGroundedNormal, Vector3.up );
		if ( normalAngleFromUp <= maxSlopeForFullUpwardsJump ) {
//			newVelocity.y += Mathf.Clamp( maxVelVertical - targetBody.velocity.y, 0f, groundJumpStartVelocity ); 
			newVelocity.y = groundJumpStartVelocity;

//			newVelocity.y += groundJumpStartVelocity;
		} else {
			// This codes makes that the jump is oriented towards the surface normal.
			// The surface normal will have more weight on the direction for steeper surfaces.
			float clampedAngle = Mathf.Min( maxSlopeForPartialUpwardsJump, normalAngleFromUp );
			float normalizedOrientationToNormal = ( clampedAngle - maxSlopeForFullUpwardsJump ) / ( maxSlopeForPartialUpwardsJump - maxSlopeForFullUpwardsJump );
			Vector3 actualJumpNormal = (normalizedOrientationToNormal * m_lastGroundedNormal) + ((1f - normalizedOrientationToNormal) * Vector3.up);
			actualJumpNormal.Normalize();

			Vector3 appliedVelocity = actualJumpNormal * groundJumpStartVelocity;
//			if ( appliedVelocity.z > 0f ) {
//				appliedVelocity.z = Mathf.Min( appliedVelocity.z, Mathf.Max( 0f, maxVelHorizontal - targetBody.velocity.z ) );				
//			} else if ( appliedVelocity.z < 0f ) {
//				appliedVelocity.z = Mathf.Max( appliedVelocity.z, Mathf.Min( 0f, -maxVelHorizontal - targetBody.velocity.z ) );				
//			}
//			if ( appliedVelocity.y > 0f ) {
//				appliedVelocity.y = Mathf.Min( appliedVelocity.y, Mathf.Max( 0f, maxVelVertical - targetBody.velocity.y ) );				
//			} else if ( appliedVelocity.y < 0f ) {
//				appliedVelocity.y = Mathf.Max( appliedVelocity.y, Mathf.Min( 0f, -maxVelVertical - targetBody.velocity.y ) );				
//			}

			//newVelocity += appliedVelocity;
			newVelocity = appliedVelocity;
		}

		targetBody.velocity = newVelocity;
		
		_timeLastJumpStarted = Time.fixedTime;
		m_lastGroundedTime = float.NegativeInfinity;
		_saltando = true;
		
		Debug.DrawLine(targetBody.position, targetBody.position + ( m_lastGroundedNormal * 0.5f ), Color.green, .5f, false ); 	
	}

	private void DoHover () 
	{
		if ( Time.fixedTime <= _timeLastJumpStarted + hoverMaxTime )
		{
			Vector3 newVelocity = targetBody.velocity;
			newVelocity.y += Time.fixedDeltaTime * hoverAccel;
			targetBody.velocity = newVelocity;
		} else {
			_saltando = false;
		}
	}
		
	private void DoStartAirJump ()
	{
		// Differently from the regular ground jump, air jumps increase the velocity of the 
		// target in the direction that the main body is facing.
		targetBody.velocity = targetBody.transform.up * airJumpStartVelocity;

		m_currentAirJumpCount++;
		_timeLastJumpStarted = Time.fixedTime;
		_saltando = true;
		
		Debug.DrawLine(targetBody.position, targetBody.position + ( targetBody.rotation * Vector3.up * 0.5f ), Color.green, .5f, false ); 	
	}


	void FixedUpdate () 
	{		
		Debug.DrawLine(targetBody.position, targetBody.position + ( -targetBody.transform.up * distanciaRayoTerreno ), Color.white, 0f, false ); 

		// AÑADIR UN DELAY PARA FACILITAR EL SALTO CUANDO SE ABANDONA UNA PLATAFORMA.
		
		//		if ( !desiredJumpState || spikes._clavados ) {
//			_saltando = false;
//		}

		CheckIfGrounded();

		if ( _saltando ) {
			if ( desiredHoverState ) {
				DoHover();
			}
		} else {
			if ( desiredJumpState ) {
				if ( CanGroundJump() ) {
					DoStartGroundJump();
				} else if ( CanAirJump() ) {
					DoStartAirJump();
				}
			} else {
				_saltando = false;
			}
		}
	}
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class test_angularVelocity_lockedRigidbody : MonoBehaviour {

	public float maxAngVel = 60f;

	public float turnSpeed = 200000f;


	private Rigidbody m_rigidBody;


	// Use this for initialization
	void Start () {
		m_rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {
		float horizInput = Input.GetAxis( "moverHorizontal" );

		m_rigidBody.maxAngularVelocity = maxAngVel;
		
		// Vector3 newAngVel = m_rigidBody.angularVelocity;
		// newAngVel.z += horizInput * turnSpeed * Time.fixedDeltaTime;
		// newAngVel.x = newAngVel.y = 0f;
		// m_rigidBody.angularVelocity = newAngVel;

//		m_rigidBody.AddRelativeTorque( horizInput * turnSpeed * Time.fixedDeltaTime, 0f, 0f, ForceMode.Acceleration );
//		m_rigidBody.AddRelativeTorque( 0f, 0f, horizInput * turnSpeed * Time.fixedDeltaTime, ForceMode.Acceleration );
		m_rigidBody.AddTorque( horizInput * turnSpeed * Time.fixedDeltaTime, 0f, 0f, ForceMode.Acceleration );
//		m_rigidBody.AddTorque( 0f, 0f, -horizInput * turnSpeed * Time.fixedDeltaTime, ForceMode.Acceleration );

//		m_rigidBody.angularVelocity = new Vector3( 
//		              Mathf.Clamp( m_rigidBody.angularVelocity.x, -maxAngVel, maxAngVel ),
//		              Mathf.Clamp( m_rigidBody.angularVelocity.y, -maxAngVel, maxAngVel ),
//		              Mathf.Clamp( m_rigidBody.angularVelocity.z, -maxAngVel, maxAngVel )
//		);
	}
}

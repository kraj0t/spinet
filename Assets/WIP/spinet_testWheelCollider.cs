using UnityEngine;
using System.Collections;

public class spinet_testWheelCollider : MonoBehaviour {

	public WheelCollider[] wheels;
	public float accelForce;
	public float brakeForce;

	private float m_desiredAccel = 0f;


	void Start () {	
	}


	void Update () {
		m_desiredAccel = Input.GetAxis("moverHorizontal");
	}


	void FixedUpdate () {
		foreach ( WheelCollider wc in wheels ) {
			if ( m_desiredAccel != 0f ) {
				wc.motorTorque = m_desiredAccel * accelForce;
				wc.brakeTorque = 0f;
			} else {
				wc.motorTorque = 0f;
				wc.brakeTorque = brakeForce;
			}
		}
	}
}

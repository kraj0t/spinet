using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class test_flyAroundInput : MonoBehaviour {

	public float _DEBUG_accel = 4.0f;


	private Rigidbody m_rigidbody;


	void Start () {
		m_rigidbody = GetComponent<Rigidbody>();
	}


	void Update () {
		float horiz = Input.GetAxis( "moverHorizontal" );
		float verti = Input.GetAxis( "salto" ) - Input.GetAxis( "pinchos" );
		m_rigidbody.velocity += new Vector3( horiz, verti, 0.0f ) * _DEBUG_accel * Time.deltaTime;
		
		float giroHoriz = -Input.GetAxis( "girarHorizontal" );
		m_rigidbody.angularVelocity += new Vector3( 0f, 0f, giroHoriz ) * _DEBUG_accel * Time.deltaTime;
	}
}

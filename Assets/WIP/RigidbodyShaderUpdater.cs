using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class RigidbodyShaderUpdater : MonoBehaviour {

	public Rigidbody sourceRigidbody;

	public float velocitySmoothFactor = 0.1f;
	public float velocitySpringForce = 10f;
	public float velocitySpringDampness = 0.9f;

	public float angularVelocitySmoothFactor = 0.1f;
	public float angularVelocitySpringForce = 10f;
	public float angularVelocitySpringDampness = 0.9f;


	private Renderer m_renderer;

	private Vector3 m_smoothVelocity;
	private Vector3 m_smoothDampVelocity_smoothVelocity;
	private Vector3 m_velocityAccel;

	private Vector3 m_smoothAngularVelocity;
	private Vector3 m_smoothDampVelocity_smoothAngularVelocity;
	private Vector3 m_angularVelocityAccel;



	public static float Spring (float from, float to, float time)
	{
		time = Mathf.Clamp01(time);
		time = (Mathf.Sin(time * Mathf.PI * (.2f + 2.5f * time * time * time)) * Mathf.Pow(1f - time, 2.2f) + time) * (1f + (1.2f * (1f - time)));
		return from + (to - from) * time;
	}

	public static Vector3 Spring (Vector3 from, Vector3 to, float time)
	{
		return new Vector3( Spring(from.x, to.x, time), Spring(from.y, to.y, time), Spring(from.z, to.z, time) );
	}


	void Start () 
	{
		m_renderer = GetComponent<Renderer>();
	}


	void OnEnabled () 
	{
		m_smoothVelocity = sourceRigidbody.velocity;
		m_smoothAngularVelocity = sourceRigidbody.angularVelocity;
		m_velocityAccel = sourceRigidbody.velocity;
		m_angularVelocityAccel = sourceRigidbody.angularVelocity;
	}


	void FixedUpdate () {
		m_smoothVelocity = Vector3.SmoothDamp( m_smoothVelocity, sourceRigidbody.velocity, ref m_smoothDampVelocity_smoothVelocity, velocitySmoothFactor );
		m_smoothAngularVelocity = Vector3.SmoothDamp( m_smoothAngularVelocity, sourceRigidbody.angularVelocity, ref m_smoothDampVelocity_smoothAngularVelocity, angularVelocitySmoothFactor );

		Vector3 velocityResetVector = sourceRigidbody.velocity - m_smoothVelocity;
		m_velocityAccel += velocitySpringForce * velocityResetVector;
		m_velocityAccel *= velocitySpringDampness;
		m_smoothVelocity += m_velocityAccel * Time.deltaTime;

		Vector3 angularVelocityResetVector = sourceRigidbody.angularVelocity - m_smoothAngularVelocity;
		m_angularVelocityAccel += angularVelocitySpringForce * angularVelocityResetVector;
		m_angularVelocityAccel *= angularVelocitySpringDampness;
		m_smoothAngularVelocity += m_angularVelocityAccel * Time.deltaTime;
	}

	void Update ()
	{
		Matrix4x4 modelMatrix = Matrix4x4.TRS( transform.position, transform.rotation, transform.localScale );
		Matrix4x4 modelMatrixI = modelMatrix.inverse;

		Vector3 smoothVelocityOS = modelMatrixI * m_smoothVelocity;
		Vector3 smoothAngularVelocityOS = modelMatrixI * m_smoothAngularVelocity;

		m_renderer.material.SetVector( "_velocityOS", smoothVelocityOS );
		m_renderer.material.SetVector( "_angularVelocityOS", smoothAngularVelocityOS );
	}

}

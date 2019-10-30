using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class JetpackParticlesRenderer : MonoBehaviour {

	public JetpackController jetpack;

	public float emissionRate = 20f;


	private ParticleSystem m_particleSystem;


	// Use this for initialization
	void Start () {
		m_particleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		m_particleSystem.emissionRate = jetpack.desiredForce * emissionRate;
	}
}

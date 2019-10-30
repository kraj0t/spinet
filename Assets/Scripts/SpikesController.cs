using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikesController : MonoBehaviour {

	// true if the spikes should be enabled.
	// The input code should set this.
	public bool desiredState = false;

	public Rigidbody chasis;
	public WheelsController wheelsController;
	public Collider[] spikeWheelsColliders;
	public Collider[] wheelsCollidersWhenSpikesAreIn;
	public Collider[] wheelsCollidersWhenSpikesAreOut;
		
	// En que superficies nos podemos pinchar.
	public LayerMask colisionesPinchables;
	
	// Distancia hasta la que se hace el raycast para buscar el terreno.
	//public float distanciaRayoTerreno = 0.3f;
	
	public float gravedadFake = 100f;






	public float wheelsMaxAngularVelocity = 25f;
	public float engineMaxAngularVelocity = 20f;	
	public float potencia = 6000f;
	public float freno = 99999f;
	public float angularVelocityLossWhenAboveEngineMax = 200f;



	private float m_oldWheelsMaxAngularVelocity;
	private float m_oldEngineMaxAngularVelocity;
	private float m_oldPotencia;
	private float m_oldFreno;
	private float m_oldAngularVelocityLossWhenAboveEngineMax;

	[SerializeField]
	[HideInInspector]
	private Vector3 m_previousGravityNormal = Vector3.zero;
	public float DEBUG_gravityDirSmoothingTime = 0.2f;
	public float influenciaInputEnGravedad = 0.2f;
	private Vector3[] m_previousGravityNormalArray;


	internal bool _pinchosFuera = false;
	internal bool _clavados = false;


	// Use this for initialization
	void Start () {	
		m_previousGravityNormalArray = new Vector3[wheelsController.wheels.Length];
	}
	
	// Update is called once per frame
	void Update () {	
	}


	private void OnSpikesOut()
	{
		foreach ( Collider c in spikeWheelsColliders ) {
			c.enabled = true;
		}
		foreach ( Collider c in wheelsCollidersWhenSpikesAreIn ) {
			c.enabled = false;
		}
		foreach ( Collider c in wheelsCollidersWhenSpikesAreOut ) {
			c.enabled = true;
		}

		m_oldWheelsMaxAngularVelocity = wheelsController.wheelsMaxAngularVelocity;
		m_oldEngineMaxAngularVelocity = wheelsController.engineMaxAngularVelocity;	
		m_oldPotencia = wheelsController.potencia;
		m_oldFreno = wheelsController.freno;
		m_oldAngularVelocityLossWhenAboveEngineMax = wheelsController.angularVelocityLossWhenAboveEngineMax;

		wheelsController.wheelsMaxAngularVelocity = wheelsMaxAngularVelocity;
		wheelsController.engineMaxAngularVelocity = engineMaxAngularVelocity;	
		wheelsController.potencia = potencia;
		wheelsController.freno = freno;
		wheelsController.angularVelocityLossWhenAboveEngineMax = angularVelocityLossWhenAboveEngineMax;
	}


	private void OnSpikesIn() 
	{		
		foreach ( Collider c in spikeWheelsColliders ) {
			c.enabled = false;
		}
		foreach ( Collider c in wheelsCollidersWhenSpikesAreOut ) {
			c.enabled = false;
		}
		foreach ( Collider c in wheelsCollidersWhenSpikesAreIn ) {
			c.enabled = true;
		}

		wheelsController.wheelsMaxAngularVelocity = m_oldWheelsMaxAngularVelocity;
		wheelsController.engineMaxAngularVelocity = m_oldEngineMaxAngularVelocity;	
		wheelsController.potencia = m_oldPotencia;
		wheelsController.freno = m_oldFreno;
		wheelsController.angularVelocityLossWhenAboveEngineMax = m_oldAngularVelocityLossWhenAboveEngineMax;
	}


	void FixedUpdate () 
	{
//		wheelsController.brakeWhenIdle = desiredState;

		if ( desiredState && !_pinchosFuera ) {
			OnSpikesOut();
		} else if ( !desiredState && _pinchosFuera ) {
			OnSpikesIn();
		}
		_pinchosFuera = desiredState;



		if ( !desiredState ) {
			if ( _clavados ) {
				// TODO: DESCLAVARSE DE LA PARED!!!!
				_clavados = false;
			}
		} else {
			if ( !wheelsController.IsAnyWheelGrounded() ) {
				_clavados = false;
			} else {
				// At least one wheel is touching the ground.
				// We will now gather all the points where the wheels contacted geometry, for latter use.
				List<Vector3> _contacts = new List<Vector3>();

				foreach ( CollisionRecorder colRecord in wheelsController.wheels ) {
					foreach ( Collision col in colRecord.GetColisiones() ) {
						if ( ( 1 << col.gameObject.layer & colisionesPinchables.value ) != 0 ) {
							foreach ( ContactPoint cont in col.contacts ) {
								_contacts.Add( cont.point );
//								_contacts.Add( cont.normal );
								DebugX.DrawPoint( cont.point, Color.yellow, 0.05f );
							}
						}
					}
				}

				if ( _contacts.Count == 0 )	{
					_clavados = false;
				} else {
					bool justHooked = false;
					if ( !_clavados ) {
						justHooked = true;

						// When the spikes become hooked, we immediately stop the car.
						chasis.velocity = Vector3.zero;
						chasis.angularVelocity = Vector3.zero;
//						chasis.Sleep();
//						chasis.isKinematic = true;
//						chasis.isKinematic = false;
						foreach ( CollisionRecorder colRecord in wheelsController.wheels ) {
							colRecord.GetComponent<Rigidbody>().velocity = Vector3.zero;
							colRecord.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
//							colRecord.GetComponent<Rigidbody>().Sleep();
//							colRecord.GetComponent<Rigidbody>().isKinematic = true;
//							colRecord.GetComponent<Rigidbody>().isKinematic = false;
						}
						chasis.velocity = Vector3.zero;
						chasis.angularVelocity = Vector3.zero;
//						chasis.Sleep();
//						chasis.isKinematic = true;
//						chasis.isKinematic = false;
					}
					_clavados = true;

					Vector3 _midPoint = Vector3.zero;
					foreach ( Vector3 v3 in _contacts ) {
						_midPoint += v3;
					}
					_midPoint /= _contacts.Count;

					Vector3 normalAtMidPoint = chasis.position - _midPoint;
//					Vector3 normalAtMidPoint = _midPoint;
					normalAtMidPoint.Normalize();



					/// LA NORMAL HA DE CALCULARSE DESDE LAS RUEDAS, NO DESDE EL CHASIS
					normalAtMidPoint = Vector3.zero;
					foreach ( CollisionRecorder colRecord in wheelsController.wheels ) {
						if ( colRecord.GetColisiones().Length != 0 ) {
							Vector3 _thisMidPoint = Vector3.zero;
							int numPoints = 0;
							foreach ( Collision col in colRecord.GetColisiones() ) {
								if ( ( 1 << col.gameObject.layer & colisionesPinchables.value ) != 0 ) {
									foreach ( ContactPoint cont in col.contacts ) {
										_thisMidPoint += cont.point;
										numPoints++;
									}
								}
							}
							_thisMidPoint /= numPoints;
							Vector3 thisNormalAtMidPoint = colRecord.GetComponent<Rigidbody>().position - _thisMidPoint;
							thisNormalAtMidPoint.Normalize();
							normalAtMidPoint += thisNormalAtMidPoint;
						}
					}
					normalAtMidPoint.Normalize();
					/// end LA NORMAL HA DE CALCULARSE DESDE LAS RUEDAS, NO DESDE EL CHASIS







					/// TEST 2
//					if ( justHooked ) {
//						m_previousGravityNormal = _midPoint;
//					} else {
//						m_previousGravityNormal = Vector3.Lerp( m_previousGravityNormal, _midPoint, Time.fixedDeltaTime / DEBUG_gravityDirSmoothingTime );
//					}
//					DebugX.DrawPoint( m_previousGravityNormal, Color.blue, 0.075f );
//					normalAtMidPoint = chasis.position - m_previousGravityNormal;
//					normalAtMidPoint.Normalize();
					/// END TEST 2


					//// TEST
					float moverHorizInput = Input.GetAxis("moverHorizontal");
					float girarHorizInput = Input.GetAxis("girarHorizontal");
					normalAtMidPoint += influenciaInputEnGravedad * (girarHorizInput - moverHorizInput) * chasis.transform.forward;
					/// END TEST

					/// TEST PARA QUE NO TEMBLEQUEEN LAS RUEDAS
					bool shouldSleep = false;
					if ( moverHorizInput == 0f && girarHorizInput == 0f ) {
						bool allSleepy = true;
						foreach ( CollisionRecorder colRecord in wheelsController.wheels ) {
							if ( Mathf.Abs(colRecord.GetComponent<Rigidbody>().angularVelocity.x) > 1f && colRecord.GetComponent<Rigidbody>().velocity.sqrMagnitude > 0.1f ) {
								allSleepy = false;
							}
							colRecord.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
						}
//						shouldSleep = allSleepy;
					}
					/// end TEST PARA QUE NO TEMBLEQUEEN LAS RUEDAS

					normalAtMidPoint.Normalize();
//					normalAtMidPoint.x = 0.0f;

					if ( justHooked ) {
						m_previousGravityNormal = normalAtMidPoint;
					} else {
	//					m_previousGravityNormal = Vector3.Lerp( m_previousGravityNormal, normalAtMidPoint, Time.fixedDeltaTime / DEBUG_gravityDirSmoothingTime );
						m_previousGravityNormal = Vector3.Slerp( m_previousGravityNormal, normalAtMidPoint, Time.fixedDeltaTime / DEBUG_gravityDirSmoothingTime );
						m_previousGravityNormal.Normalize();
					}


					/// CADA RUEDA POR SEPARADO
//					for ( int i = 0; i < wheelsController.wheels.Length; i++ ) {
//						CollisionRecorder colRecord = wheelsController.wheels[i];
//						Vector3 _avgNormal = Vector3.zero;
//						foreach ( Collision col in colRecord.GetColisiones() ) {
//							if ( ( 1 << col.gameObject.layer & colisionesPinchables.value ) != 0 ) {
//								foreach ( ContactPoint cont in col.contacts ) {
//									_avgNormal += cont.normal;
//								}
//							}
//						}
//						if ( _avgNormal == Vector3.zero ) {
//							colRecord.GetComponent<Rigidbody>().AddForce( Physics.gravity, ForceMode.Acceleration );
//							Debug.DrawRay( colRecord.transform.position, Physics.gravity * 0.1f, Color.red, 0f, false );
//						} else {
//							_avgNormal.Normalize();
//							
//							m_previousGravityNormalArray[i] += influenciaInputEnGravedad * (girarHorizInput - moverHorizInput) * chasis.transform.forward;
//							_avgNormal.Normalize();
//							
//							if ( justHooked ) {
//								m_previousGravityNormalArray[i] = _avgNormal;
//							} else {
//								m_previousGravityNormalArray[i] = Vector3.Slerp( m_previousGravityNormalArray[i], _avgNormal, Time.fixedDeltaTime / DEBUG_gravityDirSmoothingTime );
//								m_previousGravityNormalArray[i].Normalize();
//							}
//							
//							colRecord.GetComponent<Rigidbody>().AddForce( -m_previousGravityNormalArray[i] * gravedadFake, ForceMode.Acceleration );
//							Debug.DrawRay( colRecord.transform.position, -m_previousGravityNormalArray[i] * 1f, Color.black, 0f, false );
//						}
//					}
					/// end CADA RUEDA POR SEPARADO


					m_previousGravityNormal.x = 0f;
					m_previousGravityNormal.Normalize();

//					chasis.AddForce( -normalAtMidPoint * gravedadFake, ForceMode.Acceleration );
					if ( shouldSleep ) {
						chasis.Sleep();
						foreach ( CollisionRecorder colRecord in wheelsController.wheels ) {
							colRecord.GetComponent<Rigidbody>().Sleep();
						}
					} else {
						chasis.AddForce( -m_previousGravityNormal * gravedadFake, ForceMode.Acceleration );
					}
					
					
					// NO FUNCIONA BIEN ESTO DE ROTAR
//					chasis.rotation.SetFromToRotation( chasis.transform.up, -normalAtMidPoint );
//					//_chasisRB.MoveRotation( Quaternion.LookRotation( -normalAtMidPoint, -chasis.transform.forward ) );

					Debug.DrawRay( chasis.position, -m_previousGravityNormal * 1f, Color.black, 0f, false );
				}
			}
		}
	}
}

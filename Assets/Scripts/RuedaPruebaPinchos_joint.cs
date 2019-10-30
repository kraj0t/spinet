using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RuedaPruebaPinchos_joint : MonoBehaviour {
	
	public float freno = 2000f;
	public float RPMMaxMotor = 35f;
	public float RPMMax = 100f;
	public float potencia = 1500f;
	public float perdidaDeVelocidad = 200f;
	
	private bool isPinchado = false;	
	private Dictionary<Collider, ConfigurableJoint> _joints = new Dictionary<Collider, ConfigurableJoint>();
	
	void Awake () {
	}
	
	void OnDrawGizmos ()
	{
		Gizmos.color = Color.green;
		if ( _joints != null && _joints.Values != null && _joints.Count > 0 )
			foreach ( ConfigurableJoint j in _joints.Values )
				Gizmos.DrawSphere( GetComponent<Rigidbody>().position + j.anchor + new Vector3( 5f, 0f, 0f ), .25f );
	}
	
	void FixedUpdate () 
	{
		GetComponent<Rigidbody>().maxAngularVelocity = RPMMax;
		
		actualizarRueda();
		
		if ( Input.GetKeyDown(KeyCode.W))
			GetComponent<Rigidbody>().velocity += new Vector3(0f, 50f, 0f);
		
		if (!Input.GetKey(KeyCode.S))
		{
			isPinchado = false;
			_joints.Clear();
		}
		
		if ( !isPinchado )
			actualizarGravedad();
	}
	
	
	
	private void actualizarGravedad ()
	{
		GetComponent<Rigidbody>().AddForce( Physics.gravity, ForceMode.Acceleration );
	}
	
	private void actualizarRueda ()
	{
		float _moverHorizInput = Input.GetAxis("moverHorizontal");
		
		Vector3 _localAV = transform.InverseTransformDirection( GetComponent<Rigidbody>().angularVelocity );;
		if ( _moverHorizInput == 0 )
			_localAV.x = Mathf.Sign( _localAV.x ) * Mathf.Max( 0f, Mathf.Abs( _localAV.x ) - ( freno * Time.fixedDeltaTime ) );
		else
		{
			if ( ( _moverHorizInput < 0 && _localAV.x > -RPMMaxMotor ) || ( _moverHorizInput > 0 && _localAV.x < RPMMaxMotor ) ) 
				_localAV.x += potencia * _moverHorizInput * Time.fixedDeltaTime;
			else
				_localAV.x -= perdidaDeVelocidad * _moverHorizInput * Time.fixedDeltaTime;
		}
		
		GetComponent<Rigidbody>().angularVelocity = transform.TransformDirection( _localAV.x, 0f, 0f );
	}
	
	
	void OnCollisionEnter ( Collision colInfo )
	{
		if ( Input.GetKey(KeyCode.S))
		{
			ConfigurableJoint _oldJoint = null;
			if ( _joints.TryGetValue( colInfo.collider, out _oldJoint ) )
			{
				_joints.Remove( colInfo.collider );
				Destroy( _oldJoint );
			}
			
	//		FixedJoint _joint = gameObject.AddComponent<FixedJoint>();
			ConfigurableJoint _joint = gameObject.AddComponent<ConfigurableJoint>();
			_joint.anchor = ( colInfo.contacts[0].point - GetComponent<Rigidbody>().position );
			_joint.xMotion = _joint.yMotion = _joint.zMotion = ConfigurableJointMotion.Locked;
			_joint.angularXMotion = ConfigurableJointMotion.Free;
			_joint.angularYMotion = _joint.angularZMotion = ConfigurableJointMotion.Locked;
	        _joint.connectedBody = colInfo.rigidbody;
			_joint.breakForce = 10f;
			_joint.breakTorque = 10f;
			
	//		_joints.Add( colInfo.collider, _joint );
			_joints[ colInfo.collider ] = _joint;
			
			isPinchado = true;
		}
	}
	
	void OnCollisionStay ( Collision colInfo )
	{
	}
	
	void OnCollisionExit ( Collision colInfo )
	{
		if ( Input.GetKey(KeyCode.S))
		{
			ConfigurableJoint _joint = _joints[ colInfo.collider ];
			_joints.Remove( colInfo.collider );
			Destroy( _joint );
		}
	}
}

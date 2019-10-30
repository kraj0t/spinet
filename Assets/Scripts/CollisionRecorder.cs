using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Este componente almacena las colisiones sufridas por el objeto durante un frame.
/// Recuerda que has de acceder a las colisiones desde FixedUpdate.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CollisionRecorder : MonoBehaviour {
	
	public static short longitudArray = 10;
  
	private List<Collision> _colisionesAnteriores;
	private List<Collision> _colisionesBuffer;
	
//		INTENTO DE HACER QUE SE PINCHE POR JOINTS
//	private Dictionary<Collider, ConfigurableJoint> _joints;
	
	// Devuelve las colisiones que hubo en el ultimo FixedUpdate.
	public Collision[] GetColisiones ()
	{
//		Debug.Log( _colisiones.ToArray().Length.ToString() );
//		if ( _colisiones.ToArray().Length == 0 )
//			Debug.Log("NADA DE COLIS");
//		if ( _colisiones.ToArray().Length >= 2 )
//			throw new UnityException("caca");
//		Debug.Log(Time.fixedTime.ToString());
		return _colisionesAnteriores.ToArray();
	}
	
	
	void Awake ()
	{
		_colisionesAnteriores = new List<Collision>( longitudArray );
		_colisionesBuffer = new List<Collision>( longitudArray );
		
//		INTENTO DE HACER QUE SE PINCHE POR JOINTS
//		_joints = new Dictionary<Collider, ConfigurableJoint>();
	}
	
	void FixedUpdate ()
	{
		_colisionesAnteriores = new List<Collision>( _colisionesBuffer );
		_colisionesBuffer.Clear();
	}
	
	void OnDrawGizmos ()
	{
//		INTENTO DE HACER QUE SE PINCHE POR JOINTS
//		Gizmos.color = Color.green;
//		if ( _joints.Count > 0 )
//			foreach ( ConfigurableJoint j in _joints.Values )
//				Gizmos.DrawSphere( rigidbody.position + j.anchor + new Vector3( 5f, 0f, 0f ), .25f );
	}
	
	void OnCollisionEnter ( Collision colInfo )
	{
		_colisionesBuffer.Add( colInfo );
		
//		INTENTO DE HACER QUE SE PINCHE POR JOINTS
//		ConfigurableJoint _oldJoint = null;
//		if ( _joints.TryGetValue( colInfo.collider, out _oldJoint ) )
//		{
//			_joints.Remove( colInfo.collider );
//			Destroy( _oldJoint );
//		}
//		
////		FixedJoint _joint = gameObject.AddComponent<FixedJoint>();
//		ConfigurableJoint _joint = gameObject.AddComponent<ConfigurableJoint>();
//		_joint.anchor = ( colInfo.contacts[0].point - rigidbody.position ) / 2f;
//		_joint.xMotion = _joint.yMotion = _joint.zMotion = ConfigurableJointMotion.Locked;
//		_joint.angularXMotion = ConfigurableJointMotion.Free;
//		_joint.angularYMotion = _joint.angularZMotion = ConfigurableJointMotion.Locked;
//        _joint.connectedBody = colInfo.rigidbody;
//		
////		_joints.Add( colInfo.collider, _joint );
//		_joints[ colInfo.collider ] = _joint;
	}
	
	void OnCollisionStay ( Collision colInfo )
	{
		_colisionesBuffer.Add( colInfo );
	}
	
	void OnCollisionExit ( Collision colInfo )
	{
//		INTENTO DE HACER QUE SE PINCHE POR JOINTS
//		ConfigurableJoint _joint = _joints[ colInfo.collider ];
//		_joints.Remove( colInfo.collider );
//		Destroy( _joint );
	}
	
}

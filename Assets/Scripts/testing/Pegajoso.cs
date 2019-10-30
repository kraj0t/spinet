using UnityEngine;
using System.Collections;

public class Pegajoso : MonoBehaviour 
{
    
	void OnCollisionEnter(Collision c) 
	{
        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = c.rigidbody;
    }
	
}
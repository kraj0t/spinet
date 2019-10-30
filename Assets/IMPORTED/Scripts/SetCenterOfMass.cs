using UnityEngine;
using System.Collections.Generic;


// Cambia el centro de masa del rigidbody de un objeto.
// El cambio se lleva a cabo en Start(), asi que si el Rigidbody cambia, el script no sirve.
[RequireComponent(typeof(Rigidbody))]
[ExecuteInEditMode()]
public class SetCenterOfMass : MonoBehaviour
{
    public Vector3 centerOfMassOverride = Vector3.zero;

    void Start()
    {
		GetComponent<Rigidbody>().centerOfMass = centerOfMassOverride;
    }

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere( GetComponent<Rigidbody>().position + centerOfMassOverride, 0.1f );
	}
}


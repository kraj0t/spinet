using UnityEngine;
using System.Collections;

public class MoverAnimadoAObjetivo : MonoBehaviour
{

	
	public Transform objetivoR;
	public Transform objetivoL;
	public string nombreEstadoR;
	public string nombreEstadoL;
	public float tiempoStart;
	public float tiempoEnd;
	
	private Animator _animator;


	// Use this for initialization
	void Start () {
		_animator = GetComponent<Animator>();	
	}

	// Update is called once per frame
	void Update () 
	{
		if (_animator)
		{
			AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
			
			if (state.IsName(nombreEstadoR) || state.IsName("Base Layer." + nombreEstadoR) )
			{
				if ( state.normalizedTime >= tiempoStart && state.normalizedTime < tiempoEnd )
				{
					transform.position = Vector3.Lerp( transform.position, objetivoR.position, Mathf.Lerp( tiempoStart, tiempoEnd, state.normalizedTime ) );
					transform.rotation = Quaternion.Lerp( transform.rotation, objetivoR.rotation, Mathf.Lerp( tiempoStart, tiempoEnd, state.normalizedTime ) );
				}
			}
			
			if (state.IsName(nombreEstadoL) || state.IsName("Base Layer." + nombreEstadoL) )
			{
				if ( state.normalizedTime >= tiempoStart && state.normalizedTime < tiempoEnd )
				{
					transform.position = Vector3.Lerp( transform.position, objetivoL.position, Mathf.Lerp( tiempoStart, tiempoEnd, state.normalizedTime ) );
					transform.rotation = Quaternion.Lerp( transform.rotation, objetivoL.rotation, Mathf.Lerp( tiempoStart, tiempoEnd, state.normalizedTime ) );
				}
			}
		}
	}
}


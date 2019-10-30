using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour {
	
	private static Dictionary<int, Checkpoint> _checkpoints = new Dictionary<int, Checkpoint>();
	
	public int indice;
	
	
	public static Checkpoint GetCheckpoint( int i )
	{
		try
		{
			return _checkpoints[ i ];
		}
		catch
		{
			return null;
		}
	}
	
	
	void OnEnable ()
	{
		_checkpoints.Add( indice, this );
	}
	
	void OnDisable ()
	{
		_checkpoints.Remove( indice );
	}
	
}

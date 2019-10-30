using UnityEngine;
using System.Collections;

public class DebugUtils : MonoBehaviour {
	
	public SpinetController coche;
	
	
	// Use this for initialization
	void Start () {
	}
	
	void Update () {
		if (coche) {
			Checkpoint _chk = coche.checkpointActual;
			if (Input.GetKeyDown (KeyCode.R)) {
					Debug.Log (_chk.indice.ToString ());
					coche.ResetToCheckpoint ();

					//			iTween.CameraFadeAdd();
			}
			if (Input.GetKeyDown (KeyCode.T)) {
					Checkpoint _newChk = Checkpoint.GetCheckpoint (_chk.indice + 1);
					for (int i = 0; _newChk == null; i++)
							_newChk = Checkpoint.GetCheckpoint (i);

					coche.checkpointActual = _newChk;			
					coche.ResetToCheckpoint ();

					//			iTween.CameraFadeAdd();
			}
		}

		if (Input.GetKeyDown(KeyCode.Y)) {
			if ( !Application.isLoadingLevel ) {
				Application.LoadLevel( (Application.loadedLevel+1) % Application.levelCount );
			}
		}
		
		hacerZoom();
	}
	
	
	private void hacerZoom ()
	{
		CameraPantallas _cp = Camera.main.GetComponent<CameraPantallas>();
		
		if ( _cp ) {
			if ( Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown( KeyCode.Keypad0 ) ) {
				_cp.zoom.enabled = !_cp.zoom.enabled;
			}
//			_cp.zoom.enabled = Input.GetKey(KeyCode.Z) || Input.GetKey( KeyCode.Keypad0 ); // Press and hold
		}
	}
}

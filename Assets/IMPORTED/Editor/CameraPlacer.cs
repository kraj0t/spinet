using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class CameraPlacer : EditorWindow
{
	private static bool _alignWithEditor = true;
	private static bool _liveUpdate = true;
	private Camera _camera;
	
	private static bool _showing = false;
 
	[ MenuItem( "Window/Camera placer %e" ) ]
	static void Launch()
	{
		CameraPlacer cp = GetWindow<CameraPlacer>( false, "Camera Placer" );		
		if ( !_showing )
		{
			cp.CheckForNewCameraSelection();
		}
		else
		{	
			cp._camera = null;
			cp.Close();
		}
		
		_showing = !_showing;
	}
	
 
	void Update()
	{
		if( _camera != null )
		{
			if ( _alignWithEditor )
			{
				_camera.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
				_camera.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
			}
		}
	}
 
	void OnSelectionChange()
	{
		CheckForNewCameraSelection();
	}

	void CheckForNewCameraSelection ()
	{
		_camera = ( Selection.activeTransform == null ) ? null : Selection.activeTransform.gameObject.GetComponent<Camera>();
	}
 
	void OnGUI()
	{
		if( _camera == null )
		{
			ToolbarGUI( "No camera selection" );
		}
		else
		{
			ToolbarGUI( _camera.gameObject.name );
		}
	}
 
	void ToolbarGUI( string title )
	{
		GUILayout.BeginHorizontal( "Toolbar" );
			GUILayout.Label( title );
			GUILayout.FlexibleSpace();
			_alignWithEditor = GUILayout.Toggle( _alignWithEditor, "Align with editor", "ToolbarButton" );
			_liveUpdate = GUILayout.Toggle( _liveUpdate, "Live update", "ToolbarButton" );
		GUILayout.EndHorizontal();
	}
}


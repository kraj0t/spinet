// Written By Aurelio Provedo ( aurelioprovedo@gmail.com )

using UnityEngine;
using System.Collections;
 
// An adaptable mouse cursor controller that ensures that it never leaves the screen.
// It can also peform easing so that the cursor moves smoothly.
// You can adapt this class to any device by changing the input axes that this class will check.
// This way, you may use the tilt, accelerometer, keys... to control the cursor.
[AddComponentMenu("Input")]
public class LockedSmoothCursor : MonoBehaviour {

	// If this is null, no cursor will be displayed.
	// You may want not to draw the cursor if you plan on adding some other special effect.
	public Texture2D cursorTexture;

	// Change this value only if you want the cursor not to appear on top of everything.
	public int cursorGUIDrawDepth = -9999;

	// Change this if you want to use the tilt, accelerometer... to control the cursor.
	public string horizontalInputAxis = "Mouse X";
	
	// Change this if you want to use the tilt, accelerometer... to control the cursor.
	public string verticalInputAxis = "Mouse Y";
	
	public float sensitivity = 10f;

	// If true, then the cursor will be forced to remain inside the screenConstraints.
	public bool constrainToScreen = true;

	// Values are relative to screen width, so 0 is left/top and 1 is right/bottom.
	// You can choose to use values below 0 and beyond 1, if you want to allow the cursor to leave the screen.
	public Rect screenConstraints = new Rect( 0, 0, 1, 1 );
	
	// Set this to a really high value if you don't want smoothed movement.
	public float easeSpeed = 5f;
	
	// Adds some inertia to the movement of the cursor.
	// Set this to zero for no inertia.
	// This value is automatically clamped between 0 and 1.
	[Range(0f,1f)]
	public float inertiaFactor = 0.5f;
	
	// The cursor's inertia decreases over time according to this value (it is multiplied by it).
	// The smaller this value is, the faster that the cursor will lose its inertia.
	// Set this to 1 for no damping, though that may result in a weird behavior.
	// This value is automatically clamped between 0 and 1.
	[Range(0f,1f)]
	public float inertiaDampFactor = 0.75f;

	// Set this to zero or negative if you don't want the cursor's speed to be limited.
	// It uses screen values, so it should go from 0 to 1.
	// Example: if set to 0.5, the cursor can travel half of the screen's width in one second.
	public float maxCursorScreenWidthSpeed = 0f;
	
	// This texture will be shown at the target position of the cursor.
	// This will only be displayed in the Editor.
	public Texture2D debugTargetCursorTexture;
	
	private Vector2 _targetPos01;
	private Vector2 _currentPos01;
	private Vector2 _inertiaVel = Vector2.zero;
	

	// Use this if you want to add some special effect, like a trail particle effect.
	public Vector2 GetCurrentCursorPosition ()
	{
		return _currentPos01;
	}
	
	// This is where the cursor is moving towards, the real desired position.
	// Use GetCurrentCursorPosition to get the position considering smooth and inertia.
	public Vector2 GetTargetCursorPosition ()
	{
		return _targetPos01;
	}


	void OnDisable () 
	{
		Cursor.lockState = CursorLockMode.None;
	}

	void Start ()
	{
		_targetPos01 = new Vector2( Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height );
		_currentPos01 = _targetPos01;
	}
	 
	void FixedUpdate () 
	{
		Cursor.lockState = CursorLockMode.Locked;
		
		// Update the desired target position for the cursor.
		Vector2 inputMovement = new Vector2( Input.GetAxis(horizontalInputAxis) / Screen.width, -Input.GetAxis(verticalInputAxis) / Screen.height ) * sensitivity;
		_targetPos01 += inputMovement;
		
		// Ensure that the cursor remains inside the desired margins.
		if ( constrainToScreen )
		{		
			_targetPos01.x = Mathf.Clamp( _targetPos01.x, screenConstraints.xMin, screenConstraints.xMax );
			_targetPos01.y = Mathf.Clamp( _targetPos01.y, screenConstraints.yMin, screenConstraints.yMax );
		}
		
		// Add some smooth to the movement.
		Vector2 easedMovement = ( _targetPos01 - _currentPos01 ) * easeSpeed * Time.fixedDeltaTime;
		
		// Update and apply inertia.
		_inertiaVel *= inertiaDampFactor;
		inertiaDampFactor = Mathf.Clamp01( inertiaDampFactor );
		_inertiaVel += easedMovement * inertiaFactor;
		easedMovement += _inertiaVel;
		
		Vector2 clampedMovement;
		// Limit the cursor speed.
		if ( maxCursorScreenWidthSpeed > 0f )
			clampedMovement = Vector2.ClampMagnitude( easedMovement, maxCursorScreenWidthSpeed * Time.fixedDeltaTime );			
		else
			clampedMovement = easedMovement;
		
		// Finally, set the actual cursor position.
		_currentPos01 += clampedMovement;
	}
	 
	void OnGUI () 
	{
		if ( cursorTexture )
		{
			GUI.depth = cursorGUIDrawDepth;
			
			if ( Application.isEditor && debugTargetCursorTexture )
			{
				GUI.DrawTexture( 
					new Rect( 
						( _targetPos01.x * Screen.width ) - (debugTargetCursorTexture.width/2),
						( _targetPos01.y * Screen.height ) - (debugTargetCursorTexture.height/2),
						debugTargetCursorTexture.width,
						debugTargetCursorTexture.height), 
					debugTargetCursorTexture);
			}
			
			GUI.DrawTexture( 
				new Rect( 
					( _currentPos01.x * Screen.width ) - (cursorTexture.width/2),
					( _currentPos01.y * Screen.height ) - (cursorTexture.height/2),
					cursorTexture.width,
					cursorTexture.height), 
				cursorTexture);			
		}
	}
	
}


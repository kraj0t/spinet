using UnityEngine;
using System.Collections;


public class OrbitCamera : MonoBehaviour
{

    /*
    CONTROLS
          WS/Arrows:    Movement
                  Q:    Climb
                  E:    Drop
              Shift:    Move faster
                Alt:    Move slower
                End:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
    */

    public Transform target;

    public Vector3 targetOffset;

	public bool processMouseInput = true;
	public bool processKeyboardInput = true;

    public float mouseSensitivity = 360f;
    public float climbSpeed = 2f;
    public float normalMoveSpeed = 2f;

    public float minDistance = 1f;
    public float maxDistance = Mathf.Infinity;

    [Range( -89, 89 )]
    public float minVerticalAngle = 5f;

    [Range( -89, 89 )]
    public float maxVerticalAngle = 50f;

    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3f;

    private Transform m_previousTarget;
    private Vector3 m_camToTarget;

    private bool m_shouldLock = true;

    void Start()
    {
        if ( target && processMouseInput ) {
			Cursor.lockState = m_shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
        }

        OnTargetChange();
    }

    private void OnTargetChange()
    {
        if ( target ) {
            Vector3 actualTargetPosition = target.position + targetOffset;
            m_camToTarget = transform.position - actualTargetPosition;
            if ( m_camToTarget.x == 0f && m_camToTarget.z == 0f ) {
                // This line avoids that the camera sits right above or below the target, as it causes problems.
                m_camToTarget.x = minDistance;
            }
            transform.LookAt( actualTargetPosition );
        }
    }

    void Update()
    {
		if ( Input.GetKeyDown( KeyCode.End ) && processMouseInput ) {
            m_shouldLock = !m_shouldLock;
			Cursor.lockState = m_shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
        }

        minDistance = Mathf.Clamp( minDistance, 0.1f, maxDistance );
        maxDistance = Mathf.Clamp( maxDistance, minDistance, Mathf.Infinity );
        minVerticalAngle = Mathf.Clamp( minVerticalAngle, -89f, maxVerticalAngle );
        maxVerticalAngle = Mathf.Clamp( maxVerticalAngle, minVerticalAngle, 89f );

        if ( target && Application.isPlaying ) {
            if ( target != m_previousTarget ) {
                OnTargetChange();
            }

            Vector3 actualTargetPosition = target.position + targetOffset;
            Vector3 camToTargetRight = Vector3.Cross( m_camToTarget, transform.up );            

	        float speedFactor = Time.deltaTime;
	        if ( processKeyboardInput ) {
	            if ( Input.GetKey( KeyCode.LeftShift ) || Input.GetKey( KeyCode.RightShift ) ) {
	                speedFactor *= fastMoveFactor;
	            }
	            if ( Input.GetKey( KeyCode.LeftAlt ) || Input.GetKey( KeyCode.RightAlt ) ) {
	                speedFactor *= slowMoveFactor;
	            }
	        }

			float horizontalTurn = 0f;

			if ( processMouseInput && Cursor.lockState == CursorLockMode.Locked ) {
				horizontalTurn += Input.GetAxis( "Mouse X" ) * mouseSensitivity * Time.deltaTime;
			}

	        if ( processKeyboardInput ) {
	            horizontalTurn += normalMoveSpeed * Mathf.Rad2Deg * speedFactor * Input.GetAxis( "Horizontal" );
	        }

			float verticalTurn = 0f;
			if ( processMouseInput && Cursor.lockState == CursorLockMode.Locked ) {
				verticalTurn += Input.GetAxis( "Mouse Y" ) * mouseSensitivity * Time.deltaTime;
			}

			Vector3 movement = Vector3.zero;

			if ( processMouseInput && Cursor.lockState == CursorLockMode.Locked ) {
				movement += -m_camToTarget * ( mouseSensitivity * speedFactor * 0.1f * Input.GetAxis( "Mouse ScrollWheel" ) );
			}

	        if ( processKeyboardInput ) {
	            movement += -m_camToTarget * ( normalMoveSpeed * speedFactor * Input.GetAxis( "Vertical" ) );
	        }
	                        
	        if ( processKeyboardInput ) {
	            float moveUpInput = 0f;
	            if ( Input.GetKey( KeyCode.Q ) ) {
	                moveUpInput += 1f;
	            }
	            if ( Input.GetKey( KeyCode.E ) ) {
	                moveUpInput -= 1f;
	            }

	            movement += Vector3.up * ( moveUpInput * climbSpeed * speedFactor );
	        }                

	        m_camToTarget = Vector3.RotateTowards( m_camToTarget, camToTargetRight, Mathf.Deg2Rad * horizontalTurn, 0f );
	        Vector3 xzVectorBeforeRotating = new Vector3( m_camToTarget.x, 0f, m_camToTarget.z );
	        m_camToTarget = Vector3.RotateTowards( m_camToTarget, Vector3.up, Mathf.Deg2Rad * verticalTurn * ( 1f-Mathf.Abs( m_camToTarget.normalized.y ) ), 0f );
	        m_camToTarget += movement;

	        // Make sure we remain in the allowed distance range.
	        m_camToTarget = m_camToTarget.normalized * Mathf.Clamp( m_camToTarget.magnitude, minDistance, maxDistance );
	        
	        // This code below makes sure that the vertical angles are respected.                        
	        float upAngle = Vector3.Angle( m_camToTarget.normalized, Vector3.up );
	        if ( upAngle < 90f - maxVerticalAngle ) {
	            Vector3 maxVerticalTurnVector = Vector3.RotateTowards( xzVectorBeforeRotating, Vector3.up, Mathf.Deg2Rad * maxVerticalAngle, 0f ).normalized;
	            m_camToTarget = maxVerticalTurnVector * m_camToTarget.magnitude;
	        }
	        if ( upAngle > 90f - minVerticalAngle ) {
	            Vector3 minVerticalTurnVector = Vector3.RotateTowards( xzVectorBeforeRotating, Vector3.up, Mathf.Deg2Rad * minVerticalAngle, 0f ).normalized;
	            m_camToTarget = minVerticalTurnVector * m_camToTarget.magnitude;
	        }
            
            transform.position = actualTargetPosition + m_camToTarget;
            transform.LookAt( actualTargetPosition );
        }

        m_previousTarget = target;
    }

    void OnGUI()
    {
        if ( Application.isEditor && Application.isPlaying ) {
            GUI.Label( new Rect( 0, 0, 500, 100 ), "- Press 'End' key to toggle Camera Orbit -" );
        }
    }
}





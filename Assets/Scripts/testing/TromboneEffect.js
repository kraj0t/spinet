var target: Transform;
var VELOCIDAD : float;

private var initHeightAtDist: float;
private var dzEnabled: boolean;


// Calculate the frustum height at a given distance from the camera.
function FrustumHeightAtDistance(distance: float) {
	return 2.0 * distance * Mathf.Tan(GetComponent.<Camera>().fieldOfView * 0.5 * Mathf.Deg2Rad);
}


// Calculate the FOV needed to get a given frustum height at a given distance.
function FOVForHeightAndDistance(height: float, distance: float) {
	return 2 * Mathf.Atan(height * 0.5 / distance) * Mathf.Rad2Deg;
}


// Start the dolly zoom effect.
function StartDZ() {
	var distance = Vector3.Distance(transform.position, target.position);
	initHeightAtDist = FrustumHeightAtDistance(distance);
	dzEnabled = true;
}


// Turn dolly zoom off.
function StopDZ() {
	dzEnabled = false;
}


function Start() {
	StartDZ();
}


function Update () {
	if (dzEnabled) {
		// Measure the new distance and readjust the FOV accordingly.
		var currDistance = Vector3.Distance(transform.position, target.position);
		GetComponent.<Camera>().fieldOfView = FOVForHeightAndDistance(initHeightAtDist, currDistance);
	}

	// Simple control to allow the camera to be moved in and out using the up/down arrows.
	var caca = 0;
	if (Input.GetKey(KeyCode.W))
		caca += 1;
	if (Input.GetKey(KeyCode.S))
		caca -= 1;
	transform.Translate(caca * Vector3.forward * Time.deltaTime * VELOCIDAD);
}
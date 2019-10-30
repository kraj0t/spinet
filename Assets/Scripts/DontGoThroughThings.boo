import UnityEngine
 
# DontGoThroughThings v1.1
# Created by Adrian on 2008-10-29.
# Original Script by Daniel Brauer
 
class DontGoThroughThings (MonoBehaviour):
 
	# ---------------------------------------- #
	# PUBLIC FIELDS
 
	# Layers the Raycast checks against
	# The game object of this script should not be on 
	# a layer set in this mask or it will collide with itself.
	public layerMask as LayerMask = System.Int32.MaxValue
 
	# How far the object is set into the object it 
	# shoud have collided with to force a physics collision
	# (Should probably be fine at 0.1 and must be between 0 and 1)
	public skinWidth as single = 0.1
 
	# Move the game object to this layer and remove 
	# the layer from the layer mask. This is a convenience 
	# feature to make sure the game object doesn't collide 
	# with itself. (set to -1 to disable)
	public switchToLayer as int = -1
 
	# Time in seconds before the script destroys itself 
	# after it has been created or initialized.
	# Convenient to protected the game object only for 
	# a critical time to avoid useless raycasts.
	# (set to -1 to disable)
	public timeToLive as single = -1
 
	# ---------------------------------------- #
	# PRIVATE FIELDS
 
	private startTime as single
 
	private originalLayer as int = -1
 
	private minimumExtent as single
	private partialExtent as single
	private sqrMinimumExtent as single
	private previousPosition as Vector3
 
	private myRigidbody as Rigidbody
 
	# ---------------------------------------- #
	# METHODS
 
	# Initialize the script on awake.
	def Awake():
		Init(timeToLive, switchToLayer)
 
	# Initialize method to be used when adding 
	# this component from a script.
	def Init(ttl as single, layer as int):
		# Switch layer of game object
		if (layer >= 0):
			originalLayer = gameObject.layer
			gameObject.layer = layer
			switchToLayer = layer
			# Clear the layer in the layer mask
			layerMask = layerMask.value & ~(1 << gameObject.layer)
 
		# Time to live
		if (ttl >= 0):
			startTime = Time.time
			timeToLive = ttl
 
		# Initialize variables
		myRigidbody = GetComponent[of Rigidbody]()
		previousPosition = myRigidbody.position
		minimumExtent = Mathf.Min(Mathf.Min(
							cast(single,GetComponent[of Collider]().bounds.extents.x), 
							cast(single,GetComponent[of Collider]().bounds.extents.y)), 
							cast(single,GetComponent[of Collider]().bounds.extents.z))
		sqrMinimumExtent = minimumExtent ** 2
		partialExtent = minimumExtent*(1.0 - skinWidth)
 
	# Collision checking
	def FixedUpdate():
		# Check time to live
		if (timeToLive > 0 and Time.time > startTime + timeToLive):
			# Restore original layer
			if (originalLayer >= 0):
				gameObject.layer = originalLayer
			Destroy(self)
			return
 
		# Only check for missed collisions if the game object moved more than its minimum extent
		if (myRigidbody):
			movementThisStep = myRigidbody.position - previousPosition
			movementSqrMagnitude = movementThisStep.sqrMagnitude
			if (movementSqrMagnitude > sqrMinimumExtent):
				movementMagnitude = Mathf.Sqrt(movementSqrMagnitude)
				hitInfo as RaycastHit
				if (Physics.Raycast(previousPosition, movementThisStep, hitInfo, 
										movementMagnitude, layerMask.value)):
					# Move rigidbody back to right before the collision was missed
					myRigidbody.MovePosition(
						hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent)
 
		previousPosition = myRigidbody.position
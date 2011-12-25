#pragma strict

// Public member data
public var motor : MovementMotor;
public var head : Transform;

public var targetDistanceMin : float = 3.0;
public var targetDistanceMax : float = 4.0;

public var weaponBehaviours : MonoBehaviour[];
public var fireFrequency : float = 2;

// Private memeber data
private var ai : AI;

private var character : Transform;

private var player : Transform;
private var lastTargetTime : float;

private var inRange : boolean = false;
private var nextRaycastTime : float = 0;
private var lastRaycastSuccessfulTime : float = 0;
private var noticeTime : float = 0;

private var firing : boolean = false;
private var lastFireTime : float = -1;
private var nextWeaponToFire : int = 0;

function Awake ()
{
	character = motor.transform;
	ai = transform.parent.GetComponentInChildren.<AI> ();
}

function OnEnable ()
{
	inRange = false;
	nextRaycastTime = Time.time + 1;
	lastRaycastSuccessfulTime = Time.time;
	noticeTime = Time.time;
}

function OnDisable ()
{
	Shoot (false);
}

function Shoot (state : boolean)
{
	Debug.Log("Shoot "+state);
	firing = state;
}

@RPC
function RPCFire()
{
	Fire();
}

function Fire ()
{
	if (weaponBehaviours[nextWeaponToFire])
	{
		weaponBehaviours[nextWeaponToFire].SendMessage ("Fire");
		nextWeaponToFire = (nextWeaponToFire + 1) % weaponBehaviours.Length;
		lastFireTime = Time.time;
	}
}

function Update ()
{
	if (player == null || Time.time - lastTargetTime > 0.2)
	{
		if (!Network.isServer)
			return;
			
		ObtainPlayerTarget();
		
		if (player == null)
			return;
			
		lastTargetTime = Time.time;
	}

	// Calculate the direction from the player to this character
	var playerDirection : Vector3 = (player.position - character.position);
	playerDirection.y = 0;
	var playerDist : float = playerDirection.magnitude;
	playerDirection /= playerDist;
	
	// Set this character to face the player,
	// that is, to face the direction from this character to the player
	motor.facingDirection = playerDirection;
	
	// For a short moment after noticing player,
	// only look at him but don't walk towards or attack yet.
	if (Time.time < noticeTime + 1.5)
	{
		motor.movementDirection = Vector3.zero;
		return;
	}
	
	if (inRange && playerDist > targetDistanceMax)
		inRange = false;
	if (!inRange && playerDist < targetDistanceMin)
		inRange = true;
	
	if (inRange)
		motor.movementDirection = Vector3.zero;
	else
		motor.movementDirection = playerDirection;
	
	if (Time.time > nextRaycastTime)
	{
		nextRaycastTime = Time.time + 1;
		if (ai.CanSeePlayer (player))
		{
			lastRaycastSuccessfulTime = Time.time;
			if (IsAimingAtPlayer ())
				Shoot (true);
			else
				Shoot (false);
		}
		else
		{
			Shoot (false);
			
			if (Time.time > lastRaycastSuccessfulTime + 5)
				ai.OnLostTrack ();
		}
	}
	
	if (firing && Time.time > lastFireTime + 1 / fireFrequency)
	{
		networkView.RPC("RPCFire", RPCMode.Others);
		Fire ();
	}
}

function IsAimingAtPlayer () : boolean
{
	var playerDirection : Vector3 = (player.position - head.position);
	playerDirection.y = 0;
	return Vector3.Angle (head.forward, playerDirection) < 15;
}

function ObtainPlayerTarget()
{
	var bestDistance : float = 99999999999.0;

	for (var go : GameObject in GameObject.FindGameObjectsWithTag("Player"))
	{
		var distance : float = (go.transform.position - character.position).sqrMagnitude;
	
		if (distance < bestDistance)
		{
			player = go.transform;
			bestDistance = distance;
		}
	}
}

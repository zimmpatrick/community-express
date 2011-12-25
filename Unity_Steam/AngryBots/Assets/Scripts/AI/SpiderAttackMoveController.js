#pragma strict

// Public member data
public var motor : MovementMotor;

public var targetDistanceMin : float = 2.0;
public var targetDistanceMax : float = 3.0;
public var proximityDistance : float = 4.0;
public var damageRadius : float = 5.0;
public var proximityBuildupTime : float = 2.0;
public var proximityOfNoReturn : float = 0.6;
public var damageAmount : float = 30.0;
public var proximityRenderer : Renderer;
public var audioSource : AudioSource;
public var blinkComponents : SelfIlluminationBlink[];
public var blinkPlane : GlowPlane;

public var intentionalExplosion : GameObject;
public var animationBehaviour : MonoBehaviour;

// Private memeber data
private var ai : AI;

private var character : Transform;

private var player : Transform;
private var lastTargetTime : float;

private var inRange : boolean = false;
private var nextRaycastTime : float = 0;
private var lastRaycastSuccessfulTime : float = 0;
private var proximityLevel : float = 0;
private var lastBlinkTime : float = 0;
private var noticeTime : float = 0;

function Awake ()
{
	character = motor.transform;
	ai = transform.parent.GetComponentInChildren.<AI> ();
	if (!blinkComponents.Length)
		blinkComponents = transform.parent.GetComponentsInChildren.<SelfIlluminationBlink> ();
}

function OnEnable ()
{
	inRange = false;
	nextRaycastTime = Time.time;
	lastRaycastSuccessfulTime = Time.time;
	noticeTime = Time.time;
	animationBehaviour.enabled = true;
	if (blinkPlane) 
		blinkPlane.renderer.enabled = false;	
}

function OnDisable ()
{
	if (proximityRenderer == null)
		Debug.LogError ("proximityRenderer is null", this);
	else if (proximityRenderer.material == null)
		Debug.LogError ("proximityRenderer.material is null", this);
	else
		proximityRenderer.material.color = Color.white;
	if (blinkPlane) 
		blinkPlane.renderer.enabled = false;
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

	if (Time.time < noticeTime + 0.7) {
		motor.movementDirection = Vector3.zero;
		return;
	}
	
	// Calculate the direction from the player to this character
	var playerDirection : Vector3 = (player.position - character.position);
	playerDirection.y = 0;
	var playerDist : float = playerDirection.magnitude;
	playerDirection /= playerDist;
	
	// Set this character to face the player,
	// that is, to face the direction from this character to the player
	//motor.facingDirection = playerDirection;
	
	if (inRange && playerDist > targetDistanceMax)
		inRange = false;
	if (!inRange && playerDist < targetDistanceMin)
		inRange = true;
	
	if (inRange)
		motor.movementDirection = Vector3.zero;
	else
		motor.movementDirection = playerDirection;
	
	if ((playerDist < proximityDistance && Time.time < lastRaycastSuccessfulTime + 1) || proximityLevel > proximityOfNoReturn)
		proximityLevel += Time.deltaTime / proximityBuildupTime;
	else
		proximityLevel -= Time.deltaTime / proximityBuildupTime;
	
	proximityLevel = Mathf.Clamp01 (proximityLevel);
	//proximityRenderer.material.color = Color.Lerp (Color.blue, Color.red, proximityLevel);
	if (proximityLevel == 1)
	{
		Explode();
		networkView.RPC("RPCExplode", RPCMode.OthersBuffered);
	}
	
	if (Time.time > nextRaycastTime)
	{
		nextRaycastTime = Time.time + 1;
		if (ai.CanSeePlayer (player))
		{
			lastRaycastSuccessfulTime = Time.time;
		}
		else {
			if (Time.time > lastRaycastSuccessfulTime + 2) {
				ai.OnLostTrack ();
			}
		}
	}

	UpdateBlink();	
	networkView.RPC("RPCUpdateBlink", RPCMode.OthersBuffered, proximityLevel);
}

@RPC
function RPCUpdateBlink(proxLevel : float)
{
	proximityLevel = proxLevel;
	UpdateBlink();
}

function UpdateBlink()
{
	var deltaBlink = 1 / Mathf.Lerp (2, 15, proximityLevel);
	if (Time.time > lastBlinkTime + deltaBlink)
	{
		lastBlinkTime = Time.time;
		proximityRenderer.material.color = Color.red;
		audioSource.Play ();
		
		for (var comp : SelfIlluminationBlink in blinkComponents)
			comp.Blink ();	

		if (blinkPlane) 
			blinkPlane.renderer.enabled = !blinkPlane.renderer.enabled;
	}

	if (Time.time > lastBlinkTime + 0.04)
		proximityRenderer.material.color = Color.white;
}

@RPC
function RPCExplode()
{
	Debug.Log("RPCExplode");
	Explode();
}

function Explode ()
{
	Debug.Log("Explode");
	if (Network.isServer)
	{
		Debug.Log("Explode2");
		var damageFraction : float = 1 - (Vector3.Distance (player.position, character.position) / damageRadius);
		
		var targetHealth : Health = player.GetComponent.<Health> ();
		if (targetHealth)
			// Apply damage
			targetHealth.OnDamage (damageAmount * damageFraction, character.position - player.position);
	
		player.rigidbody.AddExplosionForce (10, character.position, damageRadius, 0.0, ForceMode.Impulse);
	}
	
	Debug.Log("Explode3");
	Spawner.Spawn (intentionalExplosion, transform.position, Quaternion.identity);
	Spawner.Destroy (character.gameObject);
	Debug.Log("Explode4");
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

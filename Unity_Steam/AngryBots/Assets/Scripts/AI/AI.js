#pragma strict

// Public member data
public var behaviourOnSpotted : MonoBehaviour;
public var soundOnSpotted : AudioClip;
public var behaviourOnLostTrack : MonoBehaviour;

// Private memeber data
private var character : Transform;
private var player : Transform;
private var insideInterestArea : boolean = true;

function Awake () {
	character = transform;
}

function OnEnable () {
	behaviourOnLostTrack.enabled = true;
	behaviourOnSpotted.enabled = false;
}

function OnTriggerEnter (other : Collider) {
	ObtainPlayerTarget();

	if (other.transform == player && CanSeePlayer (player)) {
		OnSpotted ();
	}
}

function OnEnterInterestArea () {
	insideInterestArea = true;
}

function OnExitInterestArea () {
	insideInterestArea = false;
	OnLostTrack ();
}

function OnSpotted () {
	if (!insideInterestArea)
		return;
	if (!behaviourOnSpotted.enabled) {
		behaviourOnSpotted.enabled = true;
		behaviourOnLostTrack.enabled = false;
		
		if (audio && soundOnSpotted) {
			audio.clip = soundOnSpotted;
			audio.Play ();
		}
	}
}

function OnLostTrack () {
	if (!behaviourOnLostTrack.enabled) {
		behaviourOnLostTrack.enabled = true;
		behaviourOnSpotted.enabled = false;
	}
}

function CanSeePlayer (playerTarget : Transform) : boolean {
	if (playerTarget != null)
		player = playerTarget;
	else if (player == null)
		ObtainPlayerTarget();

	var playerDirection : Vector3 = (player.position - character.position);
	var hit : RaycastHit;
	Physics.Raycast (character.position, playerDirection, hit, playerDirection.magnitude);
	if (hit.collider && hit.collider.transform == player) {
		return true;
	}
	return false;
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

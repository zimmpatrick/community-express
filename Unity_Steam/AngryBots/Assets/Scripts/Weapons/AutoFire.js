#pragma strict

@script RequireComponent (PerFrameRaycast)

var bulletPrefab : GameObject;
var spawnPoint : Transform;
var frequency : float = 10;
var coneAngle : float = 1.5;
var firing : boolean = false;
var damagePerSecond : float = 20.0;
var forcePerSecond : float = 20.0;
var hitSoundVolume : float = 0.5;

var muzzleFlashFront : GameObject;

var owner : PlayerMoveController;

private var lastFireTime : float = -1;
private var raycast : PerFrameRaycast;

function Awake ()
{
	muzzleFlashFront.active = false;
	
	raycast = GetComponent.<PerFrameRaycast> ();
	if (spawnPoint == null)
		spawnPoint = transform;
}

function Update ()
{
	if (firing)
	{
		if (Time.time > lastFireTime + 1 / frequency)
		{
			// Spawn visual bullet
			var coneRandomRotation = Quaternion.Euler(Random.Range (-coneAngle, coneAngle), Random.Range (-coneAngle, coneAngle), 0);
			var go : GameObject = Spawner.Spawn (bulletPrefab, spawnPoint.position, spawnPoint.rotation * coneRandomRotation) as GameObject;
			var bullet : SimpleBullet = go.GetComponent.<SimpleBullet>();
			
			lastFireTime = Time.time;
			
			// Find the object hit by the raycast
			var hitInfo : RaycastHit = raycast.GetHitInfo ();
			if (hitInfo.transform)
			{
				if (owner.networkView.isMine)
				{
					// Get the health component of the target if any
					var targetHealth : Health = hitInfo.transform.GetComponent.<Health> ();
					if (targetHealth && targetHealth.health > 0.0)
					{
						// Apply damage
						targetHealth.OnDamage (damagePerSecond / frequency, -spawnPoint.forward);
						
						if (targetHealth.health <= 0.0)
							owner.OnKilledEnemy();
					}
				}
	
				// Get the rigidbody if any
				if (hitInfo.rigidbody)
				{
					// Apply force to the target object at the position of the hit point
					var force : Vector3 = transform.forward * (forcePerSecond / frequency);
					hitInfo.rigidbody.AddForceAtPosition (force, hitInfo.point, ForceMode.Impulse);
				}
				
				// Ricochet sound
				var sound : AudioClip = MaterialImpactManager.GetBulletHitSound (hitInfo.collider.sharedMaterial);
				AudioSource.PlayClipAtPoint (sound, hitInfo.point, hitSoundVolume);
				
				bullet.dist = hitInfo.distance;
			}
			else
				bullet.dist = 1000;
		}
	}
}

function OnStartFire()
{
	if (Time.timeScale == 0 || owner == null || !owner.networkView.isMine || firing)
		return;
	
	ClientStartFire();
	
	owner.networkView.RPC("RPCStartFire", RPCMode.Others);
}

function ClientStartFire()
{
	if (firing)
		return;

	firing = true;
	
	muzzleFlashFront.active = true;
	
	if (audio)
		audio.Play();
}

function OnStopFire ()
{
	if (!firing)
		return;
		
	firing = false;
	
	muzzleFlashFront.active = false;
	
	if (audio)
		audio.Stop ();

	if (owner != null && owner.networkView.isMine)
		owner.networkView.RPC("RPCStopFire", RPCMode.Others);
}
var objectPrefab : Transform;
private var spawned : boolean = false;

function Update()
{
	if (spawned)
		return;
		
	if (Network.isClient)
	{
		spawned = true;
		return;
	}

	if (Network.isServer)
	{		
		print("ObjectSpawnPrefab - OnNetworkStarted");
		Network.Instantiate(objectPrefab, transform.position, transform.rotation, 0);
		spawned = true;
	}
}

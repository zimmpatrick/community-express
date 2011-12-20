function OnNetworkInstantiate (msg : NetworkMessageInfo)
{
	// This is our own player
	if (networkView.isMine)
	{
		GetComponent("NetworkRigidbody").enabled = false;
		
		for (var go : GlowPlane in FindObjectsOfType(GlowPlane))
			go.playerTransform = GetComponent(PlayerMoveController).transform;
	}
	// This is just some remote controlled player, don't execute direct
	// user input on this
	else
	{
		name = "Remote";
		GetComponent("NetworkRigidbody").enabled = true;
	}
}
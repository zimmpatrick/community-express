function OnNetworkInstantiate (msg : NetworkMessageInfo)
{
	// If we are authoritative
	if (Network.isServer)
	{
		GetComponent("NetworkRigidbody").enabled = true;
	}
	// This is just some remote controlled player, don't execute direct
	// user input on this
	else
	{
		name = "Remote";
		GetComponent("NetworkRigidbody").enabled = true;
	}
}
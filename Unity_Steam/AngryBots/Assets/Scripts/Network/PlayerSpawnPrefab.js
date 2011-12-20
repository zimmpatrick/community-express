var playerPrefab : Transform;

function OnNetworkLoadedLevel ()
{
	Network.Instantiate(playerPrefab, transform.position, transform.rotation, 0);
}

function OnPlayerDisconnected (player : NetworkPlayer)
{
	Debug.Log("Server destroying player");
	Network.RemoveRPCs(player, 0);
	Network.DestroyPlayerObjects(player);
}

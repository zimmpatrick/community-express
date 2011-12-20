var supportedNetworkLevels : String[] = [ "mylevel" ];
var disconnectedLevel : String = "loader";

// Keep track of the last level prefix (increment each time a new level loads)
private var lastLevelPrefix = 0;

function Awake ()
{
	// Network level loading is done in a seperate channel.
	DontDestroyOnLoad(this);
	networkView.group = 1;
	Application.LoadLevel(disconnectedLevel);
}

function OnGUI ()
{
	// When network is running (server or client) then display the levels
	// configured in the supportedNetworkLevels array and allow them to be loaded
	// at the push of a button
	if (Network.peerType != NetworkPeerType.Disconnected)
	{
		GUILayout.BeginArea(Rect(0, Screen.height - 30, Screen.width, 30));
		GUILayout.BeginHorizontal();
		
		for (var level in supportedNetworkLevels)
		{
			if (GUILayout.Button(level))
			{
				// Make sure no old RPC calls are buffered and then send load level command
				Network.RemoveRPCsInGroup(0);
				Network.RemoveRPCsInGroup(1);
				// Load level with incremented level prefix (for view IDs)
				networkView.RPC( "LoadLevel", RPCMode.AllBuffered, level, lastLevelPrefix + 1);
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}

@RPC
function LoadLevel (level : String, levelPrefix : int)
{
	Debug.Log("Loading level " + level + " with prefix " + levelPrefix);
	lastLevelPrefix = levelPrefix;

	// There is no reason to send any more data over the network on the default channel,
	// because we are about to load the level, thus all those objects will get deleted anyway
	Network.SetSendingEnabled(0, false);	

	// We need to stop receiving because first the level must be loaded.
	// Once the level is loaded, RPC's and other state update attached to objects in the level are allowed to fire
	Network.isMessageQueueRunning = false;
		
	// All network views loaded from a level will get a prefix into their NetworkViewID.
	// This will prevent old updates from clients leaking into a newly created scene.
	Network.SetLevelPrefix(levelPrefix);
	Application.LoadLevel(level);
	yield;
	yield;

	// Allow receiving data again
	Network.isMessageQueueRunning = true;
	// Now the level has been loaded and we can start sending out data
	Network.SetSendingEnabled(0, true);

	// Notify our objects that the level and the network is ready
	for (var go in FindObjectsOfType(GameObject))
		go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);	
}

function OnDisconnectedFromServer ()
{
	Application.LoadLevel(disconnectedLevel);
}

@script RequireComponent(NetworkView)

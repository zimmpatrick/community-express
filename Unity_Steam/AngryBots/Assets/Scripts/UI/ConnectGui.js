//DontDestroyOnLoad(this);
var remoteIP = "127.0.0.1";
var remotePort = 8794;
var listenPort = 8793;
var remoteGUID = "";
var useNat = false;
private var connectionInfo = "";

private var showServerList : boolean = false;
private var serverList : CommunityExpressNS.Servers = null;

private var communityExpress : UnityCommunityExpress = null;

function Awake()
{
	if (FindObjectOfType(ConnectGuiMasterServer))
		this.enabled = false;

	for (var go : GameObject in FindObjectsOfType(GameObject))
	{
		communityExpress = go.GetComponentInChildren(UnityCommunityExpress);
		
		if (communityExpress != null)
			break;
	}
}

function OnGUI ()
{
	GUILayout.Space(10);
	GUILayout.BeginHorizontal();
	GUILayout.Space(200);
	if (Network.peerType == NetworkPeerType.Disconnected)
	{
		if (showServerList)
		{
			GUILayout.EndHorizontal();
			if (serverList)
			{
				for (var server : CommunityExpressNS.Server in serverList)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Space(200);
					GUILayout.Label(server.ServerName);
					if (GUILayout.Button("Connect"))
						Network.Connect(server.IP.ToString(), server.Port);						
					GUILayout.EndHorizontal();
				}
			}

			GUILayout.BeginHorizontal();
		}
		else
		{
			useNat = GUILayout.Toggle(useNat, "Use NAT punchthrough");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Space(200);

			GUILayout.BeginVertical();
			
			if (GUILayout.Button ("Connect"))
			{
				if (useNat)
				{
					if (!remoteGUID)
						Debug.LogWarning("Invalid GUID given, must be a valid one as reported by Network.player.guid or returned in a HostData struture from the master server");
					else
						Network.Connect(remoteGUID);
				}
				else
				{
					Network.Connect(remoteIP, remotePort);
				}
			}

			if (GUILayout.Button ("Start Server"))
			{
				Network.InitializeServer(32, listenPort, useNat);
				// Notify our objects that the level and the network is ready
				for (var go in FindObjectsOfType(GameObject))
					go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
					
				Debug.Log("Game Server Init="+communityExpress.GameServer.Init(false, System.Net.IPAddress.Any, listenPort, listenPort, 27015, CommunityExpressNS.EServerMode.eServerModeAuthenticationAndSecure, "Unity Community Express Server", "Spectators", "US", "CommunityExpress", "CommunityExpress", "1.0.0.0", "CE-Fake", 8, true, OnGameServerClientApproved, OnGameServerClientDenied, OnGameServerClientKick));
			}

			if (GUILayout.Button("Server List"))
			{
				showServerList = true;
				communityExpress.Matchmaking.RequestInternetServerList(null, OnServerReceivedCallback, OnServerListReceivedCallback);
			}

			GUILayout.EndVertical();
			
			if (useNat)
			{
				remoteGUID = GUILayout.TextField(remoteGUID, GUILayout.MinWidth(145));
			}
			else
			{
				remoteIP = GUILayout.TextField(remoteIP, GUILayout.MinWidth(100));
				remotePort = parseInt(GUILayout.TextField(remotePort.ToString()));
			}
		}
	}
	else
	{
		if (useNat)
			GUILayout.Label("GUID: " + Network.player.guid + " - ");
		GUILayout.Label("Local IP/port: " + Network.player.ipAddress + "/" + Network.player.port);
		GUILayout.Label(" - External IP/port: " + Network.player.externalIP + "/" + Network.player.externalPort);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(200);
		if (GUILayout.Button ("Disconnect"))
			Network.Disconnect(200);
	}
	GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
}

function OnServerInitialized()
{
	if (useNat)
		Debug.Log("==> GUID is " + Network.player.guid + ". Use this on clients to connect with NAT punchthrough.");
	Debug.Log("==> Local IP/port is " + Network.player.ipAddress + "/" + Network.player.port + ". Use this on clients to connect directly.");
}

function OnConnectedToServer()
{
	// Notify our objects that the level and the network is ready
	for (var go in FindObjectsOfType(GameObject))
		go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);		
}

function OnDisconnectedFromServer ()
{
	if (this.enabled != false)
		Application.LoadLevel(Application.loadedLevel);
	else
		FindObjectOfType(NetworkLevelLoad).OnDisconnectedFromServer();
}

function OnGameServerClientApproved(approvedPlayer : CommunityExpressNS.SteamID)
{
	Debug.Log("OnGameServerClientApproved "+approvedPlayer);
}

function OnGameServerClientDenied(deniedPlayer : CommunityExpressNS.SteamID, denyReason : CommunityExpressNS.EDenyReason, optionalText : String)
{
	Debug.Log("OnGameServerClientDenied "+deniedPlayer+" "+denyReason+" "+optionalText);
}

function OnGameServerClientKick(playerToKick : CommunityExpressNS.SteamID, denyReason : CommunityExpressNS.EDenyReason)
{
	Debug.Log("OnGameServerClientKick "+playerToKick+" "+denyReason);
}

function OnServerReceivedCallback(servers : CommunityExpressNS.Servers, server : CommunityExpressNS.Server)
{
	serverList = servers;
}

function OnServerListReceivedCallback(servers : CommunityExpressNS.Servers)
{
	serverList = servers;
}

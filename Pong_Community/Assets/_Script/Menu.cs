using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;

public class Menu : MonoBehaviour {
	
	public static Menu Instance;
	
	public bool isStart = false;
	public GameObject ball;
	public GameObject ball2;
	public GameObject paddleOne;
	public GameObject paddleTwo;
	private UnityCommunityExpress communityExpress = null;
	public CommunityExpressNS.User myPersona;
	public CommunityExpressNS.User otherPersona;
	private bool ShowFriends = false;
	private bool ShowColor = false;
	public GUIStyle userOfflineTextStyle;
	public GUIStyle userOnlineTextStyle;
	public GUIStyle userBusyTextStyle;
	public GUIStyle userAwayTextStyle;
	public GUIStyle userSnoozeTextStyle;
	public CommunityExpressNS.Stat HighestHitCount = null;
	private CommunityExpressNS.Achievement hitPaddle = null;
	private bool isPTP = true;
	public string userColor;
	public int numHits = 0;
	public int hitDisp = 0;
	private bool PlayerConnected = false;
	public bool PlayerOneReady = false;
	public bool PlayerTwoReady = false;
	public Texture2D PlayerOneImage;
	public Texture2D PlayerTwoImage;
	string[] achievement = {"ACH_WIN_ONE_GAME"};
	private CommunityExpressNS.Servers serverList = null;
	
	// Use this for initialization
	void Start () {
		
		Instance = this;
		Time.timeScale=5;
		GameObject go = GameObject.Find("CommunityExpress");
		communityExpress = go.GetComponentInChildren(typeof(UnityCommunityExpress)) as UnityCommunityExpress;
		myPersona = UnityCommunityExpress.Instance.User;
		string[] stats = { "NumWins" };
		communityExpress.UserStats.RequestCurrentStats(OnUserStatsRecieved, stats);
		
		if(!communityExpress.RemoteStorage.FileExists("UserColor")){
			communityExpress.RemoteStorage.WriteFile("UserColor", "white");
			userColor = "white";
			Debug.Log("Doesn't exist");
		}else{
			CommunityExpressNS.File file = communityExpress.RemoteStorage.GetFile("UserColor");
			//Debug.Log(string.Format("File: {0}, {1}, {2}", file.Exists, file.FileSize, file.FileName));
			
			userColor = file.ReadFile();
			Debug.Log(userColor);
		}
	}
	
	public void OnUserStatsRecieved(CommunityExpressNS.Stats stats, CommunityExpressNS.Achievements achievements)
	{
		if(HighestHitCount==null){
			HighestHitCount = communityExpress.UserStats.StatsList[0];
		}
		Debug.Log(HighestHitCount.StatValue + "  " + HighestHitCount.StatName);
		hitDisp = (int)HighestHitCount.StatValue;
		communityExpress.UserAchievements.InitializeAchievementList(achievement);
	}
	
	// Update is called once per frame
	void Update () {
		if(!isStart && Input.GetKeyDown(KeyCode.Return)){
			if(Network.isServer){
				PlayerOneReady=true;
			}
			if(Network.isClient){
				PlayerTwoReady=true;
			}
			if(PlayerOneReady&&PlayerTwoReady){
				isStart = true;
			}
			networkView.RPC("SetPlayerStartBool", RPCMode.All, PlayerOneReady, PlayerTwoReady, isStart);
		}
		
	}
	[RPC]void SetPlayerStartBool(bool p1, bool p2, bool ist){
		PlayerOneReady = p1;
		PlayerTwoReady = p2;
		isStart = ist;
	}
	public void SetColor(string colorStr){
		communityExpress.RemoteStorage.WriteFile("UserColor", colorStr);
		userColor = colorStr;
	}

    void OnPlayerConnected(NetworkPlayer player) {
		PlayerConnected = true;
    }
	void OnConnectedToServer() {
		Object tempObj;
		
		Network.Instantiate(ball2, new Vector3(-30, 0, -20), Quaternion.identity, 0);
		tempObj = Network.Instantiate(paddleTwo, new Vector3(130, 0, -20), Quaternion.identity, 1);
		tempObj.name = "PlayerTwo";
    }
	
	void OnServerReceivedCallback(CommunityExpressNS.Servers servers, CommunityExpressNS.Server server)
	{
		serverList = servers;
	}
	
	void OnServerListReceivedCallback(CommunityExpressNS.Servers servers)
	{
		serverList = servers;
	}
	
    void OnFailedToConnect(NetworkConnectionError error) {
        Debug.Log("Could not connect to server: " + error);
    }
	
	void OnGameServerClientApproved(CommunityExpressNS.SteamID approvedPlayer)
	{
		Debug.Log("OnGameServerClientApproved "+approvedPlayer);
	}
	
	void OnGameServerClientDenied(CommunityExpressNS.SteamID deniedPlayer , CommunityExpressNS.EDenyReason denyReason , string optionalText)
	{
		Debug.Log("OnGameServerClientDenied "+deniedPlayer+" "+denyReason+" "+optionalText);
	}
	
	void OnGameServerClientKick(CommunityExpressNS.SteamID playerToKick , CommunityExpressNS.EDenyReason denyReason)
	{
		Debug.Log("OnGameServerClientKick "+playerToKick+" "+denyReason);
	}

	void OnGUI(){
		Object tempObj;
		string myText="";
		if (serverList!=null)
		{
			int i = 0;
			foreach (CommunityExpressNS.Server server in serverList)
			{
				i++;
				if(server.ServerName=="Unity Community Express Server"){
				GUILayout.BeginHorizontal();
				GUILayout.Space(50);
				GUILayout.Label(server.ServerName);
				if (GUILayout.Button("Connect"))
					Network.Connect(server.IP.ToString(), server.Port);						
				GUILayout.EndHorizontal();
				
				}
				
				myText=i.ToString();
			}
			GUI.Button(new Rect(Screen.width/2+75, Screen.height/2 + 50, 150, 30), myText);
		}
		if (GUILayout.Button("Server List"))
		{
			//showServerList = true;
			Dictionary<string, string> filters = new Dictionary<string, string>();
			filters.Add("Unity Community Express Server", "CommunityExpress");
			
			communityExpress.Matchmaking.RequestInternetServerList(null, OnServerReceivedCallback, OnServerListReceivedCallback);
			//communityExpress.Matchmaking.
		}
		if(isPTP){
			ushort listenPort = 8793;
			ushort masterPort = 27015;
			IPAddress ipAd = IPAddress.Parse(Network.player.ipAddress);// ipAd = IPAd Network.player.ipAddress;
			if(GUI.Button(new Rect(Screen.width/2-75, Screen.height/2, 150, 30), "Create Game")){
				Network.InitializeServer(32, 8793);
				communityExpress.GameServer.Init(false,System.Net.IPAddress.Any , listenPort, listenPort, masterPort, masterPort, CommunityExpressNS.EServerMode.eServerModeAuthenticationAndSecure,"Unity Community Express Server", "Spectators", "US", "CommunityExpress", "CommunityExpress","1.0.0.0", "CE-Fake", 8, false,OnGameServerClientApproved, OnGameServerClientDenied, OnGameServerClientKick);
				Debug.Log(communityExpress.GameServer.ServerName);
				tempObj = Network.Instantiate(paddleOne, new Vector3(-130, 0, -20), Quaternion.identity, 0);
				Network.Instantiate(ball2, new Vector3(30, 0, -20), Quaternion.identity, 0);
				tempObj.name = "PlayerOne";
				isPTP=false;
			}
			if(GUI.Button(new Rect(Screen.width/2-75, Screen.height/2 + 50, 150, 30), "Join Game")){
				//Network.Connect("10.21.1.103", 8793);
				isPTP=false;
			}
		}
		if(!isStart&&!isPTP){
			GUI.Box(new Rect(Screen.width/2-75, Screen.height/2, 150, 100), "Player One = " + PlayerOneReady.ToString() + "\n" + "Player Two = " + PlayerTwoReady.ToString());
		}
		
			
		GUI.Box(new Rect(15, 15, 200, 64), "");
		GUI.DrawTexture(new Rect(15, 15, 64, 64), UnityCommunityExpress.Instance.ConvertImageToTexture2D(myPersona.MediumAvatar));
		GUI.Label(new Rect(80, 20, 200, 20), myPersona.PersonaName.ToString(), userOnlineTextStyle);
		GUI.Label(new Rect(80, 15+25, 200, 20), "Current Score " + numHits.ToString(), userOnlineTextStyle);
		GUI.Label(new Rect(80, 15+45, 200, 20), "Highest Score " + hitDisp.ToString(), userOnlineTextStyle);
		int y = 15;
		if(GUI.Button(new Rect(250, 15, 100, 30), "Friends")){
			ShowFriends = !ShowFriends;
			
		}
		if(ShowFriends){
			if(GUILayout.Button(communityExpress.GameServer.ServerName)){
				
		Debug.Log(communityExpress.GameServer.ServerName);
			}
			
			
			
			foreach (CommunityExpressNS.Friend f in UnityCommunityExpress.Instance.Friends)
			{
				GUI.Box(new Rect(250, y*3.5f, 200, 64), "");
				switch (f.PersonaState)
				{
					case CommunityExpressNS.EPersonaState.EPersonaStateOffline:
						GUI.Label(new Rect(320, y*3.5f+25, 200, 20), f.PersonaName.ToString(), userOfflineTextStyle);
						break;
		
					case CommunityExpressNS.EPersonaState.EPersonaStateOnline:
						GUI.Label(new Rect(320, y*3.5f+25, 200, 20), f.PersonaName.ToString(), userOnlineTextStyle);
						break;
		
					case CommunityExpressNS.EPersonaState.EPersonaStateBusy:
						GUI.Label(new Rect(320, y*3.5f+25, 200, 20), f.PersonaName.ToString(), userBusyTextStyle);
						break;
		
					case CommunityExpressNS.EPersonaState.EPersonaStateAway:
						GUI.Label(new Rect(320, y*3.5f+25, 200, 20), f.PersonaName.ToString(), userAwayTextStyle);
						break;
		
					case CommunityExpressNS.EPersonaState.EPersonaStateSnooze:
						GUI.Label(new Rect(320, y*3.5f+25, 200, 20), f.PersonaName.ToString(), userSnoozeTextStyle);
						break;
				}
				
				
				GUI.DrawTexture(new Rect(250, y*3.5f, 64, 64), UnityCommunityExpress.Instance.ConvertImageToTexture2D(f.MediumAvatar));
				//GUI.Label(new Rect(220, y*4, 200, 20), f.PersonaName.ToString());
				y += 20;
			}
		}
		
		if(GUI.Button(new Rect(850, 15, 100, 30), "Paddle Color")){
			ShowColor = !ShowColor;
			
		}
		
		if(ShowColor){
			if(GUI.Button(new Rect(850, 45, 100, 30), "White")){
				SetColor("white");
			}
			if(GUI.Button(new Rect(850, 90, 100, 30), "Blue")){
				SetColor("blue");
			}
			if(GUI.Button(new Rect(850, 135, 100, 30), "Red")){
				SetColor("red");
			}
			if(GUI.Button(new Rect(850, 180, 100, 30), "Green")){
				SetColor("green");
			}
		}
		
		
	}
		
	public void AddHitCounter(){
		hitDisp--;
		
			HighestHitCount.StatValue = hitDisp;
			communityExpress.UserStats.WriteStats();
	}
	
	public void UnlockAchievement(){
		if (hitPaddle == null)
			hitPaddle = communityExpress.UserAchievements.AchievementList[0];

		if (!hitPaddle.IsAchieved)
			communityExpress.UserAchievements.UnlockAchievement(hitPaddle, true);
	}
	
	public void MovePaddle(GameObject paddle){
		
	}
	
	
	
	public void GameOver(){
		if(hitDisp>numHits){
			Debug.Log(HighestHitCount.StatValue);
			hitDisp=0;
		}
		isStart = false;
		ball.transform.position = new Vector3(0, 0, -21);
	}
}

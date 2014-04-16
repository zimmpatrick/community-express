/*
 * Copyright © 2012 Zimmdot, LLC. All Rights Reserved.
 * 
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 
 * 2. The name of the author may not be used to endorse or promote products 
 * derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY ZIMMDOT, LLC "AS IS" AND ANY EXPRESS OR 
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; 
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF 
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;

public class Menu : MonoBehaviour {
	
	public static Menu Instance;
	
	public List<string> PlayerNames;
	public List<Texture2D> FriendImages;
	public int friendCount = 0;
	public bool isStart = false;
	public GameObject ball;
	public GameObject ball2;
	public GameObject paddleOne;
	public GameObject paddleTwo;
	private UnityCommunityExpress communityExpress = null;
	public CommunityExpressNS.User myPersona;
	public CommunityExpressNS.User otherPersona;
	public Texture2D myAvatar;
	public string myPersonaName = "NAME HERE";
	public Texture2D opponentAvatar;
	public string opponentPersonaName = "Opponent Name Here";
	public int opponentHitDisp = 0;
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
	public bool PlayerOneReady = false;
	public bool PlayerTwoReady = false;
	public bool isPlayerTwo = false;
	string[] achievement = {"ACH_WIN_ONE_GAME"};
	private CommunityExpressNS.Servers serverList = null;
	public GameObject textObj;
	private bool authSent = false;

	// Use this for initialization
	void Start () {
		Instance = this;
		GameObject go = GameObject.Find("CommunityExpress");
		communityExpress = go.GetComponentInChildren(typeof(UnityCommunityExpress)) as UnityCommunityExpress;
		myPersona = UnityCommunityExpress.Instance.User;
		myAvatar = UnityCommunityExpress.Instance.ConvertImageToTexture2D(myPersona.MediumAvatar);
		myPersonaName = myPersona.PersonaName;
		string[] stats = { "NumWins" };
		communityExpress.UserStats.RequestCurrentStats(stats);
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
		if(HighestHitCount==null && communityExpress.UserStats.StatsList.Count > 0){
			HighestHitCount = communityExpress.UserStats.StatsList[0];
		}
		
		if (HighestHitCount!=null)
		{
			Debug.Log(HighestHitCount.StatValue + "  " + HighestHitCount.StatName);
			hitDisp = (int)HighestHitCount.StatValue;
		}
		
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
		if(friendCount!=UnityCommunityExpress.Instance.Friends.Count){
			foreach (CommunityExpressNS.Friend f in UnityCommunityExpress.Instance.Friends)
			{
				FriendImages.Add(UnityCommunityExpress.Instance.ConvertImageToTexture2D(f.MediumAvatar));
			}
			friendCount = UnityCommunityExpress.Instance.Friends.Count;
		}
		if(Network.isServer){
			networkView.RPC("SetClientOpponent", RPCMode.Others, UnityCommunityExpress.Instance.User.SteamID.ToUInt64().ToString(), hitDisp);
		}
	}
	
	[RPC]void SetClientOpponent(string steamID, int clientNumWins){
		ulong idLong = ulong.Parse(steamID);
		
		CommunityExpressNS.SteamID sID = new CommunityExpressNS.SteamID(idLong);
		
		CommunityExpressNS.Friend f = UnityCommunityExpress.Instance.Friends.GetFriendBySteamID( sID);
		
		opponentAvatar = UnityCommunityExpress.Instance.ConvertImageToTexture2D(f.MediumAvatar);
		opponentPersonaName = f.PersonaName;
		
		opponentHitDisp = clientNumWins;
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
	
	void OnConnectedToServer() {
		GameObject tempObj;
		tempObj = Network.Instantiate(textObj, new Vector3(0.1f, 0.1f, 0), Quaternion.identity, 0) as GameObject;
		tempObj.guiText.text = myPersona.PersonaName;
		Network.Instantiate(ball2, new Vector3(-30, 0, -20), Quaternion.identity, 0);
		tempObj = Network.Instantiate(paddleTwo, new Vector3(130, 0, -20), Quaternion.identity, 1) as GameObject;
		tempObj.name = "PlayerTwo";
    }
	
	void OnServerReceivedCallback(CommunityExpressNS.Servers servers, CommunityExpressNS.Server server)
	{
		Debug.Log(servers.Count);
		serverList = servers;
		Debug.Log("running");
	}
	
	void OnServerListReceivedCallback(CommunityExpressNS.Servers servers)
	{
		serverList = servers;
		Debug.Log(" not running");
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
	
	void CreateNewGame(){
		Object tempObj;
		ushort listenPort = 8793;
		ushort masterPort = 27015;
		IPAddress ipAd = IPAddress.Any;
		Network.InitializeServer(32, masterPort, true);
		PlayerNames.Add(UnityCommunityExpress.Instance.User.PersonaName);
		Debug.Log(communityExpress.GameServer.Init(false, ipAd, listenPort, (ushort)(listenPort+1), masterPort, listenPort, CommunityExpressNS.EServerMode.eServerModeAuthenticationAndSecure,"Unity Community Express Server", "Spectators", "US", "CommunityExpress", "CommunityExpress","1.0.0.0", "CE-Fake", 8, false, "pong", OnGameServerClientApproved, OnGameServerClientDenied, OnGameServerClientKick));
		tempObj = Network.Instantiate(paddleOne, new Vector3(-130, 0, -20), Quaternion.identity, 0);
		Network.Instantiate(ball2, new Vector3(30, 0, -20), Quaternion.identity, 0);
		tempObj.name = "PlayerOne";
		isPTP=false;		
	}
	
	void OnGUI(){
		Object tempObj;
		string myText="";
		
		//Create server and Server browser
		//if(isPTP){
			ushort listenPort = 8793;
			ushort masterPort = 27015;
			IPAddress ipAd = IPAddress.Any; // IPAddress.Parse(Network.player.ipAddress);
			if(GUI.Button(new Rect(Screen.width/2-75, Screen.height/2, 150, 30), "Create Game")){
				CreateNewGame();
			}
		
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2 + 50, 150, 30),"Server List"))
			{
				//showServerList = true;
				Dictionary<string, string> filters = new Dictionary<string, string>();
				filters.Add("gameDir", "pong");
				communityExpress.Matchmaking.RequestInternetServerList(filters);
			}
			if (serverList!=null)
			{
				int i = 0;
				int servY = Screen.height/2;
				foreach (CommunityExpressNS.Server server in serverList)
				{
					i++;
					GUI.Label(new Rect(Screen.width/2+100, servY, 200, 30), server.ServerName);
					if (GUI.Button(new Rect(Screen.width/2+300, servY, 75, 30), "Connect")){
						Network.Connect(server.IP.ToString(), 27015);
						isPlayerTwo=true;
						isPTP=false;
						
					}
					
					servY+=50;
					
					myText=i.ToString();
				}
				GUI.Button(new Rect(Screen.width/2+75, Screen.height/2 + 50, 150, 30), myText);
			}
		//}
		if(!isStart&&!isPTP){
			GUI.Box(new Rect(Screen.width/2-75, Screen.height/2, 150, 100), "Player One = " + PlayerOneReady.ToString() + "\n" + "Player Two = " + PlayerTwoReady.ToString());
		}
		
		//Get Other Player Connected to Server
		foreach(CommunityExpressNS.Friend f in UnityCommunityExpress.Instance.GameServer.GetPlayersConnected()){
			//Compares the friend SteadID to local players SteamID
			if(f.SteamID!= myPersona.SteamID){
				//Since this is only a two player game we set the opponents data to a single value instead of a List<> of values
				opponentAvatar = UnityCommunityExpress.Instance.ConvertImageToTexture2D(f.MediumAvatar);
				opponentPersonaName = f.PersonaName;
			}
		}
		
		//Show Opponent Details
		if(opponentAvatar != null){
			GUI.Box(new Rect(15, 15, 200, 64), "");
			GUI.DrawTexture(new Rect(Screen.width - 200, 15, 64, 64), opponentAvatar);
			GUI.Label(new Rect(Screen.width - 135, 20, 200, 20), opponentPersonaName, userOnlineTextStyle);
			GUI.Label(new Rect(Screen.width - 135, 15+45, 200, 20), "Highest Score " + opponentHitDisp.ToString(), userOnlineTextStyle);
		}
		
		//Show local player details
		GUI.Box(new Rect(15, 15, 200, 64), "");
		GUI.DrawTexture(new Rect(15, 15, 64, 64), myAvatar);
		GUI.Label(new Rect(80, 20, 200, 20), myPersonaName, userOnlineTextStyle);
		GUI.Label(new Rect(80, 15+25, 200, 20), "Current Score " + numHits.ToString(), userOnlineTextStyle);
		GUI.Label(new Rect(80, 15+45, 200, 20), "Highest Score " + hitDisp.ToString(), userOnlineTextStyle);
		int y = 15;
		
		//Show persons friend list with color coded status
		if(GUI.Button(new Rect(250, 15, 100, 30), "Friends")){
			ShowFriends = !ShowFriends;
		}
		if(ShowFriends){
			int i = 0;
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
				
				
				GUI.DrawTexture(new Rect(250, y*3.5f, 64, 64), FriendImages[i]);
				//GUI.Label(new Rect(220, y*4, 200, 20), f.PersonaName.ToString());
				i++;
				y += 20;
			}
			
			
		}
		
		if(GUI.Button(new Rect(250, 50, 100, 30), "Paddle Color")){
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
		numHits++;
	}
	
	public void UnlockAchievement(){
		if (hitPaddle == null)
			hitPaddle = communityExpress.UserAchievements.AchievementList[0];
			
		if (!hitPaddle.IsAchieved)
			communityExpress.UserAchievements.UnlockAchievement(hitPaddle, true);
	}
	
	public void GameOver(){
		if(hitDisp<numHits){
			hitDisp=numHits;
			HighestHitCount.StatValue = hitDisp;
			communityExpress.UserStats.WriteStats();
		}
		LeaderBoard.Instance.UpdateLeaderboard(hitDisp);
		isStart = false;
		ball.transform.position = new Vector3(0, 0, -21);
	}
}

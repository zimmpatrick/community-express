using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkPlayerInfo : MonoBehaviour {
	
	
	public Texture2D PlayerIcon;
	public int HighestScore;
	public string PlayerName;
	public Texture2D PlayerIcon2;
	public int HighestScore2;
	public string PlayerName2;
	private UnityCommunityExpress communityExpress = null;
	private CommunityExpressNS.User myPersona;
	private Rect DataBoxRect = new Rect(15, 15, 200, 64);
	private Rect PictureBoxRect = new Rect(15, 15, 64, 64);
	private Rect NameRect = new Rect(80, 20, 200, 20);
	private Rect CurrentScoreRect = new Rect(80, 15+25, 200, 20);
	private Rect HighScoreRect = new Rect(80, 15+45, 200, 20);
	private Rect DataBoxRect2 = new Rect(Screen.width-215, 15, 200, 64);
	private Rect PictureBoxRect2 = new Rect(Screen.width-80, 15, 64, 64);
	private Rect NameRect2 = new Rect(Screen.width-200, 20, 200, 20);
	private Rect CurrentScoreRect2 = new Rect(Screen.width-200, 15+25, 200, 20);
	private Rect HighScoreRect2 =new Rect(Screen.width-200, 15+45, 200, 20);
	public GUIStyle userOfflineTextStyle;
	public GUIStyle userOnlineTextStyle;
	public GUIStyle userBusyTextStyle;
	public GUIStyle userAwayTextStyle;
	public GameObject guiFeedback;
	private bool authSent = false;
	public List<string> names = new List<string>();
	
	
	// Use this for initialization
	void Start () {
		communityExpress = UnityCommunityExpress.Instance;
		myPersona = communityExpress.User;
		
		
		
	}
	
	[RPC]void RPCInitClientAuth(string serverID)
	{
		Debug.Log("PlayerMoveController - RPCInitClientAuth");
		if (!authSent)
		{
			Debug.Log("PlayerMoveController - RPCInitClientAuth - isMine");
			guiFeedback.guiText.text = "PlayerMoveController - RPCInitClientAuth - isMine: " + serverID;
				
			byte[] authTicket = null;
			ushort exIP = (ushort)Network.player.externalPort;
			CommunityExpressNS.SteamID stID = new CommunityExpressNS.SteamID(ulong.Parse(serverID));
			System.Net.IPAddress netIP = System.Net.IPAddress.Parse(Network.player.externalIP);
			communityExpress.User.InitiateClientAuthentication(out authTicket, stID, netIP, exIP, true);
			authSent = true;
			
			networkView.RPC("RPCClientConnected", RPCMode.Server, authTicket);
		}
	}
	
	[RPC]void RPCClientConnected(byte[] authTicket)
	{
		Debug.Log("PlayerMoveController - RPCClientConnected");
		
		CommunityExpressNS.SteamID steamID = null;
		System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(networkView.owner.externalIP);
		communityExpress.GameServer.ClientConnected(ipAdd, authTicket, out steamID);
		
		//communityExpress.GameServer.RequestUserStats(steamID, OnUserStatsReceived, ["Kills"]);
	}
	
	
	// Update is called once per frame
	void Update () {
		
		
		Texture2D tempIcon = communityExpress.ConvertImageToTexture2D(myPersona.MediumAvatar);
		if(!Menu.Instance.isPlayerTwo){
			PlayerIcon = communityExpress.ConvertImageToTexture2D(myPersona.MediumAvatar);
		}else{
			//PlayerIcon2 = communityExpress.ConvertImageToTexture2D(myPersona.MediumAvatar);
		}
		if(networkView.isMine){
			networkView.RPC("GetPlayerInfo", RPCMode.All);
		}
		//Debug.Log(myPersona.SteamID.GetType());
		if (Network.isServer)
		{
			//Debug.Log("PlayerMoveController - Start - isServer");
			networkView.RPC("RPCInitClientAuth", RPCMode.Others, communityExpress.GameServer.SteamID.ToString());
			
		}
	}

	
	[RPC]void GetPlayerInfo(){
		PlayerIcon = communityExpress.ConvertImageToTexture2D(myPersona.MediumAvatar);
		PlayerIcon2 = communityExpress.ConvertImageToTexture2D(myPersona.MediumAvatar);
	}
	
	void OnGUI(){
		
		if(PlayerIcon!=null){
			GUI.Box(DataBoxRect, "");
			GUI.DrawTexture(PictureBoxRect, PlayerIcon);
			GUI.Label(NameRect, myPersona.PersonaName.ToString(), userOnlineTextStyle);
			GUI.Label(CurrentScoreRect, "Current Score " + Menu.Instance.numHits.ToString(), userOnlineTextStyle);
			GUI.Label(HighScoreRect, "Highest Score " + Menu.Instance.hitDisp.ToString(), userOnlineTextStyle);
		}
		if(PlayerIcon2!=null){
			GUI.Box(DataBoxRect2, "");
			GUI.DrawTexture(PictureBoxRect2, PlayerIcon2);
			GUI.Label(NameRect2, myPersona.PersonaName.ToString(), userOnlineTextStyle);
			GUI.Label(CurrentScoreRect2, "Current Score " + Menu.Instance.numHits.ToString(), userOnlineTextStyle);
			GUI.Label(HighScoreRect2, "Highest Score " + Menu.Instance.hitDisp.ToString(), userOnlineTextStyle);
		}
	}
}

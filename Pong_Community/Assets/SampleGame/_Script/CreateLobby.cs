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
using System;
using System.Text;
using System.Net;

public class CreateLobby : MonoBehaviour {
	
	private UnityCommunityExpress communityExpress = null;
	public CommunityExpressNS.Lobbies lobbyList = null;
	public CommunityExpressNS.Lobby joinedLobby = null;
	private int chatStartIndex = 0;
	private string chatText = "Input Message";
	private int lobbyMemberCount = 0;
	private List<string> lobbyMemberNames = new List<string>();
	private List<Texture2D> lobbyMemberAvatars = new List<Texture2D>();
	private bool hideLobbyList = true;
	private bool ShowFriends = false;
	private string chatLog = "";
	public List<Texture2D> FriendImages;
	public int friendCount = 0;
	
	
	// Use this for initialization
	void Start () {
		communityExpress = UnityCommunityExpress.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		if(friendCount!=UnityCommunityExpress.Instance.Friends.Count){
			foreach (CommunityExpressNS.Friend f in UnityCommunityExpress.Instance.Friends)
			{
				FriendImages.Add(UnityCommunityExpress.Instance.ConvertImageToTexture2D(f.MediumAvatar));
			}
			friendCount = UnityCommunityExpress.Instance.Friends.Count;
		}
	}
	
	void OnLobbyCreated(CommunityExpressNS.Lobby newLobby){
		Debug.Log("Lobby Created");
		newLobby.Join(OnLobbyJoined, OnLobbyDataUpdated, OnLobbyChatUpdated, OnLobbyChatMessage, OnLobbyGameCreated);
	}
	
	void OnListRecieved(CommunityExpressNS.Lobbies newLobbies){
		Debug.Log("Lobbies Retrieved");
		lobbyList = newLobbies;
	}
	
	void OnLobbyJoined(CommunityExpressNS.Lobby lobby, CommunityExpressNS.EChatRoomEnterResponse successMessage){
		Debug.Log("Joined Lobby" + successMessage);
		hideLobbyList = true;
		joinedLobby = lobby;
		joinedLobby.SendChatMessage("I've Joined the Lobby");
	}
	
	void OnLobbyDataUpdated(CommunityExpressNS.Lobby lobby, CommunityExpressNS.SteamID steamID, bool hasUpdatedData){
		Debug.Log("Lobby Data Updated");
	}
	
	void OnLobbyChatUpdated(CommunityExpressNS.Lobby lobby, CommunityExpressNS.SteamID steamID1, CommunityExpressNS.SteamID steamID2, CommunityExpressNS.EChatMemberStateChange memberStateChange){
		Debug.Log("Chat Updated");
	}
	
	void OnLobbyChatMessage(CommunityExpressNS.Lobby lobby, CommunityExpressNS.SteamID steamID, CommunityExpressNS.EChatEntryType entryType, byte[] chatMessage){
		Debug.Log("Chat Message");
		
		CommunityExpressNS.Friend newFriend = communityExpress.Friends.GetFriendBySteamID(steamID);
		
		chatLog = chatLog + "\n" + newFriend.PersonaName + ": " + joinedLobby.ConvertChatMessageToString(chatMessage);
	}
	
	void OnLobbyGameCreated(CommunityExpressNS.Lobby lobby, CommunityExpressNS.SteamID steamID, System.Net.IPAddress ipAddress, ushort newShort){
		Debug.Log("Lobby Game Created");
	}
	
	void OnGamePadTextInputDismissed(bool isDismissed, string textInput){
		Debug.Log("Gamepad Text Input Dismissed " + textInput);
	}
	
	void OnGUI(){
		if(joinedLobby==null){
			if(GUI.Button(new Rect(100, 100, 150, 30), "Create Lobby")){
				communityExpress.Matchmaking.CreateLobby(CommunityExpressNS.ELobbyType.k_ELobbyTypePublic,6, OnLobbyCreated);
			}
		
			if(GUI.Button(new Rect(100, 150, 150, 30), "Lobby List")){
				Debug.Log("Retrieving Lobbies");
				hideLobbyList = !hideLobbyList;
				ICollection<CommunityExpressNS.LobbyStringFilter> stringFilter = null;
				ICollection<CommunityExpressNS.LobbyIntFilter> intFilter = null;
				Dictionary<string, int> filter = null;
				ICollection<CommunityExpressNS.SteamID> steamIDFilter = null;
				communityExpress.Matchmaking.RequestLobbyList(stringFilter, intFilter ,filter , 1,CommunityExpressNS.ELobbyDistanceFilter.k_ELobbyDistanceFilterDefault,5,steamIDFilter, OnListRecieved);
			}
		
			if(!hideLobbyList){
				if(lobbyList!=null){
					int servY = 150;
					foreach (CommunityExpressNS.Lobby lobby in lobbyList)
					{
						GUI.Label(new Rect(Screen.width/2+130, servY, 300, 30), lobby.Count + " / " + lobby.GetMemberLimit());
						if (GUI.Button(new Rect(Screen.width/2+300, servY, 75, 30), "Connect")){
							lobby.Join(OnLobbyJoined, OnLobbyDataUpdated, OnLobbyChatUpdated, OnLobbyChatMessage, OnLobbyGameCreated);
							lobbyMemberCount = 0;
						}
						
						servY+=50;
					}
				}
			}
		}
		
		//Show BigPicture keyboard
		UnityCommunityExpress.Instance.BigPicture.ShowGamepadTextInput(CommunityExpressNS.EGamepadTextInputMode.k_EGamepadTextInputModeNormal, CommunityExpressNS.EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, chatText, 300, OnGamePadTextInputDismissed);
			
		if(joinedLobby!=null){
			//Chat log for lobby
			
			if(GUI.Button(new Rect(Screen.width - 250, (Screen.height - 100), 100, 50), "Send Message")){
				if(chatText!=""){
					joinedLobby.SendChatMessage(chatText);
				}else{
					chatText = "Please Enter In Text";
				}
			}
			
			
			
			GUI.TextArea(new Rect(50, Screen.height - 400, Screen.width - 300, 300),chatLog);
			
			chatText = GUI.TextField(new Rect(50, (Screen.height - 100), Screen.width - 300, 50), chatText, 300);
			
			
			//Show Players in Lobby
			
			if(joinedLobby.Count != lobbyMemberCount)
			{
				lobbyMemberNames.Clear();
				lobbyMemberAvatars.Clear();
				
				for(int i = 0;i<joinedLobby.Count;i++)
				{
					CommunityExpressNS.Friend newFriend = communityExpress.Friends.GetFriendBySteamID(joinedLobby.GetLobbyMemberByIndex(i));
					lobbyMemberNames.Add(newFriend.PersonaName);
					lobbyMemberAvatars.Add(UnityCommunityExpress.Instance.ConvertImageToTexture2D(newFriend.SmallAvatar));
					//GUI.Label(new Rect(Screen.width - 250, (Screen.height - 400), 250, 250), 
				}
				lobbyMemberCount = joinedLobby.Count;
			}
			
			int textureIndex = 0;
			foreach(string name in lobbyMemberNames){
				GUI.Label(new Rect(Screen.width - 180, (Screen.height - (400-(textureIndex*32))), 250, 250), name);
				GUI.DrawTexture(new Rect(Screen.width - 250, (Screen.height - (400-(textureIndex*32))), 32, 32), lobbyMemberAvatars[textureIndex]);
				textureIndex++;
			}
			
			//Leave Lobby
			if(GUI.Button(new Rect(50, Screen.height - 450, 150, 30), "Leave Lobby")){
				joinedLobby.Leave();
				joinedLobby = null;
				lobbyMemberCount = 0;
				
			}
			
			//Invite Friend
			if(GUI.Button(new Rect(200, Screen.height - 450, 150, 30), "Friends")){
				ShowFriends = !ShowFriends;
			}
			if(ShowFriends){
				int y = Screen.height - 500;
				int i = 0;
				foreach (CommunityExpressNS.Friend f in UnityCommunityExpress.Instance.Friends)
				{
					
					switch (f.PersonaState)
					{
						case CommunityExpressNS.EPersonaState.EPersonaStateOffline:
							GUI.Label(new Rect(250, y, 200, 20), f.PersonaName.ToString(), Menu.Instance.userOfflineTextStyle);
							break;
			
						case CommunityExpressNS.EPersonaState.EPersonaStateOnline:
							GUI.Label(new Rect(250, y, 200, 20), f.PersonaName.ToString(), Menu.Instance.userOnlineTextStyle);
							break;
			
						case CommunityExpressNS.EPersonaState.EPersonaStateBusy:
							GUI.Label(new Rect(250, y, 200, 20), f.PersonaName.ToString(), Menu.Instance.userBusyTextStyle);
							break;
			
						case CommunityExpressNS.EPersonaState.EPersonaStateAway:
							GUI.Label(new Rect(250, y, 200, 20), f.PersonaName.ToString(), Menu.Instance.userAwayTextStyle);
							break;
			
						case CommunityExpressNS.EPersonaState.EPersonaStateSnooze:
							GUI.Label(new Rect(250, y, 200, 20), f.PersonaName.ToString(), Menu.Instance.userSnoozeTextStyle);
							break;
					}
					
					
					GUI.DrawTexture(new Rect(200, y, 32, 32), FriendImages[i]);
					if(GUI.Button(new Rect(350, y, 200, 20), "Invite")){
						joinedLobby.Invite(f.SteamID);
					}
					i++;
					y -= 32;
				}
			}
			
		}
	}
}

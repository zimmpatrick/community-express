﻿// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;

namespace CommunityExpressNS
{
	using SteamAPICall_t = UInt64;
	using AppId_t = UInt32;
	using HServerListRequest = UInt32;

	// lobby type description
	public enum ELobbyType
	{
		k_ELobbyTypePrivate = 0,		// only way to join the lobby is to invite to someone else
		k_ELobbyTypeFriendsOnly = 1,	// shows for friends or invitees, but not in lobby list
		k_ELobbyTypePublic = 2,			// visible for friends and in lobby list
		k_ELobbyTypeInvisible = 3,		// returned by search, but not visible to other friends 
		//	useful if you want a user in two lobbies, for example matching groups together
		//	  a user can be in only one regular lobby, and up to two invisible lobbies
	}

	// lobby search filter tools
	public enum ELobbyComparison
	{
		k_ELobbyComparisonEqualToOrLessThan = -2,
		k_ELobbyComparisonLessThan = -1,
		k_ELobbyComparisonEqual = 0,
		k_ELobbyComparisonGreaterThan = 1,
		k_ELobbyComparisonEqualToOrGreaterThan = 2,
		k_ELobbyComparisonNotEqual = 3,
	}

	// lobby search distance. Lobby results are sorted from closest to farthest.
	public enum ELobbyDistanceFilter
	{
		k_ELobbyDistanceFilterClose,		// only lobbies in the same immediate region will be returned
		k_ELobbyDistanceFilterDefault,		// only lobbies in the same region or near by regions
		k_ELobbyDistanceFilterFar,			// for games that don't have many latency requirements, will return lobbies about half-way around the globe
		k_ELobbyDistanceFilterWorldwide,	// no filtering, will match lobbies as far as India to NY (not recommended, expect multiple seconds of latency between the clients)
	}

	//-----------------------------------------------------------------------------
	// Purpose: Chat Room Enter Responses
	//-----------------------------------------------------------------------------
	public enum EChatRoomEnterResponse
	{
		k_EChatRoomEnterResponseSuccess = 1,		// Success
		k_EChatRoomEnterResponseDoesntExist = 2,	// Chat doesn't exist (probably closed)
		k_EChatRoomEnterResponseNotAllowed = 3,		// General Denied - You don't have the permissions needed to join the chat
		k_EChatRoomEnterResponseFull = 4,			// Chat room has reached its maximum size
		k_EChatRoomEnterResponseError = 5,			// Unexpected Error
		k_EChatRoomEnterResponseBanned = 6,			// You are banned from this chat room and may not join
		k_EChatRoomEnterResponseLimited = 7,		// Joining this chat is not allowed because you are a limited user (no value on account)
		k_EChatRoomEnterResponseClanDisabled = 8,	// Attempt to join a clan chat when the clan is locked or disabled
		k_EChatRoomEnterResponseCommunityBan = 9,	// Attempt to join a chat when the user has a community lock on their account
		k_EChatRoomEnterResponseMemberBlockedYou = 10, // Join failed - some member in the chat has blocked you from joining
		k_EChatRoomEnterResponseYouBlockedMember = 11, // Join failed - you have blocked some member already in the chat
	}

	//-----------------------------------------------------------------------------
	// Purpose: Used in ChatInfo messages - fields specific to a chat member - must fit in a uint32
	//-----------------------------------------------------------------------------
	public enum EChatMemberStateChange
	{
		// Specific to joining / leaving the chatroom
		k_EChatMemberStateChangeEntered = 0x0001,		// This user has joined or is joining the chat room
		k_EChatMemberStateChangeLeft = 0x0002,			// This user has left or is leaving the chat room
		k_EChatMemberStateChangeDisconnected = 0x0004,	// User disconnected without leaving the chat first
		k_EChatMemberStateChangeKicked = 0x0008,		// User kicked
		k_EChatMemberStateChangeBanned = 0x0010,		// User kicked and banned
	};

	//-----------------------------------------------------------------------------
	// Purpose: Chat Entry Types (previously was only friend-to-friend message types)
	//-----------------------------------------------------------------------------
	public enum EChatEntryType
	{
		k_EChatEntryTypeInvalid = 0,
		k_EChatEntryTypeChatMsg = 1,			// Normal text message from another user
		k_EChatEntryTypeTyping = 2,				// Another user is typing (not used in multi-user chat)
		k_EChatEntryTypeInviteGame = 3,			// Invite from other user into that users current game
		k_EChatEntryTypeEmote = 4,				// text emote message (deprecated, should be treated as ChatMsg)
		//k_EChatEntryTypeLobbyGameStart = 5,	// lobby game is starting (dead - listen for LobbyGameCreated_t callback instead)
		k_EChatEntryTypeLeftConversation = 6,	// user has left the conversation ( closed chat window )
		// Above are previous FriendMsgType entries, now merged into more generic chat entry types
		k_EChatEntryTypeEntered = 7,			// user has entered the conversation (used in multi-user chat and group chat)
		k_EChatEntryTypeWasKicked = 8,			// user was kicked (data: 64-bit steamid of actor performing the kick)
		k_EChatEntryTypeWasBanned = 9,			// user was banned (data: 64-bit steamid of actor performing the ban)
		k_EChatEntryTypeDisconnected = 10,		// user disconnected
		k_EChatEntryTypeHistoricalChat = 11,	// a chat message from user's chat history or offilne message
	};

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct LobbyCreated_t
	{
	    internal const int k_iCallback = Events.k_iSteamMatchmakingCallbacks + 13;
		internal EResult m_eResult;
		internal UInt64 m_ulSteamIDLobby;		// chat room, zero if failed
	}

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct LobbyMatchList_t
    {
        internal const int k_iCallback = Events.k_iSteamMatchmakingCallbacks + 10;
        internal UInt32 m_nLobbiesMatching;		// Number of lobbies that matched search criteria and we have SteamIDs for
	}

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct LobbyEnter_t
    {
        internal const int k_iCallback = Events.k_iSteamMatchmakingCallbacks + 4;
        internal UInt64 m_ulSteamIDLobby;			// SteamID of the Lobby you have entered
        internal UInt32 m_rgfChatPermissions;		// Permissions of the current user
        internal Byte m_bLocked;					// If true, then only invited users may join
        internal UInt32 m_EChatRoomEnterResponse;	// EChatRoomEnterResponse
	}
	
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct LobbyDataUpdate_t
    {
        internal const int k_iCallback = Events.k_iSteamMatchmakingCallbacks + 5;
        internal UInt64 m_ulSteamIDLobby;		// steamID of the Lobby
        internal UInt64 m_ulSteamIDMember;	// steamID of the member whose data changed, or the room itself
        internal Byte m_bSuccess;				// true if we lobby data was successfully changed; 
											// will only be false if RequestLobbyData() was called on a lobby that no longer exists
	};

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct LobbyChatUpdate_t
    {
        internal const int k_iCallback = Events.k_iSteamMatchmakingCallbacks + 6;
        internal UInt64 m_ulSteamIDLobby;				// Lobby ID
        internal UInt64 m_ulSteamIDUserChanged;		// user who's status in the lobby just changed - can be recipient
        internal UInt64 m_ulSteamIDMakingChange;		// Chat member who made the change (different from SteamIDUserChange if kicking, muting, etc.)
													// for example, if one user kicks another from the lobby, this will be set to the id of the user who initiated the kick
        internal UInt32 m_rgfChatMemberStateChange;	// bitfield of EChatMemberStateChange values
	};

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct LobbyChatMsg_t
    {
        internal const int k_iCallback = Events.k_iSteamMatchmakingCallbacks + 7;
        internal UInt64 m_ulSteamIDLobby;	// the lobby id this is in
        internal UInt64 m_ulSteamIDUser;	// steamID of the user who has sent this message
        internal Byte m_eChatEntryType;	// type of message
        internal UInt32 m_iChatID;		// index of the chat entry to lookup
	};

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct LobbyGameCreated_t
    {
        internal const int k_iCallback = Events.k_iSteamMatchmakingCallbacks + 9;
		public UInt64 m_ulSteamIDLobby;			// the lobby we were in
		public UInt64 m_ulSteamIDGameServer;	// the new game server that has been created or found for the lobby members
		public UInt32 m_unIP;					// IP & Port of the game server (if any)
		public UInt16 m_usPort;
	};

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct servernetadr_t
	{
		public UInt16 m_usConnectionPort;	// (in HOST byte order)
		public UInt16 m_usQueryPort;
		public UInt32 m_unIP;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct gameserveritem_t
	{
		public servernetadr_t m_NetAdr;				// IP/Query Port/Connection Port for this server
		public Int32 m_nPing;						// current ping time in milliseconds
		public Byte m_bHadSuccessfulResponse;	// server has responded successfully in the past
		public Byte m_bDoNotRefresh;				// server is marked as not responding and should no longer be refreshed
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public char[] m_szGameDir;					// current game directory
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public char[] m_szMap;						// current map
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public char[] m_szGameDescription;			// game description
		public UInt32 m_nAppID;						// Steam App ID of this server
		public Int32 m_nPlayers;					// current number of players on the server
		public Int32 m_nMaxPlayers;					// Maximum players that can join this server
		public Int32 m_nBotPlayers;					// Number of bots (i.e simulated players) on this server
		public Byte m_bPassword;					// true if this server needs a password to join
		public Byte m_bSecure;					// Is this server protected by VAC
		public UInt32 m_ulTimeLastPlayed;			// time (in unix time) when this server was last played on (for favorite/history servers)
		public Int32 m_nServerVersion;				// server version as reported to Steam
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public char[] m_szServerName;				//  Game server name
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public char[] m_szGameTags;					// the tags this server exposes
		public UInt64 m_steamID;					// steamID of the game server - invalid if it's doesn't have one (old server, or not connected to Steam)
	}

	public struct LobbyStringFilter
	{
		public String key;
		public String value;
		public ELobbyComparison comparison;
	}

	public struct LobbyIntFilter
	{
		public String key;
		public Int32 value;
		public ELobbyComparison comparison;
	}

	delegate void OnMatchmakingServerReceivededFromSteam(HServerListRequest request, ref gameserveritem_t callbackData);
	delegate void OnMatchmakingServerListReceivededFromSteam(HServerListRequest request);

	public class Matchmaking
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamMatchmaking();
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamMatchmakingServers();
		[DllImport("CommunityExpressSW")]
		private static extern UInt32 SteamUnityAPI_SteamUtils_GetAppID();
		[DllImport("CommunityExpressSW")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamMatchmaking_CreateLobby(IntPtr matchmaking, ELobbyType lobbyType, Int32 maxMembers);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListStringFilter(IntPtr matchmaking, [MarshalAs(UnmanagedType.LPStr)] String key,
			[MarshalAs(UnmanagedType.LPStr)] String value, ELobbyComparison comparisonType);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListNumericalFilter(IntPtr matchmaking, [MarshalAs(UnmanagedType.LPStr)] String key,
			int value, ELobbyComparison comparisonType);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListNearValueFilter(IntPtr matchmaking, [MarshalAs(UnmanagedType.LPStr)] String key,
			int value);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable(IntPtr matchmaking, int slotsAvailable);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListDistanceFilter(IntPtr matchmaking, ELobbyDistanceFilter lobbyDistanceFilter);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListResultCountFilter(IntPtr matchmaking, int maxResults);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter(IntPtr matchmaking, UInt64 steamIDLobby);
		[DllImport("CommunityExpressSW")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamMatchmaking_RequestLobbyList(IntPtr matchmaking);
		[DllImport("CommunityExpressSW")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamMatchmaking_JoinLobby(IntPtr matchmaking, UInt64 steamIDLobby);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamMatchmaking_LeaveLobby(IntPtr matchmaking, UInt64 steamIDLobby);
		[DllImport("CommunityExpressSW")]
		private static extern UInt64 SteamUnityAPI_SteamMatchmaking_GetLobbyByIndex(IntPtr matchmaking, Int32 lobbyIndex);
		[DllImport("CommunityExpressSW")]
		private static extern Int32 SteamUnityAPI_SteamMatchmaking_GetLobbyChatEntry(IntPtr matchmaking, UInt64 steamIDLobby, Int32 chatID, out UInt64 steamID,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] Byte[] data, Int32 dataLength, out EChatEntryType chatEntryType);
		[DllImport("CommunityExpressSW")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestInternetServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("CommunityExpressSW")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestLANServerList(IntPtr matchmakingServers, AppId_t appId,
			IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("CommunityExpressSW")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestSpectatorServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("CommunityExpressSW")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestHistoryServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("CommunityExpressSW")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestFavoriteServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("CommunityExpressSW")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestFriendServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamMatchmakingServers_ReleaseRequest(IntPtr matchmakingServers, HServerListRequest request);

		public const HServerListRequest HServerListRequest_Invalid = 0x0;

		private IntPtr _matchmaking;
		private Lobbies _lobbyList = new Lobbies();
		private SteamAPICall_t _lobbyListRequest = 0;
		private Lobby _lobbyJoined = null;

		private IntPtr _matchmakingServers;
		private HServerListRequest _serverListRequest = HServerListRequest_Invalid;
		private Servers _serverList;
		private Dictionary<String, String> _serverFilters;

		private OnMatchmakingServerReceivededFromSteam _onServerReceivedFromSteam = null;
        private OnMatchmakingServerListReceivededFromSteam _onServerListReceivedFromSteam = null;
        private CommunityExpress _ce = CommunityExpress.Instance;

		internal Matchmaking()
		{
			_matchmaking = SteamUnityAPI_SteamMatchmaking();
			_matchmakingServers = SteamUnityAPI_SteamMatchmakingServers();
            
            _ce.AddEventHandler(LobbyCreated_t.k_iCallback, new CommunityExpress.OnEventHandler<LobbyCreated_t>(Events_LobbyCreated));
            _ce.AddEventHandler(LobbyEnter_t.k_iCallback, new CommunityExpress.OnEventHandler<LobbyEnter_t>(Event_LobbyJoinedCallback));
            _ce.AddEventHandler(LobbyDataUpdate_t.k_iCallback, new CommunityExpress.OnEventHandler<LobbyDataUpdate_t>(Event_LobbyDataUpdatedCallback));
            _ce.AddEventHandler(LobbyChatUpdate_t.k_iCallback, new CommunityExpress.OnEventHandler<LobbyChatUpdate_t>(Event_LobbyChatUpdatedCallback));
            _ce.AddEventHandler(LobbyChatMsg_t.k_iCallback, new CommunityExpress.OnEventHandler<LobbyChatMsg_t>(Event_LobbyChatMessageCallback));
            _ce.AddEventHandler(LobbyGameCreated_t.k_iCallback, new CommunityExpress.OnEventHandler<LobbyGameCreated_t>(Event_LobbyGameCreatedCallback));
		}

		public void CreateLobby(ELobbyType lobbyType, Int32 maxMembers)
		{
			if (CommunityExpress.Instance.IsGameServerInitialized)
			{
				System.Diagnostics.Debug.WriteLine("Unable to create a Lobby after a Game Server has been initialized.");
				throw new Exception("A Lobby cannot be created after a Steam Game Server has been initialized.");
			}
            SteamUnityAPI_SteamMatchmaking_CreateLobby(_matchmaking, lobbyType, maxMembers);
		}

        public delegate void CreateLobbyHandler(Matchmaking sender, Lobby lobby, EResult result);
        public event CreateLobbyHandler LobbyCreated;

        private void Events_LobbyCreated(LobbyCreated_t callbackData, bool Success, SteamAPICall_t hSteamAPICall)
        {
            Lobby lobby = new Lobby(null, new SteamID(callbackData.m_ulSteamIDLobby));

            if (LobbyCreated != null)
            {
                LobbyCreated(this, lobby, callbackData.m_eResult);
            }
        }

		public void RequestLobbyList(ICollection<LobbyStringFilter> stringFilters, ICollection<LobbyIntFilter> intFilters, Dictionary<String, Int32> nearValueFilters, Int32 requiredSlotsAvailable, ELobbyDistanceFilter lobbyDistance, Int32 maxResults, ICollection<SteamID> compatibleSteamIDs)
        {
            _ce.AddEventHandler(LobbyMatchList_t.k_iCallback, new CommunityExpress.OnEventHandler<LobbyMatchList_t>(Events_LobbyListReceivedCallback));

            if (_lobbyListRequest != 0)
            {
                CancelCurrentLobbyListRequest();
            }

			if (stringFilters != null)
			{
				foreach (LobbyStringFilter f in stringFilters)
				{
					SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListStringFilter(_matchmaking, f.key, f.value, f.comparison);
				}
			}
		
			if (intFilters != null)
			{
				foreach (LobbyIntFilter f in intFilters)
				{
					SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListNumericalFilter(_matchmaking, f.key, f.value, f.comparison);
				}
			}
		
			if (nearValueFilters != null)
			{
				foreach (KeyValuePair<String, Int32> kvp in nearValueFilters)
				{
					SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListNearValueFilter(_matchmaking, kvp.Key, kvp.Value);
				}
			}

			if (compatibleSteamIDs != null)
			{
				foreach (SteamID id in compatibleSteamIDs)
				{
					SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter(_matchmaking, id.ToUInt64());
				}
			}

			if (requiredSlotsAvailable != 0)
			{
				SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable(_matchmaking, requiredSlotsAvailable);
			}

			if (maxResults != 0)
			{
				SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListResultCountFilter(_matchmaking, maxResults);
			}

			SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListDistanceFilter(_matchmaking, lobbyDistance);

            _lobbyListRequest = SteamUnityAPI_SteamMatchmaking_RequestLobbyList(_matchmaking);
		}

        public delegate void LobbyListHandler(Matchmaking sender, Lobbies lobbyList);
        public event LobbyListHandler LobbyListReceived;

        private void Events_LobbyListReceivedCallback(LobbyMatchList_t callbackData, bool Success, SteamAPICall_t hSteamAPICall)
		{
			Lobbies lobbyList = new Lobbies();
			Lobby lobby;

			for (int i = 0; i < callbackData.m_nLobbiesMatching; i++)
			{
				UInt64 id = SteamUnityAPI_SteamMatchmaking_GetLobbyByIndex(_matchmaking, i);

				lobby = null;
				foreach (Lobby l in _lobbyList)
				{
					if (l.SteamID == id)
					{
						lobby = l;
						break;
					}
				}

				if (lobby == null)
				{
					lobby = new Lobby(_lobbyList, new SteamID(id));
				}

				lobbyList.Add(lobby);
			}

            LobbyListReceived(this, lobbyList);

            _ce.RemoveEventHandler<LobbyMatchList_t>(LobbyMatchList_t.k_iCallback, Events_LobbyListReceivedCallback);
		}

		public void CancelCurrentLobbyListRequest()
        {
            _ce.RemoveEventHandler<LobbyMatchList_t>(LobbyMatchList_t.k_iCallback, Events_LobbyListReceivedCallback);
			_lobbyListRequest = 0;
		}

		public Lobby JoinLobby(SteamID steamIDLobby)
		{
			Lobby lobby = null;

			foreach (Lobby l in _lobbyList)
			{
				if (l.SteamID == steamIDLobby)
				{
					lobby = l;
					break;
				}
			}

			if (lobby == null)
			{
				lobby = new Lobby(_lobbyList, steamIDLobby);
				_lobbyList.Add(lobby);
			}

			JoinLobby(lobby);

			return lobby;
		}

		public void JoinLobby(Lobby lobby)
		{
			if (_lobbyJoined != null)
			{
				LeaveLobby();
			}

			_lobbyJoined = lobby;

			SteamUnityAPI_SteamMatchmaking_JoinLobby(_matchmaking, lobby.SteamID.ToUInt64());
		}

        public delegate void LobbyJoinedHandler(Matchmaking sender, Lobby lobby, EChatRoomEnterResponse chatRoomEnterResponse);
        public event LobbyJoinedHandler LobbyJoined;

        private void Event_LobbyJoinedCallback(LobbyEnter_t callbackData, bool Success, SteamAPICall_t hSteamAPICall)
		{
			if (_lobbyJoined == null)
			{
				_lobbyJoined = new Lobby(_lobbyList, new SteamID(callbackData.m_ulSteamIDLobby));
				_lobbyList.Add(_lobbyJoined);
			}

			_lobbyJoined.IsLocked = callbackData.m_bLocked != 0;
			_lobbyJoined.ChatPermissions = callbackData.m_rgfChatPermissions;

            if (LobbyJoined != null)
            {
                LobbyJoined(this, _lobbyJoined, (EChatRoomEnterResponse)callbackData.m_EChatRoomEnterResponse);
            }
		}

		public Boolean LeaveLobby()
		{
			if (_lobbyJoined != null)
			{
				SteamUnityAPI_SteamMatchmaking_LeaveLobby(_matchmaking, _lobbyJoined.SteamID.ToUInt64());
				_lobbyJoined = null;
				return true;
			}

			return false;
		}

        public delegate void LobbyDataUpdatedHandler(Matchmaking sender, SteamID ID, bool Success);
        public event LobbyDataUpdatedHandler LobbyDataUpdated;

        void Event_LobbyDataUpdatedCallback(LobbyDataUpdate_t callbackData, bool Success, SteamAPICall_t hSteamAPICall)
		{
            if (LobbyDataUpdated != null)
            {
                LobbyDataUpdated(this, new SteamID(callbackData.m_ulSteamIDMember), callbackData.m_bSuccess != 0);
            }
		}

        public delegate void LobbyChatUpdatedHandler(Matchmaking sender, Lobby lobby, SteamID UserIDChanged, SteamID MakingIDChange, EChatMemberStateChange MemberStateChange);
        public event LobbyChatUpdatedHandler LobbyChatUpdated;

        void Event_LobbyChatUpdatedCallback(LobbyChatUpdate_t callbackData, bool Success, SteamAPICall_t hSteamAPICall)
		{
            if (LobbyChatUpdated != null)
            {
                LobbyChatUpdated(this, _lobbyJoined, new SteamID(callbackData.m_ulSteamIDUserChanged), new SteamID(callbackData.m_ulSteamIDMakingChange),
				    (EChatMemberStateChange)callbackData.m_rgfChatMemberStateChange);
            }
		}

        public delegate void LobbyChatMessageHandler(Matchmaking sender, Lobby lobby, SteamID ID, EChatEntryType chatEntryType, Byte[] data);
        public event LobbyChatMessageHandler LobbyChatMessage;

        void Event_LobbyChatMessageCallback(LobbyChatMsg_t callbackData, bool Success, SteamAPICall_t hSteamAPICall)
		{
			UInt64 steamID;
			Byte[] data = new Byte[4096];
			EChatEntryType chatEntryType;

			Array.Resize(ref data, SteamUnityAPI_SteamMatchmaking_GetLobbyChatEntry(_matchmaking, callbackData.m_ulSteamIDLobby, (Int32)callbackData.m_iChatID,
				out steamID, data, 4096, out chatEntryType));
            if (LobbyChatMessage != null)
            {
                LobbyChatMessage(this, _lobbyJoined, new SteamID(steamID), chatEntryType, data);
            }
		}

        public delegate void LobbyGameCreatedHandler(Matchmaking sender, Lobby lobby, SteamID GameServerID, IPAddress ipAddress, UInt16 port);
        public event LobbyGameCreatedHandler LobbyGameCreated;

        void Event_LobbyGameCreatedCallback(LobbyGameCreated_t callbackData, bool Success, SteamAPICall_t hSteamAPICall)
		{
			IPAddress ip = new IPAddress(new byte[] {(byte)(callbackData.m_unIP >> 24), (byte)(callbackData.m_unIP >> 16), (byte)(callbackData.m_unIP >> 8), (byte)callbackData.m_unIP});
            if (LobbyGameCreated != null)
            {
                LobbyGameCreated(this, _lobbyJoined, new SteamID(callbackData.m_ulSteamIDGameServer), ip, callbackData.m_usPort);
            }
		}

		private void PrepServerListRequest(Dictionary<String, String> filters, out String[] keys, out String[] values)
		{
			if (_serverListRequest != HServerListRequest_Invalid)
				CancelCurrentServerListRequest();

			_serverList = new Servers();
			_serverFilters = filters;

			if (_onServerReceivedFromSteam == null)
			{
				_onServerReceivedFromSteam = new OnMatchmakingServerReceivededFromSteam(OnServerReceived);
				_onServerListReceivedFromSteam = new OnMatchmakingServerListReceivededFromSteam(OnServerListComplete);
			}

			if (_serverFilters != null)
			{
				keys = new String[_serverFilters.Keys.Count];
				values = new String[_serverFilters.Values.Count];

				_serverFilters.Keys.CopyTo(keys, 0);
				_serverFilters.Values.CopyTo(values, 0);
			}
			else
			{
				keys = null;
				values = null;
			}
		}

		public Servers RequestInternetServerList(Dictionary<String, String> filters)
		{
			String[] keys, values;

			PrepServerListRequest(filters, out keys, out values);

			if (_serverFilters != null)
			{
				_serverListRequest = SteamUnityAPI_SteamMatchmakingServers_RequestInternetServerList(_matchmakingServers, (AppId_t)SteamUnityAPI_SteamUtils_GetAppID(),
					keys, values, (UInt32)_serverFilters.Count, Marshal.GetFunctionPointerForDelegate(_onServerReceivedFromSteam),
					Marshal.GetFunctionPointerForDelegate(_onServerListReceivedFromSteam));
			}
			else
			{
				_serverListRequest = SteamUnityAPI_SteamMatchmakingServers_RequestInternetServerList(_matchmakingServers, (AppId_t)SteamUnityAPI_SteamUtils_GetAppID(),
					null, null, 0, Marshal.GetFunctionPointerForDelegate(_onServerReceivedFromSteam), Marshal.GetFunctionPointerForDelegate(_onServerListReceivedFromSteam));
			}

			return _serverList;
		}

		public Servers RequestLANServerList()
		{
			String[] keys, values;

			PrepServerListRequest(null, out keys, out values);

			_serverListRequest = SteamUnityAPI_SteamMatchmakingServers_RequestLANServerList(_matchmakingServers, (AppId_t)SteamUnityAPI_SteamUtils_GetAppID(),
				Marshal.GetFunctionPointerForDelegate(_onServerReceivedFromSteam), Marshal.GetFunctionPointerForDelegate(_onServerListReceivedFromSteam));

			return _serverList;
		}

		public Servers RequestSpecatorServerList(Dictionary<String, String> filters)
		{
			String[] keys, values;

			PrepServerListRequest(filters, out keys, out values);

			if (_serverFilters != null)
			{
				_serverListRequest = SteamUnityAPI_SteamMatchmakingServers_RequestSpectatorServerList(_matchmakingServers, (AppId_t)SteamUnityAPI_SteamUtils_GetAppID(),
					keys, values, (UInt32)_serverFilters.Count, Marshal.GetFunctionPointerForDelegate(_onServerReceivedFromSteam),
					Marshal.GetFunctionPointerForDelegate(_onServerListReceivedFromSteam));
			}
			else
			{
				_serverListRequest = SteamUnityAPI_SteamMatchmakingServers_RequestSpectatorServerList(_matchmakingServers, (AppId_t)SteamUnityAPI_SteamUtils_GetAppID(),
					null, null, 0, Marshal.GetFunctionPointerForDelegate(_onServerReceivedFromSteam), Marshal.GetFunctionPointerForDelegate(_onServerListReceivedFromSteam));
			}

			return _serverList;
		}

		public Servers RequestHistoryServerList(Dictionary<String, String> filters)
		{
			String[] keys, values;

			PrepServerListRequest(filters, out keys, out values);

			if (_serverFilters != null)
			{
				_serverListRequest = SteamUnityAPI_SteamMatchmakingServers_RequestHistoryServerList(_matchmakingServers, (AppId_t)SteamUnityAPI_SteamUtils_GetAppID(),
					keys, values, (UInt32)_serverFilters.Count, Marshal.GetFunctionPointerForDelegate(_onServerReceivedFromSteam),
					Marshal.GetFunctionPointerForDelegate(_onServerListReceivedFromSteam));
			}
			else
			{
				_serverListRequest = SteamUnityAPI_SteamMatchmakingServers_RequestHistoryServerList(_matchmakingServers, (AppId_t)SteamUnityAPI_SteamUtils_GetAppID(),
					null, null, 0, Marshal.GetFunctionPointerForDelegate(_onServerReceivedFromSteam), Marshal.GetFunctionPointerForDelegate(_onServerListReceivedFromSteam));
			}

			return _serverList;
		}

		public Servers RequestFavoriteServerList(Dictionary<String, String> filters)
		{
			String[] keys, values;

			PrepServerListRequest(filters, out keys, out values);

			if (_serverFilters != null)
			{
				_serverListRequest = SteamUnityAPI_SteamMatchmakingServers_RequestFavoriteServerList(_matchmakingServers, (AppId_t)SteamUnityAPI_SteamUtils_GetAppID(),
					keys, values, (UInt32)_serverFilters.Count, Marshal.GetFunctionPointerForDelegate(_onServerReceivedFromSteam),
					Marshal.GetFunctionPointerForDelegate(_onServerListReceivedFromSteam));
			}
			else
			{
				_serverListRequest = SteamUnityAPI_SteamMatchmakingServers_RequestFavoriteServerList(_matchmakingServers, (AppId_t)SteamUnityAPI_SteamUtils_GetAppID(),
					null, null, 0, Marshal.GetFunctionPointerForDelegate(_onServerReceivedFromSteam), Marshal.GetFunctionPointerForDelegate(_onServerListReceivedFromSteam));
			}

			return _serverList;
		}

		public Servers RequestFriendServerList(Dictionary<String, String> filters)
		{
			String[] keys, values;

			PrepServerListRequest(filters, out keys, out values);

			if (_serverFilters != null)
			{
				_serverListRequest = SteamUnityAPI_SteamMatchmakingServers_RequestFriendServerList(_matchmakingServers, (AppId_t)SteamUnityAPI_SteamUtils_GetAppID(),
					keys, values, (UInt32)_serverFilters.Count, Marshal.GetFunctionPointerForDelegate(_onServerReceivedFromSteam),
					Marshal.GetFunctionPointerForDelegate(_onServerListReceivedFromSteam));
			}
			else
			{
				_serverListRequest = SteamUnityAPI_SteamMatchmakingServers_RequestFriendServerList(_matchmakingServers, (AppId_t)SteamUnityAPI_SteamUtils_GetAppID(),
					null, null, 0, Marshal.GetFunctionPointerForDelegate(_onServerReceivedFromSteam), Marshal.GetFunctionPointerForDelegate(_onServerListReceivedFromSteam));
			}

			return _serverList;
		}

		private int StrLen(char[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				if (buffer[i] == '\0') return i;
			}

			return buffer.Length;
		}

		private String CharArrayToString(char[] buffer)
		{
			if (buffer == null) return string.Empty;

			return new String(buffer, 0, StrLen(buffer));
		}

        public delegate void OnServerReceivedHandler(Matchmaking sender, Servers serverList, Server server);
        public event OnServerReceivedHandler ServerReceived;

		private void OnServerReceived(HServerListRequest request, ref gameserveritem_t callbackData)
		{
			if (request != _serverListRequest)
				return;

			UInt32 ip = callbackData.m_NetAdr.m_unIP;
			IPAddress ipAddress = new IPAddress(new byte[] { (byte)(ip >> 24), (byte)(ip >> 16), (byte)(ip >> 8), (byte)ip });

			Server server = new Server(callbackData.m_nServerVersion, ipAddress, callbackData.m_NetAdr.m_usConnectionPort, callbackData.m_NetAdr.m_usQueryPort, callbackData.m_nPing,
				CharArrayToString(callbackData.m_szServerName), CharArrayToString(callbackData.m_szMap), CharArrayToString(callbackData.m_szGameDescription), callbackData.m_bSecure != 0,
				callbackData.m_bPassword != 0, callbackData.m_nPlayers, callbackData.m_nMaxPlayers, callbackData.m_nBotPlayers, CharArrayToString(callbackData.m_szGameTags),
				CharArrayToString(callbackData.m_szGameDir), callbackData.m_nAppID);

			_serverList.Add(server);

            if (ServerReceived != null)
			{
                ServerReceived(this, _serverList, server);
			}
		}

        public delegate void OnServerListReceivedHandler(Matchmaking sender, Servers serverList);
        public event OnServerListReceivedHandler ServerListReceived;

		private void OnServerListComplete(HServerListRequest request)
		{
			if (request != _serverListRequest)
				return;

			_serverListRequest = HServerListRequest_Invalid;

            if (ServerListReceived != null)
			{
                ServerListReceived(this, _serverList);
			}
		}

		public void CancelCurrentServerListRequest()
		{
			if (_serverListRequest == HServerListRequest_Invalid)
				return;

			SteamUnityAPI_SteamMatchmakingServers_ReleaseRequest(_matchmaking, _serverListRequest);

			_serverListRequest = HServerListRequest_Invalid;
		}
	}
}

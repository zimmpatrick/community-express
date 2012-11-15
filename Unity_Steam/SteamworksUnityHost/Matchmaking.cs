// Copyright (c) 2011-2012, Zimmdot, LLC
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

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct LobbyCreated_t
	{
		public EResult m_eResult;
		public UInt64 m_ulSteamIDLobby;		// chat room, zero if failed
	}

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct LobbyMatchList_t
	{
		public UInt32 m_nLobbiesMatching;		// Number of lobbies that matched search criteria and we have SteamIDs for
	}

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct LobbyEnter_t
	{
		public UInt64 m_ulSteamIDLobby;			// SteamID of the Lobby you have entered
		public UInt32 m_rgfChatPermissions;		// Permissions of the current user
		public Byte m_bLocked;					// If true, then only invited users may join
		public UInt32 m_EChatRoomEnterResponse;	// EChatRoomEnterResponse
	}
	
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

	delegate void OnMatchmakingLobbyCreatedBySteam(ref LobbyCreated_t callbackData);
	public delegate void OnLobbyCreated(Lobby lobby);
	delegate void OnMatchmakingLobbyListReceivedFromSteam(ref LobbyMatchList_t callbackData);
	public delegate void OnLobbyListReceived(Lobbies lobbies);
	delegate void OnMatchmakingLobbyJoinedFromSteam(ref LobbyEnter_t callbackData);
	public delegate void OnLobbyJoined(Lobby lobby, EChatRoomEnterResponse chatRoomEnterResponse);

	delegate void OnMatchmakingServerReceivededFromSteam(HServerListRequest request, ref gameserveritem_t callbackData);
	delegate void OnMatchmakingServerListReceivededFromSteam(HServerListRequest request);
	public delegate void OnServerReceived(Servers serverList, Server server);
	public delegate void OnServerListReceived(Servers serverList);

	public class Matchmaking
	{
		[DllImport("CommunityExpressSW.dll")]
		private static extern IntPtr SteamUnityAPI_SteamMatchmaking();
		[DllImport("CommunityExpressSW.dll")]
		private static extern IntPtr SteamUnityAPI_SteamMatchmakingServers();
		[DllImport("CommunityExpressSW.dll")]
		private static extern UInt32 SteamUnityAPI_SteamUtils_GetAppID();
		[DllImport("CommunityExpressSW.dll")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamMatchmaking_CreateLobby(IntPtr matchmaking, ELobbyType lobbyType, Int32 maxMembers);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListStringFilter(IntPtr matchmaking, [MarshalAs(UnmanagedType.LPStr)] String key, [MarshalAs(UnmanagedType.LPStr)] String value, ELobbyComparison comparisonType);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListNumericalFilter(IntPtr matchmaking, [MarshalAs(UnmanagedType.LPStr)] String key, int value, ELobbyComparison comparisonType);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListNearValueFilter(IntPtr matchmaking, [MarshalAs(UnmanagedType.LPStr)] String key, int value);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable(IntPtr matchmaking, int slotsAvailable);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListDistanceFilter(IntPtr matchmaking, ELobbyDistanceFilter lobbyDistanceFilter);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListResultCountFilter(IntPtr matchmaking, int maxResults);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter(IntPtr matchmaking, UInt64 steamIDLobby);
		[DllImport("CommunityExpressSW.dll")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamMatchmaking_RequestLobbyList(IntPtr matchmaking);
		[DllImport("CommunityExpressSW.dll")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamMatchmaking_JoinLobby(IntPtr matchmaking, UInt64 steamIDLobby);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_LeaveLobby(IntPtr matchmaking, UInt64 steamIDLobby);
		[DllImport("CommunityExpressSW.dll")]
		private static extern UInt64 SteamUnityAPI_SteamMatchmaking_GetLobbyByIndex(IntPtr matchmaking, Int32 lobbyIndex);
		[DllImport("CommunityExpressSW.dll")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestInternetServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("CommunityExpressSW.dll")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestLANServerList(IntPtr matchmakingServers, AppId_t appId,
			IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("CommunityExpressSW.dll")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestSpectatorServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("CommunityExpressSW.dll")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestHistoryServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("CommunityExpressSW.dll")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestFavoriteServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("CommunityExpressSW.dll")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestFriendServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamMatchmakingServers_ReleaseRequest(IntPtr matchmakingServers, HServerListRequest request);

		public const HServerListRequest HServerListRequest_Invalid = 0x0;

		private IntPtr _matchmaking;
		private Lobbies _lobbyList = new Lobbies();
		private SteamAPICall_t _lobbyListRequest = 0;
		private OnLobbyCreated _onLobbyCreated;
		private OnLobbyListReceived _onLobbyListReceived;
		private Lobby _lobbyJoined = null;
		private OnLobbyJoined _onLobbyJoined;

		private IntPtr _matchmakingServers;
		private HServerListRequest _serverListRequest = HServerListRequest_Invalid;
		private Servers _serverList;
		private Dictionary<String, String> _serverFilters;
		private OnServerReceived _onServerReceived = null;
		private OnServerListReceived _onServerListReceived = null;
		private OnMatchmakingServerReceivededFromSteam _onServerReceivedFromSteam = null;
		private OnMatchmakingServerListReceivededFromSteam _onServerListReceivedFromSteam = null;

		internal Matchmaking()
		{
			_matchmaking = SteamUnityAPI_SteamMatchmaking();
			_matchmakingServers = SteamUnityAPI_SteamMatchmakingServers();
		}

		public void CreateLobby(ELobbyType lobbyType, Int32 maxMembers, OnLobbyCreated onLobbyCreated)
		{
			if (CommunityExpress.Instance.IsGameServerInitialized)
			{
				System.Diagnostics.Debug.WriteLine("Unable to create a Lobby after a Game Server has been initialized.");
				throw new Exception("A Lobby cannot be created after a Steam Game Server has been initialized.");
			}

			_onLobbyCreated = onLobbyCreated;

			CommunityExpress.Instance.AddCreateLobbyCallback(SteamUnityAPI_SteamMatchmaking_CreateLobby(_matchmaking, lobbyType, maxMembers), OnLobbyCreatedCallback);
		}

		private void OnLobbyCreatedCallback(ref LobbyCreated_t callbackData)
		{
			_onLobbyCreated(new Lobby(null, new SteamID(callbackData.m_ulSteamIDLobby)));
		}

		public void RequestLobbyList(ICollection<LobbyStringFilter> stringFilters, ICollection<LobbyIntFilter> intFilters, Dictionary<String, Int32> nearValueFilters, Int32 requiredSlotsAvailable, ELobbyDistanceFilter lobbyDistance, Int32 maxResults, ICollection<SteamID> compatibleSteamIDs, OnLobbyListReceived onLobbyListReceived)
		{
			if (_lobbyListRequest != 0)
				CancelCurrentLobbyListRequest();

			_onLobbyListReceived = onLobbyListReceived;

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

			CommunityExpress.Instance.AddLobbyListRequestCallback(SteamUnityAPI_SteamMatchmaking_RequestLobbyList(_matchmaking), OnLobbyListReceivedCallback);
		}

		private void OnLobbyListReceivedCallback(ref LobbyMatchList_t callbackData)
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

			_onLobbyListReceived(lobbyList);
		}

		public void CancelCurrentLobbyListRequest()
		{
			CommunityExpress.Instance.RemoveLobbyListRequestCallback(_lobbyListRequest, OnLobbyListReceivedCallback);
			_lobbyListRequest = 0;
		}

		public void JoinLobby(SteamID steamIDLobby, OnLobbyJoined onLobbyJoined)
		{
			if (_lobbyJoined != null)
			{
				LeaveLobby();
			}

			foreach (Lobby l in _lobbyList)
			{
				if (l.SteamID == steamIDLobby)
				{
					_lobbyJoined = l;
					break;
				}
			}

			_onLobbyJoined = onLobbyJoined;

			CommunityExpress.Instance.AddLobbyJoinedCallback(SteamUnityAPI_SteamMatchmaking_JoinLobby(_matchmaking, steamIDLobby.ToUInt64()), OnLobbyJoinedCallback);
		}

		public void JoinLobby(Lobby lobby, OnLobbyJoined onLobbyJoined)
		{
			if (_lobbyJoined != null)
			{
				LeaveLobby();
			}

			_lobbyJoined = lobby;
			_onLobbyJoined = onLobbyJoined;

			CommunityExpress.Instance.AddLobbyJoinedCallback(SteamUnityAPI_SteamMatchmaking_JoinLobby(_matchmaking, lobby.SteamID.ToUInt64()), OnLobbyJoinedCallback);
		}

		private void OnLobbyJoinedCallback(ref LobbyEnter_t callbackData)
		{
			if (_lobbyJoined == null)
			{
				_lobbyJoined = new Lobby(null, new SteamID(callbackData.m_ulSteamIDLobby));
				_lobbyList.Add(_lobbyJoined);
			}

			_lobbyJoined.IsLocked = callbackData.m_bLocked != 0;
			_lobbyJoined.ChatPermissions = callbackData.m_rgfChatPermissions;

			_onLobbyJoined(_lobbyJoined, (EChatRoomEnterResponse)callbackData.m_EChatRoomEnterResponse);
		}

		public void LeaveLobby()
		{
			if (_lobbyJoined != null)
			{
				SteamUnityAPI_SteamMatchmaking_LeaveLobby(_matchmaking, _lobbyJoined.SteamID.ToUInt64());
				_lobbyJoined = null;
			}
		}

		private void PrepServerListRequest(Dictionary<String, String> filters, OnServerReceived onServerReceived, OnServerListReceived onServerListReceived,
			out String[] keys, out String[] values)
		{
			if (_serverListRequest != HServerListRequest_Invalid)
				CancelCurrentServerListRequest();

			_serverList = new Servers();
			_serverFilters = filters;

			_onServerReceived = onServerReceived;
			_onServerListReceived = onServerListReceived;

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

		public Servers RequestInternetServerList(Dictionary<String, String> filters, OnServerReceived onServerReceived, OnServerListReceived onServerListReceived)
		{
			String[] keys, values;

			PrepServerListRequest(filters, onServerReceived, onServerListReceived, out keys, out values);

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

		public Servers RequestLANServerList(OnServerReceived onServerReceived, OnServerListReceived onServerListReceived)
		{
			String[] keys, values;

			PrepServerListRequest(null, onServerReceived, onServerListReceived, out keys, out values);

			_serverListRequest = SteamUnityAPI_SteamMatchmakingServers_RequestLANServerList(_matchmakingServers, (AppId_t)SteamUnityAPI_SteamUtils_GetAppID(),
				Marshal.GetFunctionPointerForDelegate(_onServerReceivedFromSteam), Marshal.GetFunctionPointerForDelegate(_onServerListReceivedFromSteam));

			return _serverList;
		}

		public Servers RequestSpecatorServerList(Dictionary<String, String> filters, OnServerReceived onServerReceived, OnServerListReceived onServerListReceived)
		{
			String[] keys, values;

			PrepServerListRequest(filters, onServerReceived, onServerListReceived, out keys, out values);

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

		public Servers RequestHistoryServerList(Dictionary<String, String> filters, OnServerReceived onServerReceived, OnServerListReceived onServerListReceived)
		{
			String[] keys, values;

			PrepServerListRequest(filters, onServerReceived, onServerListReceived, out keys, out values);

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

		public Servers RequestFavoriteServerList(Dictionary<String, String> filters, OnServerReceived onServerReceived, OnServerListReceived onServerListReceived)
		{
			String[] keys, values;

			PrepServerListRequest(filters, onServerReceived, onServerListReceived, out keys, out values);

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

		public Servers RequestFriendServerList(Dictionary<String, String> filters, OnServerReceived onServerReceived, OnServerListReceived onServerListReceived)
		{
			String[] keys, values;

			PrepServerListRequest(filters, onServerReceived, onServerListReceived, out keys, out values);

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

			if (_onServerReceived != null)
			{
				_onServerReceived(_serverList, server);
			}
		}

		private void OnServerListComplete(HServerListRequest request)
		{
			if (request != _serverListRequest)
				return;

			_serverListRequest = HServerListRequest_Invalid;

			if (_onServerListReceived != null)
			{
				_onServerListReceived(_serverList);
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

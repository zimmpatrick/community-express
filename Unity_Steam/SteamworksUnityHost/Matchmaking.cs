using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;

namespace SteamworksUnityHost
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
		//    useful if you want a user in two lobbies, for example matching groups together
		//	  a user can be in only one regular lobby, and up to two invisible lobbies
	};

	// lobby search filter tools
	public enum ELobbyComparison
	{
		k_ELobbyComparisonEqualToOrLessThan = -2,
		k_ELobbyComparisonLessThan = -1,
		k_ELobbyComparisonEqual = 0,
		k_ELobbyComparisonGreaterThan = 1,
		k_ELobbyComparisonEqualToOrGreaterThan = 2,
		k_ELobbyComparisonNotEqual = 3,
	};

	// lobby search distance. Lobby results are sorted from closest to farthest.
	public enum ELobbyDistanceFilter
	{
		k_ELobbyDistanceFilterClose,		// only lobbies in the same immediate region will be returned
		k_ELobbyDistanceFilterDefault,		// only lobbies in the same region or near by regions
		k_ELobbyDistanceFilterFar,			// for games that don't have many latency requirements, will return lobbies about half-way around the globe
		k_ELobbyDistanceFilterWorldwide,	// no filtering, will match lobbies as far as India to NY (not recommended, expect multiple seconds of latency between the clients)
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct LobbyCreated_t
	{
		public EResult m_eResult;
		public UInt64 m_ulSteamIDLobby;		// chat room, zero if failed
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct LobbyMatchList_t
	{
		public UInt32 m_nLobbiesMatching;		// Number of lobbies that matched search criteria and we have SteamIDs for
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct servernetadr_t
	{
		public UInt16 m_usConnectionPort;	// (in HOST byte order)
		public UInt16 m_usQueryPort;
		public UInt32 m_unIP;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct gameserveritem_t
	{
		public servernetadr_t m_NetAdr;				// IP/Query Port/Connection Port for this server
		public Int32 m_nPing;						// current ping time in milliseconds
		public Boolean m_bHadSuccessfulResponse;	// server has responded successfully in the past
		public Boolean m_bDoNotRefresh;				// server is marked as not responding and should no longer be refreshed
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
		public Boolean m_bPassword;					// true if this server needs a password to join
		public Boolean m_bSecure;					// Is this server protected by VAC
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

	delegate void OnMatchmakingServerReceivededFromSteam(HServerListRequest request, ref gameserveritem_t callbackData);
	delegate void OnMatchmakingServerListReceivededFromSteam(HServerListRequest request);
	public delegate void OnServerReceived(Servers serverList, Server server);
	public delegate void OnServerListReceived(Servers serverList);

	public class Matchmaking
	{
		[DllImport("SteamworksUnity.dll")]
		private static extern IntPtr SteamUnityAPI_SteamMatchmaking();
		[DllImport("SteamworksUnity.dll")]
		private static extern IntPtr SteamUnityAPI_SteamMatchmakingServers();
		[DllImport("SteamworksUnity.dll")]
		private static extern UInt64 SteamUnityAPI_SteamUtils_GetAppID();
		[DllImport("SteamworksUnity.dll")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamMatchmaking_CreateLobby(IntPtr matchmaking, ELobbyType lobbyType, Int32 maxMembers);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListStringFilter(IntPtr matchmaking, [MarshalAs(UnmanagedType.LPStr)] String key, [MarshalAs(UnmanagedType.LPStr)] String value, ELobbyComparison comparisonType);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListNumericalFilter(IntPtr matchmaking, [MarshalAs(UnmanagedType.LPStr)] String key, int value, ELobbyComparison comparisonType);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListNearValueFilter(IntPtr matchmaking, [MarshalAs(UnmanagedType.LPStr)] String key, int value);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable(IntPtr matchmaking, int slotsAvailable);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListDistanceFilter(IntPtr matchmaking, ELobbyDistanceFilter lobbyDistanceFilter);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListResultCountFilter(IntPtr matchmaking, int maxResults);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter(IntPtr matchmaking, UInt64 steamIDLobby);
		[DllImport("SteamworksUnity.dll")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamMatchmaking_RequestLobbyList(IntPtr matchmaking);
		[DllImport("SteamworksUnity.dll")]
		private static extern UInt64 SteamUnityAPI_SteamMatchmaking_GetLobbyByIndex(IntPtr matchmaking, Int32 lobbyIndex);
		[DllImport("SteamworksUnity.dll")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestInternetServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("SteamworksUnity.dll")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestLANServerList(IntPtr matchmakingServers, AppId_t appId,
			IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("SteamworksUnity.dll")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestSpectatorServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("SteamworksUnity.dll")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestHistoryServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("SteamworksUnity.dll")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestFavoriteServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("SteamworksUnity.dll")]
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestFriendServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamMatchmakingServers_ReleaseRequest(IntPtr matchmakingServers, HServerListRequest request);

		public const HServerListRequest HServerListRequest_Invalid = 0x0;

		private IntPtr _matchmaking;
		private Lobbies _lobbyList;
		private SteamAPICall_t _lobbyListRequest = 0;
		private OnLobbyCreated _onLobbyCreated;
		private OnLobbyListReceived _onLobbyListReceived;

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
			_onLobbyCreated = onLobbyCreated;

			SteamUnity.Instance.AddCreateLobbyCallback(SteamUnityAPI_SteamMatchmaking_CreateLobby(_matchmaking, lobbyType, maxMembers), OnLobbyCreatedCallback);
		}

		private void OnLobbyCreatedCallback(ref LobbyCreated_t callbackData)
		{
			_onLobbyCreated(new Lobby(null, new SteamID(callbackData.m_ulSteamIDLobby)));
		}

		public Lobbies RequestLobbyList(ICollection<LobbyStringFilter> stringFilters, ICollection<LobbyIntFilter> intFilters, Dictionary<String, Int32> nearValueFilters, Int32 requiredSlotsAvailable, ELobbyDistanceFilter lobbyDistance, Int32 maxResults, ICollection<SteamID> compatibleSteamIDs, OnLobbyListReceived onLobbyListReceived)
		{
			if (_lobbyListRequest != 0)
				CancelCurrentLobbyListRequest();

			_lobbyList = new Lobbies();

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

			SteamUnity.Instance.AddLobbyListRequestCallback(SteamUnityAPI_SteamMatchmaking_RequestLobbyList(_matchmaking), OnLobbyListReceivedCallback);

			return _lobbyList;
		}

		private void OnLobbyListReceivedCallback(ref LobbyMatchList_t CallbackData)
		{
			for (int i = 0; i < CallbackData.m_nLobbiesMatching; i++)
			{
				_lobbyList.Add(new Lobby(_lobbyList, new SteamID(SteamUnityAPI_SteamMatchmaking_GetLobbyByIndex(_matchmaking, i))));
			}

			_onLobbyListReceived(_lobbyList);
		}

		public void CancelCurrentLobbyListRequest()
		{
			SteamUnity.Instance.RemoveLobbyListRequestCallback(_lobbyListRequest, OnLobbyListReceivedCallback);
			_lobbyListRequest = 0;
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
				keys = new String[] { };
				values = new String[] { };

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

		private void OnServerReceived(HServerListRequest request, ref gameserveritem_t callbackData)
		{
			if (request != _serverListRequest)
				return;

			UInt32 ip = callbackData.m_NetAdr.m_unIP;
			IPAddress ipAddress = new IPAddress(new byte[] { (byte)(ip >> 24), (byte)(ip >> 16), (byte)(ip >> 8), (byte)ip });

			Server server = new Server(callbackData.m_nServerVersion, ipAddress, callbackData.m_NetAdr.m_usConnectionPort, callbackData.m_NetAdr.m_usQueryPort, callbackData.m_nPing,
				new String(callbackData.m_szServerName), new String(callbackData.m_szMap), new String(callbackData.m_szGameDescription), callbackData.m_bSecure,
				callbackData.m_bPassword, callbackData.m_nPlayers, callbackData.m_nMaxPlayers, callbackData.m_nBotPlayers);

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

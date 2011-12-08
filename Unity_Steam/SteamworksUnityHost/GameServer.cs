using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;

namespace SteamworksUnityHost
{
	// client has been approved to connect to this game server
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct GSClientApprove_t
	{
		public UInt64 m_SteamID;
	};

	// client has been denied to connection to this game server
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct GSClientDeny_t
	{
		public UInt64 m_SteamID;
		public EDenyReason m_eDenyReason;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public char[] m_rgchOptionalText;
	};

	// request the game server should kick the user
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct GSClientKick_t
	{
		public UInt64 m_SteamID;
		public EDenyReason m_eDenyReason;
	};

	// received when the game server requests to be displayed as secure (VAC protected)
	// m_bSecure is true if the game server should display itself as secure to users, false otherwise
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct GSPolicyResponse_t
	{
		public Byte m_bSecure;
	};

	delegate void OnGameServerClientApprovedBySteam(ref GSClientApprove_t callbackData);
	public delegate void OnGameServerClientApproved(SteamID approvedPlayer);

    delegate void OnGameServerClientDeniedBySteam(ref GSClientDeny_t callbackData);
	public delegate void OnGameServerClientDenied(SteamID deniedPlayer, EDenyReason denyReason, String optionalText);

    delegate void OnGameServerClientKickFromSteam(ref GSClientKick_t callbackData);
	public delegate void OnGameServerClientKick(SteamID playerToKick, EDenyReason denyReason);

    delegate void OnGameServerPolicyResponseFromSteam(ref GSPolicyResponse_t callbackData);
	public delegate void OnGameServerPolicyResponse();

	public class GameServer
	{
		[DllImport("SteamworksUnity.dll")]
		private static extern IntPtr SteamUnityAPI_SteamGameServer();
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamGameServer_Init(UInt32 ip, UInt16 masterServerPort, UInt16 port, UInt16 queryPort,
			EServerMode serverMode, [MarshalAs(UnmanagedType.LPStr)] String gameVersion);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamGameServer_SetCallbacks(IntPtr OnGameServerClientApproved, IntPtr OnGameServerClientDeny,
			IntPtr OnGameServerClientKick, IntPtr OnGameServerPolicyResponse);
		[DllImport("SteamworksUnity.dll")]
		private static extern UInt64 SteamUnityAPI_SteamGameServer_GetSteamID(IntPtr gameserver);
		[DllImport("SteamworksUnity.dll")]
		private static extern UInt32 SteamUnityAPI_SteamGameServer_GetPublicIP(IntPtr gameserver);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamGameServer_SetBasicServerData(IntPtr gameserver, Boolean isDedicated,
			[MarshalAs(UnmanagedType.LPStr)] String gameName, [MarshalAs(UnmanagedType.LPStr)] String gameDescription);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamGameServer_LogOnAnonymous(IntPtr gameserver);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamGameServer_UpdateServerStatus(IntPtr gameserver, Int32 maxClients, Int32 bots,
			[MarshalAs(UnmanagedType.LPStr)] String serverName, [MarshalAs(UnmanagedType.LPStr)] String spectatorServerName,
			[MarshalAs(UnmanagedType.LPStr)] String mapName, Boolean passworded);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamGameServer_SendUserConnectAndAuthenticate(IntPtr gameserver, UInt32 ipClient,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] Byte[] authTicket, UInt32 authTicketSize, out UInt64 steamIDClient);
		[DllImport("SteamworksUnity.dll")]
		private static extern UInt64 SteamUnityAPI_SteamGameServer_CreateUnauthenticatedUserConnection(IntPtr gameserver);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamGameServer_SendUserDisconnect(IntPtr gameserver, UInt64 steamIDClient);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamGameServer_UpdateUserData(IntPtr gameserver, UInt64 steamID,
			[MarshalAs(UnmanagedType.LPStr)] String name, UInt32 score);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamGameServer_Shutdown();
		
		private IntPtr _gameserver;

		private Boolean _isInitialized = false;
		private Boolean _vacSecured = false;
		private Boolean _isDedicated = false;
		private String _serverName;
		private String _spectatorServerName;
		private String _regionName;
		private String _gameName;
		private String _gameDescription;
		private UInt16 _maxClients = 0;
		private Boolean _isPassworded = false;
		private String _mapName;
		private List<SteamID> _playersPendingAuth = new List<SteamID>();
		private List<SteamID> _playersConnected = new List<SteamID>();
		private List<SteamID> _botsConnected = new List<SteamID>();

		private OnGameServerClientApprovedBySteam _privateOnGameServerClientApproved = null;
		private OnGameServerClientApproved _onGameServerClientApproved;

		private OnGameServerClientDeniedBySteam _privateOnGameServerClientDenied = null;
		private OnGameServerClientDenied _onGameServerClientDenied;

		private OnGameServerClientKickFromSteam _privateOnGameServerClientKick = null;
		private OnGameServerClientKick _onGameServerClientKick;

		private OnGameServerPolicyResponseFromSteam _privateOnGameServerPolicyResponse = null;

		internal GameServer()
		{
			_gameserver = SteamUnityAPI_SteamGameServer();
		}

		~GameServer()
		{
			Shutdown();
		}

		public Boolean Init(Boolean isDedicated, IPAddress ip, UInt16 port, UInt16 queryPort, UInt16 masterServerPort, EServerMode serverMode,
			String serverName, String spectatorServerName, String regionName, String gameName, String gameDescription, String gameVersion, String mapName,
			UInt16 maxClients, Boolean isPassworded, OnGameServerClientApproved onGameServerClientApproved, OnGameServerClientDenied onGameServerClientDenied,
			OnGameServerClientKick onGameServerClientKick)
		{
			Byte[] serverIPBytes = ip.GetAddressBytes();
			UInt32 serverIP = (UInt32)serverIPBytes[0] << 24 | (UInt32)serverIPBytes[1] << 16 | (UInt32)serverIPBytes[2] << 8 | (UInt32)serverIPBytes[3];

			_playersPendingAuth.Clear();
			_playersConnected.Clear();
			_botsConnected.Clear();

			_onGameServerClientApproved = onGameServerClientApproved;
			_onGameServerClientDenied = onGameServerClientDenied;
			_onGameServerClientKick = onGameServerClientKick;

			if (_privateOnGameServerClientApproved == null)
			{
				_privateOnGameServerClientApproved = new OnGameServerClientApprovedBySteam(OnGameServerClientApprovedCallback);
				_privateOnGameServerClientDenied = new OnGameServerClientDeniedBySteam(OnGameServerClientDeniedCallback);
				_privateOnGameServerClientKick = new OnGameServerClientKickFromSteam(OnGameServerClientKickCallback);
				_privateOnGameServerPolicyResponse = new OnGameServerPolicyResponseFromSteam(OnGameServerPolicyResponseCallback);
			}

			SteamUnityAPI_SteamGameServer_SetCallbacks(Marshal.GetFunctionPointerForDelegate(_privateOnGameServerClientApproved),
				Marshal.GetFunctionPointerForDelegate(_privateOnGameServerClientDenied),
				Marshal.GetFunctionPointerForDelegate(_privateOnGameServerClientKick),
				Marshal.GetFunctionPointerForDelegate(_privateOnGameServerPolicyResponse));

			if (SteamUnityAPI_SteamGameServer_Init(serverIP, masterServerPort, port, queryPort, serverMode, gameVersion))
			{
				_isInitialized = true;
				_gameserver = SteamUnityAPI_SteamGameServer();
				_isDedicated = isDedicated;
				_serverName = serverName;
				_spectatorServerName = spectatorServerName;
				_mapName = mapName;
				_regionName = regionName;
				_gameName = gameName;
				_gameDescription = gameDescription;
				_maxClients = maxClients;
				_isPassworded = isPassworded;

				SendBasicServerStatus();
				SteamUnityAPI_SteamGameServer_LogOnAnonymous(_gameserver);
				SendUpdatedServerStatus();

				return true;
			}

			return false;
		}

		public Boolean ClientConnected(IPAddress ipClient, Byte[] authTicket, out SteamID steamIDClient)
		{
			Byte[] clientIPBytes = ipClient.GetAddressBytes();
			UInt32 clientIP = (UInt32)clientIPBytes[0] << 24 | (UInt32)clientIPBytes[1] << 16 | (UInt32)clientIPBytes[2] << 8 | (UInt32)clientIPBytes[3];
			UInt64 clientSteamID;

			if (SteamUnityAPI_SteamGameServer_SendUserConnectAndAuthenticate(_gameserver, clientIP, authTicket, (UInt32)authTicket.Length, out clientSteamID))
			{
				steamIDClient = new SteamID(clientSteamID);
				_playersPendingAuth.Add(steamIDClient);

				SendUpdatedServerStatus();

				return true;
			}

			_onGameServerClientDenied(null, EDenyReason.EDenyGeneric, "SteamGameServer::SendUserConnectAndAuthenticate failed");
			steamIDClient = null;
			return false;
		}

		public SteamID AddBot()
		{
			SteamID ret = new SteamID(SteamUnityAPI_SteamGameServer_CreateUnauthenticatedUserConnection(_gameserver));

			_botsConnected.Add(ret);

			return ret;
		}

		public Boolean UpdateUserDetails(SteamID steamID, String displayableName, UInt32 score)
		{
			return SteamUnityAPI_SteamGameServer_UpdateUserData(_gameserver, steamID.ToUInt64(), displayableName, score);
		}

		public void ClientDisconnected(SteamID steamIDClient)
		{
			Boolean found = false;

			foreach (SteamID s in _playersPendingAuth)
			{
				if (s == steamIDClient)
				{
					SteamUnityAPI_SteamGameServer_SendUserDisconnect(_gameserver, steamIDClient.ToUInt64());
					found = true;
					break;
				}
			}

			if (!found)
			{
				foreach (SteamID s in _playersConnected)
				{
					if (s == steamIDClient)
					{
						SteamUnityAPI_SteamGameServer_SendUserDisconnect(_gameserver, steamIDClient.ToUInt64());
						found = true;
						break;
					}
				}
			}

			if (!found)
			{
				foreach (SteamID s in _botsConnected)
				{
					if (s == steamIDClient)
					{
						SteamUnityAPI_SteamGameServer_SendUserDisconnect(_gameserver, steamIDClient.ToUInt64());
						found = true;
						break;
					}
				}
			}

			_playersPendingAuth.Remove(steamIDClient);
			_playersConnected.Remove(steamIDClient);
			_botsConnected.Remove(steamIDClient);
		}

		private void OnGameServerClientApprovedCallback(ref GSClientApprove_t callbackData)
		{
			foreach (SteamID s in _playersPendingAuth)
			{
				if (s == callbackData.m_SteamID)
				{
					_onGameServerClientApproved(s);
					_playersConnected.Add(s);
					_playersPendingAuth.Remove(s);
					break;
				}
			}
		}

        private void OnGameServerClientDeniedCallback(ref GSClientDeny_t callbackData)
		{
			foreach (SteamID s in _playersPendingAuth)
			{
				if (s == callbackData.m_SteamID)
				{
					_onGameServerClientDenied(s, callbackData.m_eDenyReason, new String(callbackData.m_rgchOptionalText));
					_playersPendingAuth.Remove(s);
					_playersConnected.Remove(s);
					break;
				}
			}
		}

        private void OnGameServerClientKickCallback(ref GSClientKick_t callbackData)
		{
			Boolean found = false;

			foreach (SteamID s in _playersPendingAuth)
			{
				if (s == callbackData.m_SteamID)
				{
					_onGameServerClientKick(s, callbackData.m_eDenyReason);
					_playersPendingAuth.Remove(s);
					found = true;
					break;
				}
			}

			foreach (SteamID s in _playersConnected)
			{
				if (s == callbackData.m_SteamID)
				{
					if (!found)
					{
						_onGameServerClientKick(s, callbackData.m_eDenyReason);
					}

					_playersConnected.Remove(s);
					break;
				}
			}
		}

		private void OnGameServerPolicyResponseCallback(ref GSPolicyResponse_t callbackData)
		{
			_vacSecured = callbackData.m_bSecure != 0;
		}

		private void SendBasicServerStatus()
		{
			SteamUnityAPI_SteamGameServer_SetBasicServerData(_gameserver, _isDedicated, _gameName, _gameDescription);
		}

		private void SendUpdatedServerStatus()
		{
			SteamUnityAPI_SteamGameServer_UpdateServerStatus(_gameserver, _maxClients, _botsConnected.Count, _serverName, _spectatorServerName, _mapName, _isPassworded);
		}

		public void RequestUserStats(SteamID steamID, OnUserStatsReceived onUserStatsReceived, IEnumerable<String> requestedStats)
		{
			Stats stats = new Stats();
			stats.Init(steamID, true);
			stats.RequestCurrentStats(onUserStatsReceived, requestedStats);
		}

		public void RequestUserAchievements(SteamID steamID, OnUserStatsReceived onUserStatsReceived, IEnumerable<String> requestedAchievements)
		{
			Achievements achievements = new Achievements();
			achievements.Init(steamID, true);
			achievements.RequestCurrentAchievements(onUserStatsReceived, requestedAchievements);
		}

		public void Shutdown()
		{
			SteamUnityAPI_SteamGameServer_Shutdown();
		}

		public Boolean IsInitialized
		{
			get { return _isInitialized; }
		}

		public IPAddress PublicIP
		{
			get
			{
				UInt32 ip = SteamUnityAPI_SteamGameServer_GetPublicIP(_gameserver);
				return new IPAddress(new byte[] { (byte)(ip >> 24), (byte)(ip >> 16), (byte)(ip >> 8), (byte)ip });
			}
		}

		public Boolean IsVacSecured
		{
			get { return _vacSecured; }
		}

		public SteamID SteamID
		{
			get { return new SteamID(SteamUnityAPI_SteamGameServer_GetSteamID(_gameserver)); }
		}

		public Boolean IsDedicated
		{
			get { return _isDedicated; }
			set { _isDedicated = value; SendBasicServerStatus(); }
		}

		public String ServerName
		{
			get { return _serverName; }
			set { _serverName = value; SendUpdatedServerStatus(); }
		}

		public String SpectatorServerName
		{
			get { return _spectatorServerName; }
			set { _spectatorServerName = value; SendUpdatedServerStatus(); }
		}

		public String RegionName
		{
			get { return _regionName; }
			set { _regionName = value; SendBasicServerStatus(); }
		}

		public String GameName
		{
			get { return _gameName; }
			set { _gameName = value; SendBasicServerStatus(); }
		}

		public String GameDescription
		{
			get { return _gameDescription; }
			set { _gameDescription = value; SendBasicServerStatus(); }
		}

		public UInt16 MaxClients
		{
			get { return _maxClients; }
			set { _maxClients = value; SendBasicServerStatus(); }
		}

		public Boolean IsPassworded
		{
			get { return _isPassworded; }
			set { _isPassworded = value; SendBasicServerStatus(); }
		}

		public String MapName
		{
			get { return _mapName; }
			set { _mapName = value; SendUpdatedServerStatus(); }
		}
	}
}

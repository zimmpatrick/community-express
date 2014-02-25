// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;

namespace CommunityExpressNS
{
    using AppId_t = UInt32;
    using SteamAPICall_t = UInt64;
    using SteamID_t = UInt64;

	// client has been approved to connect to this game server
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct GSClientApprove_t
	{
        internal const int k_iCallback = Events.k_iSteamGameServerCallbacks + 1;
		public UInt64 m_SteamID;
	};

	// client has been denied to connection to this game server
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct GSClientDeny_t
	{
        internal const int k_iCallback = Events.k_iSteamGameServerCallbacks + 2;
		public UInt64 m_SteamID;
		public EDenyReason m_eDenyReason;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public char[] m_rgchOptionalText;
	};

	// request the game server should kick the user
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct GSClientKick_t
	{
        internal const int k_iCallback = Events.k_iSteamGameServerCallbacks + 3;
		public UInt64 m_SteamID;
		public EDenyReason m_eDenyReason;
	};
    
    // client achievement info
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct GSClientAchievementStatus_t
    {
        internal const int k_iCallback = Events.k_iSteamGameServerCallbacks + 6;
	    internal UInt64 m_SteamID;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	    internal char[] m_pchAchievement;
	    internal bool m_bUnlocked;
    };

	// received when the game server requests to be displayed as secure (VAC protected)
	// m_bSecure is true if the game server should display itself as secure to users, false otherwise
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct GSPolicyResponse_t
	{
        internal const int k_iCallback = Events.k_iSteamGameServerCallbacks + 15;
		public Byte m_bSecure;
	};
    
    // GS gameplay stats info
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct GSGameplayStats_t
    {
	    internal const int k_iCallback = Events.k_iSteamGameServerCallbacks + 7;
        internal EResult m_eResult;					// Result of the call
        internal Int32 m_nRank;					// Overall rank of the server (0-based)
        internal UInt32 m_unTotalConnects;			// Total number of clients who have ever connected to the server
        internal UInt32 m_unTotalMinutesPlayed;		// Total number of minutes ever played on the server
    };

    
    // send as a reply to RequestUserGroupStatus()
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct GSClientGroupStatus_t
    {
	    internal const int k_iCallback = Events.k_iSteamGameServerCallbacks + 8;
        internal SteamID_t m_SteamIDUser;
        internal SteamID_t m_SteamIDGroup;
        internal bool m_bMember;
        internal bool m_bOfficer;
    };
    
    // Sent as a reply to GetServerReputation()
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct GSReputation_t
    {
	    internal const int k_iCallback = Events.k_iSteamGameServerCallbacks + 9;
        internal EResult m_eResult;				// Result of the call;
        internal UInt32 m_unReputationScore;	// The reputation score for the game server
        internal bool m_bBanned;				// True if the server is banned from the Steam master servers

	    // The following members are only filled out if m_bBanned is true. They will all 
	    // be set to zero otherwise. Master server bans are by IP so it is possible to be
	    // banned even when the score is good high if there is a bad server on another port.
	    // This information can be used to determine which server is bad.

        internal UInt32 m_unBannedIP;		// The IP of the banned server
        internal UInt16 m_usBannedPort;		// The port of the banned server
        internal UInt64 m_ulBannedGameID;	// The game ID the banned server is serving
        internal UInt32 m_unBanExpires;		// Time the ban expires, expressed in the Unix epoch (seconds since 1/1/1970)
    };
    
    // Sent as a reply to AssociateWithClan()
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct AssociateWithClanResult_t
    {
	    internal const int k_iCallback = Events.k_iSteamGameServerCallbacks + 10;
	    internal EResult	m_eResult;				// Result of the call;
    };
    
    // Sent as a reply to ComputeNewPlayerCompatibility()
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct ComputeNewPlayerCompatibilityResult_t
    {
	    internal const int k_iCallback = Events.k_iSteamGameServerCallbacks + 11;
        internal EResult m_eResult;				// Result of the call;
        internal int m_cPlayersThatDontLikeCandidate;
        internal int m_cPlayersThatCandidateDoesntLike;
        internal int m_cClanPlayersThatDontLikeCandidate;
        internal SteamID_t m_SteamIDCandidate;
    };
    
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct SteamServerConnectFailure_t
    {
        internal const int k_iCallback = Events.k_iSteamUserCallbacks + 2;
	    internal EResult m_eResult;
    };
    
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct SteamServersConnected_t
    {
	    internal const int k_iCallback = Events.k_iSteamUserCallbacks + 1;
    };

	delegate void OnGameServerClientApprovedBySteam(ref GSClientApprove_t callbackData);
    /// <summary>
    /// When server client is approved
    /// </summary>
    /// <param name="approvedPlayer">Approved user</param>
	public delegate void OnGameServerClientApproved(SteamID approvedPlayer);

	delegate void OnGameServerClientDeniedBySteam(ref GSClientDeny_t callbackData);
    /// <summary>
    /// When server client is denied
    /// </summary>
    /// <param name="deniedPlayer">Denied user</param>
    /// <param name="denyReason">Why user was denied</param>
    /// <param name="optionalText">Text for user</param>
	public delegate void OnGameServerClientDenied(SteamID deniedPlayer, EDenyReason denyReason, String optionalText);

	delegate void OnGameServerClientKickFromSteam(ref GSClientKick_t callbackData);
    /// <summary>
    /// When server client is kicked
    /// </summary>
    /// <param name="playerToKick">Kicked user</param>
    /// <param name="denyReason">Why user was kicked</param>
	public delegate void OnGameServerClientKick(SteamID playerToKick, EDenyReason denyReason);

	delegate void OnGameServerPolicyResponseFromSteam(ref GSPolicyResponse_t callbackData);
    /// <summary>
    /// When server policy response is received
    /// </summary>
	public delegate void OnGameServerPolicyResponse();
    /// <summary>
    /// Information about game server
    /// </summary>
	public class GameServer
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamGameServer();
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamGameServer_Init(UInt32 ip, UInt16 masterServerPort, UInt16 port, UInt16 queryPort,
			EServerMode serverMode, [MarshalAs(UnmanagedType.LPStr)] String gameVersion);
		[DllImport("CommunityExpressSW")]
		private static extern UInt64 SteamUnityAPI_SteamGameServer_GetSteamID(IntPtr gameserver);
		[DllImport("CommunityExpressSW")]
		private static extern UInt32 SteamUnityAPI_SteamGameServer_GetPublicIP(IntPtr gameserver);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamGameServer_SetBasicServerData(IntPtr gameserver, Boolean isDedicated,
			[MarshalAs(UnmanagedType.LPStr)] String gameName, [MarshalAs(UnmanagedType.LPStr)] String gameDescription, [MarshalAs(UnmanagedType.LPStr)] String modDir);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamGameServer_LogOnAnonymous(IntPtr gameserver);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamGameServer_UpdateServerStatus(IntPtr gameserver, Int32 maxClients, Int32 bots,
			[MarshalAs(UnmanagedType.LPStr)] String serverName, [MarshalAs(UnmanagedType.LPStr)] String spectatorServerName, UInt16 spectatorPort,
			[MarshalAs(UnmanagedType.LPStr)] String regionName, [MarshalAs(UnmanagedType.LPStr)] String mapName, Boolean passworded);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamGameServer_SendUserConnectAndAuthenticate(IntPtr gameserver, UInt32 ipClient,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] Byte[] authTicket, UInt32 authTicketSize, out UInt64 steamIDClient);
		[DllImport("CommunityExpressSW")]
		private static extern UInt64 SteamUnityAPI_SteamGameServer_CreateUnauthenticatedUserConnection(IntPtr gameserver);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamGameServer_SendUserDisconnect(IntPtr gameserver, UInt64 steamIDClient);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamGameServer_UpdateUserData(IntPtr gameserver, UInt64 steamID,
			[MarshalAs(UnmanagedType.LPStr)] String name, UInt32 score);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamGameServer_SetKeyValues(IntPtr gameserver,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values, Int32 count);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamGameServer_SetGameTags(IntPtr gameserver, [MarshalAs(UnmanagedType.LPStr)] String tags);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamGameServer_SetGameData(IntPtr gameserver, [MarshalAs(UnmanagedType.LPStr)] String data);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamGameServer_Shutdown();
		[DllImport("CommunityExpressSW")]
        private static extern void SteamUnityAPI_SteamGameServer_GetGameplayStats(IntPtr gameserver);
		[DllImport("CommunityExpressSW")]
        private static extern void SteamUnityAPI_SteamGameServer_AssociateWithClan(IntPtr gameserver, UInt64 userID);
		[DllImport("CommunityExpressSW")]
        private static extern void SteamUnityAPI_SteamGameServer_RequestUserGroupStatus(IntPtr gameserver, UInt64 userID, UInt64 groupID);
		[DllImport("CommunityExpressSW")]
        private static extern void SteamUnityAPI_SteamGameServer_ComputeNewPlayerCompatibility(IntPtr gameserver, UInt64 userID);
		
        
        
		private IntPtr _gameServer;
        private CommunityExpress _ce;

		private Boolean _isInitialized = false;
		private Boolean _vacSecured = false;
		private Boolean _isDedicated = false;
		private String _serverName;
		private String _spectatorServerName;
		private UInt16 _spectatorPort;
		private String _regionName;
		private String _gameName;
		private String _gameDescription;
		private UInt16 _maxClients = 0;
		private Boolean _isPassworded = false;
		private String _mapName;
		private Dictionary<String, String> _keyValues;
		private String _gameTags;
		private String _gameData;
		private String _modDir;
		private List<SteamID> _playersPendingAuth = new List<SteamID>();
		private List<SteamID> _playersConnected = new List<SteamID>();
		private List<SteamID> _botsConnected = new List<SteamID>();

		private OnGameServerClientDeniedBySteam _internalOnGameServerClientDenied = null;
		private OnGameServerClientDenied _onGameServerClientDenied;

		private OnGameServerClientKickFromSteam _internalOnGameServerClientKick = null;
		private OnGameServerClientKick _onGameServerClientKick;

		private OnGameServerPolicyResponseFromSteam _internalOnGameServerPolicyResponse = null;

		internal GameServer(CommunityExpress ce)
		{
            _ce = ce;
			_gameServer = SteamUnityAPI_SteamGameServer();

            _ce.AddEventHandler(GSClientApprove_t.k_iCallback, new CommunityExpress.OnEventHandler<GSClientApprove_t>(Events_GSClientApprove));
            _ce.AddEventHandler(GSClientDeny_t.k_iCallback, new CommunityExpress.OnEventHandler<GSClientDeny_t>(Events_GSClientDeny));
            _ce.AddEventHandler(GSClientKick_t.k_iCallback, new CommunityExpress.OnEventHandler<GSClientKick_t>(Events_GSClientKick));
            _ce.AddEventHandler(GSClientAchievementStatus_t.k_iCallback, new CommunityExpress.OnEventHandler<GSClientAchievementStatus_t>(Events_GSClientAchievementStatus));
            _ce.AddEventHandler(GSPolicyResponse_t.k_iCallback, new CommunityExpress.OnEventHandler<GSPolicyResponse_t>(Events_GSPolicyResponse));
            _ce.AddEventHandler(GSGameplayStats_t.k_iCallback, new CommunityExpress.OnEventHandler<GSGameplayStats_t>(Events_GSGameplayStats));
            _ce.AddEventHandler(GSClientGroupStatus_t.k_iCallback, new CommunityExpress.OnEventHandler<GSClientGroupStatus_t>(Events_GSClientGroupStatus));
            _ce.AddEventHandler(GSReputation_t.k_iCallback, new CommunityExpress.OnEventHandler<GSReputation_t>(Events_GSReputation));
            _ce.AddEventHandler(AssociateWithClanResult_t.k_iCallback, new CommunityExpress.OnEventHandler<AssociateWithClanResult_t>(Events_AssociateWithClanResult));
            _ce.AddEventHandler(ComputeNewPlayerCompatibilityResult_t.k_iCallback, new CommunityExpress.OnEventHandler<ComputeNewPlayerCompatibilityResult_t>(Events_ComputeNewPlayerCompatibilityResult));
            _ce.AddEventHandler(SteamServerConnectFailure_t.k_iCallback, new CommunityExpress.OnEventHandler<SteamServerConnectFailure_t>(Events_SteamServerConnectFailure));
            _ce.AddEventHandler(SteamServersConnected_t.k_iCallback, new CommunityExpress.OnEventHandler<SteamServersConnected_t>(Events_SteamServersConnected));
            
		}
        
        public delegate void GSClientApproveHandler(GameServer sender, SteamID steamID);
        public event GSClientApproveHandler ServerClientApproved;

        private void Events_GSClientApprove(GSClientApprove_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            foreach (SteamID s in _playersPendingAuth)
            {
                if (s == recv.m_SteamID)
                {
                    if (ServerClientApproved != null)
                    {
                        ServerClientApproved(this, s);
                    }
                    _playersConnected.Add(s);
                    _playersPendingAuth.Remove(s);
                    break;
                }
            }
        }

        public delegate void GSClientDenyHandler(GameServer sender, SteamID steamID, EDenyReason denyReason, String optionalText);
        public event GSClientDenyHandler ServerClientDenied;

        private void Events_GSClientDeny(GSClientDeny_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            foreach (SteamID s in _playersPendingAuth)
            {
                if (s == recv.m_SteamID)
                {
                    if (ServerClientDenied != null)
                    {
                        ServerClientDenied(this, s, recv.m_eDenyReason, recv.m_rgchOptionalText.ToString());
                    }
                    _playersConnected.Remove(s);
                    _playersPendingAuth.Remove(s);
                    break;
                }
            }
        }

        public delegate void GSClientKickHandler(GameServer sender, SteamID steamID, EDenyReason denyReason);
        public event GSClientKickHandler ServerClientKicked;

        private void Events_GSClientKick(GSClientKick_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            Boolean found = false;

            foreach (SteamID s in _playersPendingAuth)
            {
                if (s == recv.m_SteamID)
                {
                    if (ServerClientKicked != null)
                    {
                        ServerClientKicked(this, s, recv.m_eDenyReason);
                    }
                    _playersPendingAuth.Remove(s);
                    found = true;
                    break;
                }
            }

            foreach (SteamID s in _playersConnected)
            {
                if (s == recv.m_SteamID)
                {
                    if (!found)
                    {
                        if (ServerClientKicked != null)
                        {
                            ServerClientKicked(this, s, recv.m_eDenyReason);
                        }
                    }

                    _playersConnected.Remove(s);
                    break;
                }
            }
        }
        
        public delegate void GSClientAchievementStatusHandler(GameServer sender, SteamID steamID, String achievement, bool isUnlocked);
        public event GSClientAchievementStatusHandler ClientAchievementStatus;
        
        private void Events_GSClientAchievementStatus(GSClientAchievementStatus_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (ClientAchievementStatus != null)
            {
                ClientAchievementStatus(this, new SteamID(recv.m_SteamID), recv.m_pchAchievement.ToString(), recv.m_bUnlocked);
            }
        }

        public delegate void GSPolicyResponseHandler(GameServer sender, bool isSecure);
        public event GSPolicyResponseHandler ServerPolicy;

        private void Events_GSPolicyResponse(GSPolicyResponse_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            _vacSecured = recv.m_bSecure != 0;
            if (ServerPolicy != null)
            {
                ServerPolicy(this, recv.m_bSecure != 0);
            }
        }

        public delegate void GSGameplayStatsHandler(GameServer sender, int serverRank, UInt32 totalConnections, UInt32 totalMinutesPlayed, EResult result);
        public event GSGameplayStatsHandler ServerStats;

        private void Events_GSGameplayStats(GSGameplayStats_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (ServerStats != null)
            {
                ServerStats(this, recv.m_nRank, recv.m_unTotalConnects, recv.m_unTotalMinutesPlayed, recv.m_eResult);
            }
        }

        public delegate void GSClientGroupStatusHandler(GameServer sender, SteamID userID, SteamID groupID, bool isMember, bool isOfficer);
        public event GSClientGroupStatusHandler GroupStatus;

        private void Events_GSClientGroupStatus(GSClientGroupStatus_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (GroupStatus != null)
            {
                GroupStatus(this, new SteamID(recv.m_SteamIDUser), new SteamID(recv.m_SteamIDGroup), recv.m_bMember, recv.m_bOfficer);
            }
        }

        public delegate void GSReputationHandler(GameServer sender, UInt32 reputationScore, bool isBanned, UInt32 bannedIP, UInt16 bannedPort, UInt64 bannedGameID, UInt32 expireTime, EResult result);
        public event GSReputationHandler ServerReputation;

        private void Events_GSReputation(GSReputation_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (ServerReputation != null)
            {
                ServerReputation(this, recv.m_unReputationScore, recv.m_bBanned, recv.m_unBannedIP, recv.m_usBannedPort, recv.m_ulBannedGameID, recv.m_unBanExpires, recv.m_eResult);
            }
        }

        public delegate void AssociateWithClanResultHandler(GameServer sender, EResult result);
        public event AssociateWithClanResultHandler AssociatWithClanResult;

        private void Events_AssociateWithClanResult(AssociateWithClanResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (AssociatWithClanResult != null)
            {
                AssociatWithClanResult(this, recv.m_eResult);
            }
        }

        public delegate void ComputeNewPlayerCompatibilityResultHandler(GameServer sender, EResult result);
        public event ComputeNewPlayerCompatibilityResultHandler ComputeNewPlayerCompatibilityResult;

        private void Events_ComputeNewPlayerCompatibilityResult(ComputeNewPlayerCompatibilityResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (AssociatWithClanResult != null)
            {
                AssociatWithClanResult(this, recv.m_eResult);
            }
        }

        public delegate void SteamServerConnectFailureHandler(GameServer sender, EResult result);
        public event SteamServerConnectFailureHandler SteamServerConnectFailure;

        private void Events_SteamServerConnectFailure(SteamServerConnectFailure_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (SteamServerConnectFailure != null)
            {
                SteamServerConnectFailure(this, recv.m_eResult);
            }
        }

        public delegate void SteamServersConnectedHandler(GameServer sender);
        public event SteamServersConnectedHandler SteamServersConnected;

        private void Events_SteamServersConnected(SteamServersConnected_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (SteamServersConnected != null)
            {
                SteamServersConnected(this);
            }
        }
        
        /// <summary>
        /// Finalize game server
        /// </summary>
		~GameServer()
		{
			Shutdown();
		}

        /// <summary>
        /// Initialize GameServer and set server properties which may not be changed.
        /// </summary>
        /// <param name="isDedicated">If the server is dedicated</param>
        /// <param name="ip">IP address of the server</param>
        /// <param name="port">Port of the server</param>
        /// <param name="queryPort">Port DNS queries are sent to</param>
        /// <param name="masterServerPort">The port the main server is running from</param>
        /// <param name="spectatorPort">The port the game spectator joins from</param>
        /// <param name="serverMode">Mode the server is in</param>
        /// <param name="serverName">Name of the server</param>
        /// <param name="spectatorServerName">Name of the server the spectator sees</param>
        /// <param name="regionName">Region the server is in</param>
        /// <param name="gameName">Game the server is running</param>
        /// <param name="gameDescription">Description of the game</param>
        /// <param name="gameVersion">Version the game is running</param>
        /// <param name="mapName">Name of the map the server is running</param>
        /// <param name="maxClients">Maximum number of clients allowed in the server</param>
        /// <param name="isPassworded">If the server has a password</param>
        /// <param name="onGameServerClientApproved">If the server client is allowed to join the server</param>
        /// <param name="onGameServerClientDenied">If the server client is not allowed to join the server</param>
        /// <param name="onGameServerClientKick">If the server client was kicked from the server</param>
        /// <returns>true if server is created</returns>
		public Boolean Init(Boolean isDedicated, IPAddress ip, UInt16 port, UInt16 queryPort, UInt16 masterServerPort, UInt16 spectatorPort,
			EServerMode serverMode, String serverName, String spectatorServerName, String regionName, String gameName, String gameDescription,
			String gameVersion, String mapName, UInt16 maxClients, Boolean isPassworded, OnGameServerClientApproved onGameServerClientApproved,
			OnGameServerClientDenied onGameServerClientDenied, OnGameServerClientKick onGameServerClientKick)
		{
			return Init(isDedicated, ip, port, queryPort, masterServerPort, spectatorPort,
			serverMode, serverName, spectatorServerName, regionName, gameName, gameDescription,
			gameVersion, mapName, maxClients, isPassworded, string.Empty, onGameServerClientApproved,
			onGameServerClientDenied, onGameServerClientKick);
		}
        /// <summary>
        /// Initialize GameServer and set server properties which may not be changed.
        /// </summary>
        /// <param name="isDedicated">If the server is dedicated</param>
        /// <param name="ip">IP address of the server</param>
        /// <param name="port">Port of the server</param>
        /// <param name="queryPort">Port DNS queries are sent to</param>
        /// <param name="masterServerPort">The port the main server is running from</param>
        /// <param name="spectatorPort">The port the game spectator joins from</param>
        /// <param name="serverMode">Mode the server is in</param>
        /// <param name="serverName">Name of the server</param>
        /// <param name="spectatorServerName">Name of the server the spectator sees</param>
        /// <param name="regionName">Region the server is in</param>
        /// <param name="gameName">Game the server is running</param>
        /// <param name="gameDescription">Description of the game</param>
        /// <param name="gameVersion">Version the game is running</param>
        /// <param name="mapName">Name of the map the server is running</param>
        /// <param name="maxClients">Maximum number of clients allowed in the server</param>
        /// <param name="isPassworded">If the server has a password</param>
        /// <param name="modDir">Directory of mods needed for server</param>
        /// <param name="onGameServerClientApproved">If the server client is allowed to join the server</param>
        /// <param name="onGameServerClientDenied">If the server client is not allowed to join the server</param>
        /// <param name="onGameServerClientKick">If the server client was kicked from the server</param>
        /// <returns>true if server is created</returns>
		public Boolean Init(Boolean isDedicated, IPAddress ip, UInt16 port, UInt16 queryPort, UInt16 masterServerPort, UInt16 spectatorPort,
			EServerMode serverMode, String serverName, String spectatorServerName, String regionName, String gameName, String gameDescription,
			String gameVersion, String mapName, UInt16 maxClients, Boolean isPassworded, String modDir, OnGameServerClientApproved onGameServerClientApproved,
			OnGameServerClientDenied onGameServerClientDenied, OnGameServerClientKick onGameServerClientKick)
		{
			Byte[] serverIPBytes = ip.GetAddressBytes();
			UInt32 serverIP = (UInt32)serverIPBytes[0] << 24 | (UInt32)serverIPBytes[1] << 16 | (UInt32)serverIPBytes[2] << 8 | (UInt32)serverIPBytes[3];

			_playersPendingAuth.Clear();
			_playersConnected.Clear();
			_botsConnected.Clear();

			_onGameServerClientDenied = onGameServerClientDenied;
			_onGameServerClientKick = onGameServerClientKick;

			_internalOnGameServerClientKick = new OnGameServerClientKickFromSteam(OnGameServerClientKickCallback);
			_internalOnGameServerPolicyResponse = new OnGameServerPolicyResponseFromSteam(OnGameServerPolicyResponseCallback);

			/*SteamUnityAPI_SteamGameServer_SetCallbacks(Marshal.GetFunctionPointerForDelegate(_internalOnGameServerClientApproved),
				Marshal.GetFunctionPointerForDelegate(_internalOnGameServerClientDenied),
				Marshal.GetFunctionPointerForDelegate(_internalOnGameServerClientKick),
				Marshal.GetFunctionPointerForDelegate(_internalOnGameServerPolicyResponse));*/

			if (SteamUnityAPI_SteamGameServer_Init(serverIP, masterServerPort, port, queryPort, serverMode, gameVersion))
			{
				_isInitialized = true;
				_gameServer = SteamUnityAPI_SteamGameServer();
				_isDedicated = isDedicated;
				_serverName = serverName;
				_spectatorServerName = spectatorServerName;
				_spectatorPort = spectatorPort;
				_mapName = mapName;
				_regionName = regionName;
				_gameName = gameName;
				_gameDescription = gameDescription;
				_maxClients = maxClients;
				_isPassworded = isPassworded;
				_modDir = modDir;

				SendBasicServerStatus();
				SteamUnityAPI_SteamGameServer_LogOnAnonymous(_gameServer);
				SendUpdatedServerStatus();

				return true;
			}

			return false;
		}

        public void ComputeNewPlayerCompatibility( SteamID playerID )
        {
            SteamUnityAPI_SteamGameServer_ComputeNewPlayerCompatibility(_gameServer, playerID.ToUInt64());
        }

        /// <summary>
        /// Ask for the gameplay stats for the server. Results returned in ServerStats callback
        /// </summary>
        public void GetGameplayStats()
        {
            SteamUnityAPI_SteamGameServer_GetGameplayStats(_gameServer);
        }
        /// <summary>
        /// Associate this game server with this clan for the purposes of computing player compatability
        /// </summary>
        public void AssociateWithGroup(Group g)
        {
            SteamUnityAPI_SteamGameServer_AssociateWithClan(_gameServer, g.SteamID.ToUInt64());
        }
        /// <summary>
        /// Ask if a user in in the specified group, results returns async by GSUserGroupStatus_t
        /// returns false if you're not connected to the steam servers and thus cannot ask
        /// </summary>
        public void RequestUserGroupStatus(SteamID userID, SteamID groupID)
        {
            SteamUnityAPI_SteamGameServer_RequestUserGroupStatus(_gameServer, userID.ToUInt64(), groupID.ToUInt64());
        }
        /// <summary>
        /// Client tries to connect to the GameServer
        /// </summary>
        /// <param name="ipClient">Ip of the client computer</param>
        /// <param name="authTicket">Client's authentication ticket</param>
        /// <param name="steamIDClient">Client's Steam ID</param>
        /// <returns>true if the client can connect</returns>
		public Boolean ClientConnected(IPAddress ipClient, Byte[] authTicket, out SteamID steamIDClient)
		{
			Byte[] clientIPBytes = ipClient.GetAddressBytes();
			UInt32 clientIP = (UInt32)clientIPBytes[0] << 24 | (UInt32)clientIPBytes[1] << 16 | (UInt32)clientIPBytes[2] << 8 | (UInt32)clientIPBytes[3];
			UInt64 clientSteamID;

			if (SteamUnityAPI_SteamGameServer_SendUserConnectAndAuthenticate(_gameServer, clientIP, authTicket, (UInt32)authTicket.Length, out clientSteamID))
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
        /// <summary>
        /// Adds a bot to the server
        /// </summary>
        /// <returns>true if the bot is added</returns>
		public SteamID AddBot()
		{
			SteamID ret = new SteamID(SteamUnityAPI_SteamGameServer_CreateUnauthenticatedUserConnection(_gameServer));

			_botsConnected.Add(ret);

			return ret;
		}
        /// <summary>
        /// Updates the current user's details
        /// </summary>
        /// <param name="steamID">User's Steam ID</param>
        /// <param name="displayableName">User's display name</param>
        /// <param name="score">User's score</param>
        /// <returns>true if the details are updated successfully</returns>
		public Boolean UpdateUserDetails(SteamID steamID, String displayableName, UInt32 score)
		{
			return SteamUnityAPI_SteamGameServer_UpdateUserData(_gameServer, steamID.ToUInt64(), displayableName, score);
		}
        /// <summary>
        /// Client disconnects from the server
        /// </summary>
        /// <param name="steamIDClient">Client's Steam ID</param>
		public void ClientDisconnected(SteamID steamIDClient)
		{
			Boolean found = false;

			foreach (SteamID s in _playersPendingAuth)
			{
				if (s == steamIDClient)
				{
					SteamUnityAPI_SteamGameServer_SendUserDisconnect(_gameServer, steamIDClient.ToUInt64());
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
						SteamUnityAPI_SteamGameServer_SendUserDisconnect(_gameServer, steamIDClient.ToUInt64());
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
						SteamUnityAPI_SteamGameServer_SendUserDisconnect(_gameServer, steamIDClient.ToUInt64());
						found = true;
						break;
					}
				}
			}

			_playersPendingAuth.Remove(steamIDClient);
			_playersConnected.Remove(steamIDClient);
			_botsConnected.Remove(steamIDClient);
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
        /// <summary>
        /// Gets list of players connected to the server
        /// </summary>
        /// <returns>List of connected players</returns>
		public ICollection<Friend> GetPlayersConnected()
		{
			List<Friend> friends = new List<Friend>();

			foreach (SteamID id in _playersConnected)
			{
				friends.Add(new Friend(CommunityExpress.Instance.Friends, 
					id));
			}

			return friends;
		}

		private void OnGameServerPolicyResponseCallback(ref GSPolicyResponse_t callbackData)
		{
			_vacSecured = callbackData.m_bSecure != 0;
		}

		private void SendBasicServerStatus()
		{
			SteamUnityAPI_SteamGameServer_SetBasicServerData(_gameServer, _isDedicated, _gameName, _gameDescription, _modDir);
		}

		internal void SendUpdatedServerStatus()
		{
			SteamUnityAPI_SteamGameServer_UpdateServerStatus(_gameServer, _maxClients, _botsConnected.Count, _serverName, _spectatorServerName, _spectatorPort, _regionName, _mapName, false);
		}
        /// <summary>
        /// Requests stats from a user on the server
        /// </summary>
        /// <param name="steamID">Steam ID for the user</param>
        /// <param name="requestedStats">Stats requested from the user</param>
		public void RequestUserStats(SteamID steamID, IEnumerable<String> requestedStats)
		{
            Stats stats = new Stats(_ce, steamID, true);
			stats.RequestCurrentStats(requestedStats);
		}
        /// <summary>
        /// Requests achievements from a user on the server
        /// </summary>
        /// <param name="steamID">Steam ID for the user</param>
        /// <param name="requestedAchievements">Achievements requested from the user</param>
		public void RequestUserAchievements(SteamID steamID, IEnumerable<String> requestedAchievements)
		{
			Achievements achievements = new Achievements(_ce, steamID, true);
			achievements.RequestCurrentAchievements(requestedAchievements);
		}
        /// <summary>
        /// Shuts down the server
        /// </summary>
		public void Shutdown()
		{
			SteamUnityAPI_SteamGameServer_Shutdown();
		}
        /// <summary>
        /// Checks if the server is running
        /// </summary>
		public Boolean IsInitialized
		{
			get { return _isInitialized; }
		}
        /// <summary>
        /// IP of the server
        /// </summary>
		public IPAddress PublicIP
		{
			get
			{
				UInt32 ip = SteamUnityAPI_SteamGameServer_GetPublicIP(_gameServer);
				return new IPAddress(new byte[] { (byte)(ip >> 24), (byte)(ip >> 16), (byte)(ip >> 8), (byte)ip });
			}
		}
        /// <summary>
        /// Checks if the server is Valve Anti-Cheat secured
        /// </summary>
		public Boolean IsVacSecured
		{
			get { return _vacSecured; }
		}
        /// <summary>
        /// Gets the Steam ID of the server
        /// </summary>
		public SteamID SteamID
		{
			get { return new SteamID(SteamUnityAPI_SteamGameServer_GetSteamID(_gameServer)); }
		}
        /// <summary>
        /// Checks if the server is a Dedicated Server
        /// </summary>
		public Boolean IsDedicated
		{
			get { return _isDedicated; }
			set { _isDedicated = value; SendBasicServerStatus(); }
		}
        /// <summary>
        /// Gets the server's name
        /// </summary>
		public String ServerName
		{
			get { return _serverName; }
			set { _serverName = value; SendUpdatedServerStatus(); }
		}
        /// <summary>
        /// Gets the name of a spectator server
        /// </summary>
		public String SpectatorServerName
		{
			get { return _spectatorServerName; }
			set { _spectatorServerName = value; SendUpdatedServerStatus(); }
		}
        /// <summary>
        /// Gets the region the server is hosted in
        /// </summary>
		public String RegionName
		{
			get { return _regionName; }
			set { _regionName = value; SendUpdatedServerStatus(); }
		}
        /// <summary>
        /// Gets the name of the game the server is running
        /// </summary>
		public String GameName
		{
			get { return _gameName; }
			set { _gameName = value; SendBasicServerStatus(); }
		}
        /// <summary>
        /// Gets the description of the game the server is running
        /// </summary>
		public String GameDescription
		{
			get { return _gameDescription; }
			set { _gameDescription = value; SendBasicServerStatus(); }
		}
        /// <summary>
        /// Gets the maximum number of clients allowed in the server
        /// </summary>
		public UInt16 MaxClients
		{
			get { return _maxClients; }
			set { _maxClients = value; SendBasicServerStatus(); }
		}
        /// <summary>
        /// Checks if the server has a password
        /// </summary>
		public Boolean IsPassworded
		{
			get { return _isPassworded; }
			set { _isPassworded = value; SendBasicServerStatus(); }
		}
        /// <summary>
        /// Gets the name of the map the server is running on
        /// </summary>
		public String MapName
		{
			get { return _mapName; }
			set { _mapName = value; SendUpdatedServerStatus(); }
		}
        /// <summary>
        /// Defines key values strings
        /// </summary>
		public Dictionary<String, String> KeyValues
		{
			get
			{
				return _keyValues;
			}

			set
			{
				_keyValues = value;

				String[] keys = new String[_keyValues.Keys.Count], values = new String[_keyValues.Values.Count];
				_keyValues.Keys.CopyTo(keys, 0);
				_keyValues.Values.CopyTo(values, 0);

				SteamUnityAPI_SteamGameServer_SetKeyValues(_gameServer, keys, values, _keyValues.Count);
			}
		}
        /// <summary>
        /// Gets the tags of the game the server is running
        /// </summary>
		public String GameTags
		{
			get { return _gameTags; }
			set { _gameTags = value; SteamUnityAPI_SteamGameServer_SetGameTags(_gameServer, _gameTags); }
		}
        /// <summary>
        /// Gets the data of the game the server is running
        /// </summary>
		public String GameData
		{
			get { return _gameData; }
			set { _gameData = value; SteamUnityAPI_SteamGameServer_SetGameTags(_gameServer, _gameData); }
		}
	}
}

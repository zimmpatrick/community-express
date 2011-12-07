using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;

namespace SteamworksUnityHost
{
	using AppId_t = UInt32;
	using HServerListRequest = UInt32;

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
		private static extern HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestInternetServerList(IntPtr matchmakingServers, AppId_t appId,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] keys,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] String[] values,
			UInt32 keyvalueCount, IntPtr serverReceivedCallback, IntPtr serverListReceivedCallback);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamMatchmakingServers_ReleaseRequest(IntPtr matchmakingServers, HServerListRequest request);

		public const HServerListRequest HServerListRequest_Invalid = 0x0;

		private IntPtr _matchmaking;
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

		public Servers RequestInternetServerList(Dictionary<String, String> filters, OnServerReceived onServerReceived, OnServerListReceived onServerListReceived)
		{
			if (_serverListRequest != HServerListRequest_Invalid)
				return null;

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
				String[] keys = new String[]{};
				String[] values = new String[]{};

				_serverFilters.Keys.CopyTo(keys, 0);
				_serverFilters.Values.CopyTo(values, 0);

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

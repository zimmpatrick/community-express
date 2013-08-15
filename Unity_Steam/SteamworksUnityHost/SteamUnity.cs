﻿// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CommunityExpressNS
{
	using SteamAPICall_t = UInt64;
	using AppId_t = UInt32;
	using System.Reflection;
	using System.IO;
	using System.Security.Cryptography;
	using System.Security.Cryptography.Xml;
	using System.Xml;

	public sealed class CommunityExpress
	{
		[DllImport("CommunityExpressSW")]
		private static extern bool SteamUnityAPI_RestartAppIfNecessary(uint unOwnAppID);
		[DllImport("CommunityExpressSW")]
        private static extern bool SteamUnityAPI_Init(IntPtr OnChallengeResponse, IntPtr OnCallback);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_RunCallbacks();
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamGameServer_RunCallbacks();
		[DllImport("CommunityExpressSW")]
		private static extern AppId_t SteamUnityAPI_SteamUtils_GetAppID();
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamUtils_IsAPICallCompleted(SteamAPICall_t callHandle, out Byte failed);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetAPICallResult(SteamAPICall_t callHandle, IntPtr pCallback, int cubCallback, int iCallbackExpected, out Byte failed);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetGameServerUserStatsReceivedResult(SteamAPICall_t callHandle, out GSStatsReceived_t result, out Byte failed);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetLobbyCreatedResult(SteamAPICall_t callHandle, out LobbyCreated_t result, out Byte failed);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetLobbyListReceivedResult(SteamAPICall_t callHandle, out LobbyMatchList_t result, out Byte failed);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetLobbyEnteredResult(SteamAPICall_t callHandle, out LobbyEnter_t result, out Byte failed);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_Shutdown();
		[DllImport("CommunityExpressSW")]
		private static extern bool SteamUnityAPI_SetWarningMessageHook(IntPtr OnSteamAPIDebugTextHook);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_WriteMiniDump(uint exceptionCode, IntPtr exceptionInfo, uint buildID);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SetMiniDumpComment([MarshalAs(UnmanagedType.LPStr)] String comment);
		[DllImport("CommunityExpressSW")]
		private static extern bool SteamUnityAPI_IsSteamRunning();

		delegate UInt64 OnChallengeResponseFromSteam(UInt64 challenge);
		private OnChallengeResponseFromSteam _challengeResponse;
        delegate void OnCallbackFromSteam(Int32 k_iCallback, IntPtr pvParam, Boolean bIOFailure, SteamAPICall_t hSteamAPICall);
        private OnCallbackFromSteam _callback;

		delegate void OnSteamAPIDebugTextHook(Int32 nSeverity, IntPtr pchDebugText);
		private OnSteamAPIDebugTextHook _steamAPIDebugTextHook;
		
		public delegate void LogMessage(string msg);
        public event LogMessage Logger;

        public delegate void ShutdownRequestHandler(CommunityExpress sender);
        public event ShutdownRequestHandler ShutdownRequested;

        public delegate void LowBatteryHandler(CommunityExpress sender, LowBatteryArgs e);
        public event LowBatteryHandler LowBattery;

        public class LowBatteryArgs : System.EventArgs
        {
            internal LowBatteryArgs(LowBatteryPower_t args)
            {
                MinutesBatteryLeft = args.m_nMinutesBatteryLeft;
            }

            public int MinutesBatteryLeft
            {
                get;
                private set;
            }
        }


		private static readonly CommunityExpress _instance = new CommunityExpress();
		private bool _shutdown = false;

		private App _app = null;
		private User _user = null;
		private GameServer _gameserver = null;
		private Friends _friends = null;
		private Groups _groups = null;
		private Stats _userStats = null;
		private Friends _serverFriends = null;
		private Achievements _achievements = null;
		private Leaderboards _leaderboards = null;
		private Matchmaking _matchmaking = null;
		private RemoteStorage _remoteStorage = null;
		private Networking _networking = null;
		private InGamePurchasing _inGamePurchasing = null;
		private SteamWebAPI _steamWebAPI = null;
		private BigPicture _bigPicture = null;
        private UserGeneratedContent _ugc = null;
        
        internal delegate void OnEventHandler<T>(T pvParam, Boolean bIOFailure, SteamAPICall_t hSteamAPICall);

        private MethodInfo _internalOnEvent = null;
        private Dictionary<int, Type> _internalEventFactory = new Dictionary<int, Type>();
        private Dictionary<int, MethodInfo> _internalOnEventFactory = new Dictionary<int, MethodInfo>();
        private Dictionary<Int32, object> _internalHandlers = new Dictionary<Int32, object>();

		private List<SteamAPICall_t> _gameserverUserStatsReceivedCallHandles = new List<SteamAPICall_t>();
		private List<OnUserStatsReceivedFromSteam> _gameserverUserStatsReceivedCallbacks = new List<OnUserStatsReceivedFromSteam>();

		private SteamAPICall_t _userGetEncryptedAppTicketCallHandle = 0;
		private OnUserGetEncryptedAppTicketFromSteam _userGetEncryptedAppTicketCallback;

		private SteamAPICall_t _lobbyCreatedCallHandle = 0;
		private OnLobbyCreatedBySteam _lobbyCreatedCallback;

		private SteamAPICall_t _lobbyListReceivedCallHandle = 0;
		private OnLobbyListReceivedFromSteam _lobbyListReceivedCallback;

		private SteamAPICall_t _lobbyJoinedCallHandle = 0;
		private OnLobbyJoinedFromSteam _lobbyJoinedCallback;

		public const uint k_uAppIdInvalid = 0x0;

		private CommunityExpress() { }
		~CommunityExpress() { Shutdown(); }

		public static CommunityExpress Instance
		{
			get
			{
				return _instance;
			}
		}

		public bool RestartAppIfNecessary(uint unOwnAppID)
		{
			return SteamUnityAPI_RestartAppIfNecessary(unOwnAppID);
		}

		public void WriteMiniDump(uint exceptionCode, IntPtr exceptionInfo, uint buildID)
		{
			SteamUnityAPI_WriteMiniDump(exceptionCode, exceptionInfo, buildID);
		}

		public void SetMiniDumpComment(String comment)
		{
			SteamUnityAPI_SetMiniDumpComment(comment);
		}

		public bool Initialize()
		{
            foreach (MethodInfo mi in typeof(CommunityExpress).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (mi.Name == "InternalOnEvent" && 
                    mi.IsGenericMethod)
                {
                    _internalOnEvent = mi;
                    break;
                }
            }
            
			_challengeResponse = new OnChallengeResponseFromSteam(OnChallengeResponseCallback);
            _callback = new OnCallbackFromSteam(OnCallback);

			if (SteamUnityAPI_Init(Marshal.GetFunctionPointerForDelegate(_challengeResponse),
                                   Marshal.GetFunctionPointerForDelegate(_callback)))
			{
				_steamAPIDebugTextHook = new OnSteamAPIDebugTextHook(OnSteamAPIDebugTextHookCallback);
				SteamUnityAPI_SetWarningMessageHook(Marshal.GetFunctionPointerForDelegate(_steamAPIDebugTextHook));

				// ValidateLicense();
				return true;
			}

			return false;
		}

		public void Log(string msg)
		{
            if (Logger != null)
			{
                Logger(msg);
			}

			System.Diagnostics.Debug.WriteLine(msg);
		}

		public void RunCallbacks()
		{
			SteamUnityAPI_RunCallbacks();

			if (_gameserver != null && _gameserver.IsInitialized)
			{
				SteamUnityAPI_SteamGameServer_RunCallbacks();

				for (int i = 0; i < _gameserverUserStatsReceivedCallHandles.Count; i++)
				{
					SteamAPICall_t h = _gameserverUserStatsReceivedCallHandles[i];

					Byte failed;
					if (SteamUnityAPI_SteamUtils_IsAPICallCompleted(h, out failed))
					{
						GSStatsReceived_t callbackData = new GSStatsReceived_t();
						UserStatsReceived_t morphedCallbackData = new UserStatsReceived_t();
						SteamUnityAPI_SteamUtils_GetGameServerUserStatsReceivedResult(h, out callbackData, out failed);

						morphedCallbackData.m_nGameID = SteamUnityAPI_SteamUtils_GetAppID();
						morphedCallbackData.m_steamIDUser = callbackData.m_steamIDUser;
						morphedCallbackData.m_eResult = callbackData.m_eResult;

						_gameserverUserStatsReceivedCallbacks[i](ref morphedCallbackData);
						_gameserverUserStatsReceivedCallHandles.RemoveAt(i);
						_gameserverUserStatsReceivedCallbacks.RemoveAt(i);

						i--;
					}
				}
			}

			if (_networking != null && _networking.IsInitialized)
				_networking.CheckForNewP2PPackets();

			if (_userGetEncryptedAppTicketCallHandle != 0)
			{
				Byte failed;
				if (SteamUnityAPI_SteamUtils_IsAPICallCompleted(_userGetEncryptedAppTicketCallHandle, out failed))
				{
					_userGetEncryptedAppTicketCallback();
					_userGetEncryptedAppTicketCallHandle = 0;
				}
			}

			if (_lobbyCreatedCallHandle != 0)
			{
				Byte failed;
				if (SteamUnityAPI_SteamUtils_IsAPICallCompleted(_lobbyCreatedCallHandle, out failed))
				{
					LobbyCreated_t callbackData = new LobbyCreated_t();
					SteamUnityAPI_SteamUtils_GetLobbyCreatedResult(_lobbyCreatedCallHandle, out callbackData, out failed);

					_lobbyCreatedCallback(ref callbackData);

					_lobbyCreatedCallHandle = 0;
				}
			}

			if (_lobbyListReceivedCallHandle != 0)
			{
				Byte failed;
				if (SteamUnityAPI_SteamUtils_IsAPICallCompleted(_lobbyListReceivedCallHandle, out failed))
				{
					LobbyMatchList_t callbackData = new LobbyMatchList_t();
					SteamUnityAPI_SteamUtils_GetLobbyListReceivedResult(_lobbyListReceivedCallHandle, out callbackData, out failed);

					_lobbyListReceivedCallback(ref callbackData);

					_lobbyListReceivedCallHandle = 0;
				}
			}

			if (_lobbyJoinedCallHandle != 0)
			{
				Byte failed;
				if (SteamUnityAPI_SteamUtils_IsAPICallCompleted(_lobbyJoinedCallHandle, out failed))
				{
					LobbyEnter_t callbackData = new LobbyEnter_t();
					SteamUnityAPI_SteamUtils_GetLobbyEnteredResult(_lobbyJoinedCallHandle, out callbackData, out failed);

					_lobbyJoinedCallback(ref callbackData);

					_lobbyJoinedCallHandle = 0;
				}
			}
		}

		public void Shutdown()
		{
			// todo: make thread safe?
			if (!_shutdown)
			{
				_shutdown = true;
				SteamUnityAPI_Shutdown();
			}
		}

		public AppId_t AppID
		{
			get { return SteamUnityAPI_SteamUtils_GetAppID(); }
		}

		public App App
		{
			get
			{
				if (_app == null)
				{
					_app = new App();
				}

				return _app;
			}
		}

		public User User
		{
			get
			{
				if (_user == null)
				{
					_user = new User();
				}

				return _user;
			}
		}

		public GameServer GameServer
		{
			get
			{
				if (_gameserver == null)
				{
					_gameserver = new GameServer(this);
				}

				return _gameserver;
			}
		}

		public Friends Friends
		{
			get
			{
				if (_friends == null)
				{
					_friends = new Friends();
				}

				return _friends;
			}
		}

		public Groups Groups
		{
			get
			{
				if (_groups == null)
				{
					_groups = new Groups(Friends);
				}

				return _groups;
			}
		}

		public Stats UserStats
		{
			get
			{
				if (_userStats == null)
				{
					_userStats = new Stats(this, User.SteamID);
				}

				return _userStats;
			}
		}

		public Achievements UserAchievements
		{
			get
			{
				if (_achievements == null)
				{
					_achievements = new Achievements(this, User.SteamID);
				}

				return _achievements;
			}
		}

		public Leaderboards Leaderboards
		{
			get
			{
				if (_leaderboards == null)
				{
					_leaderboards = new Leaderboards(this);
				}

				return _leaderboards; 
			}
		}

		public Matchmaking Matchmaking
		{
			get
			{
				if (_matchmaking == null)
				{
					_matchmaking = new Matchmaking();
				}

				return _matchmaking;
			}
		}

		public RemoteStorage RemoteStorage
		{
			get
			{
				if (_remoteStorage == null)
				{
					_remoteStorage = new RemoteStorage();
				}

				return _remoteStorage;
			}
		}

		public Networking Networking
		{
			get
			{
				if (_networking == null)
				{
					_networking = new Networking();
				}

				return _networking;
			}
		}

		public InGamePurchasing InGamePurchasing
		{
			get
			{
				if (_inGamePurchasing == null)
				{
					_inGamePurchasing = new InGamePurchasing();
				}

				return _inGamePurchasing;
			}
		}

		public SteamWebAPI SteamWebAPI
		{
			get
			{
				if (_steamWebAPI == null)
				{
					_steamWebAPI = new SteamWebAPI();
				}

				return _steamWebAPI;
			}
		}

		public BigPicture BigPicture
		{
			get
			{
				if (_bigPicture == null)
				{
					_bigPicture = new BigPicture(this);
				}

				return _bigPicture;
			}
		}

        public UserGeneratedContent UserGeneratedContent
        {
            get 
            {
                if (_ugc == null) {
                    _ugc = new UserGeneratedContent();
                }

                return _ugc;
            }
        }

		public Boolean IsGameServerInitialized
		{
			get { return _gameserver != null && _gameserver.IsInitialized; }
		}

        public Boolean IsCommunityRunning
        {
            get { return SteamUnityAPI_IsSteamRunning(); }
        }
        
        private void RegisterEventFactory(int k_iCallback, Type type)
        {
            if (!_internalEventFactory.ContainsKey(k_iCallback)) 
            {
                _internalEventFactory.Add(k_iCallback, type);
            }

            if (!_internalOnEventFactory.ContainsKey(k_iCallback)) 
            {
                _internalOnEventFactory.Add(k_iCallback, _internalOnEvent.MakeGenericMethod(type));
            }
        }

        internal void AddEventHandler<T>(Int32 k_iCallback, OnEventHandler<T> handler)
        {
            RegisterEventFactory(k_iCallback, typeof(T));

            if (_internalHandlers.ContainsKey(k_iCallback))
            {
                _internalHandlers[k_iCallback] = (OnEventHandler<T>)_internalHandlers[k_iCallback] + handler;
            }
            else
            {
                _internalHandlers[k_iCallback] = handler;
            }
        }

        internal void RemoveEventHandler<T>(Int32 k_iCallback, OnEventHandler<T> handler)
        {
            if (_internalHandlers.ContainsKey(k_iCallback))
            {
                _internalHandlers[k_iCallback] = (OnEventHandler<T>)_internalHandlers[k_iCallback] - handler;
            }
        }

        private void InternalOnEvent<T>(Int32 k_iCallback, T pvParam, Boolean bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            // internal callbacks
            ((OnEventHandler<T>)_internalHandlers[k_iCallback])(pvParam, bIOFailure, hSteamAPICall);
        }

        internal void OnEvent(Int32 k_iCallback, IntPtr pvParam, Boolean bIOFailure, SteamAPICall_t hSteamAPICall)
        {
           
            
                /*
            else if (k_iCallback == SteamShutdown_t.k_iCallback)
            {
                Console.WriteLine("shutdown");

                Process.GetCurrentProcess().Kill();
            }
            else if (k_iCallback == SteamAPICallCompleted_t.k_iCallback)
            {
                SteamAPICallCompleted_t p = (SteamAPICallCompleted_t)Marshal.PtrToStructure(pvParam, typeof(SteamAPICallCompleted_t));

                if (_apiCalls.ContainsKey(p.m_hAsyncCall))
                {
                    IAsynchronousCall call = _apiCalls[p.m_hAsyncCall];

                    int sizeOf = Marshal.SizeOf(typeof(LeaderboardFindResult_t));
                    IntPtr unmanagedAddr = Marshal.AllocHGlobal(sizeOf);

                    byte failed;
                    SteamUnityAPI_SteamUtils_GetAPICallResult(p.m_hAsyncCall, unmanagedAddr, sizeOf, LeaderboardFindResult_t.k_iCallback, out failed);
                    LeaderboardFindResult_t findLearderboardResult = (LeaderboardFindResult_t)Marshal.PtrToStructure(unmanagedAddr, typeof(LeaderboardFindResult_t));

                    Marshal.FreeHGlobal(unmanagedAddr);
                    unmanagedAddr = IntPtr.Zero;

                    Leaderboards.LeaderboardRecievedArgs hack = new Leaderboards.LeaderboardRecievedArgs(call.Sender as Leaderboards, findLearderboardResult);

                    call.Complete(hack.Leaderboard);
                }
            }
            else
            {

                Console.WriteLine(k_iCallback);
            }*/
        }

		internal void AddGameServerUserStatsReceivedCallback(SteamAPICall_t handle, OnUserStatsReceivedFromSteam callback)
		{
			_gameserverUserStatsReceivedCallHandles.Add(handle);
			_gameserverUserStatsReceivedCallbacks.Add(callback);
		}

		internal void AddUserGetEncryptedAppTicketCallback(SteamAPICall_t handle, OnUserGetEncryptedAppTicketFromSteam callback)
		{
			_userGetEncryptedAppTicketCallHandle = handle;
			_userGetEncryptedAppTicketCallback = callback;
		}

		internal void AddCreateLobbyCallback(SteamAPICall_t handle, OnLobbyCreatedBySteam callback)
		{
			_lobbyCreatedCallHandle = handle;
			_lobbyCreatedCallback = callback;
		}

		internal void AddLobbyListRequestCallback(SteamAPICall_t handle, OnLobbyListReceivedFromSteam callback)
		{
			_lobbyListReceivedCallHandle = handle;
			_lobbyListReceivedCallback = callback;
		}

		internal void RemoveLobbyListRequestCallback(SteamAPICall_t handle, OnLobbyListReceivedFromSteam callback)
		{
			_lobbyListReceivedCallHandle = 0;
		}

		internal void AddLobbyJoinedCallback(SteamAPICall_t handle, OnLobbyJoinedFromSteam callback)
		{
			_lobbyJoinedCallHandle = handle;
			_lobbyJoinedCallback = callback;
		}

		private void OnSteamAPIDebugTextHookCallback(Int32 nSeverity, IntPtr pchDebugText)
		{
			String msg = Marshal.PtrToStringAnsi(pchDebugText);

			Log(msg);
		}

		private UInt64 OnChallengeResponseCallback(UInt64 challenge)
		{
			// Put a real functional test in here
			return (UInt64)Math.Sqrt((double)challenge);
		}

        private void OnCallback(Int32 k_iCallback, IntPtr pvParam, Boolean bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (_internalHandlers.ContainsKey(k_iCallback) &&
               _internalEventFactory.ContainsKey(k_iCallback) &&
               _internalOnEventFactory.ContainsKey(k_iCallback) &&
                _internalHandlers[k_iCallback] != null)
            {
                object obj = Marshal.PtrToStructure(pvParam, _internalEventFactory[k_iCallback]);

                _internalOnEventFactory[k_iCallback].Invoke(this, new object[] { k_iCallback, obj, bIOFailure, hSteamAPICall });
            }

            if (k_iCallback == SteamShutdown_t.k_iCallback)
            {
                if (ShutdownRequested != null) ShutdownRequested(this);
            }
            else if (k_iCallback == LowBatteryPower_t.k_iCallback)
            {
                if (LowBattery != null)
                {
                    LowBatteryPower_t lbp = (LowBatteryPower_t)Marshal.PtrToStructure(pvParam, typeof(LowBatteryPower_t));

                    LowBattery(this, new LowBatteryArgs(lbp));
                }
            }
        }

        [Conditional("LICENSED")]
		private void ValidateLicense()
		{
			string dllPath = Assembly.GetExecutingAssembly().Location;
			string dllDirectory = Path.GetDirectoryName(dllPath);
			string licensePath = Path.Combine(dllDirectory, "CESDKLicense.xml");

            if (!System.IO.File.Exists(licensePath))
            {
                throw new LicenseException(
                        string.Format("Error: License '{0}' not found.", licensePath));
            }


			UInt32 appId = SteamUnityAPI_SteamUtils_GetAppID();

			string xmlkey = Properties.Resources.CommunityExpressSDK;

			// Create an RSA crypto service provider from the embedded
			// XML document resource (the public key).
			using (RSACryptoServiceProvider csp = new RSACryptoServiceProvider())
			{
				csp.FromXmlString(xmlkey);

				// Load the signed XML license file.
				XmlDocument xmldoc = new XmlDocument();
				xmldoc.Load(licensePath);

				// Create the signed XML object.
				SignedXml sxml = new SignedXml(xmldoc);

				try
				{
					// Get the XML Signature node and load it into the signed XML object.
					XmlNode dsig = xmldoc.GetElementsByTagName("Signature",
						SignedXml.XmlDsigNamespaceUrl)[0];
					sxml.LoadXml((XmlElement)dsig);
				}
				catch (Exception e)
				{
                    throw new LicenseException("Error: no signature found.", e);
				}

				// Verify the signature.
				if (!sxml.CheckSignature(csp))
				{
                    throw new LicenseException(
						string.Format("Error: License '{0}' invalid.", licensePath));
				}

				try
				{
					XmlNodeList appIdNode = xmldoc.GetElementsByTagName("AppId");
					if (appIdNode == null || appIdNode.Count == 0 ||
						string.IsNullOrEmpty(appIdNode[0].InnerText))
					{
                        throw new LicenseException("AppId missing from license");
					}

					if (appIdNode[0].InnerText != appId.ToString())
					{
                        throw new LicenseException(
							string.Format("AppId mismatch: {0} vs {1}",
							appIdNode[0].InnerText, appId));
					}
				}
				catch (Exception e)
				{
                    throw new LicenseException(
						string.Format("Error: License '{0}' invalid.", licensePath), e);
				}
			}
		}
	}
}

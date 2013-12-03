// Copyright (c) 2011-2013, Zimmdot, LLC
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

    /// <summary>
    /// Community Express driver
    /// </summary>
	public sealed class CommunityExpress
	{
        
	    [StructLayout(LayoutKind.Sequential, Pack = 8)]
	    internal struct LobbyMatchList_t
        {
            internal const int k_iCallback = Events.k_iSteamMatchmakingCallbacks + 10;
            internal UInt32 m_nLobbiesMatching;		// Number of lobbies that matched search criteria and we have SteamIDs for
	    }


        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_RestartAppIfNecessary(uint unOwnAppID);
		[DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SteamUnityAPI_Init(IntPtr OnChallengeResponse, IntPtr OnCallback);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_RunCallbacks();
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamGameServer_RunCallbacks();
		[DllImport("CommunityExpressSW")]
		private static extern AppId_t SteamUnityAPI_SteamUtils_GetAppID();
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamUtils_IsAPICallCompleted(SteamAPICall_t callHandle, out Byte failed);
		[DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetAPICallResult(SteamAPICall_t callHandle, IntPtr pCallback, int cubCallback, int iCallbackExpected, out Byte failed);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetGameServerUserStatsReceivedResult(SteamAPICall_t callHandle, out GSStatsReceived_t result, out Byte failed);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetLobbyCreatedResult(SteamAPICall_t callHandle, out LobbyCreated_t result, out Byte failed);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetLobbyListReceivedResult(SteamAPICall_t callHandle, out LobbyMatchList_t result, out Byte failed);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetLobbyEnteredResult(SteamAPICall_t callHandle, out LobbyEnter_t result, out Byte failed);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_Shutdown();
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_SetWarningMessageHook(IntPtr OnSteamAPIDebugTextHook);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_WriteMiniDump(uint exceptionCode, IntPtr exceptionInfo, uint buildID);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SetMiniDumpComment([MarshalAs(UnmanagedType.LPStr)] String comment);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_IsSteamRunning();

		delegate UInt64 OnChallengeResponseFromSteam(UInt64 challenge);
		private OnChallengeResponseFromSteam _challengeResponse;
        delegate void OnCallbackFromSteam(Int32 k_iCallback, IntPtr pvParam, Boolean bIOFailure, SteamAPICall_t hSteamAPICall);
        private OnCallbackFromSteam _callback;

		delegate void OnSteamAPIDebugTextHook(Int32 nSeverity, IntPtr pchDebugText);
		private OnSteamAPIDebugTextHook _steamAPIDebugTextHook;
		/// <summary>
		/// Log Meggage
		/// </summary>
		/// <param name="msg">Message to log</param>
		public delegate void LogMessage(string msg);
        /// <summary>
        /// Message is logged
        /// </summary>
        public event LogMessage Logger;
        /// <summary>
        /// Request to shutdown
        /// </summary>
        /// <param name="sender">Sender of request</param>
        public delegate void ShutdownRequestHandler(CommunityExpress sender);
        /// <summary>
        /// Shutdown is requested
        /// </summary>
        public event ShutdownRequestHandler ShutdownRequested;
       
        /// <summary>
        /// Arguments for low battery
        /// </summary>
        public class LowBatteryArgs : System.EventArgs
        {
            internal LowBatteryArgs(LowBatteryPower_t args)
            {
                MinutesBatteryLeft = args.m_nMinutesBatteryLeft;
            }
            /// <summary>
            /// Number of battery minutes left
            /// </summary>
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
        public SteamAPICall_t _lobbyRequest = 0;
        
        internal delegate void OnEventHandler<T>(T pvParam, Boolean bIOFailure, SteamAPICall_t hSteamAPICall);

        private MethodInfo _internalOnEvent = null;
        private Dictionary<int, Type> _internalEventFactory = new Dictionary<int, Type>();
        private Dictionary<int, MethodInfo> _internalOnEventFactory = new Dictionary<int, MethodInfo>();
        private Dictionary<Int32, object> _internalHandlers = new Dictionary<Int32, object>();

		private List<SteamAPICall_t> _gameserverUserStatsReceivedCallHandles = new List<SteamAPICall_t>();
		private List<OnUserStatsReceivedFromSteam> _gameserverUserStatsReceivedCallbacks = new List<OnUserStatsReceivedFromSteam>();

		private SteamAPICall_t _userGetEncryptedAppTicketCallHandle = 0;
		private OnUserGetEncryptedAppTicketFromSteam _userGetEncryptedAppTicketCallback;
        /// <summary>
        /// Invalid app ID
        /// </summary>
		public const uint k_uAppIdInvalid = 0x0;

		private CommunityExpress() { }
        /// <summary>
        /// Finalize Community Express
        /// </summary>
		~CommunityExpress() { Shutdown(); }
        /// <summary>
        /// Instance of community express running
        /// </summary>
		public static CommunityExpress Instance
		{
			get
			{
				return _instance;
			}
		}
        /// <summary>
        /// Restarts app if needed
        /// </summary>
        /// <param name="unOwnAppID">If the current user does not own the current app's ID</param>
        /// <returns>true if restart needed</returns>
		public bool RestartAppIfNecessary(uint unOwnAppID)
		{
			return SteamUnityAPI_RestartAppIfNecessary(unOwnAppID);
		}
        /// <summary>
        /// Writes a mini-dump for the app
        /// </summary>
        /// <param name="exceptionCode">Code for the exception</param>
        /// <param name="exceptionInfo">Information about the exception</param>
        /// <param name="buildID">ID of the build</param>
		public void WriteMiniDump(uint exceptionCode, IntPtr exceptionInfo, uint buildID)
		{
			SteamUnityAPI_WriteMiniDump(exceptionCode, exceptionInfo, buildID);
		}
        /// <summary>
        /// Sets a mini-dump comment in a message box
        /// </summary>
        /// <param name="comment">Comment for the message box</param>
		public void SetMiniDumpComment(String comment)
		{
			SteamUnityAPI_SetMiniDumpComment(comment);
		}
        /// <summary>
        /// Initializes the program
        /// </summary>
        /// <returns>true if initialized</returns>
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

				ValidateLicense();
                
                // startup friends so we start getting events, etc.
                _friends = new Friends(this);

				return true;
            }

			return false;
		}
        /// <summary>
        /// Log message
        /// </summary>
        /// <param name="msg">Message to log</param>
		public void Log(string msg)
		{
            if (Logger != null)
			{
                Logger(msg);
			}

			System.Diagnostics.Debug.WriteLine(msg);
		}

        private void RunUploads()
        {

            byte[] data = new byte[1024];
            List<FileStream> toRemove = new List<FileStream>();

            foreach (FileStream fs in _uploadStreams.Keys)
            {
                FileWriteStream stream = _uploadStreams[fs];

                int read = fs.Read(data, 0, 1024);
                if (read != 0)
                {
                    stream.WriteChunk(data, read);
                }
                else
                {
                    stream.Close();
                    fs.Close();

                    toRemove.Add(fs);
                }

            }

            foreach (FileStream fs in toRemove)
            {
                _uploadStreams.Remove(fs);
            }
        }
        /// <summary>
        /// Runs callbacks for uploads
        /// </summary>
		public void RunCallbacks()
		{
			SteamUnityAPI_RunCallbacks();

            RunUploads();
            Byte failed;
            if (_lobbyRequest != 0)
            {
                LobbyMatchList_t _lobbyListResult;
                if (SteamUnityAPI_SteamUtils_IsAPICallCompleted(_lobbyRequest, out failed))
                {
                    SteamUnityAPI_SteamUtils_GetLobbyListReceivedResult(_lobbyRequest, out _lobbyListResult, out failed);
                    Matchmaking.GetLobbyList(_lobbyListResult);
                }
            }

			if (_gameserver != null && _gameserver.IsInitialized)
			{
				SteamUnityAPI_SteamGameServer_RunCallbacks();

				for (int i = 0; i < _gameserverUserStatsReceivedCallHandles.Count; i++)
				{
					SteamAPICall_t h = _gameserverUserStatsReceivedCallHandles[i];

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
				if (SteamUnityAPI_SteamUtils_IsAPICallCompleted(_userGetEncryptedAppTicketCallHandle, out failed))
				{
					_userGetEncryptedAppTicketCallback();
					_userGetEncryptedAppTicketCallHandle = 0;
				}
			}
		}
        /// <summary>
        /// Shuts down the Unity API
        /// </summary>
		public void Shutdown()
		{
			// todo: make thread safe?
			if (!_shutdown)
			{
				_shutdown = true;
				SteamUnityAPI_Shutdown();
			}
		}
        /// <summary>
        /// The ID of the app
        /// </summary>
		public AppId_t AppID
		{
			get { return SteamUnityAPI_SteamUtils_GetAppID(); }
		}
        /// <summary>
        /// Gets App.cs
        /// </summary>
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
        /// <summary>
        /// Gets User.cs
        /// </summary>
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
        /// <summary>
        /// Gets GameServer.cs
        /// </summary>
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
        /// <summary>
        /// Gets Friends.cs
        /// </summary>
		public Friends Friends
		{
			get
            {
                if (_friends == null)
                {
                    _friends = new Friends(this);
                }

				return _friends;
			}
		}
        /// <summary>
        /// Gets Groups.cs
        /// </summary>
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
        /// <summary>
        /// Gets UserStats.cs
        /// </summary>
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
        /// <summary>
        /// Gets Achivements.cs
        /// </summary>
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
        /// <summary>
        /// Gets Leaderboards.cs
        /// </summary>
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
        /// <summary>
        /// Gets Matchmaking.cs
        /// </summary>
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
        /// <summary>
        /// Gets RemoteStorage.cs
        /// </summary>
		public RemoteStorage RemoteStorage
		{
			get
			{
				if (_remoteStorage == null)
				{
					_remoteStorage = new RemoteStorage(this);
				}

				return _remoteStorage;
			}
		}
        /// <summary>
        /// Gets Networking.cs
        /// </summary>
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
        /// <summary>
        /// Gets InGamePurchasing.cs
        /// </summary>
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
        /// <summary>
        /// Gets SteamWebAPI.cs
        /// </summary>
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
        /// <summary>
        /// Gets BigPicture.cs
        /// </summary>
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
        /// <summary>
        /// Gets UserGeneratedContent.cs
        /// </summary>
        public UserGeneratedContent UserGeneratedContent
        {
            get 
            {
                if (_ugc == null) {
                    _ugc = new UserGeneratedContent(this);
                }

                return _ugc;
            }
        }
        /// <summary>
        /// Checks if the GameServer is running
        /// </summary>
		public Boolean IsGameServerInitialized
		{
			get { return _gameserver != null && _gameserver.IsInitialized; }
		}
        /// <summary>
        /// Checks if Community is running
        /// </summary>
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

        private Dictionary<FileStream, FileWriteStream> _uploadStreams = new Dictionary<FileStream, FileWriteStream>();

        internal void AddFileWriterUpload(string fileName, FileWriteStream stream)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            _uploadStreams.Add(fs, stream);
        }

        private void InternalOnEvent<T>(Int32 k_iCallback, T pvParam, Boolean bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            // internal callbacks
            ((OnEventHandler<T>)_internalHandlers[k_iCallback])(pvParam, bIOFailure, hSteamAPICall);
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

            //Console.WriteLine(k_iCallback);
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

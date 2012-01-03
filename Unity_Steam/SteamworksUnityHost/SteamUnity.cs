using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
	using SteamAPICall_t = UInt64;
	using System.Reflection;
	using System.IO;
	using System.Security.Cryptography;
	using System.Security.Cryptography.Xml;
	using System.Xml;

	public sealed class CommunityExpress
	{
		[DllImport("CommunityExpressSW.dll")]
		private static extern bool SteamUnityAPI_RestartAppIfNecessary(uint unOwnAppID);
		[DllImport("CommunityExpressSW.dll")]
		private static extern bool SteamUnityAPI_Init(IntPtr OnChallengeResponse);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_RunCallbacks();
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamGameServer_RunCallbacks();
		[DllImport("CommunityExpressSW.dll")]
		private static extern UInt32 SteamUnityAPI_SteamUtils_GetAppID();
		[DllImport("CommunityExpressSW.dll")]
		private static extern Boolean SteamUnityAPI_SteamUtils_IsAPICallCompleted(SteamAPICall_t callHandle, out Byte failed);
		[DllImport("CommunityExpressSW.dll")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetGameServerUserStatsReceivedResult(SteamAPICall_t callHandle, out GSStatsReceived_t result, out Byte failed);
		[DllImport("CommunityExpressSW.dll")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetGameStatsSessionIssuedResult(SteamAPICall_t callHandle, out GameStatsSessionIssued_t result, out Byte failed);
		[DllImport("CommunityExpressSW.dll")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetLobbyCreatedResult(SteamAPICall_t callHandle, out LobbyCreated_t result, out Byte failed);
		[DllImport("CommunityExpressSW.dll")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetLobbyListReceivedResult(SteamAPICall_t callHandle, out LobbyMatchList_t result, out Byte failed);
		[DllImport("CommunityExpressSW.dll")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetLobbyEnteredResult(SteamAPICall_t callHandle, out LobbyEnter_t result, out Byte failed);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_Shutdown();

		delegate UInt64 OnChallengeResponseFromSteam(UInt64 challenge);
		private OnChallengeResponseFromSteam _challengeResponse;

		private static readonly CommunityExpress _instance = new CommunityExpress();
		private bool _shutdown = false;

		private User _user = null;
		private GameServer _gameserver = null;
		private Friends _friends = null;
		private Groups _groups = null;
		private Stats _userStats = null;
		private Achievements _achievements = null;
		private Leaderboards _leaderboards = null;
		private Matchmaking _matchmaking = null;
		private RemoteStorage _remoteStorage = null;
		private Networking _networking = null;

		private List<SteamAPICall_t> _gameserverUserStatsReceivedCallHandles = new List<SteamAPICall_t>();
		private List<OnUserStatsReceivedFromSteam> _gameserverUserStatsReceivedCallbacks = new List<OnUserStatsReceivedFromSteam>();

		private List<SteamAPICall_t> _gamestatsSessionIssuedCallHandles = new List<SteamAPICall_t>();
		private List<OnGameStatsSessionIssuedBySteam> _gamestatsSessionIssuedCallbacks = new List<OnGameStatsSessionIssuedBySteam>();

		private SteamAPICall_t _userGetEncryptedAppTicketCallHandle = 0;
		private OnUserGetEncryptedAppTicketFromSteam _userGetEncryptedAppTicketCallback;

		private SteamAPICall_t _lobbyCreatedCallHandle = 0;
		private OnMatchmakingLobbyCreatedBySteam _lobbyCreatedCallback;

		private SteamAPICall_t _lobbyListReceivedCallHandle = 0;
		private OnMatchmakingLobbyListReceivedFromSteam _lobbyListReceivedCallback;

		private SteamAPICall_t _lobbyJoinedCallHandle = 0;
		private OnMatchmakingLobbyJoinedFromSteam _lobbyJoinedCallback;

		private CommunityExpress() { }
		~CommunityExpress() { Shutdown(); }


		public const uint k_uAppIdInvalid = 0x0;

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

		public bool Initialize()
		{
			_challengeResponse = new OnChallengeResponseFromSteam(OnChallengeResponseCallback);

			if (SteamUnityAPI_Init(Marshal.GetFunctionPointerForDelegate(_challengeResponse)))
			{
				ValidateLicense();
				return true;
			}

			return false;
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

			for (int i = 0; i < _gamestatsSessionIssuedCallHandles.Count; i++)
			{
				SteamAPICall_t h = _gamestatsSessionIssuedCallHandles[i];

				Byte failed;
				if (SteamUnityAPI_SteamUtils_IsAPICallCompleted(h, out failed))
				{
					GameStatsSessionIssued_t callbackData = new GameStatsSessionIssued_t();
					SteamUnityAPI_SteamUtils_GetGameStatsSessionIssuedResult(h, out callbackData, out failed);

					_gamestatsSessionIssuedCallbacks[i](ref callbackData);
					_gamestatsSessionIssuedCallHandles.RemoveAt(i);
					_gamestatsSessionIssuedCallbacks.RemoveAt(i);

					i--;
				}
			}

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

		public bool InitalizeGameServer()
		{
			return true;
		}

		public UInt32 AppID
		{
			get { return SteamUnityAPI_SteamUtils_GetAppID(); }
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
					_gameserver = new GameServer();
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
					if (_friends == null)
					{
						_friends = new Friends();
					}

					_groups = new Groups(_friends);
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
					_userStats = new Stats();
					_userStats.Init();
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
					_achievements = new Achievements();
					_achievements.Init();
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
					_leaderboards = new Leaderboards();
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

		public GameStats CreateNewGameStats(OnGameStatsSessionInitialized onGameStatsSessionInitialized, Boolean gameserver, SteamID steamID = null)
		{
			GameStats gamestats;

			gamestats = new GameStats();

			if (gameserver)
			{
				gamestats.StartNewSession(onGameStatsSessionInitialized, EGameStatsAccountType.k_EGameStatsAccountType_SteamGameServer, steamID);
			}
			else
			{
				gamestats.StartNewSession(onGameStatsSessionInitialized, EGameStatsAccountType.k_EGameStatsAccountType_Steam, steamID);
			}

			return gamestats;
		}

		public Boolean IsGameServerInitialized
		{
			get { return _gameserver != null && _gameserver.IsInitialized; }
		}

		internal void AddGameServerUserStatsReceivedCallback(SteamAPICall_t handle, OnUserStatsReceivedFromSteam callback)
		{
			_gameserverUserStatsReceivedCallHandles.Add(handle);
			_gameserverUserStatsReceivedCallbacks.Add(callback);
		}

		internal void AddGameStatsSessionIssuedCallback(SteamAPICall_t handle, OnGameStatsSessionIssuedBySteam callback)
		{
			_gamestatsSessionIssuedCallHandles.Add(handle);
			_gamestatsSessionIssuedCallbacks.Add(callback);
		}

		internal void AddUserGetEncryptedAppTicketCallback(SteamAPICall_t handle, OnUserGetEncryptedAppTicketFromSteam callback)
		{
			_userGetEncryptedAppTicketCallHandle = handle;
			_userGetEncryptedAppTicketCallback = callback;
		}

		internal void AddCreateLobbyCallback(SteamAPICall_t handle, OnMatchmakingLobbyCreatedBySteam callback)
		{
			_lobbyCreatedCallHandle = handle;
			_lobbyCreatedCallback = callback;
		}

		internal void AddLobbyListRequestCallback(SteamAPICall_t handle, OnMatchmakingLobbyListReceivedFromSteam callback)
		{
			_lobbyListReceivedCallHandle = handle;
			_lobbyListReceivedCallback = callback;
		}

		internal void RemoveLobbyListRequestCallback(SteamAPICall_t handle, OnMatchmakingLobbyListReceivedFromSteam callback)
		{
			_lobbyListReceivedCallHandle = 0;
		}

		internal void AddLobbyJoinedCallback(SteamAPICall_t handle, OnMatchmakingLobbyJoinedFromSteam callback)
		{
			_lobbyJoinedCallHandle = handle;
			_lobbyJoinedCallback = callback;
		}

		private UInt64 OnChallengeResponseCallback(UInt64 challenge)
		{
			// Put a real functional test in here
			return (UInt64)Math.Sqrt((double)challenge);
		}

		private void ValidateLicense()
		{
			string dllPath = Assembly.GetExecutingAssembly().Location;
			string dllDirectory = Path.GetDirectoryName(dllPath);
			string licensePath = Path.Combine(dllDirectory, "CESDKLicense.xml");

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
					throw new Exception("Error: no signature found.", e);
				}

				// Verify the signature.
				if (!sxml.CheckSignature(csp))
				{
					throw new Exception(
						string.Format("Error: License '{0}' invalid.", licensePath));
				}

				try
				{
					XmlNodeList appIdNode = xmldoc.GetElementsByTagName("AppId");
					if (appIdNode == null || appIdNode.Count == 0 ||
						string.IsNullOrEmpty(appIdNode[0].InnerText))
					{
						throw new Exception("AppId missing from license");
					}

					if (appIdNode[0].InnerText != appId.ToString())
					{
						throw new Exception(
							string.Format("AppId mismatch: {0} vs {1}",
							appIdNode[0].InnerText, appId));
					}
				}
				catch (Exception e)
				{
					throw new Exception(
						string.Format("Error: License '{0}' invalid.", licensePath), e);
				}
			}
		}
	}
}

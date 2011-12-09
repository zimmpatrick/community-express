using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SteamworksUnityHost
{
	using SteamAPICall_t = UInt64;

	public sealed class SteamUnity
	{
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_RestartAppIfNecessary(uint unOwnAppID);
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_Init(IntPtr OnChallengeResponse);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_RunCallbacks();
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamGameServer_RunCallbacks();
		[DllImport("SteamworksUnity.dll")]
		private static extern UInt64 SteamUnityAPI_SteamUtils_GetAppID();
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamUtils_IsAPICallCompleted(SteamAPICall_t callHandle, out Byte failed);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetGameServerUserStatsReceivedResult(SteamAPICall_t callHandle, out GSStatsReceived_t result, out Byte failed);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetGameStatsSessionIssuedResult(SteamAPICall_t callHandle, out GameStatsSessionIssued_t result, out Byte failed);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetLobbyCreatedResult(SteamAPICall_t callHandle, out LobbyCreated_t result, out Byte failed);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetLobbyListReceivedResult(SteamAPICall_t callHandle, out LobbyMatchList_t result, out Byte failed);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_Shutdown();

        delegate UInt64 OnChallengeResponseFromSteam(UInt64 challenge);
        private OnChallengeResponseFromSteam _challengeResponse;

		private static readonly SteamUnity _instance = new SteamUnity();
		private bool shutdown = false;

		private GameServer _gameserver = null;
		private Friends _friends = null;
		private Groups _groups = null;
		private Stats _userStats = null;
		private Achievements _achievements = null;
		private Leaderboards _leaderboards = null;
		private Matchmaking _matchmaking = null;

		private List<SteamAPICall_t> _gameserverUserStatsReceivedCallHandles = new List<SteamAPICall_t>();
		private List<OnUserStatsReceivedFromSteam> _gameserverUserStatsReceivedCallbacks = new List<OnUserStatsReceivedFromSteam>();

		private List<SteamAPICall_t> _gamestatsSessionIssuedCallHandles = new List<SteamAPICall_t>();
		private List<OnGameStatsSessionIssuedBySteam> _gamestatsSessionIssuedCallbacks = new List<OnGameStatsSessionIssuedBySteam>();

		private List<SteamAPICall_t> _lobbyCreatedCallHandles = new List<SteamAPICall_t>();
		private List<OnMatchmakingLobbyCreatedBySteam> _lobbyCreatedCallbacks = new List<OnMatchmakingLobbyCreatedBySteam>();

		private List<SteamAPICall_t> _lobbyListReceivedCallHandles = new List<SteamAPICall_t>();
		private List<OnMatchmakingLobbyListReceivedFromSteam> _lobbyListReceivedCallbacks = new List<OnMatchmakingLobbyListReceivedFromSteam>();

		private SteamUnity() { }
		~SteamUnity() { Shutdown(); }

		public const uint k_uAppIdInvalid = 0x0;

		public static SteamUnity Instance
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

            return SteamUnityAPI_Init(Marshal.GetFunctionPointerForDelegate(_challengeResponse));
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

			for (int i = 0; i < _lobbyCreatedCallHandles.Count; i++)
			{
				SteamAPICall_t h = _lobbyCreatedCallHandles[i];

				Byte failed;
				if (SteamUnityAPI_SteamUtils_IsAPICallCompleted(h, out failed))
				{
					LobbyCreated_t callbackData = new LobbyCreated_t();
					SteamUnityAPI_SteamUtils_GetLobbyCreatedResult(h, out callbackData, out failed);

					_lobbyCreatedCallbacks[i](ref callbackData);
					_lobbyCreatedCallHandles.RemoveAt(i);
					_lobbyCreatedCallbacks.RemoveAt(i);

					i--;
				}
			}

			for (int i = 0; i < _lobbyListReceivedCallHandles.Count; i++)
			{
				SteamAPICall_t h = _lobbyListReceivedCallHandles[i];

				Byte failed;
				if (SteamUnityAPI_SteamUtils_IsAPICallCompleted(h, out failed))
				{
					LobbyMatchList_t callbackData = new LobbyMatchList_t();
					SteamUnityAPI_SteamUtils_GetLobbyListReceivedResult(h, out callbackData, out failed);

					_lobbyListReceivedCallbacks[i](ref callbackData);
					_lobbyListReceivedCallHandles.RemoveAt(i);
					_lobbyListReceivedCallbacks.RemoveAt(i);

					i--;
				}
			}
		}

		public void Shutdown()
		{
			// todo: make thread safe?
			if (!shutdown)
			{
				shutdown = true;
				SteamUnityAPI_Shutdown();
			}
		}

		public bool InitalizeGameServer()
		{
			return true;
		}

		public RemoteStorage RemoteStorage
		{
			get
			{
				return new RemoteStorage();
			}
		}

		public User User
		{
			get
			{
				return new User();
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

		internal void AddCreateLobbyCallback(SteamAPICall_t handle, OnMatchmakingLobbyCreatedBySteam callback)
		{
			_lobbyCreatedCallHandles.Add(handle);
			_lobbyCreatedCallbacks.Add(callback);
		}

		internal void AddLobbyListRequestCallback(SteamAPICall_t handle, OnMatchmakingLobbyListReceivedFromSteam callback)
		{
			_lobbyListReceivedCallHandles.Add(handle);
			_lobbyListReceivedCallbacks.Add(callback);
		}

		internal void RemoveLobbyListRequestCallback(SteamAPICall_t handle, OnMatchmakingLobbyListReceivedFromSteam callback)
		{
			_lobbyListReceivedCallHandles.Remove(handle);
			_lobbyListReceivedCallbacks.Remove(callback);
		}

        private UInt64 OnChallengeResponseCallback(UInt64 challenge)
        {
            // TODO: Put a real functional test in here
            return (UInt64)Math.Sqrt((double)challenge);
        }
	}
}

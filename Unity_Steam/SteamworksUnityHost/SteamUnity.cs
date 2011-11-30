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
		private static extern bool SteamUnityAPI_Init();
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
		private static extern void SteamUnityAPI_Shutdown();

		private static readonly SteamUnity _instance = new SteamUnity();
		private bool shutdown = false;

		private GameServer _gameserver = null;
		private Friends _friends = null;
		private Groups _groups = null;
		private Stats _userStats = null;
		private Achievements _achievements = null;

		private List<SteamAPICall_t> _gameserverUserStatsReceivedCallHandles = new List<SteamAPICall_t>();
		private List<OnUserStatsReceivedFromSteam> _gameserverUserStatsReceivedCallbacks = new List<OnUserStatsReceivedFromSteam>();

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
			return SteamUnityAPI_Init();
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
				return new Leaderboards();
			}
		}

		internal void AddGameServerUserStatsReceivedCallback(SteamAPICall_t handle, OnUserStatsReceivedFromSteam callback)
		{
			_gameserverUserStatsReceivedCallHandles.Add(handle);
			_gameserverUserStatsReceivedCallbacks.Add(callback);
		}
	}
}

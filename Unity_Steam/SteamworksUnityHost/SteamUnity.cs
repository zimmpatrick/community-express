using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SteamworksUnityHost
{
	public sealed class SteamUnity
	{
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_RestartAppIfNecessary(uint unOwnAppID);
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_Init();
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_RunCallbacks();
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_Shutdown();

		private static readonly SteamUnity _instance = new SteamUnity();
		private bool shutdown = false;

		private Friends _friends = null;
		private Groups _groups = null;

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
				return new GameServer();
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
				return new Stats();
			}
		}

		public Achievements UserAchievements
		{
			get
			{
				return new Achievements();
			}
		}

		public Leaderboards Leaderboards
		{
			get
			{
				return new Leaderboards();
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using SteamworksUnityHost;

namespace SteamworksUnityTest
{
	class Program
	{
		private static bool _statsReceived = false;
		private static bool _leaderboardReceived = false;

		static int Main(string[] args)
		{
			SteamUnity s = SteamUnity.Instance;

#if false
			if (s.RestartAppIfNecessary(SteamUnity.k_uAppIdInvalid))
			{
				// if Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the 
				// local Steam client and also launches this game again.

				// Once you get a public Steam AppID assigned for this game, you need to replace k_uAppIdInvalid with it and
				// removed steam_appid.txt from the game depot.
				return 1;
			}
#endif

			// Initialize SteamAPI, if this fails we bail out since we depend on Steam for lots of stuff.
			// You don't necessarily have to though if you write your code to check whether all the Steam
			// interfaces are NULL before using them and provide alternate paths when they are unavailable.
			//
			// This will also load the in-game steam overlay dll into your process.  That dll is normally
			// injected by steam when it launches games, but by calling this you cause it to always load,
			// even when not launched via steam.
			if (!s.Initialize())
			{
				System.Diagnostics.Debug.WriteLine( "SteamAPI_Init() failed" );
				throw new Exception("Steam must be running to play this game (SteamAPI_Init() failed).");
			}

			RemoteStorage r = s.RemoteStorage;
			Console.WriteLine(r.FileCount);

			User u = s.User;
			Console.WriteLine(u.LoggedOn);
			Console.WriteLine(u.SteamID);

			GameServer gs = s.GameServer;
			Console.WriteLine(gs.SteamID);

			Console.WriteLine("Friends: ");
			foreach (Friend f in s.Friends)
			{
				Console.WriteLine("{0} - {1} - {2}", f.Id, f.PersonaName, f.PersonaState);
			}

			Stats stats = s.UserStats;
			stats.RequestCurrentStats(MyOnUserStatsReceivedCallback, 
                new string[] { "Kills", "DamageHealed" } );

			while (!_statsReceived)
			{
				s.RunCallbacks();
			}

			/*/ Testing of the Stats Write functionality
			stats.StatsList[0].StatValue = (Int32)stats.StatsList[0].StatValue + 1;
			stats.WriteStats();

			_statsReceived = false;
			stats.RequestCurrentStats(MyOnUserStatsReceivedCallback,
				new string[] { "Kills", "DamageHealed" });

			while (!_statsReceived)
			{
				s.RunCallbacks();
			}
			//*/

			Achievements achievements = s.UserAchievements;
			achievements.InitializeAchievementList(new string[] { "KillEnemyUsingBloatAcid", "KillHalloweenPatriarchInBedlam", "DecapBurningHalloweenZedInBedlam", "Kill250HalloweenZedsInBedlam", "WinBedlamHardHalloween", "Kill25HalloweenScrakesInBedlam", "Kill5HalloweenZedsWithoutReload", "Unlock6ofHalloweenAchievements" });
			MyOnUserStatsReceivedCallback(null, achievements);

			Leaderboards leaderboards = s.Leaderboards;
			leaderboards.FindOrCreateLeaderboard(MyOnLeaderboardRetrievedCallback, "TestLeaderboard", ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);

			while (!_leaderboardReceived)
			{
				s.RunCallbacks();
			}

			if (r.FileCount > 0)
			{
				int fileSize;
				string name = r.GetFileNameAndSize(0, out fileSize);
				string msg = r.FileRead(name, fileSize);
				Console.WriteLine(msg);
			}

			Console.In.ReadLine();

			return 0;
		}

		public static void MyOnUserStatsReceivedCallback(Stats stats, Achievements achievements)
		{
			if (stats != null)
			{
				Console.WriteLine("Stats: ");
				foreach (Stat s in stats)
				{
					Console.WriteLine("{0} - {1} - {2} - {3} - {4}", s.StatName, s.StatValue, s.StatValue.GetType().Name,
						s.StatValue is float,
						s.StatValue is int);
				}
			}

			if (achievements != null)
			{
				Console.WriteLine("Achievements: ");
				foreach (Achievement a in achievements)
				{
					Console.WriteLine("{0} - {1}", a.AchievementName, a.IsAchieved);
				}
			}

			_statsReceived = true;
		}

		public static void MyOnLeaderboardRetrievedCallback(Leaderboard leaderboard)
		{
			_leaderboardReceived = true;

			Console.WriteLine("Leaderboard Retrieved: {0} - {1} - {2}", leaderboard.LeaderboardName, leaderboard.SortMethod, leaderboard.DisplayType);
		}
	}
}

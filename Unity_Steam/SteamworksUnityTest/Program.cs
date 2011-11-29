using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using SteamworksUnityHost;

namespace SteamworksUnityTest
{
	class Program
	{
		private static bool _statsReceived = false;
		private static bool _leaderboardEntriesReceived = false;
		private static bool _userAuthenticationCompleted = false;

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
			const UInt16 gsPort = 8793;
			if (gs.Init(0, gsPort, gsPort, 0, 27015, EServerMode.eServerModeAuthenticationAndSecure, "killingfloor", "1.0.0.0", MyOnGSClientApproved, MyOnGSClientDenied, MyOnGSClientKick))
			{
				Console.WriteLine("GameServer: {0}", gs.SteamID);
			}
			else
			{
				Console.WriteLine("GameServer Failed to Initialize");
			}

			Console.WriteLine("Friends: ");
			foreach (Friend f in s.Friends)
			{
				Console.WriteLine("  {0} - {1} - {2}", f.SteamID, f.PersonaName, f.PersonaState);
			}

			Console.WriteLine("Groups/Clans: ");
			foreach (Group g in s.Groups)
			{
				Console.WriteLine("  {0} - {1} - {2} - {3}", g.SteamID, g.ClanTag, g.GroupName, g.Count);

				foreach (Friend f in g)
				{
					Console.WriteLine("     {0} - {1} - {2}", f.SteamID, f.PersonaName, f.PersonaState);
				}
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

			while (!_leaderboardEntriesReceived)
			{
				s.RunCallbacks();
			}

			// The server would have had to send down its SteamID and its VAC status to allow the generation of the Steam Auth Ticket
			Byte[] authTicket;
			if (u.InitiateClientAuthentication(out authTicket, gs.SteamID, IPAddress.Loopback, gsPort, true))
			{
				// The client would have had to send up the authTicket and then the server would learn the client's Steam ID here
				SteamID steamIDClient;

				if (gs.ClientConnected(IPAddress.Loopback, authTicket, out steamIDClient))
				{
					while (!_userAuthenticationCompleted)
					{
						s.RunCallbacks();
					}
				}
			}

			Console.WriteLine("User Authentication Complete - Delaying 1 second before Disconnecting User");
			Thread.Sleep(1000);

			gs.ClientDisconnected(u.SteamID);
			u.OnDisconnect();

			Console.WriteLine("User Disconnected");

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

		public static void MyOnGSClientApproved(SteamID approvedPlayer)
		{
			_userAuthenticationCompleted = true;

			Console.WriteLine("OnGameServerClientApproved - {0}", approvedPlayer.ToString());
		}

		public static void MyOnGSClientDenied(SteamID deniedPlayer, EDenyReason denyReason, String optionalText)
		{
			_userAuthenticationCompleted = true;

			if (deniedPlayer == null)
			{
				Console.WriteLine("OnGameServerClientDenied - Unknown Player - {0} - {1}", denyReason, optionalText);
			}
			else
			{
				Console.WriteLine("OnGameServerClientDenied - {0} - {1} - {2}", deniedPlayer.ToString(), denyReason, optionalText);
			}
		}

		public static void MyOnGSClientKick(SteamID playerToKick, EDenyReason denyReason)
		{
			_userAuthenticationCompleted = true;

			Console.WriteLine("OnGameServerClientKick - {0} - {1}", playerToKick.ToString(), denyReason);
		}

		public static void MyOnUserStatsReceivedCallback(Stats stats, Achievements achievements)
		{
			if (stats != null)
			{
				Console.WriteLine("Stats: ");
				foreach (Stat s in stats)
				{
					Console.WriteLine("  {0} - {1} - {2} - {3} - {4}", s.StatName, s.StatValue, s.StatValue.GetType().Name,
						s.StatValue is float,
						s.StatValue is int);
				}
			}

			if (achievements != null)
			{
				Console.WriteLine("Achievements: ");
				foreach (Achievement a in achievements)
				{
					Console.WriteLine("  {0} - {1}", a.AchievementName, a.IsAchieved);
				}
			}

			_statsReceived = true;
		}

		public static void MyOnLeaderboardRetrievedCallback(Leaderboard leaderboard)
		{
			if (leaderboard != null)
			{
				Console.WriteLine("Leaderboard Retrieved: {0} - {1} - {2}", leaderboard.LeaderboardName, leaderboard.SortMethod, leaderboard.DisplayType);

				leaderboard.UploadLeaderboardScore(ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, 913, new List<Int32> { 123, 456, 789 });

				leaderboard.RequestLeaderboardEntries(0, 2, 3, MyOnLeaderboardEntriesRetrievedCallback);
			}
			else
			{
				Console.WriteLine("Failed to Retreive Leaderboard");
			}
		}

		public static void MyOnLeaderboardEntriesRetrievedCallback(LeaderboardEntries leaderboardEntries)
		{
			_leaderboardEntriesReceived = true;

			if (leaderboardEntries != null)
			{
				Console.WriteLine("  Leaderboard Entries Retrieved: {0} - {1}", leaderboardEntries.Leaderboard.LeaderboardName, leaderboardEntries.Count);

				foreach (LeaderboardEntry l in leaderboardEntries)
				{
					Console.WriteLine("    Leaderboard Entry: {0} - {1} - {2}", l.GlobalRank, l.SteamID.ToString(), l.Score);

					if (l.ScoreDetails != null)
					{
						foreach (Int32 i in l.ScoreDetails)
						{
							Console.WriteLine("      Leaderboard Entry Detail: {0}", i);
						}
					}
				}
			}
			else
			{
				Console.WriteLine("  Failed to Retreive Leaderboard Entries");
			}
		}
	}
}

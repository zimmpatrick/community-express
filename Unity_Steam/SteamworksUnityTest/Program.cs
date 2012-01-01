using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using CommunityExpressNS;

namespace SteamworksUnityTest
{
	class Program
	{
		private static bool _statsReceived = false;
		private static bool _leaderboardEntriesReceived = false;
		private static bool _userAuthenticationCompleted = false;
		//private static bool _gamestatsInitialized = false;
		private static bool _serversReceived = false;
		private static bool _lobbyCreated = false;
		private static bool _lobbyListReceived = false;
		private static bool _lobbyJoined = false;

		private static Lobby _lobby;
		private static Servers _serverList = null;

		static int Main(string[] args)
		{
			CommunityExpress s = CommunityExpress.Instance;

#if false
			if (s.RestartAppIfNecessary(CommunityExpress.k_uAppIdInvalid))
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
			Console.WriteLine("Remote Storage: Files={0} AvailableSpace={1}", r.Count, r.AvailableSpace);

			r.WriteFile("CloudTest.txt", "I has file!");
			s.RunCallbacks();

			if (r.Count > 0)
			{
				foreach (File f in r)
				{
					Console.WriteLine("  {0}: {1}", f.FileName, f.ReadFile());
				}
			}

			r.DeleteFile("CloudTest.txt");
			s.RunCallbacks();
			Console.WriteLine("  Remote Storage: Files={0} AvailableSpace={1}", r.Count, r.AvailableSpace);

			User u = s.User;
			Console.WriteLine(u.LoggedOn);
			Console.WriteLine(u.SteamID);

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
					Console.WriteLine("	 {0} - {1} - {2}", f.SteamID, f.PersonaName, f.PersonaState);
				}
			}

			// Request a "list" of servers(should only be 1)
			Matchmaking m = s.Matchmaking;

			m.CreateLobby(ELobbyType.k_ELobbyTypeInvisible, 2, MyOnLobbyCreated);
			while (!_lobbyCreated)
			{
				s.RunCallbacks();
			}

			m.RequestLobbyList(null, null, null, 0, ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide, 100, null, MyOnLobbyListReceived);
			while (!_lobbyListReceived)
			{
				s.RunCallbacks();
			}

			m.JoinLobby(_lobby, MyOnLobbyJoined);
			while (!_lobbyJoined)
			{
				s.RunCallbacks();
			}

			m.LeaveLobby();
			Console.WriteLine("Exited from Lobby");

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
			achievements.InitializeAchievementList(new string[] { "KillEnemyUsingBloatAcid", "KillHalloweenPatriarchInBedlam",
				"DecapBurningHalloweenZedInBedlam", "Kill250HalloweenZedsInBedlam", "WinBedlamHardHalloween", "Kill25HalloweenScrakesInBedlam",
				"Kill5HalloweenZedsWithoutReload", "Unlock6ofHalloweenAchievements" });
			MyOnUserStatsReceivedCallback(null, achievements);

			Leaderboards leaderboards = s.Leaderboards;
			leaderboards.FindOrCreateLeaderboard(MyOnLeaderboardRetrievedCallback, "TestLeaderboard",
				ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);

			while (!_leaderboardEntriesReceived)
			{
				s.RunCallbacks();
			}

			GameServer gs = s.GameServer;
			const UInt16 gsPort = 8793;
			if (gs.Init(false, new IPAddress(0), gsPort, gsPort, 27015, EServerMode.eServerModeAuthenticationAndSecure, "Fake Unity Server",
				"Fake Unity Spec Server", "US", "Killing Floor", "Killing Floor", "1.0.2.9", "KF-FakeMap", 2, true,
				MyOnGSClientApproved, MyOnGSClientDenied, MyOnGSClientKick))
			{
				Console.WriteLine("GameServer: {0}", gs.SteamID);
			}
			else
			{
				Console.WriteLine("GameServer Failed to Initialize");
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

			Console.WriteLine("Requesting Stats through Game Server");
			gs.RequestUserStats(u.SteamID, MyOnUserStatsReceivedCallback,
				new string[] { "Kills", "DamageHealed", "Testing1" });

			_statsReceived = false;
			while (!_statsReceived)
			{
				s.RunCallbacks();
			}

			Console.WriteLine("Requesting Achievements through Game Server");
			gs.RequestUserAchievements(u.SteamID, MyOnUserStatsReceivedCallback,
				new string[] { "KillEnemyUsingBloatAcid", "KillHalloweenPatriarchInBedlam", "DecapBurningHalloweenZedInBedlam", "Kill250HalloweenZedsInBedlam",
					"WinBedlamHardHalloween", "Kill25HalloweenScrakesInBedlam", "Kill5HalloweenZedsWithoutReload", "Unlock6ofHalloweenAchievements" });

			_statsReceived = false;
			while (!_statsReceived)
			{
				s.RunCallbacks();
			}

			// There's currently no way to test Game Stats =/
/*			GameStats gamestats = s.CreateNewGameStats(MyOnGameStatsSessionInitialized, false);
			while (!_gamestatsInitialized)
			{
				s.RunCallbacks();
			}

			if (gamestats.IsInitialized)
			{
				Console.WriteLine("Writing some fake Game Stats");

				gamestats.AddSessionValue("FakeIntValue", 100);
				gamestats.AddSessionValue("FakeInt64Value", UInt64.MaxValue - 10);
				gamestats.AddSessionValue("FakeFloatValue", 1.013f);
				gamestats.AddSessionValue("FakeStringValue", "Stringy");
				UInt64 rowId = gamestats.CreateNewRow("FakeTable");
				gamestats.AddRowValue(rowId, "FakeIntValue", 100);
				gamestats.AddRowValue(rowId, "FakeInt64Value", UInt64.MaxValue - 10);
				gamestats.AddRowValue(rowId, "FakeFloatValue", 1.013f);
				gamestats.AddRowValue(rowId, "FakeStringValue", "Stringy");
				gamestats.CommitRow(rowId);
				gamestats.EndCurrentSession();

				Console.WriteLine("Done writing fake Game Stats");
			}
*/
			gs.ServerName = "Updated KF Server";

			SteamID bot = gs.AddBot();

			gs.UpdateUserDetails(bot, "FakeBot", 68);
			gs.UpdateUserDetails(u.SteamID, "FakePlayer", 69);

			// Give the system plenty of time to get the server into the server list
			Thread.Sleep(1000);
			s.RunCallbacks();
			Thread.Sleep(1000);
			s.RunCallbacks();

			Console.WriteLine("Requesting Server List(Only our server will show details):");
			m.RequestInternetServerList(null, MyOnServerReceivedCallback, MyOnServerListReceivedCallback);
			while (!_serversReceived)
			{
				s.RunCallbacks();
			}

			Console.WriteLine("");
			Console.WriteLine("Server List Complete! Server Count: {0}", _serverList.Count);

			Console.WriteLine("Delaying 1 second before Disconnecting User");
			Thread.Sleep(1000);

			gs.ClientDisconnected(u.SteamID);
			u.OnDisconnect();

			Console.WriteLine("User Disconnected");

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
					Console.WriteLine("	Leaderboard Entry: {0} - {1} - {2}", l.GlobalRank, l.SteamID.ToString(), l.Score);

					if (l.ScoreDetails != null)
					{
						foreach (Int32 i in l.ScoreDetails)
						{
							Console.WriteLine("	  Leaderboard Entry Detail: {0}", i);
						}
					}
				}
			}
			else
			{
				Console.WriteLine("  Failed to Retreive Leaderboard Entries");
			}
		}

/*		public static void MyOnGameStatsSessionInitialized(GameStats gamestats)
		{
			if (gamestats != null)
			{
				Console.WriteLine("GameStats object Initialized");
			}
			else
			{
				Console.WriteLine("GameStats object Initialization failed");
			}

			_gamestatsInitialized = true;
		}
*/
		public static void MyOnLobbyCreated(Lobby lobby)
		{
			Console.WriteLine("Lobby Created {0}", lobby.SteamID);
			_lobby = lobby;
			_lobbyCreated = true;
		}

		public static void MyOnLobbyListReceived(Lobbies lobbyList)
		{
			Console.WriteLine("Lobby List Received:");

			foreach (Lobby l in lobbyList)
			{
				Console.WriteLine("  Lobby: {0}", l.SteamID);
			}

			_lobbyListReceived = true;
		}

		public static void MyOnLobbyJoined(Lobby lobby, EChatRoomEnterResponse chatRoomEnterResponse)
		{
			Console.WriteLine("Lobby Joined {0}", lobby.SteamID);
			_lobbyJoined = true;
		}

		public static void MyOnServerReceivedCallback(Servers serverList, Server server)
		{
			_serverList = serverList;

			if (server.MapName != "KF - FakeMap")
				Console.Write(".");
			else
			{
				Console.WriteLine("");
				Console.WriteLine("  {0} - {1} - {2}/{3}", server.ServerName, server.IP, server.Players, server.MaxPlayers);
			}
		}

		public static void MyOnServerListReceivedCallback(Servers serverList)
		{
			_serverList = serverList;
			_serversReceived = true;
		}
	}
}

// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

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
		private static bool _inGamePurchaseCompleted = false;
		private static bool _userAuthenticationCompleted = false;
		//private static bool _gamestatsInitialized = false;
		private static bool _serversReceived = false;
		private static bool _lobbyCreated = false;
		private static bool _lobbyListReceived = false;
		private static bool _lobbyJoined = false;
		private static bool _packetReceived = false;

		private static Lobby _lobby;
		private static Servers _serverList = null;

		private const String _webAPIKey = "6477773857A981BC6F4F50D7CAFD59E4";

		static int Main(string[] args)
		{
			CommunityExpress cesdk = CommunityExpress.Instance;

#if false
			if (cesdk.RestartAppIfNecessary(CommunityExpress.k_uAppIdInvalid))
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
			if (!cesdk.Initialize())
			{
				System.Diagnostics.Debug.WriteLine( "SteamAPI_Init() failed" );
				throw new Exception("Steam must be running to play this game (SteamAPI_Init() failed).");
			}

            Console.WriteLine("Signed in as: {0}", cesdk.User.PersonaName);
           
            Image image = cesdk.User.SmallAvatar;
            if (image != null)
                Console.WriteLine("Small: {0}x{1} ({2})", image.Width, image.Height, image.AsBytes().Length);
         
            image = cesdk.User.MediumAvatar;
            if (image != null)
                Console.WriteLine("Medium: {0}x{1} ({2})", image.Width, image.Height, image.AsBytes().Length);
				
			RemoteStorage remoteStorage = cesdk.RemoteStorage;
			Console.WriteLine("Remote Storage: Files={0} AvailableSpace={1}", remoteStorage.Count, remoteStorage.AvailableSpace);

			remoteStorage.WriteFile("CloudTest.txt", "I has file!");
			cesdk.RunCallbacks();

			if (remoteStorage.Count > 0)
			{
				foreach (File f in remoteStorage)
				{
					Console.WriteLine("  {0}: {1}", f.FileName, f.ReadFile());
				}
			}

			remoteStorage.DeleteFile("CloudTest.txt");
			cesdk.RunCallbacks();
			Console.WriteLine("  Remote Storage: Files={0} AvailableSpace={1}", remoteStorage.Count, remoteStorage.AvailableSpace);

			User user = cesdk.User;
			Console.WriteLine(user.LoggedOn);
			Console.WriteLine(user.SteamID);

			Console.WriteLine("Friends: ");
			foreach (Friend friend in cesdk.Friends)
			{
				Console.WriteLine("  {0} - {1} - {2}", friend.SteamID, friend.PersonaName, friend.PersonaState);
			}

			Console.WriteLine("Groups/Clans: ");
			foreach (Group group in cesdk.Groups)
			{
				Console.WriteLine("  {0} - {1} - {2} - {3}", group.SteamID, group.ClanTag, group.GroupName, group.Count);

				foreach (Friend friend in group)
				{
					Console.WriteLine("	 {0} - {1} - {2}", friend.SteamID, friend.PersonaName, friend.PersonaState);
				}
			}

			// Request a "list" of servers(should only be 1)
			Matchmaking matchmaking = cesdk.Matchmaking;

			matchmaking.CreateLobby(ELobbyType.k_ELobbyTypeInvisible, 2, MyOnLobbyCreated);
			while (!_lobbyCreated)
			{
				cesdk.RunCallbacks();
			}

			matchmaking.RequestLobbyList(null, null, null, 0, ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide, 100, null, MyOnLobbyListReceived);
			while (!_lobbyListReceived)
			{
				cesdk.RunCallbacks();
			}

			matchmaking.JoinLobby(_lobby, MyOnLobbyJoined);
			while (!_lobbyJoined)
			{
				cesdk.RunCallbacks();
			}

			matchmaking.LeaveLobby();
			Console.WriteLine("Exited from Lobby");

			Stats stats = cesdk.UserStats;
			stats.RequestCurrentStats(MyOnUserStatsReceivedCallback, 
				new string[] { "Kills", "DamageHealed" } );

			while (!_statsReceived)
			{
				cesdk.RunCallbacks();
			}

			/*/ Testing of the Stats Write functionality
			stats.StatsList[0].StatValue = (Int32)stats.StatsList[0].StatValue + 1;
			stats.WriteStats();

			_statsReceived = false;
			stats.RequestCurrentStats(MyOnUserStatsReceivedCallback,
				new string[] { "Kills", "DamageHealed" });

			while (!_statsReceived)
			{
				cesdk.RunCallbacks();
			}
			//*/

			Achievements achievements = cesdk.UserAchievements;
			achievements.InitializeAchievementList(new string[] { "KillEnemyUsingBloatAcid", "KillHalloweenPatriarchInBedlam",
				"DecapBurningHalloweenZedInBedlam", "Kill250HalloweenZedsInBedlam", "WinBedlamHardHalloween", "Kill25HalloweenScrakesInBedlam",
				"Kill5HalloweenZedsWithoutReload", "Unlock6ofHalloweenAchievements" });
			MyOnUserStatsReceivedCallback(null, achievements);

			Leaderboards leaderboards = cesdk.Leaderboards;
			leaderboards.FindOrCreateLeaderboard(MyOnLeaderboardRetrievedCallback, "TestLeaderboard",
				ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);

			while (!_leaderboardEntriesReceived)
			{
				cesdk.RunCallbacks();
			}

			Networking networking = cesdk.Networking;
			networking.Init(true, null, null, MyOnP2PPacketReceived);

			Console.WriteLine("Sending P2P Packet To Self");
			networking.SendP2PPacket(user.SteamID, "Blah Blah Blah", EP2PSend.k_EP2PSendReliable);

			while (!_packetReceived)
			{
				cesdk.RunCallbacks();
			}

			// NOTE: It is suggested that games call NewPurchase when the user enters the store rather than waiting until they are
			//       ready to check out, as info about the user must be fetched from Steam's server before a purchase can be completed
			Console.WriteLine("Creating fake in game purchase");
			InGamePurchase purchase = cesdk.InGamePurchasing.NewPurchase(true, _webAPIKey, (UInt64)new Random().Next());
			Console.WriteLine("Sleeping to mimic user perusing our store(OrderID: " + purchase.OrderID.ToString() + ")");
			Thread.Sleep(5000);
			purchase.AddItem(104, 1, 0.99, "Fake item description", "Fake category");
			purchase.AddItem(104, 10, 0.50, "Fake item 2", "Fake category 2");
			Console.WriteLine("Starting purchase process");
			if (cesdk.InGamePurchasing.StartPurchase(purchase, MyOnInGamePurchaseComplete))
			{
				while (!_inGamePurchaseCompleted)
				{
					cesdk.RunCallbacks();
				}
			}

			GameServer gameserver = cesdk.GameServer;
			const UInt16 gsPort = 8793;
			if (gameserver.Init(false, new IPAddress(0), gsPort, gsPort + 1, 27015, gsPort, EServerMode.eServerModeAuthenticationAndSecure, "Fake Unity Server",
				"Fake Unity Spec Server", "US", "Killing Floor", "Killing Floor", "1.0.2.9", "KF-FakeMap", 2, true,
				MyOnGSClientApproved, MyOnGSClientDenied, MyOnGSClientKick))
			{
				Console.WriteLine("GameServer: {0}", gameserver.SteamID);
			}
			else
			{
				Console.WriteLine("GameServer Failed to Initialize");
			}

			// The server would have had to send down its SteamID and its VAC status to allow the generation of the Steam Auth Ticket
			Byte[] authTicket;
			if (user.InitiateClientAuthentication(out authTicket, gameserver.SteamID, IPAddress.Loopback, gsPort, true))
			{
				// The client would have had to send up the authTicket and then the server would learn the client's Steam ID here
				SteamID steamIDClient;

				if (gameserver.ClientConnected(IPAddress.Loopback, authTicket, out steamIDClient))
				{
					while (!_userAuthenticationCompleted)
					{
						cesdk.RunCallbacks();
					}
				}
			}

			Console.WriteLine("Requesting Stats through Game Server");
			gameserver.RequestUserStats(user.SteamID, MyOnUserStatsReceivedCallback,
				new string[] { "Kills", "DamageHealed", "Testing1" });

			_statsReceived = false;
			while (!_statsReceived)
			{
				cesdk.RunCallbacks();
			}

			Console.WriteLine("Requesting Achievements through Game Server");
			gameserver.RequestUserAchievements(user.SteamID, MyOnUserStatsReceivedCallback,
				new string[] { "KillEnemyUsingBloatAcid", "KillHalloweenPatriarchInBedlam", "DecapBurningHalloweenZedInBedlam", "Kill250HalloweenZedsInBedlam",
					"WinBedlamHardHalloween", "Kill25HalloweenScrakesInBedlam", "Kill5HalloweenZedsWithoutReload", "Unlock6ofHalloweenAchievements" });

			_statsReceived = false;
			while (!_statsReceived)
			{
				cesdk.RunCallbacks();
			}

			// There's currently no way to test Game Stats =/
/*			GameStats gamestats = cesdk.CreateNewGameStats(MyOnGameStatsSessionInitialized, false);
			while (!_gamestatsInitialized)
			{
				cesdk.RunCallbacks();
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
			gameserver.ServerName = "Updated KF Server";

			SteamID bot = gameserver.AddBot();

			gameserver.UpdateUserDetails(bot, "FakeBot", 68);
			gameserver.UpdateUserDetails(user.SteamID, "FakePlayer", 69);

			// Give the system plenty of time to get the server into the server list
			Thread.Sleep(1000);
			cesdk.RunCallbacks();
			Thread.Sleep(1000);
			cesdk.RunCallbacks();

			Console.WriteLine("Requesting Server List(Only our server will show details):");
			Dictionary<string, string> filters = new Dictionary<string, string>();
			filters.Add("gameName", "EON");
			matchmaking.RequestInternetServerList(filters, MyOnServerReceivedCallback, MyOnServerListReceivedCallback);
			while (!_serversReceived)
			{
				cesdk.RunCallbacks();
			}

			Console.WriteLine("");
			Console.WriteLine("Server List Complete! Server Count: {0}", _serverList.Count);

			Console.WriteLine("Delaying 1 second before Disconnecting User");
			Thread.Sleep(1000);

			gameserver.ClientDisconnected(user.SteamID);
			user.OnDisconnect();

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
					Byte[] iconData = a.IconData;
					if (iconData != null)
						Console.WriteLine("  {0} - {1} - {2}x{3}({4}) - {5}: {6}", a.AchievementName, a.IsAchieved, a.IconWidth, a.IconHeight, iconData.Length, a.DisplayName, a.DisplayDescription);
					else
						Console.WriteLine("  {0} - {1} - {2}: {3}", a.AchievementName, a.IsAchieved, a.DisplayName, a.DisplayDescription);
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

		public static void MyOnInGamePurchaseComplete(InGamePurchase purchase, bool success)
		{
			Console.WriteLine("In Game Purchase(" + purchase.OrderID.ToString() +") Complete: " + success.ToString());
			_inGamePurchaseCompleted = true;
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

		public static void MyOnP2PPacketReceived(SteamID steamID, Byte[] data, Int32 channel)
		{
			//Console.WriteLine("  Packet Received from {0}: {1}", steamID.ToString(), System.Text.Encoding.ASCII.GetString(data));

			_packetReceived = true;
		}
	}
}

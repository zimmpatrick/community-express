// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using CommunityExpressNS;
using System.Linq;

namespace SteamworksUnityTest
{
	class Program
	{
		private static bool _largeAvatarReceived = false;
		private static bool _statsReceived = false;
		private static bool _leaderboardEntriesReceived = false;
		private static bool _inGamePurchaseCompleted = false;
		private static bool _userAuthenticationCompleted = false;
		private static bool _serversReceived = false;
		private static bool _lobbyCreated = false;
		private static bool _lobbyListReceived = false;
		private static bool _lobbyJoined = false;
		private static bool _packetReceived = false;

		private static Lobby _lobby;
		private static Servers _serverList = null;

		private const String _webAPIKey = "6477773857A981BC6F4F50D7CAFD59E4";

        static private void onLog(string msg)
        {
            Console.WriteLine(msg);
        }

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
            cesdk.Logger += new CommunityExpress.LogMessage(onLog);

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

            Console.WriteLine("Signed in as: {0} {1} {2}", cesdk.User.PersonaName, cesdk.User.LoggedOn,
                cesdk.User.SteamLevel, cesdk.User.GetGameBadgeLevel(1, false));

            Stats stats = cesdk.UserStats;

            User user = cesdk.User;

            Matchmaking matchmaking = cesdk.Matchmaking;

            cesdk.Friends.PersonaStateChange += new Friends.PersonaStateChangeHandler(Friends_PersonaStateChange);


            cesdk.User.Authentication.AuthSessionTicketResponseReceived += new OnAuthSessionTicketResponseReceived(Authentication_AuthSessionTicketResponseReceived);

            SessionTicket st = cesdk.User.Authentication.GetAuthSessionTicket();
            if (st != null) st.Cancel();

            st = cesdk.User.Authentication.GetAuthSessionTicket();


            Console.WriteLine("Friends: ");
            foreach (Friend friend in cesdk.Friends)
            {
                if (friend.PersonaState == EPersonaState.EPersonaStateOffline)
                {
                    Console.WriteLine("{0,20} ({1}) is Offline", friend.PersonaName, friend.SteamID);                
                }
                else
                {
                    Console.WriteLine("{0,20} ({1}) is Online  ({2})", friend.PersonaName, friend.SteamID, friend.PersonaState);
                }
            }

            foreach (Friend f in cesdk.Friends)
            {
                Console.WriteLine("{0}: {2}/{1}", f.PersonaName, 
                    f.GamePlayed.GameID.AppID, f.GamePlayed.GameID.Type); 
            }

            /*

            cesdk.UserStats.UserStatsReceived += (Stats sender, Stats.UserStatsReceivedArgs e) =>
            {
                Console.WriteLine("woot!");
                _statsReceived = true;
            };

            cesdk.UserStats.UserStatsStored += (Stats sender, Stats.UserStatsStoredArgs e) =>
            {
                Console.WriteLine("woot!");
                _statsReceived = true;
            };

            stats.RequestCurrentStats(
                new string[] { "Kills", "DamageHealed", "TotalZedTime" },
                new Type[] { typeof(int), typeof(int), typeof(float) });

            while (!_statsReceived)
            {
                cesdk.RunCallbacks();
            }

            _statsReceived = false;
            stats.StatsList[2].StatValue = 3.1f;
            stats.WriteStats();

            while (!_statsReceived)
            {
                cesdk.RunCallbacks();
            }


            cesdk.BigPicture.GamepadTextInputDismissed += new BigPicture.OnGamepadTextInputDismissed(MyOnGamepadTextInputDismissed);
            cesdk.BigPicture.ShowGamepadTextInput(EGamepadTextInputMode.k_EGamepadTextInputModeNormal, EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, "Tell Me!", 255);


			Image image = cesdk.User.SmallAvatar;
			if (image != null)
			{
				Console.WriteLine("Small: {0}x{1} ({2})", image.Width, image.Height, image.AsBytes().Length);
			}

			image = cesdk.User.MediumAvatar;
			if (image != null)
			{
				Console.WriteLine("Medium: {0}x{1} ({2})", image.Width, image.Height, image.AsBytes().Length);
			}

            cesdk.UserGeneratedContent.EnumeratePublishedWorkshopFiles (UserGeneratedContent.EWorkshopEnumerationType.k_EWorkshopEnumerationTypeTrending,
                                                                        0, 50, 30,
                                                                        null,
                                                                        null);

           cesdk.User.GetLargeAvatar(MyOnUserLargeAvatarReceived);
			while (!_largeAvatarReceived)
			{
				cesdk.RunCallbacks();
			}
			image = cesdk.User.LargeAvatar;
			if (image != null)
			{
				Console.WriteLine("Large: {0}x{1} ({2})", image.Width, image.Height, image.AsBytes().Length);
			}


            SessionTicket st = cesdk.User.Authentication.GetAuthSessionTicket(null);
            if (st != null) st.Cancel();

            st = cesdk.User.Authentication.GetAuthSessionTicket(null);
            

			RemoteStorage remoteStorage = cesdk.RemoteStorage;
			Console.WriteLine("Remote Storage: Files={0} AvailableSpace={1} {2} {3}", remoteStorage.Count, remoteStorage.AvailableSpace, remoteStorage.FileExists("tits.asd"), remoteStorage.GetFileSize("titty"));
			remoteStorage.WriteFile("CloudTest.txt", "I has file!");
			cesdk.RunCallbacks();

            uint appID;
            bool isAvailable;
            string DLCName;

            cesdk.App.GetDLCDataByIndex(0, out appID, out isAvailable, out DLCName, 255);

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

			Console.WriteLine(user.LoggedOn);
			Console.WriteLine(user.SteamID);

			

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

			matchmaking.JoinLobby(_lobby, MyOnLobbyJoined, MyOnLobbyDataUpdated, MyOnLobbyChatUpdated, MyOnLobbyChatMessage, MyOnLobbyGameCreated);
			while (!_lobbyJoined)
			{
				cesdk.RunCallbacks();
			}

			matchmaking.LeaveLobby();
			Console.WriteLine("Exited from Lobby");

			stats = cesdk.UserStats;
			stats.RequestCurrentStats(
				new string[] { "Kills", "DamageHealed", "TotalZedTime" },
                new Type[] { typeof(int), typeof(int), typeof(float) } );

			while (!_statsReceived)
			{
				cesdk.RunCallbacks();
			}

			// Testing of the Stats Write functionality
			stats.StatsList[2].StatValue = 2.3f;
			stats.WriteStats();

			_statsReceived = false;
			stats.RequestCurrentStats(
                new string[] { "Kills", "DamageHealed", "TotalZedTime" },
                new Type[] { typeof(int), typeof(int), typeof(float) } );
            
			while (!_statsReceived)
			{
				cesdk.RunCallbacks();
            }
            */
            _statsReceived = false;
            
            CommunityExpress.Instance.Leaderboards.LeaderboardReceived += new Leaderboards.LeaderboardRetrievedHandler(OnReceiveLeaderboard);
           // CommunityExpress.Instance.Leaderboards.FindLeaderboard("Pong Lead");
            
           // cesdk.UserGeneratedContent.EnumerateUserSharedWorkshopFiles(new SteamID(76561197975509070), 0, null, null);
            
            /// BEGIN PUBLISH
            //cesdk.RemoteStorage.AsyncWriteUpload(@"C:\Program Files (x86)\Steam\steamapps\common\Guncraft\Content\Maps\Winterfell.png", @"bacon.png");
            //cesdk.RemoteStorage.AsyncWriteUpload(@"C:\Program Files (x86)\Steam\steamapps\common\Guncraft\Content\Maps\Winterfell.png", @"Content\Maps\Winterfell.png");

            cesdk.RemoteStorage.FileWriteStreamClosed += (RemoteStorage sender, RemoteStorage.FileWriteStreamCloseArgs e) =>
            {
                cesdk.RemoteStorage.FileShare(e.FileWriteStream.FileName);
            };

            cesdk.UserGeneratedContent.FileProgress += (UserGeneratedContent sender, float progress) =>
            {
                Console.WriteLine("Program Progress " + progress);
            };

            cesdk.RemoteStorage.FileShared += (RemoteStorage sender, RemoteStorage.RemoteStorageFileShareResultArgs e) =>
            {
                if (e.RemainingFiles == 0)
                {
                    Console.WriteLine("Publishing File");

                    cesdk.UserGeneratedContent.PublishWorkshopFile(@"bacon.png",
                        @"bacon.png",
                        241720,
                        "Abduction - Workshop Level",
                        "This is Abduction, we have toast here too",
                        UserGeneratedContent.ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityFriendsOnly,
                        new string[] { "Map", "Tower" },
                        UserGeneratedContent.EWorkshopFileType.k_EWorkshopFileTypeCommunity);
                     
                    //
                }
            };

            cesdk.UserGeneratedContent.EnumerateUserSubscribedFiles(0);
            //cesdk.UserGeneratedContent.EnumeratePublishedWorkshopFiles(UserGeneratedContent.EWorkshopEnumerationType.k_EWorkshopEnumerationTypeTrending, 0, 50, 10, null, null);
            /// END PUBLISH
            //cesdk.Friends.ActivateGameOverlay(EGameOverlay.EGameOverlayFriends);

            cesdk.Friends.GameOverlayActivated += (bool isActivated) =>
            {
                Console.WriteLine("Callback received:: Activated = " + isActivated);
            };

            /// BEGIN SEARCH
            cesdk.UserGeneratedContent.EnumerateFileDetails += (UserGeneratedContent sender, UserGeneratedContent.EnumeratePublishedFileResultArgs e) =>
            {
                foreach (UserGeneratedContent.PublishedFile p in e.PublishedFiles)
                {
                    Console.WriteLine("  " + p.Description);
                    if (p.Tags.Contains("Map"))
                    {
                        /*
                        cesdk.UserGeneratedContent.UpdatePublishedFile(p.ID,
                            null,
                            @"Content\Maps\Valley.png",
                            "New Title",
                            "New Description",
                            null,
                            null);
                        */
                    }
                }
                if (e.PublishedFiles.Count == 0)
                {
                    Console.WriteLine("No Subscribed Files");
                }
            };

            cesdk.UserGeneratedContent.FileDetails += (UserGeneratedContent sender, UserGeneratedContent.PublishedFileDetailsResultArgs result) =>
            {
                Console.WriteLine("Got file details : " + result.PublishedFile.ID);
            };

            cesdk.UserGeneratedContent.FileSubscribed += (UserGeneratedContent sender, UserGeneratedContent.PublishedFileResultArgs result) =>
            {
                Console.WriteLine("File Subscribed : " + result);
                cesdk.UserGeneratedContent.GetPublishedFileDetails(result.PublishedFileId);
            };

            cesdk.UserGeneratedContent.FileUnsubscribed += (UserGeneratedContent sender, UserGeneratedContent.PublishedFileResultArgs result) =>
            {
                Console.WriteLine("File Unsubscribed : " + result);
            };

            cesdk.UserGeneratedContent.FileUpdated += (UserGeneratedContent Sender, UserGeneratedContent.PublishedFileResultArgs result) =>
            {
                Console.WriteLine("File Updated : " + result);

                cesdk.UserGeneratedContent.GetPublishedFileDetails(result.PublishedFileId);
            };

            cesdk.UserGeneratedContent.PublishedPreviewFileDownloaded += (UserGeneratedContent sender, UserGeneratedContent.PublishedFileDownloadResultArgs pf) =>
            {
                // add to UI
                Console.WriteLine(pf.PublishedFile.Title + ": " + pf.FileName);
            };

            cesdk.UserGeneratedContent.PublishedFileDownloaded += (UserGeneratedContent sender, UserGeneratedContent.PublishedFileDownloadResultArgs pf) =>
            {
                // ready to play!
                Console.WriteLine(pf.FileName);
            };

            /// END SEARCH

            while (true)
            {
                cesdk.RunCallbacks();
            }

         //   cesdk.UserGeneratedContent.UserFiles += { PublishedFile p)
         //   {
         //       p.Download("Maps/wrgfre.map");
          //  }

           
            //cesdk.UserGeneratedContent.EnumerateUserSharedWorkshopFiles(new SteamID(76561197975509070), 0, null, null);

            while (!_statsReceived)
            {
                cesdk.RunCallbacks();
            }
			
            cesdk.UserGeneratedContent.EnumeratePublishedWorkshopFiles( UserGeneratedContent.EWorkshopEnumerationType.k_EWorkshopEnumerationTypeTrending, 
                0, 50, 10, null, null);

            while (!_statsReceived)
            {
                cesdk.RunCallbacks();
            }
			

			Achievements achievements = cesdk.UserAchievements;
			achievements.InitializeAchievementList(new string[] { "KillEnemyUsingBloatAcid", "KillHalloweenPatriarchInBedlam",
				"DecapBurningHalloweenZedInBedlam", "Kill250HalloweenZedsInBedlam", "WinBedlamHardHalloween", "Kill25HalloweenScrakesInBedlam",
				"Kill5HalloweenZedsWithoutReload", "Unlock6ofHalloweenAchievements" });
			MyOnUserStatsReceivedCallback(null, achievements);

			Leaderboards leaderboards = cesdk.Leaderboards;

            leaderboards.LeaderboardReceived += new Leaderboards.LeaderboardRetrievedHandler(MyOnLeaderboardRetrievedCallback);
            leaderboards.FindLeaderboard("TestLeaderboard");
           
			while (!_leaderboardEntriesReceived)
			{
				cesdk.RunCallbacks();
			}

			/*
			Networking networking = cesdk.Networking;
			networking.Init(true, null, null, MyOnP2PPacketReceived);

			Console.WriteLine("Sending P2P Packet To Self");
			networking.SendP2PPacket(user.SteamID, "Blah Blah Blah", EP2PSend.k_EP2PSendReliable);

			while (!_packetReceived)
			{
				cesdk.RunCallbacks();
			}

			*/

			/*

			// NOTE: It is suggested that games call NewPurchase when the user enters the store rather than waiting until they are
			//	   ready to check out, as info about the user must be fetched from Steam's server before a purchase can be completed
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
			
			*/
			
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

                    foreach (Friend friend in gameserver.GetPlayersConnected())
                    {
                        Console.WriteLine(friend.PersonaName);
                    }
                }
            }

			/*
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

			gameserver.ServerName = "Updated KF Server";

			SteamID bot = gameserver.AddBot();

			gameserver.UpdateUserDetails(bot, "FakeBot", 68);
			gameserver.UpdateUserDetails(user.SteamID, "FakePlayer", 69);

			// Give the system plenty of time to get the server into the server list
			Thread.Sleep(1000);
			cesdk.RunCallbacks();
			Thread.Sleep(1000);
			cesdk.RunCallbacks();
			*/

			Console.WriteLine("Requesting Server List(Only our server will show details):");
			Dictionary<string, string> filters = new Dictionary<string, string>();
			// filters.Add("gamedir", "EON");
			//filters.Add("secure", "1");
			filters.Add("secure", "1");

            matchmaking.ServerReceived += new Matchmaking.OnServerReceivedHandler(MyOnServerReceivedCallback);
            matchmaking.ServerListReceived += new Matchmaking.OnServerListReceivedHandler(MyOnServerListReceivedCallback);

			matchmaking.RequestInternetServerList(filters);
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

        static void Friends_PersonaStateChange(Friends sender, SteamID ID, int ChangeFlags, bool result)
        {
            Friend f = sender.GetFriendBySteamID(ID);
            Console.WriteLine("{0}: {1}", f.PersonaName, f.PersonaState);
        }

        public static void OnReceiveLeaderboard(Leaderboards board, Leaderboard l)
        {
            CommunityExpress.Instance.Leaderboards.LeaderboardReceived -= OnReceiveLeaderboard;
            if (l != null)
            {
                l.LeaderboardEntriesReceived += OnEntriesReceived;
                l.RequestFriendLeaderboardEntries(5);
            }
            Console.WriteLine("callback retrieved");
           // l.RequestFriendLeaderboardEntries(5, MyOnLeaderboardEntriesRetrievedCallback);
        }

        public static void OnEntriesReceived(Leaderboard board, LeaderboardEntries l)
        {
            //CommunityExpress.Instance.Leaderboards.LeaderboardReceived -= OnEntriesReceived;
            if (l != null)
            {
                
            }
            Console.WriteLine("callback retrieved");
            // l.RequestFriendLeaderboardEntries(5, MyOnLeaderboardEntriesRetrievedCallback);
        }

        static void Authentication_AuthSessionTicketResponseReceived(User user, SessionTicket st)
        {
            Console.WriteLine(st.AutheticationTicket);
        }

        public static void MyOnGamepadTextInputDismissed(Boolean submitted, String text)
        {
        }

		public static void MyOnUserLargeAvatarReceived(SteamID steamID, Image avatar)
		{
			_largeAvatarReceived = true;

			Console.WriteLine("OnUserLargeAvatarReceived - {0}", steamID.ToString());
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

		public static void MyOnLeaderboardRetrievedCallback(Leaderboards leaderboards, Leaderboard leaderboard)
		{
			if (leaderboard != null)
			{
				Console.WriteLine("Leaderboard Retrieved: {0} - {1} - {2} - {3}", leaderboard.LeaderboardName, leaderboard.EntryCount, leaderboard.SortMethod, leaderboard.DisplayType);

				leaderboard.UploadLeaderboardScore(ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, 913, new List<Int32> { 123, 456, 789 });

				//leaderboard.RequestLeaderboardEntries(0, 2, 3, MyOnLeaderboardEntriesRetrievedCallback);
			}
			else
            {
                _leaderboardEntriesReceived = true;
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

		public static void MyOnLobbyDataUpdated(Lobby lobby, SteamID user, Boolean success)
		{
		}

		public static void MyOnLobbyChatUpdated(Lobby lobby, SteamID user, SteamID changer, EChatMemberStateChange memberStateChange)
		{
		}

		public static void MyOnLobbyChatMessage(Lobby lobby, SteamID user, EChatEntryType type, Byte[] data)
		{
		}

		public static void MyOnLobbyGameCreated(Lobby lobby, SteamID server, IPAddress ip, UInt16 port)
		{
		}

		public static void MyOnServerReceivedCallback(Matchmaking m, Servers serverList, Server server)
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

        public static void MyOnServerListReceivedCallback(Matchmaking m, Servers serverList)
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

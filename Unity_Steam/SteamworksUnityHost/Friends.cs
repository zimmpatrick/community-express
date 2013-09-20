﻿// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
    using AppId_t = UInt32;
    using SteamAPICall_t = UInt64;
    using SteamID_t = UInt64;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct GameOverlayActivated_t
    {
        internal const int k_iCallback = Events.k_iSteamFriendsCallbacks + 31;
	    
        [MarshalAs(UnmanagedType.U1)]
        internal bool m_bActive;	// true if it's just been activated, false otherwise
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct AvatarImageLoaded_t
    {
        internal const int k_iCallback = Events.k_iSteamFriendsCallbacks + 34;
        internal SteamID_t m_steamID; // steamid the avatar has been loaded for
        internal int m_iImage; // the image index of the now loaded image
        internal int m_iWide; // width of the loaded image
        internal int m_iTall; // height of the loaded image
    };
    
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct PersonaStateChange_t
    {
        internal const int k_iCallback = Events.k_iSteamFriendsCallbacks + 4;

        internal SteamID_t m_ulSteamID;		// steamID of the friend who changed
        internal int m_nChangeFlags;		// what's changed
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct FriendGameInfo_t
    {
        internal UInt64 m_gameID;
        internal UInt32 m_unGameIP;
        internal UInt16 m_usGamePort;
        internal UInt16 m_usQueryPort;
        internal SteamID_t m_steamIDLobby;
    };

    internal struct FriendRichPresenceUpdate_t
    {
	    internal const int k_iCallback = Events.k_iSteamFriendsCallbacks + 36;
        internal SteamID_t m_steamIDFriend;	// friend who's rich presence has changed
        internal AppId_t m_nAppID;			// the appID of the game (should always be the current game)
    };

    /// <summary>
    /// Personas that can be changed
    /// </summary>
    public enum EPersonaChange
    {
        k_EPersonaChangeName = 0x0001,
        k_EPersonaChangeStatus = 0x0002,
        k_EPersonaChangeComeOnline = 0x0004,
        k_EPersonaChangeGoneOffline = 0x0008,
        k_EPersonaChangeGamePlayed = 0x0010,
        k_EPersonaChangeGameServer = 0x0020,
        k_EPersonaChangeAvatar = 0x0040,
        k_EPersonaChangeJoinedSource = 0x0080,
        k_EPersonaChangeLeftSource = 0x0100,
        k_EPersonaChangeRelationshipChanged = 0x0200,
        k_EPersonaChangeNameFirstSet = 0x0400,
        k_EPersonaChangeFacebookInfo = 0x0800,
        k_EPersonaChangeNickname = 0x1000,
        k_EPersonaChangeSteamLevel = 0x2000,
    };

    /// <summary>
    /// Set of relationships to other users
    /// </summary>
	public enum EFriendRelationship
	{
		k_EFriendRelationshipNone = 0,
		k_EFriendRelationshipBlocked = 1,
		k_EFriendRelationshipRequestRecipient = 2,
		k_EFriendRelationshipFriend = 3,
		k_EFriendRelationshipRequestInitiator = 4,
		k_EFriendRelationshipIgnored = 5,
		k_EFriendRelationshipIgnoredFriend = 6,
	};

    /// <summary>
    /// List of states a friend can be in
    /// </summary>
	public enum EPersonaState
	{
		EPersonaStateOffline = 0,		// friend is not currently logged on
		EPersonaStateOnline = 1,		// friend is logged on
		EPersonaStateBusy = 2,			// user is on, but busy
		EPersonaStateAway = 3,			// auto-away feature
		EPersonaStateSnooze = 4			// auto-away for a long time
	};
    /// <summary>
    /// Flags for enumerating friends list, or quickly checking a the relationship between users
    /// </summary>
	public enum EFriendFlags
	{
		k_EFriendFlagNone = 0x00,
		k_EFriendFlagBlocked = 0x01,
		k_EFriendFlagFriendshipRequested = 0x02,
		k_EFriendFlagImmediate = 0x04,			// "regular" friend
		k_EFriendFlagClanMember = 0x08,
		k_EFriendFlagOnGameServer = 0x10,
		// k_EFriendFlagHasPlayedWith	= 0x20,	// not currently used
		// k_EFriendFlagFriendOfFriend	= 0x40, // not currently used
		k_EFriendFlagRequestingFriendship = 0x80,
		k_EFriendFlagRequestingInfo = 0x100,
		k_EFriendFlagIgnored = 0x200,
		k_EFriendFlagIgnoredFriend = 0x400,
		k_EFriendFlagAll = 0xFFFF,
	};

    /// <summary>
    /// Flags are passed as parameters to the store
    /// </summary>
	public enum EOverlayToStoreFlag
	{
		k_EOverlayToStoreFlag_None = 0,
		k_EOverlayToStoreFlag_AddToCart = 1,
		k_EOverlayToStoreFlag_AddToCartAndShow = 2,
	};

    /// <summary>
    /// Arguments for which overlay is open
    /// </summary>
    public enum EGameOverlay
    {
        EGameOverlayFriends = 0,
        EGameOverlayCommunity = 1,
        EGameOverlayPlayers = 2,
        EGameOverlaySettings = 3,
        EGameOverlayOfficialGameGroup = 4,
        EGameOverlayStats = 5,
        EGameOverlayAchievements = 6,
    };
    /// <summary>
    /// Arguments for which overlay is visible to user
    /// </summary>
    public enum EGameOverlayToUser
    {
        EGameOverlayToUserSteamId = 0,
        EGameOverlayToUserChat = 1,
        EGameOverlayToUserJoinTrade = 2,
        EGameOverlayToUserStats = 3,
        EGameOverlayToUserAchievements = 4,
        EGameOverlayToUserFriendAdd = 5,
        EGameOverlayToUserFriendRemove = 6,
        EGameOverlayToUserFriendRequestAccept = 7,
        EGameOverlayToUserFriendRequestIgnore = 8,
    };

    public class FriendGameInfo
    {
        internal FriendGameInfo(FriendGameInfo_t friendInfo)
        {
            GameID = new GameID(friendInfo.m_gameID);
            GameIP = friendInfo.m_unGameIP;
            GamePort = friendInfo.m_usGamePort;
            QueryPort = friendInfo.m_usQueryPort;
            Lobby = new Lobby(null, new SteamID(friendInfo.m_steamIDLobby));
        }

        public GameID GameID
        {
            get;
            private set;
        }

        public UInt32 GameIP
        {
            get;
            private set;
        }

        public UInt32 GamePort
        {
            get;
            private set;
        }

        public UInt32 QueryPort
        {
            get;
            private set;
        }

        public Lobby Lobby
        {
            get;
            private set;
        }
    };



	delegate void OnLargeAvatarReceivedFromSteam(ref AvatarImageLoaded_t CallbackData);
    /// <summary>
    /// When large avatar is recieved
    /// </summary>
    /// <param name="steamID">ID of avatar's user</param>
    /// <param name="avatar">Avatar to recieve</param>
	public delegate void OnLargeAvatarReceived(SteamID steamID, Image avatar);

    /// <summary>
    /// Lists the current user's friends
    /// </summary>
	public class Friends : ICollection<Friend>
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamFriends();
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamFriends_GetFriendCount(IntPtr friends, int friendFlags);
		[DllImport("CommunityExpressSW")]
		private static extern UInt64 SteamUnityAPI_SteamFriends_GetFriendByIndex(IntPtr friends, int iFriend, int friendFlags);
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamFriends_GetFriendPersonaName(IntPtr friends, UInt64 steamIDFriend);
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamFriends_GetFriendPersonaState(IntPtr friends, UInt64 steamIDFriend);
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamFriends_GetSmallFriendAvatar(IntPtr friends, UInt64 steamIDFriend);
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamFriends_GetMediumFriendAvatar(IntPtr friends, UInt64 steamIDFriend);
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamFriends_GetLargeFriendAvatar(IntPtr friends, UInt64 steamIDFriend);
		[DllImport("CommunityExpressSW")]
        private static extern void SteamUnityAPI_SteamFriends_ActivateGameOverlay(IntPtr friends, [MarshalAs(UnmanagedType.LPStr)] String dialog);
		[DllImport("CommunityExpressSW")]
        private static extern void SteamUnityAPI_SteamFriends_ActivateGameOverlayToUser(IntPtr friends, [MarshalAs(UnmanagedType.LPStr)] String dialog, UInt64 steamIDUser);
		[DllImport("CommunityExpressSW")]
        private static extern void SteamUnityAPI_SteamFriends_ActivateGameOverlayToWebPage(IntPtr friends, [MarshalAs(UnmanagedType.LPStr)] String url);
		[DllImport("CommunityExpressSW")]
        private static extern void SteamUnityAPI_SteamFriends_ActivateGameOverlayToStore(IntPtr friends, AppId_t appID, EOverlayToStoreFlag flag);
        [DllImport("CommunityExpressSW")]
        private static extern void SteamUnityAPI_SteamFriends_RequestFriendRichPresence(IntPtr friends, UInt64 steamIDFriend);
        [DllImport("CommunityExpressSW")]
        private static extern AppId_t SteamUnityAPI_SteamFriends_GetFriendCoplayGame(IntPtr friends, UInt64 steamIDFriend);
        [DllImport("CommunityExpressSW")]
        private static extern FriendGameInfo_t SteamUnityAPI_SteamFriends_GetFriendGamePlayed(IntPtr friends, UInt64 steamIDFriend);

		private IntPtr _friends;
		private EFriendFlags _friendFlags;
		private OnLargeAvatarReceivedFromSteam _internalOnLargeAvatarReceived = null;
		private OnLargeAvatarReceived _largeAvatarReceivedCallback;

		private class FriendEnumator : IEnumerator<Friend>
		{
			private int _index;
			private Friends _friends;

			public FriendEnumator(Friends friends)
			{
				_friends = friends;
				_index = -1;
			}

			public Friend Current 
			{
				get
				{
					SteamID id = _friends.GetFriendByIndex(_index);
					return new Friend(_friends, id);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public bool MoveNext()
			{
				_index++;
				return _index < _friends.Count;
			}

			public void Reset()
			{
				_index = -1;
			}

			public void Dispose()
			{
			}
		}

        private CommunityExpress _ce;

        internal Friends(CommunityExpress ce)
        {
            _ce = ce;

			_friends = SteamUnityAPI_SteamFriends();
			_friendFlags = EFriendFlags.k_EFriendFlagImmediate;

            _ce.AddEventHandler(GameOverlayActivated_t.k_iCallback, new CommunityExpress.OnEventHandler<GameOverlayActivated_t>(Events_GameOverlayActivated));
            _ce.AddEventHandler(AvatarImageLoaded_t.k_iCallback, new CommunityExpress.OnEventHandler<AvatarImageLoaded_t>(Events_AvatarImagedLoaded));
            _ce.AddEventHandler(PersonaStateChange_t.k_iCallback, new CommunityExpress.OnEventHandler<PersonaStateChange_t>(Events_PersonaStateChange));
            _ce.AddEventHandler(FriendRichPresenceUpdate_t.k_iCallback, new CommunityExpress.OnEventHandler<FriendRichPresenceUpdate_t>(Events_FriendRichPresenceUpdate));

		}
        /// <summary>
        /// Game overlay is activated
        /// </summary>
        /// <param name="result">Is overlay activated successfully</param>
        public delegate void GameOverlayActivatedHandler(bool result);
        /// <summary>
        /// Game overlay is activated
        /// </summary>
        public event GameOverlayActivatedHandler GameOverlayActivated;

        private void Events_GameOverlayActivated(GameOverlayActivated_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (GameOverlayActivated != null)
            {
                GameOverlayActivated(recv.m_bActive);
            }
        }

        public delegate void PersonaStateChangeHandler(Friends sender, SteamID ID, int ChangeFlags, bool result);
        public event PersonaStateChangeHandler PersonaStateChange;

        private void Events_PersonaStateChange(PersonaStateChange_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (PersonaStateChange != null)
            {
                PersonaStateChange(this, new SteamID(recv.m_ulSteamID), recv.m_nChangeFlags, bIOFailure);
                //GetFriendCoplayGame(new SteamID(recv.m_ulSteamID));
                GetFriendGamePlayed(new SteamID(recv.m_ulSteamID));
               // RequestFriendRichPresence(new SteamID(recv.m_ulSteamID));
            }
        }

        public delegate void FriendRichPresenceUpdateHandler(Friends sender, SteamID ID, AppId_t appID, bool result);
        public event FriendRichPresenceUpdateHandler FriendRichPresenceUpdate;

        private void Events_FriendRichPresenceUpdate(FriendRichPresenceUpdate_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (FriendRichPresenceUpdate != null)
            {
                FriendRichPresenceUpdate(this, new SteamID(recv.m_steamIDFriend), recv.m_nAppID, bIOFailure);
            }
        }

        private void Events_AvatarImagedLoaded(AvatarImageLoaded_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            Image.NotifyAvatarImageLoaded(new SteamID(recv.m_steamID), new Image(recv.m_iImage), bIOFailure);
        }

		internal Friends(EFriendFlags friendFlags)
		{
			_friends = SteamUnityAPI_SteamFriends();
			_friendFlags = friendFlags;
		}
		
		private SteamID GetFriendByIndex(int iFriend)
		{
			return new SteamID(SteamUnityAPI_SteamFriends_GetFriendByIndex(_friends, iFriend, (int)_friendFlags));
		}

		internal String GetFriendPersonaName(SteamID steamIDFriend)
		{
            return Marshal.PtrToStringAnsi(SteamUnityAPI_SteamFriends_GetFriendPersonaName(_friends, steamIDFriend.ToUInt64()));
		}

		internal EPersonaState GetFriendPersonaState(SteamID steamIDFriend)
		{
			int personaState = SteamUnityAPI_SteamFriends_GetFriendPersonaState(_friends, steamIDFriend.ToUInt64());
			return (EPersonaState)personaState;
		}

		internal Image GetSmallFriendAvatar(SteamID steamIDFriend)
		{
			int id = SteamUnityAPI_SteamFriends_GetSmallFriendAvatar(_friends, steamIDFriend.ToUInt64());
			if (id > 0)
			{
				return new Image(id);
			}

			return null;
		}

		internal Image GetMediumFriendAvatar(SteamID steamIDFriend)
		{
			int id = SteamUnityAPI_SteamFriends_GetMediumFriendAvatar(_friends, steamIDFriend.ToUInt64());
            if (id > 0)
			{
				return new Image(id);
			}

			return null;
		}

		internal Image GetLargeFriendAvatar(SteamID steamIDFriend)
		{
			int id = SteamUnityAPI_SteamFriends_GetLargeFriendAvatar(_friends, steamIDFriend.ToUInt64());
            if (id > 0)
            {
                Image img = new Image(id);

                Image.NotifyAvatarImageLoaded(steamIDFriend, img, true);
                return img;
            }

			return null;
		}
        /// <summary>
        /// Gets a friend from their user ID
        /// </summary>
        /// <param name="steamIDFriend">User's Steam ID</param>
        /// <returns>true if gotten</returns>
        public Friend GetFriendBySteamID(SteamID steamIDFriend)
        {
            Friend newFriend = new Friend(CommunityExpress.Instance.Friends, steamIDFriend);
            return newFriend;
        }

        public FriendGameInfo GetFriendGamePlayed(SteamID steamIDFriend)
        {
            FriendGameInfo_t friendGameInfo = SteamUnityAPI_SteamFriends_GetFriendGamePlayed(_friends, steamIDFriend.ToUInt64());
            return new FriendGameInfo(friendGameInfo);
        }

		private void InternalOnLargeAvatarReceived(ref AvatarImageLoaded_t CallbackData)
		{
			_largeAvatarReceivedCallback(new SteamID(CallbackData.m_steamID), new Image(CallbackData.m_iImage));
		}

        /// <summary>
        /// Brings up the in-game friend overlay
        /// </summary>
        /// <param name="dialog">Overlay dialog</param>
        public void ActivateGameOverlay(EGameOverlay dialog)
		{
            string[] strdialogs = { "Friends", "Community", "Players", "Settings", "OfficialGameGroup", "Stats", "Achievements" };

            SteamUnityAPI_SteamFriends_ActivateGameOverlay(_friends, strdialogs[(int)dialog]);
		}

        /// <summary>
        /// Brings up the overlay for the current user
        /// </summary>
        /// <param name="dialog">Overlay dialog</param>
        /// <param name="user">The user who recieves the overlay</param>
        public void ActivateGameOverlayToUser(EGameOverlayToUser dialog, SteamID user)
        {
            string[] strdialogs = { "steamid" , "chat", "jointrade", "stats", "achievements", "friendadd", "friendremove",
                "friendrequestaccept", "friendrequestignore" };

            SteamUnityAPI_SteamFriends_ActivateGameOverlayToUser(_friends, strdialogs[3], user.ToUInt64());
		}

        public void RequestFriendRichPresence(SteamID id)
		{
            SteamUnityAPI_SteamFriends_RequestFriendRichPresence(_friends, id.ToUInt64());
		}
        
        private void GetFriendCoplayGame(SteamID id)
        {
            AppId_t appID = SteamUnityAPI_SteamFriends_GetFriendCoplayGame(_friends, id.ToUInt64());
            Console.WriteLine(appID);
        }

        /// <summary>
        /// Adds the Steam overlay to a website
        /// </summary>
        /// <param name="url">Webpage to add the overlay to</param>
		public void ActivateGameOverlayToWebPage(String url)
		{
            SteamUnityAPI_SteamFriends_ActivateGameOverlayToWebPage(_friends, url);
		}
        /// <summary>
        /// Adds the Steam overlay to an app on the Steam store
        /// </summary>
        /// <param name="appID">Store app to add the overlay to</param>
        /// <param name="flag">Flags are passed as parameters to the store</param>
		public void ActivateGameOverlayToStore(AppId_t appID, EOverlayToStoreFlag flag)
		{
            SteamUnityAPI_SteamFriends_ActivateGameOverlayToStore(_friends, appID, flag);
		}
        /// <summary>
        /// Counts the list of friends
        /// </summary>
		public int Count
		{
			get { return SteamUnityAPI_SteamFriends_GetFriendCount(_friends, (int)_friendFlags); }
		}
        /// <summary>
        /// Determines if the list is read-only
        /// </summary>
		public bool IsReadOnly
		{
			get { return true; }
		}
        /// <summary>
        /// Adds a new friend
        /// </summary>
        /// <param name="item">The friend to be added</param>
		public void Add(Friend item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Clears the list of friends
        /// </summary>
		public void Clear()
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Checks if the list contains a certain friend
        /// </summary>
        /// <param name="item">Friend to be checked for</param>
        /// <returns>true if the friend is found</returns>
		public bool Contains(Friend item)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Copies the list to an index
        /// </summary>
        /// <param name="array">Array of friends</param>
        /// <param name="arrayIndex">(Index to copy to</param>
		public void CopyTo(Friend[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Removes a friend from the list
        /// </summary>
        /// <param name="item">Friend to be removed</param>
        /// <returns>true if friend is removed</returns>
		public bool Remove(Friend item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns>true if enumerator is gotten</returns>
		public IEnumerator<Friend> GetEnumerator()
		{
			return new FriendEnumator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

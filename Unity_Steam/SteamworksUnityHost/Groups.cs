/*
 * Community Express SDK
 * http://www.communityexpresssdk.com/
 *
 * Copyright (c) 2011-2014, Zimmdot, LLC
 * All rights reserved.
 *
 * Subject to terms and condition provided in LICENSE.txt
 * Dual licensed under a Commercial Development and LGPL licenses.
 */

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

    //Clan
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct GameConnectedClanChatMsg_t
    {
        internal const int k_iCallback = Events.k_iSteamFriendsCallbacks + 38;
        internal SteamID_t m_steamIDClanChat;
        internal SteamID_t m_steamIDUser;
        internal int m_iMessageID;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct GameConnectedChatJoin_t
    {
        internal const int k_iCallback = Events.k_iSteamFriendsCallbacks + 39;
        internal SteamID_t m_steamIDClanChat;
        internal SteamID_t m_steamIDUser;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct GameConnectedChatLeave_t
    {
        internal const int k_iCallback = Events.k_iSteamFriendsCallbacks + 40;
        internal SteamID_t m_steamIDClanChat;
        internal SteamID_t m_steamIDUser;
        internal bool m_bKicked;		// true if admin kicked
        internal bool m_bDropped;	// true if Steam connection dropped
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct DownloadClanActivityCountsResult_t
    {
        internal const int k_iCallback = Events.k_iSteamFriendsCallbacks + 41;
        internal bool m_bSuccess;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct JoinClanChatRoomCompletionResult_t
    {
        internal const int k_iCallback = Events.k_iSteamFriendsCallbacks + 42;
        internal SteamID_t m_steamIDClanChat;
        internal EChatRoomEnterResponse m_eChatRoomEnterResponse;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct ClanOfficerListResponse_t
    {
        internal const int k_iCallback = Events.k_iSteamFriendsCallbacks + 35;
        internal SteamID_t m_steamIDClan;
        internal int m_cOfficers;
        internal bool m_bSuccess;
    };

    /// <summary>
    /// List of groups the user is in
    /// </summary>
	public class Groups : ICollection<Group>
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamFriends();
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamFriends_GetClanCount(IntPtr friends);
		[DllImport("CommunityExpressSW")]
		private static extern UInt64 SteamUnityAPI_SteamFriends_GetClanByIndex(IntPtr friends, int iClan);
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamFriends_GetClanName(IntPtr friends, UInt64 steamIDClan);
		[DllImport("CommunityExpressSW")]
        private static extern IntPtr SteamUnityAPI_SteamFriends_GetClanTag(IntPtr friends, UInt64 steamIDClan);
        [DllImport("CommunityExpressSW")]
        private static extern SteamAPICall_t SteamUnityAPI_SteamFriends_RequestClanOfficerList(IntPtr friends, UInt64 steamIDClan);
        [DllImport("CommunityExpressSW")]
        private static extern SteamAPICall_t SteamUnityAPI_SteamFriends_DownloadClanActivityCounts(IntPtr friends, UInt64[] steamIDClan, int cClansToRequest);

		private IntPtr _friends;
		private Friends _friendsRef;

		private class GroupEnumator : IEnumerator<Group>
		{
			private int _index;
			private Groups _groups;
			private Friends _friendsRef;

			public GroupEnumator(Groups groups, Friends friendsRef)
			{
				_groups = groups;
				_friendsRef = friendsRef;
				_index = -1;
			}

			public Group Current
			{
				get
				{
					SteamID id = _groups.GetGroupByIndex(_index);
					return new Group(_groups, _friendsRef, id);
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
				return _index < _groups.Count;
			}

			public void Reset()
			{
				_index = -1;
			}

			public void Dispose()
			{
			}
		}

		internal Groups(Friends friendsRef)
		{
			_friends = SteamUnityAPI_SteamFriends();
			_friendsRef = friendsRef;
            CommunityExpress _ce = CommunityExpress.Instance;

            _ce.AddEventHandler(DownloadClanActivityCountsResult_t.k_iCallback, new CommunityExpress.OnEventHandler<DownloadClanActivityCountsResult_t>(Events_DownloadGroupActivityCounts));
            _ce.AddEventHandler(JoinClanChatRoomCompletionResult_t.k_iCallback, new CommunityExpress.OnEventHandler<JoinClanChatRoomCompletionResult_t>(Events_JoinClanChatRoomCompletion));
            _ce.AddEventHandler(GameConnectedClanChatMsg_t.k_iCallback, new CommunityExpress.OnEventHandler<GameConnectedClanChatMsg_t>(Events_GameConnectedClanChatMsg));
            _ce.AddEventHandler(ClanOfficerListResponse_t.k_iCallback, new CommunityExpress.OnEventHandler<ClanOfficerListResponse_t>(Events_ClanOfficerList));
            _ce.AddEventHandler(GameConnectedChatJoin_t.k_iCallback, new CommunityExpress.OnEventHandler<GameConnectedChatJoin_t>(Events_GameConnectedChatJoin));
            _ce.AddEventHandler(GameConnectedChatLeave_t.k_iCallback, new CommunityExpress.OnEventHandler<GameConnectedChatLeave_t>(Events_GameConnectedChatLeave));
		}

        public void RequestClanOfficerList(SteamID clanID)
        {
            SteamUnityAPI_SteamFriends_RequestClanOfficerList(_friends, clanID.ToUInt64());
        }

        public void DownloadGroupActivity(IEnumerable<SteamID> groupIDs)
        {
            List<UInt64> ids = new List<UInt64>();
            foreach (SteamID id in groupIDs)
            {
                ids.Add(id.ToUInt64());
            }
            UInt64[] newIDs = new UInt64[ids.Count];
            newIDs = ids.ToArray();
            SteamUnityAPI_SteamFriends_DownloadClanActivityCounts(_friends, newIDs, newIDs.Length);
        }

        public delegate void DownloadGroupActivityCountsHandler(Groups sender, bool result);
        public event DownloadGroupActivityCountsHandler DownloadGroupActivityCounts;

        private void Events_DownloadGroupActivityCounts(DownloadClanActivityCountsResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (DownloadGroupActivityCounts != null)
            {
                DownloadGroupActivityCounts(this, recv.m_bSuccess);
            }
        }
        
        public delegate void JoinClanChatRoomCompletionHandler(Groups sender, SteamID ClanID, EChatRoomEnterResponse RoomResponce, bool result);
        public event JoinClanChatRoomCompletionHandler JoinClanChatRoomCompletion;

        private void Events_JoinClanChatRoomCompletion(JoinClanChatRoomCompletionResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (JoinClanChatRoomCompletion != null)
            {
                JoinClanChatRoomCompletion(this, new SteamID(recv.m_steamIDClanChat), recv.m_eChatRoomEnterResponse, bIOFailure);
            }
        }

        public delegate void GameConnectedClanChatHandler(Groups sender, int MessageID, SteamID ClanChatID, SteamID UserID, bool result);
        public event GameConnectedClanChatHandler GameConnectedClanChatMsg;

        private void Events_GameConnectedClanChatMsg(GameConnectedClanChatMsg_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (GameConnectedClanChatMsg != null)
            {
                GameConnectedClanChatMsg(this, recv.m_iMessageID, new SteamID(recv.m_steamIDClanChat), new SteamID(recv.m_steamIDUser), bIOFailure);
            }
        }

        public delegate void ClanOfficerListHandler(Groups sender, SteamID ClanID, int officers, bool result);
        public event ClanOfficerListHandler ClanOfficerList;

        private void Events_ClanOfficerList(ClanOfficerListResponse_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (ClanOfficerList != null)
            {
                ClanOfficerList(this, new SteamID(recv.m_steamIDClan), recv.m_cOfficers, recv.m_bSuccess);
            }
        }

        public delegate void GameConnectedChatHandler(Groups sender, SteamID ClanChatID, SteamID UserID, bool result);
        public event GameConnectedChatHandler GameConnectedChatJoin;

        private void Events_GameConnectedChatJoin(GameConnectedChatJoin_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (GameConnectedChatJoin != null)
            {
                GameConnectedChatJoin(this, new SteamID(recv.m_steamIDClanChat), new SteamID(recv.m_steamIDUser), bIOFailure);
            }
        }

        public delegate void GameConnectedChatLeaveHandler(Groups sender, SteamID ClanChatID, SteamID UserID, bool kicked, bool connectionLost, bool result);
        public event GameConnectedChatLeaveHandler GameConnectedChatLeave;

        private void Events_GameConnectedChatLeave(GameConnectedChatLeave_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (GameConnectedChatLeave != null)
            {
                GameConnectedChatLeave(this, new SteamID(recv.m_steamIDClanChat), new SteamID(recv.m_steamIDUser), recv.m_bKicked, recv.m_bDropped, bIOFailure);
            }
        }

		private SteamID GetGroupByIndex(int iGroup)
		{
			return new SteamID(SteamUnityAPI_SteamFriends_GetClanByIndex(_friends, iGroup));
		}

		internal String GetGroupName(SteamID steamIDGroup)
		{
			return Marshal.PtrToStringAnsi(SteamUnityAPI_SteamFriends_GetClanName(_friends, steamIDGroup.ToUInt64()));
		}

		internal String GetClanTag(SteamID steamIDClan)
		{
			return Marshal.PtrToStringAnsi(SteamUnityAPI_SteamFriends_GetClanTag(_friends, steamIDClan.ToUInt64()));
		}
        /// <summary>
        /// Counts the number of groups
        /// </summary>
		public int Count
		{
			get { return SteamUnityAPI_SteamFriends_GetClanCount(_friends); }
		}
        /// <summary>
        /// If the list is read-only
        /// </summary>
		public bool IsReadOnly
		{
			get { return true; }
		}
        /// <summary>
        /// Adds group to list
        /// </summary>
        /// <param name="item">Group to add</param>
		public void Add(Group item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Clears list
        /// </summary>
		public void Clear()
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Checks if list contains certain group
        /// </summary>
        /// <param name="item">Group to check for</param>
        /// <returns>true if group found</returns>
		public bool Contains(Group item)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Copies list to index
        /// </summary>
        /// <param name="array">Array of groups</param>
        /// <param name="arrayIndex">Index to copy to</param>
		public void CopyTo(Group[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Removes group from list
        /// </summary>
        /// <param name="item">Group to remove</param>
        /// <returns>true if group removed</returns>
		public bool Remove(Group item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Tries to get enumerator
        /// </summary>
        /// <returns>true if enumerator gotten</returns>
		public IEnumerator<Group> GetEnumerator()
		{
			return new GroupEnumator(this, _friendsRef);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

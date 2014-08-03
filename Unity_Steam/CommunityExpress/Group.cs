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
    /// <summary>
    /// Information about a group
    /// </summary>
	public class Group : ICollection<Friend>
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamFriends();
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamFriends_GetFriendCountFromSource(IntPtr friends, UInt64 steamIDSource);
		[DllImport("CommunityExpressSW")]
		private static extern UInt64 SteamUnityAPI_SteamFriends_GetFriendFromSourceByIndex(IntPtr friends, UInt64 steamIDSource, int iFriend);

		private IntPtr _friends;
		private Friends _friendsRef;

		private Groups _groups;
		private SteamID _id;

		private class FriendEnumator : IEnumerator<Friend>
		{
			private int _index;
			private Group _group;
			private Friends _friends;

			public FriendEnumator(Group group, Friends friends)
			{
				_group = group;
				_friends = friends;
				_index = -1;
			}

			public Friend Current
			{
				get
				{
					SteamID id = _group.GetGroupMemberByIndex(_index);
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
				return _index < _group.Count;
			}

			public void Reset()
			{
				_index = -1;
			}

			public void Dispose()
			{
			}
		}

		internal Group(Groups groups, Friends friendsRef, SteamID id)
		{
			_friends = SteamUnityAPI_SteamFriends();
			_groups = groups;
			_friendsRef = friendsRef;
			_id = id;
		}

		private SteamID GetGroupMemberByIndex(int iGroupMember)
		{
			return new SteamID(SteamUnityAPI_SteamFriends_GetFriendFromSourceByIndex(_friends, _id.ToUInt64(), iGroupMember));
		}
        /// <summary>
        /// Name of the group
        /// </summary>
		public String GroupName
		{
			get { return _groups.GetGroupName(_id); }
		}
        /// <summary>
        /// Clan tag for the group
        /// </summary>
		public String ClanTag
		{
			get { return _groups.GetClanTag(_id); }
		}
        /// <summary>
        /// ID of the group
        /// </summary>
		public SteamID SteamID
		{
			get { return _id; }
		}
        /// <summary>
        /// Number of people in the group
        /// </summary>
		public int Count
		{
			get { return SteamUnityAPI_SteamFriends_GetFriendCountFromSource(_friends, _id.ToUInt64()); }
		}
        /// <summary>
        /// If the group is read-only
        /// </summary>
		public bool IsReadOnly
		{
			get { return true; }
		}
        /// <summary>
        /// Add new member
        /// </summary>
        /// <param name="item">Member to add</param>
		public void Add(Friend item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Clears group
        /// </summary>
		public void Clear()
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Checks if a group contains a specific member
        /// </summary>
        /// <param name="item">Member to check for</param>
        /// <returns>true if member found</returns>
		public bool Contains(Friend item)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Copies group member list to index
        /// </summary>
        /// <param name="array">Array of group members</param>
        /// <param name="arrayIndex">Index to copy to</param>
		public void CopyTo(Friend[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Removes member from group
        /// </summary>
        /// <param name="item">Member to remove</param>
        /// <returns>true if member removed</returns>
		public bool Remove(Friend item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Tries to get the enumerator
        /// </summary>
        /// <returns>true if enumerator gotten</returns>
		public IEnumerator<Friend> GetEnumerator()
		{
			return new FriendEnumator(this, _friendsRef);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
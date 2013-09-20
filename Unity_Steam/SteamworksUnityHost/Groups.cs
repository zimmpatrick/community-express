// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
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

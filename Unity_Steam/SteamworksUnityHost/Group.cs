// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
	public class Group : ICollection<Friend>
	{
		[DllImport("CommunityExpressSW.dll")]
		private static extern IntPtr SteamUnityAPI_SteamFriends();
		[DllImport("CommunityExpressSW.dll")]
		private static extern int SteamUnityAPI_SteamFriends_GetFriendCountFromSource(IntPtr friends, UInt64 steamIDSource);
		[DllImport("CommunityExpressSW.dll")]
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

		public String GroupName
		{
			get { return _groups.GetGroupName(_id); }
		}

		public String ClanTag
		{
			get { return _groups.GetClanTag(_id); }
		}

		public SteamID SteamID
		{
			get { return _id; }
		}

		public int Count
		{
			get { return SteamUnityAPI_SteamFriends_GetFriendCountFromSource(_friends, _id.ToUInt64()); }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void Add(Friend item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(Friend item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(Friend[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(Friend item)
		{
			throw new NotSupportedException();
		}

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
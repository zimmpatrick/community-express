// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
	public class Groups : ICollection<Group>
	{
		[DllImport("CommunityExpressSW.dll")]
		private static extern IntPtr SteamUnityAPI_SteamFriends();
		[DllImport("CommunityExpressSW.dll")]
		private static extern int SteamUnityAPI_SteamFriends_GetClanCount(IntPtr friends);
		[DllImport("CommunityExpressSW.dll")]
		private static extern UInt64 SteamUnityAPI_SteamFriends_GetClanByIndex(IntPtr friends, int iClan);
		[DllImport("CommunityExpressSW.dll")]
		[return: MarshalAs(UnmanagedType.LPStr, SizeParamIndex = 0)]
		private static extern String SteamUnityAPI_SteamFriends_GetClanName(IntPtr friends, UInt64 steamIDClan);
		[DllImport("CommunityExpressSW.dll")]
		[return: MarshalAs(UnmanagedType.LPStr, SizeParamIndex = 0)]
		private static extern String SteamUnityAPI_SteamFriends_GetClanTag(IntPtr friends, UInt64 steamIDClan);

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
			return SteamUnityAPI_SteamFriends_GetClanName(_friends, steamIDGroup.ToUInt64());
		}

		internal String GetClanTag(SteamID steamIDClan)
		{
			return SteamUnityAPI_SteamFriends_GetClanTag(_friends, steamIDClan.ToUInt64());
		}

		public int Count
		{
			get { return SteamUnityAPI_SteamFriends_GetClanCount(_friends); }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void Add(Group item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(Group item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(Group[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(Group item)
		{
			throw new NotSupportedException();
		}

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

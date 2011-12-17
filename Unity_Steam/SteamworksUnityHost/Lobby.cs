using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
	public class Lobby : ICollection<Friend>
	{
		[DllImport("CommunityExpressSW.dll")]
		private static extern IntPtr SteamUnityAPI_SteamMatchmaking();
		[DllImport("CommunityExpressSW.dll")]
		private static extern int SteamUnityAPI_SteamMatchmaking_GetNumLobbyMembers(IntPtr matchmaking, UInt64 steamIDLobby);
		[DllImport("CommunityExpressSW.dll")]
		private static extern UInt64 SteamUnityAPI_SteamMatchmaking_GetLobbyMemberByIndex(IntPtr matchmaking, UInt64 steamIDLobby, int iLobbyMember);

		private IntPtr _matchmaking;

		private Lobbies _lobbies;
		private SteamID _id;
		private Boolean _isLocked;
		private UInt32 _chatPermissions;

		private class FriendEnumator : IEnumerator<Friend>
		{
			private int _index;
			private Lobby _lobby;
			private Friends _friends;

			public FriendEnumator(Lobby lobby, Friends friends)
			{
				_lobby = lobby;
				_friends = friends;
				_index = -1;
			}

			public Friend Current
			{
				get
				{
					SteamID id = _lobby.GetLobbyMemberByIndex(_index);
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
				return _index < _lobby.Count;
			}

			public void Reset()
			{
				_index = -1;
			}

			public void Dispose()
			{
			}
		}

		internal Lobby(Lobbies lobbies, SteamID id)
		{
			_matchmaking = SteamUnityAPI_SteamMatchmaking();
			_lobbies = lobbies;
			_id = id;
		}

		private SteamID GetLobbyMemberByIndex(int iLobbyMember)
		{
			return new SteamID(SteamUnityAPI_SteamMatchmaking_GetLobbyMemberByIndex(_matchmaking, _id.ToUInt64(), iLobbyMember));
		}

		public SteamID SteamID
		{
			get { return _id; }
		}

		public Boolean IsLocked
		{
			get { return _isLocked; }
			internal set { _isLocked = value; }
		}

		public UInt32 ChatPermissions
		{
			get { return _chatPermissions; }
			internal set { _chatPermissions = value; }
		}

		public int Count
		{
			get { return SteamUnityAPI_SteamMatchmaking_GetNumLobbyMembers(_matchmaking, _id.ToUInt64()); }
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
			return new FriendEnumator(this, CommunityExpress.Instance.Friends);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
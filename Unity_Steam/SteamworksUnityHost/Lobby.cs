// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Net;

/*
 * ADD:
	Get/SetLobbyOwner
	SetLinkedLobby
 */
namespace CommunityExpressNS
{
	public class Lobby : ICollection<Friend>
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamMatchmaking();
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamMatchmaking_InviteUserToLobby(IntPtr matchmaking, UInt64 steamIDLobby, UInt64 steamID);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamMatchmaking_LeaveLobby(IntPtr matchmaking, UInt64 steamIDLobby);
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamMatchmaking_GetNumLobbyMembers(IntPtr matchmaking, UInt64 steamIDLobby);
		[DllImport("CommunityExpressSW")]
		private static extern UInt64 SteamUnityAPI_SteamMatchmaking_GetLobbyMemberByIndex(IntPtr matchmaking, UInt64 steamIDLobby, int iLobbyMember);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_RequestLobbyData(IntPtr matchmaking, UInt64 steamIDLobby);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_SetLobbyData(IntPtr matchmaking, UInt64 steamIDLobby,
			[MarshalAs(UnmanagedType.LPStr)] String key, [MarshalAs(UnmanagedType.LPStr)] String value);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_DeleteLobbyData(IntPtr matchmaking, UInt64 steamIDLobby,
			[MarshalAs(UnmanagedType.LPStr)] String key);
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamMatchmaking_GetLobbyMemberData(IntPtr matchmaking, UInt64 steamIDLobby, UInt64 steamIDUser,
			[MarshalAs(UnmanagedType.LPStr)] String pchKey);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamMatchmaking_SetLobbyMemberData(IntPtr matchmaking, UInt64 steamIDLobby,
			[MarshalAs(UnmanagedType.LPStr)] String pchKey, [MarshalAs(UnmanagedType.LPStr)] String pchValue);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_SendLobbyChatMsg(IntPtr matchmaking, UInt64 steamIDLobby, IntPtr data, Int32 dataLength);
		[DllImport("CommunityExpressSW")]
		private static extern Int32 SteamUnityAPI_SteamMatchmaking_GetLobbyChatEntry(IntPtr matchmaking, UInt64 steamIDLobby, Int32 chatID, out UInt64 steamID,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] Byte[] data, Int32 dataLength, out EChatEntryType chatEntryType);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamMatchmaking_SetLobbyGameServer(IntPtr matchmaking, UInt64 steamIDLobby, UInt32 gameServerIP,
			UInt16 gameServerPort, UInt64 steamIDGameServer);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_GetLobbyGameServer(IntPtr matchmaking, UInt64 steamIDLobby, out UInt32 gameServerIP,
			out UInt16 gameServerPort, out UInt64 steamIDGameServer);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_SetLobbyMemberLimit(IntPtr matchmaking, UInt64 steamIDLobby, Int32 maxMembers);
		[DllImport("CommunityExpressSW")]
		private static extern Int32 SteamUnityAPI_SteamMatchmaking_GetLobbyMemberLimit(IntPtr matchmaking, UInt64 steamIDLobby);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_SetLobbyType(IntPtr matchmaking, UInt64 steamIDLobby, ELobbyType lobbyType);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_SetLobbyJoinable(IntPtr matchmaking, UInt64 steamIDLobby, Boolean lobbyJoinable);
		[DllImport("CommunityExpressSW")]
		private static extern UInt64 SteamUnityAPI_SteamMatchmaking_GetLobbyOwner(IntPtr matchmaking, UInt64 steamIDLobby);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_SetLobbyOwner(IntPtr matchmaking, UInt64 steamIDLobby, UInt64 steamIDNewOwner);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_SetLinkedLobby(IntPtr matchmaking, UInt64 steamIDLobby, UInt64 steamIDLobbyDependent);

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

            CommunityExpress.Instance.Matchmaking.LobbyDataUpdated += new Matchmaking.LobbyDataUpdatedHandler(Matchmaking_LobbyDataUpdated);
		}

		public void Invite(SteamID user)
		{
			SteamUnityAPI_SteamMatchmaking_InviteUserToLobby(_matchmaking, _id.ToUInt64(), user.ToUInt64());
		}

		public void Invite(User user)
		{
			SteamUnityAPI_SteamMatchmaking_InviteUserToLobby(_matchmaking, _id.ToUInt64(), user.SteamID.ToUInt64());
		}

		public void Join()
		{
			CommunityExpress.Instance.Matchmaking.JoinLobby(this);
		}

		public void Leave()
		{
			if (!CommunityExpress.Instance.Matchmaking.LeaveLobby())
			{
				SteamUnityAPI_SteamMatchmaking_LeaveLobby(_matchmaking, _id.ToUInt64());
			}
		}

		public SteamID GetLobbyMemberByIndex(int iLobbyMember)
		{
			return new SteamID(SteamUnityAPI_SteamMatchmaking_GetLobbyMemberByIndex(_matchmaking, _id.ToUInt64(), iLobbyMember));
		}

		// Must call this to request Lobby Data if we are not yet a member of this lobby
		public Boolean RequestData()
		{
			return SteamUnityAPI_SteamMatchmaking_RequestLobbyData(_matchmaking, _id.ToUInt64());
		}

		public LobbyData GetData()
		{
			return new LobbyData(_matchmaking, this);
		}

		public Boolean SetData(String key, String value)
		{
			return SteamUnityAPI_SteamMatchmaking_SetLobbyData(_matchmaking, _id.ToUInt64(), key, value);
		}

		public Boolean DeleteData(String key)
		{
			return SteamUnityAPI_SteamMatchmaking_DeleteLobbyData(_matchmaking, _id.ToUInt64(), key);
		}

        public delegate void LobbyDataUpdatedHandler(Lobby sender, bool Success);
        public event LobbyDataUpdatedHandler LobbyDataUpdated;

        void Matchmaking_LobbyDataUpdated(Matchmaking sender, SteamID lobbyId, bool Success)
		{
            if (SteamID == lobbyId)
            {
                if (LobbyDataUpdated != null)
                {
                    LobbyDataUpdated(this, Success);
                }
            }
		}

		public String GetMemberData(SteamID user, String key)
		{
            return Marshal.PtrToStringAnsi(SteamUnityAPI_SteamMatchmaking_GetLobbyMemberData(_matchmaking, _id.ToUInt64(), user.ToUInt64(), key));
		}

		public void SetMemberData(String key, String value)
		{
			SteamUnityAPI_SteamMatchmaking_SetLobbyMemberData(_matchmaking, _id.ToUInt64(), key, value);
		}

		public Boolean SendChatMessage(String message)
		{
			IntPtr data = Marshal.StringToHGlobalUni(message);
			Boolean result = SteamUnityAPI_SteamMatchmaking_SendLobbyChatMsg(_matchmaking, _id.ToUInt64(), data, (message.Length + 1) * sizeof(char));
			Marshal.FreeHGlobal(data);
			return result;
		}

		public Boolean SendChatMessage(Byte[] data)
		{
			return SteamUnityAPI_SteamMatchmaking_SendLobbyChatMsg(_matchmaking, _id.ToUInt64(), Marshal.UnsafeAddrOfPinnedArrayElement(data, 0), data.Length);
		}

		public Byte[] GetChatEntry(Int32 chatID, out SteamID steamID, out EChatEntryType chatEntryType)
		{
			UInt64 steamID64;
			Byte[] data = new Byte[4096];

			Array.Resize(ref data, SteamUnityAPI_SteamMatchmaking_GetLobbyChatEntry(_matchmaking, _id.ToUInt64(), chatID, out steamID64, data, 4096, out chatEntryType));
			steamID = new SteamID(steamID64);

			return data;
		}

		public String GetChatEntryAsString(Int32 chatID, out SteamID steamID, out EChatEntryType chatEntryType)
		{
			return ConvertChatMessageToString(GetChatEntry(chatID, out steamID, out chatEntryType));
		}

		public String ConvertChatMessageToString(Byte[] message)
		{
			return Marshal.PtrToStringUni(Marshal.UnsafeAddrOfPinnedArrayElement(message, 0));
		}

		public void SetGameServer(IPAddress ip, UInt16 port)
		{
			Byte[] serverIPBytes = ip.GetAddressBytes();
			UInt32 serverIP = (UInt32)serverIPBytes[0] << 24 | (UInt32)serverIPBytes[1] << 16 | (UInt32)serverIPBytes[2] << 8 | (UInt32)serverIPBytes[3];

			SteamUnityAPI_SteamMatchmaking_SetLobbyGameServer(_matchmaking, _id.ToUInt64(), serverIP, port, 0);
		}

		public void SetGameServer(SteamID steamID, UInt16 port)
		{
			SteamUnityAPI_SteamMatchmaking_SetLobbyGameServer(_matchmaking, _id.ToUInt64(), 0, port, steamID.ToUInt64());
		}
	
		public Boolean GetGameServer(out IPAddress ip, out UInt16 port, out SteamID steamID)
		{
			UInt32 serverIP;
			UInt64 steamID64;

			Boolean result = SteamUnityAPI_SteamMatchmaking_GetLobbyGameServer(_matchmaking, _id.ToUInt64(), out serverIP, out port, out steamID64);
			ip = new IPAddress(new byte[] {(byte)(serverIP >> 24), (byte)(serverIP >> 16), (byte)(serverIP >> 8), (byte)serverIP});
			steamID = new SteamID(steamID64);

			return result;
		}

		public Boolean SetMemberLimit(Int32 maxMembers)
		{
			return SteamUnityAPI_SteamMatchmaking_SetLobbyMemberLimit(_matchmaking, _id.ToUInt64(), maxMembers);
		}

		public Int32 GetMemberLimit()
		{
			return SteamUnityAPI_SteamMatchmaking_GetLobbyMemberLimit(_matchmaking, _id.ToUInt64());
		}

		public Boolean SetType(ELobbyType lobbyType)
		{
			return SteamUnityAPI_SteamMatchmaking_SetLobbyType(_matchmaking, _id.ToUInt64(), lobbyType);
		}

		public Boolean SetJoinable(Boolean joinable)
		{
			return SteamUnityAPI_SteamMatchmaking_SetLobbyJoinable(_matchmaking, _id.ToUInt64(), joinable);
		}

		public SteamID GetLobbyOwner()
		{
			return new SteamID(SteamUnityAPI_SteamMatchmaking_GetLobbyOwner(_matchmaking, _id.ToUInt64()));
		}

		public Boolean SetLobbyOwner(SteamID newOwner)
		{
			return SteamUnityAPI_SteamMatchmaking_SetLobbyOwner(_matchmaking, _id.ToUInt64(), newOwner.ToUInt64());
		}

		public Boolean SetLinkedLobby(SteamID linkedLobby)
		{
			return SteamUnityAPI_SteamMatchmaking_SetLinkedLobby(_matchmaking, _id.ToUInt64(), linkedLobby.ToUInt64());
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
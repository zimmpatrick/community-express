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
    /// <summary>
    /// Information about a lobby
    /// </summary>
	public class Lobby : ICollection<Friend>
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamMatchmaking();
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_InviteUserToLobby(IntPtr matchmaking, UInt64 steamIDLobby, UInt64 steamID);
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
        /// <summary>
        /// Invite a user to a lobby
        /// </summary>
        /// <param name="user">SteamID of user to invite</param>
		public bool Invite(SteamID user)
		{
			return SteamUnityAPI_SteamMatchmaking_InviteUserToLobby(_matchmaking, _id.ToUInt64(), user.ToUInt64());
		}
        /// <summary>
        /// Invite a user to a lobby
        /// </summary>
        /// <param name="user">Name of user to invite</param>
		public void Invite(User user)
		{
			SteamUnityAPI_SteamMatchmaking_InviteUserToLobby(_matchmaking, _id.ToUInt64(), user.SteamID.ToUInt64());
		}
        /// <summary>
        /// Join a lobby
        /// </summary>
		public void Join()
		{
			CommunityExpress.Instance.Matchmaking.JoinLobby(this);
		}
        /// <summary>
        /// Leave a lobby
        /// </summary>
		public void Leave()
		{
			if (!CommunityExpress.Instance.Matchmaking.LeaveLobby())
			{
				SteamUnityAPI_SteamMatchmaking_LeaveLobby(_matchmaking, _id.ToUInt64());
			}
		}
        /// <summary>
        /// Gets the Steam ID of a lobby member
        /// </summary>
        /// <param name="iLobbyMember">Lobby member to get</param>
        /// <returns>Steam ID of member</returns>
		public SteamID GetLobbyMemberByIndex(int iLobbyMember)
		{
			return new SteamID(SteamUnityAPI_SteamMatchmaking_GetLobbyMemberByIndex(_matchmaking, _id.ToUInt64(), iLobbyMember));
		}

        /// <summary>
        /// Must call this to request Lobby Data if we are not yet a member of this lobby
        /// </summary>
        /// <returns>true if lobby data gotten</returns>
		public Boolean RequestData()
		{
			return SteamUnityAPI_SteamMatchmaking_RequestLobbyData(_matchmaking, _id.ToUInt64());
		}
        /// <summary>
        /// Gets data about lobby
        /// </summary>
        /// <returns>Lobby data</returns>
		public LobbyData GetData()
		{
			return new LobbyData(_matchmaking, this);
		}
        /// <summary>
        /// Sets lobby data
        /// </summary>
        /// <param name="key">Lobby key</param>
        /// <param name="value">Value to change</param>
        /// <returns>true if data set</returns>
		public Boolean SetData(String key, String value)
		{
			return SteamUnityAPI_SteamMatchmaking_SetLobbyData(_matchmaking, _id.ToUInt64(), key, value);
		}
        /// <summary>
        /// Deletes lobby data
        /// </summary>
        /// <param name="key">Lobby key</param>
        /// <returns>true if data deleted</returns>
		public Boolean DeleteData(String key)
		{
			return SteamUnityAPI_SteamMatchmaking_DeleteLobbyData(_matchmaking, _id.ToUInt64(), key);
		}
        /// <summary>
        /// Lobby data is updated
        /// </summary>
        /// <param name="sender">Sender of request</param>
        /// <param name="Success">If update is successful</param>
        public delegate void LobbyDataUpdatedHandler(Lobby sender, bool Success);
        /// <summary>
        /// Lobby data is updated
        /// </summary>
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
        /// <summary>
        /// Gets data about a member
        /// </summary>
        /// <param name="user">User to get data from</param>
        /// <param name="key">Lobby key</param>
        /// <returns>Member data</returns>
		public String GetMemberData(SteamID user, String key)
		{
            return Marshal.PtrToStringAnsi(SteamUnityAPI_SteamMatchmaking_GetLobbyMemberData(_matchmaking, _id.ToUInt64(), user.ToUInt64(), key));
		}
        /// <summary>
        /// Sets data about a lobby member
        /// </summary>
        /// <param name="key">Lobby key</param>
        /// <param name="value">Value to set</param>
		public void SetMemberData(String key, String value)
		{
			SteamUnityAPI_SteamMatchmaking_SetLobbyMemberData(_matchmaking, _id.ToUInt64(), key, value);
		}
        /// <summary>
        /// Sends chat message in string form
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>true if sent</returns>
		public Boolean SendChatMessage(String message)
		{
			IntPtr data = Marshal.StringToHGlobalUni(message);
			Boolean result = SteamUnityAPI_SteamMatchmaking_SendLobbyChatMsg(_matchmaking, _id.ToUInt64(), data, (message.Length + 1) * sizeof(char));
			Marshal.FreeHGlobal(data);
			return result;
		}
        /// <summary>
        /// Sends chat message in byte form
        /// </summary>
        /// <param name="data">Data of message to send</param>
        /// <returns>true if sent</returns>
		public Boolean SendChatMessage(Byte[] data)
		{
			return SteamUnityAPI_SteamMatchmaking_SendLobbyChatMsg(_matchmaking, _id.ToUInt64(), Marshal.UnsafeAddrOfPinnedArrayElement(data, 0), data.Length);
		}
        /// <summary>
        /// Gets chat message sent from another member
        /// </summary>
        /// <param name="chatID">ID of chat message</param>
        /// <param name="steamID">Steam ID of sender</param>
        /// <param name="chatEntryType">Type of chat entry</param>
        /// <returns>Byte form of message</returns>
		public Byte[] GetChatEntry(Int32 chatID, out SteamID steamID, out EChatEntryType chatEntryType)
		{
			UInt64 steamID64;
			Byte[] data = new Byte[4096];

			Array.Resize(ref data, SteamUnityAPI_SteamMatchmaking_GetLobbyChatEntry(_matchmaking, _id.ToUInt64(), chatID, out steamID64, data, 4096, out chatEntryType));
			steamID = new SteamID(steamID64);

			return data;
		}
        /// <summary>
        /// Gets chat message sent from another member in string format
        /// </summary>
        /// <param name="chatID">ID of chat message</param>
        /// <param name="steamID">Steam ID of sender</param>
        /// <param name="chatEntryType">Type of chat entry</param>
        /// <returns>Chat message</returns>
		public String GetChatEntryAsString(Int32 chatID, out SteamID steamID, out EChatEntryType chatEntryType)
		{
			return ConvertChatMessageToString(GetChatEntry(chatID, out steamID, out chatEntryType));
		}
        /// <summary>
        /// Converts byte message to string message
        /// </summary>
        /// <param name="message">Recieved chat message</param>
        /// <returns>Chat message</returns>
		public String ConvertChatMessageToString(Byte[] message)
		{
			return Marshal.PtrToStringUni(Marshal.UnsafeAddrOfPinnedArrayElement(message, 0));
		}
        /// <summary>
        /// Sets a game server up
        /// </summary>
        /// <param name="ip">IP of game server</param>
        /// <param name="port">Port for the game server</param>
		public void SetGameServer(IPAddress ip, UInt16 port)
		{
			Byte[] serverIPBytes = ip.GetAddressBytes();
			UInt32 serverIP = (UInt32)serverIPBytes[0] << 24 | (UInt32)serverIPBytes[1] << 16 | (UInt32)serverIPBytes[2] << 8 | (UInt32)serverIPBytes[3];

			SteamUnityAPI_SteamMatchmaking_SetLobbyGameServer(_matchmaking, _id.ToUInt64(), serverIP, port, 0);
		}
        /// <summary>
        /// Sets a game server
        /// </summary>
        /// <param name="steamID">Game server ID</param>
        /// <param name="port">Port for the game server</param>
		public void SetGameServer(SteamID steamID, UInt16 port)
		{
			SteamUnityAPI_SteamMatchmaking_SetLobbyGameServer(_matchmaking, _id.ToUInt64(), 0, port, steamID.ToUInt64());
		}
	    /// <summary>
	    /// Gets a running game server
	    /// </summary>
	    /// <param name="ip">IP of the game server</param>
	    /// <param name="port">Port for the game server</param>
	    /// <param name="steamID">Game server ID</param>
	    /// <returns>true if gotten</returns>
		public Boolean GetGameServer(out IPAddress ip, out UInt16 port, out SteamID steamID)
		{
			UInt32 serverIP;
			UInt64 steamID64;

			Boolean result = SteamUnityAPI_SteamMatchmaking_GetLobbyGameServer(_matchmaking, _id.ToUInt64(), out serverIP, out port, out steamID64);
			ip = new IPAddress(new byte[] {(byte)(serverIP >> 24), (byte)(serverIP >> 16), (byte)(serverIP >> 8), (byte)serverIP});
			steamID = new SteamID(steamID64);

			return result;
		}
        /// <summary>
        /// Sets lobby member limit
        /// </summary>
        /// <param name="maxMembers">Max number of members in lobby</param>
        /// <returns>true if limit set</returns>
		public Boolean SetMemberLimit(Int32 maxMembers)
		{
			return SteamUnityAPI_SteamMatchmaking_SetLobbyMemberLimit(_matchmaking, _id.ToUInt64(), maxMembers);
		}
        /// <summary>
        /// Gets max number of members in lobby
        /// </summary>
        /// <returns>Max number of members in lobby</returns>
		public Int32 GetMemberLimit()
		{
			return SteamUnityAPI_SteamMatchmaking_GetLobbyMemberLimit(_matchmaking, _id.ToUInt64());
		}
        /// <summary>
        /// Sets the type of lobby
        /// </summary>
        /// <param name="lobbyType">Type of lobby</param>
        /// <returns>true if lobby type set</returns>
		public Boolean SetType(ELobbyType lobbyType)
		{
			return SteamUnityAPI_SteamMatchmaking_SetLobbyType(_matchmaking, _id.ToUInt64(), lobbyType);
		}
        /// <summary>
        /// Sets if the lobby is joinable or not
        /// </summary>
        /// <param name="joinable">If the lobby is joinable or not</param>
        /// <returns>true if joinability set</returns>
		public Boolean SetJoinable(Boolean joinable)
		{
			return SteamUnityAPI_SteamMatchmaking_SetLobbyJoinable(_matchmaking, _id.ToUInt64(), joinable);
		}
        /// <summary>
        /// Gets the owner of the lobby's Steam ID
        /// </summary>
        /// <returns>Steam ID of lobby owner</returns>
		public SteamID GetLobbyOwner()
		{
			return new SteamID(SteamUnityAPI_SteamMatchmaking_GetLobbyOwner(_matchmaking, _id.ToUInt64()));
		}
        /// <summary>
        /// Sets a user as the lobby owner
        /// </summary>
        /// <param name="newOwner">Steam ID of new lobby owner</param>
        /// <returns>true if owner set</returns>
		public Boolean SetLobbyOwner(SteamID newOwner)
		{
			return SteamUnityAPI_SteamMatchmaking_SetLobbyOwner(_matchmaking, _id.ToUInt64(), newOwner.ToUInt64());
		}
        /// <summary>
        /// Sets up a linked lobby
        /// </summary>
        /// <param name="linkedLobby">ID of lobby to link to</param>
        /// <returns>true if linked</returns>
		public Boolean SetLinkedLobby(SteamID linkedLobby)
		{
			return SteamUnityAPI_SteamMatchmaking_SetLinkedLobby(_matchmaking, _id.ToUInt64(), linkedLobby.ToUInt64());
		}
        /// <summary>
        /// Steam ID of lobby
        /// </summary>
		public SteamID SteamID
		{
			get { return _id; }
		}
        /// <summary>
        /// If the lobby is locked
        /// </summary>
		public Boolean IsLocked
		{
			get { return _isLocked; }
			internal set { _isLocked = value; }
		}
        /// <summary>
        /// Chat permission of lobby
        /// </summary>
		public UInt32 ChatPermissions
		{
			get { return _chatPermissions; }
			internal set { _chatPermissions = value; }
		}
        /// <summary>
        /// Count of users in lobby
        /// </summary>
		public int Count
		{
			get { return SteamUnityAPI_SteamMatchmaking_GetNumLobbyMembers(_matchmaking, _id.ToUInt64()); }
		}
        /// <summary>
        /// If the lobby is read-only
        /// </summary>
		public bool IsReadOnly
		{
			get { return true; }
		}
        /// <summary>
        /// Adds a user to the lobby
        /// </summary>
        /// <param name="item">User to add</param>
		public void Add(Friend item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Clears users in lobby
        /// </summary>
		public void Clear()
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Checks to see if lobby contains a certain user
        /// </summary>
        /// <param name="item">User to check for</param>
        /// <returns>true if user found</returns>
		public bool Contains(Friend item)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Copies lobby members to index
        /// </summary>
        /// <param name="array">Array of members</param>
        /// <param name="arrayIndex">Index to copy to</param>
		public void CopyTo(Friend[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Removes user from lobby
        /// </summary>
        /// <param name="item">User to remove</param>
        /// <returns>true if user removed</returns>
		public bool Remove(Friend item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Gets enumerator for lobby
        /// </summary>
        /// <returns>true if enumerator gotten</returns>
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
// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
	public class LobbyData : ICollection<KeyValuePair<String, String>>
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamMatchmaking_GetLobbyData(IntPtr matchmaking, UInt64 steamIDLobby, [MarshalAs(UnmanagedType.LPStr)] String key);
		[DllImport("CommunityExpressSW")]
		private static extern Int32 SteamUnityAPI_SteamMatchmaking_GetLobbyDataCount(IntPtr matchmaking, UInt64 steamIDLobby);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_GetLobbyDataByIndex(IntPtr matchmaking, UInt64 steamIDLobby, Int32 dataIndex, IntPtr key,
			Int32 maxKeyLength, IntPtr value, Int32 maxValueLength);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_SetLobbyData(IntPtr matchmaking, UInt64 steamIDLobby, [MarshalAs(UnmanagedType.LPStr)] String key,
			[MarshalAs(UnmanagedType.LPStr)] String value);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamMatchmaking_DeleteLobbyData(IntPtr matchmaking, UInt64 steamIDLobby, [MarshalAs(UnmanagedType.LPStr)] String key);

		private IntPtr _matchmaking;
		private Lobby _lobby;

		private class KeyValueEnumator : IEnumerator<KeyValuePair<String, String>>
		{
			private int _index;
			private LobbyData _lobbyData;

			public KeyValueEnumator(LobbyData lobbyData)
			{
				_index = -1;
				_lobbyData = lobbyData;
			}

			public KeyValuePair<String, String> Current
			{
				get
				{
					String key, value;
					_lobbyData.GetDataByIndex(_index, out key, 256, out value, 256);
					return new KeyValuePair<String, String>(key, value);
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
				return _index < _lobbyData.Count;
			}

			public void Reset()
			{
				_index = -1;
			}

			public void Dispose()
			{
			}
		}

		internal LobbyData(IntPtr matchmaking, Lobby lobby)
		{
			_matchmaking = matchmaking;
			_lobby = lobby;
		}

		public String GetValue(String key)
		{
            return Marshal.PtrToStringAnsi(SteamUnityAPI_SteamMatchmaking_GetLobbyData(_matchmaking, _lobby.SteamID.ToUInt64(), key));
		}

		public void GetDataByIndex(Int32 index, out String key, Int32 maxKeyLength, out String value, Int32 maxValueLength)
		{
			IntPtr keyDataPtr = Marshal.AllocHGlobal(maxKeyLength + 1);
			IntPtr valueDataPtr = Marshal.AllocHGlobal(maxValueLength + 1);

			SteamUnityAPI_SteamMatchmaking_GetLobbyDataByIndex(_matchmaking, _lobby.SteamID.ToUInt64(), index, keyDataPtr, maxKeyLength, valueDataPtr, maxValueLength);

			key = Marshal.PtrToStringAnsi(keyDataPtr);
			value = Marshal.PtrToStringAnsi(valueDataPtr);

			Marshal.FreeHGlobal(keyDataPtr);
			Marshal.FreeHGlobal(valueDataPtr);
		}

		public Boolean SetLobbyData(String key, String value)
		{
			return SteamUnityAPI_SteamMatchmaking_SetLobbyData(_matchmaking, _lobby.SteamID.ToUInt64(), key, value);
		}

		public Boolean DeleteLobbyData(String key)
		{
			return SteamUnityAPI_SteamMatchmaking_DeleteLobbyData(_matchmaking, _lobby.SteamID.ToUInt64(), key);
		}

		public int Count
		{
			get { return SteamUnityAPI_SteamMatchmaking_GetLobbyDataCount(_matchmaking, _lobby.SteamID.ToUInt64()); }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void Add(KeyValuePair<String, String> item)
		{
			SteamUnityAPI_SteamMatchmaking_SetLobbyData(_matchmaking, _lobby.SteamID.ToUInt64(), item.Key, item.Value);
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(KeyValuePair<String, String> item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<String, String>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<String, String> item)
		{
			return SteamUnityAPI_SteamMatchmaking_DeleteLobbyData(_matchmaking, _lobby.SteamID.ToUInt64(), item.Key);
		}

		public IEnumerator<KeyValuePair<String, String>> GetEnumerator()
		{
			return new KeyValueEnumator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
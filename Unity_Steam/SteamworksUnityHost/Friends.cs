// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
	using AppId_t = UInt32;

	//-----------------------------------------------------------------------------
	// Purpose: set of relationships to other users
	//-----------------------------------------------------------------------------
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

	//-----------------------------------------------------------------------------
	// Purpose: list of states a friend can be in
	//-----------------------------------------------------------------------------
	public enum EPersonaState
	{
		EPersonaStateOffline = 0,		// friend is not currently logged on
		EPersonaStateOnline = 1,		// friend is logged on
		EPersonaStateBusy = 2,			// user is on, but busy
		EPersonaStateAway = 3,			// auto-away feature
		EPersonaStateSnooze = 4			// auto-away for a long time
	};

	//-----------------------------------------------------------------------------
	// Purpose: flags for enumerating friends list, or quickly checking a the relationship between users
	//-----------------------------------------------------------------------------
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

	//-----------------------------------------------------------------------------
	// Purpose flags are passed as parameters to the store
	//-----------------------------------------------------------------------------
	public enum EOverlayToStoreFlag
	{
		k_EOverlayToStoreFlag_None = 0,
		k_EOverlayToStoreFlag_AddToCart = 1,
		k_EOverlayToStoreFlag_AddToCartAndShow = 2,
	};

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct AvatarImageLoaded_t
	{
		public UInt64 m_steamID; // steamid the avatar has been loaded for
		public int m_iImage; // the image index of the now loaded image
		public int m_iWide; // width of the loaded image
		public int m_iTall; // height of the loaded image
	}

	delegate void OnLargeAvatarReceivedFromSteam(ref AvatarImageLoaded_t CallbackData);
	public delegate void OnLargeAvatarReceived(SteamID steamID, Image avatar);

	public class Friends : ICollection<Friend>
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamFriends();
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamFriends_GetFriendCount(IntPtr friends, int friendFlags);
		[DllImport("CommunityExpressSW")]
		private static extern UInt64 SteamUnityAPI_SteamFriends_GetFriendByIndex(IntPtr friends, int iFriend, int friendFlags);
		[DllImport("CommunityExpressSW")]
		[return: MarshalAs(UnmanagedType.LPStr, SizeParamIndex = 0)]
		private static extern String SteamUnityAPI_SteamFriends_GetFriendPersonaName(IntPtr friends, UInt64 steamIDFriend);
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamFriends_GetFriendPersonaState(IntPtr friends, UInt64 steamIDFriend);
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamFriends_GetSmallFriendAvatar(IntPtr friends, UInt64 steamIDFriend);
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamFriends_GetMediumFriendAvatar(IntPtr friends, UInt64 steamIDFriend);
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamFriends_GetLargeFriendAvatar(IntPtr friends, UInt64 steamIDFriend, IntPtr OnAvatarReceivedCallback);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamFriends_ActivateGameOverlay([MarshalAs(UnmanagedType.LPStr)] String dialog);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamFriends_ActivateGameOverlayToUser([MarshalAs(UnmanagedType.LPStr)] String dialog, UInt64 steamIDUser);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamFriends_ActivateGameOverlayToWebPage([MarshalAs(UnmanagedType.LPStr)] String url);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamFriends_ActivateGameOverlayToStore(AppId_t appID, EOverlayToStoreFlag flag);

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

		internal Friends()
		{
			_friends = SteamUnityAPI_SteamFriends();
			_friendFlags = EFriendFlags.k_EFriendFlagImmediate;
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
			return SteamUnityAPI_SteamFriends_GetFriendPersonaName(_friends, steamIDFriend.ToUInt64());
		}

		internal EPersonaState GetFriendPersonaState(SteamID steamIDFriend)
		{
			int personaState = SteamUnityAPI_SteamFriends_GetFriendPersonaState(_friends, steamIDFriend.ToUInt64());
			return (EPersonaState)personaState;
		}

		internal Image GetSmallFriendAvatar(SteamID steamIDFriend)
		{
			int id = SteamUnityAPI_SteamFriends_GetSmallFriendAvatar(_friends, steamIDFriend.ToUInt64());
			if (id != -1)
			{
				return new Image(id);
			}

			return null;
		}

		internal Image GetMediumFriendAvatar(SteamID steamIDFriend)
		{
			int id = SteamUnityAPI_SteamFriends_GetMediumFriendAvatar(_friends, steamIDFriend.ToUInt64());
			if (id != -1)
			{
				return new Image(id);
			}

			return null;
		}

		internal Image GetLargeFriendAvatar(SteamID steamIDFriend, OnLargeAvatarReceived largeAvatarReceivedCallback)
		{
			_largeAvatarReceivedCallback = largeAvatarReceivedCallback;

			if (_internalOnLargeAvatarReceived == null)
			{
				_internalOnLargeAvatarReceived = new OnLargeAvatarReceivedFromSteam(InternalOnLargeAvatarReceived);
			}

			int id = SteamUnityAPI_SteamFriends_GetLargeFriendAvatar(_friends, steamIDFriend.ToUInt64(), Marshal.GetFunctionPointerForDelegate(_internalOnLargeAvatarReceived));
			if (id != -1)
			{
				Image img = new Image(id);

				if (_largeAvatarReceivedCallback != null)
				{
					_largeAvatarReceivedCallback(steamIDFriend, img);
				}

				return img;
			}

			return null;
		}

        public Friend GetFriendBySteamID(SteamID steamIDFriend)
        {
            Friend newFriend = new Friend(CommunityExpress.Instance.Friends, steamIDFriend);
            return newFriend;
        }

		private void InternalOnLargeAvatarReceived(ref AvatarImageLoaded_t CallbackData)
		{
			_largeAvatarReceivedCallback(new SteamID(CallbackData.m_steamID), new Image(CallbackData.m_iImage));
		}

		public void ActivateGameOverlay(String dialog)
		{
			SteamUnityAPI_SteamFriends_ActivateGameOverlay(dialog);
		}

		public void ActivateGameOverlayToUser(String dialog, SteamID user)
		{
			SteamUnityAPI_SteamFriends_ActivateGameOverlayToUser(dialog, user.ToUInt64());
		}

		public void ActivateGameOverlayToWebPage(String url)
		{
			SteamUnityAPI_SteamFriends_ActivateGameOverlayToWebPage(url);
		}

		public void ActivateGameOverlayToStore(AppId_t appID, EOverlayToStoreFlag flag)
		{
			SteamUnityAPI_SteamFriends_ActivateGameOverlayToStore(appID, flag);
		}

		public int Count
		{
			get { return SteamUnityAPI_SteamFriends_GetFriendCount(_friends, (int)_friendFlags); }
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
			return new FriendEnumator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

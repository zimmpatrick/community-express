// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
	public enum EP2PSessionError
	{
		k_EP2PSessionErrorNone = 0,
		k_EP2PSessionErrorNotRunningApp = 1,			// target is not running the same game
		k_EP2PSessionErrorNoRightsToApp = 2,			// local user doesn't own the app that is running
		k_EP2PSessionErrorDestinationNotLoggedIn = 3,	// target user isn't connected to Steam
		k_EP2PSessionErrorTimeout = 4,					// target isn't responding, perhaps not calling AcceptP2PSessionWithUser()
		// corporate firewalls can also block this (NAT traversal is not firewall traversal)
		// make sure that UDP ports 3478, 4379, and 4380 are open in an outbound direction
		k_EP2PSessionErrorMax = 5
	}

	// SendP2PPacket() send types
	// Typically k_EP2PSendUnreliable is what you want for UDP-like packets, k_EP2PSendReliable for TCP-like packets
	public enum EP2PSend
	{
		// Basic UDP send. Packets can't be bigger than 1200 bytes (your typical MTU size). Can be lost, or arrive out of order (rare).
		// The sending API does have some knowledge of the underlying connection, so if there is no NAT-traversal accomplished or
		// there is a recognized adjustment happening on the connection, the packet will be batched until the connection is open again.
		k_EP2PSendUnreliable = 0,

		// As above, but if the underlying p2p connection isn't yet established the packet will just be thrown away. Using this on the first
		// packet sent to a remote host almost guarantees the packet will be dropped.
		// This is only really useful for kinds of data that should never buffer up, i.e. voice payload packets
		k_EP2PSendUnreliableNoDelay = 1,

		// Reliable message send. Can send up to 1MB of data in a single message. 
		// Does fragmentation/re-assembly of messages under the hood, as well as a sliding window for efficient sends of large chunks of data. 
		k_EP2PSendReliable = 2,

		// As above, but applies the Nagle algorithm to the send - sends will accumulate 
		// until the current MTU size (typically ~1200 bytes, but can change) or ~200ms has passed (Nagle algorithm). 
		// Useful if you want to send a set of smaller messages but have the coalesced into a single packet
		// Since the reliable stream is all ordered, you can do several small message sends with k_EP2PSendReliableWithBuffering and then
		// do a normal k_EP2PSendReliable to force all the buffered data to be sent.
		k_EP2PSendReliableWithBuffering = 3,

	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct P2PSessionRequest_t
	{
		public UInt64 m_steamID;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct P2PSessionConnectFail_t
	{
		public UInt64 m_steamID;
		public Byte m_eP2PSessionError;
	}

	delegate void OnNewP2PSessionFromSteam(P2PSessionRequest_t callbackData);
	public delegate Boolean OnNewP2PSession(SteamID steamID);

	delegate void OnSendP2PPacketFailedFromSteam(P2PSessionConnectFail_t callbackData);
	public delegate void OnSendP2PPacketFailed(SteamID steamID, EP2PSessionError error);

	public delegate void OnP2PPacketReceived(SteamID steamID, Byte[] data, Int32 channel);

	public class Networking
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamNetworking();
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamNetworking_SetCallbacks(IntPtr onNewP2PSession, IntPtr onSendP2PPacketFailed);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamNetworking_AllowP2PPacketRelay(IntPtr networking, Boolean allow);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamNetworking_AcceptP2PSessionWithUser(IntPtr networking, UInt64 steamID);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamNetworking_SendP2PPacket(IntPtr networking, UInt64 steamID, IntPtr data, UInt32 dataLength,
			Byte sendType, int channel);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamNetworking_IsP2PPacketAvailable(IntPtr networking, out UInt32 packetSize, Int32 channel);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamNetworking_ReadP2PPacket(IntPtr networking, IntPtr data, UInt32 packetSize,
			out UInt64 steamID, Int32 channel);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamNetworking_CloseP2PSession(IntPtr networking, UInt64 steamID);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamNetworking_CloseP2PChannel(IntPtr networking, UInt64 steamID, Int32 channel);

		private IntPtr _networking;
		private Boolean _isInitialized = false;
		private Int32[] _listenChannels = new Int32[] {0};

		private OnNewP2PSessionFromSteam _internalOnNewP2PSession = null;
		private OnNewP2PSession _onNewP2PSession;

		private OnSendP2PPacketFailedFromSteam _internalOnSendP2PPacketFailed = null;
		private OnSendP2PPacketFailed _onSendP2PPacketFailed;

		private OnP2PPacketReceived _onP2PPacketReceived;

		internal Networking()
		{
			_networking = SteamUnityAPI_SteamNetworking();
		}

		public void Init(Boolean allowPacketRelay, OnNewP2PSession onNewP2PSession, OnSendP2PPacketFailed onSendP2PPacketFailed, OnP2PPacketReceived onP2PPacketReceived)
		{
			_onNewP2PSession = onNewP2PSession;
			_onSendP2PPacketFailed = onSendP2PPacketFailed;
			_onP2PPacketReceived = onP2PPacketReceived;

			if (_internalOnNewP2PSession == null)
			{
				_internalOnNewP2PSession = new OnNewP2PSessionFromSteam(OnNewP2PSession);
				_internalOnSendP2PPacketFailed = new OnSendP2PPacketFailedFromSteam(OnSendP2PPacketFailed);
			}

			SteamUnityAPI_SteamNetworking_SetCallbacks(Marshal.GetFunctionPointerForDelegate(_internalOnNewP2PSession),
				Marshal.GetFunctionPointerForDelegate(_internalOnSendP2PPacketFailed));

			SteamUnityAPI_SteamNetworking_AllowP2PPacketRelay(_networking, allowPacketRelay);

			_isInitialized = true;
		}

		private void OnNewP2PSession(P2PSessionRequest_t callbackData)
		{
			if (_onNewP2PSession(new SteamID(callbackData.m_steamID)))
				SteamUnityAPI_SteamNetworking_AcceptP2PSessionWithUser(_networking, callbackData.m_steamID);
		}

		public void SendP2PPacket(SteamID steamID, String data, EP2PSend sendType, Int32 channel = 0)
		{
			SteamUnityAPI_SteamNetworking_SendP2PPacket(_networking, steamID.ToUInt64(), Marshal.StringToHGlobalAnsi(data), (UInt32)data.Length, (Byte)sendType, channel);
		}

		public void SendP2PPacket(SteamID steamID, Byte[] data, EP2PSend sendType, Int32 channel = 0)
		{
			IntPtr dataPtr = Marshal.AllocHGlobal(data.Length);
			Marshal.Copy(data, 0, dataPtr, data.Length);

			SteamUnityAPI_SteamNetworking_SendP2PPacket(_networking, steamID.ToUInt64(), dataPtr, (UInt32)data.Length, (Byte)sendType, channel);

			Marshal.FreeHGlobal(dataPtr);
		}

		private void OnSendP2PPacketFailed(P2PSessionConnectFail_t callbackData)
		{
			_onSendP2PPacketFailed(new SteamID(callbackData.m_steamID), (EP2PSessionError)callbackData.m_eP2PSessionError);
		}

		public void SetListenChannels(Int32[] listenChannels)
		{
			_listenChannels = listenChannels;
		}

		internal void CheckForNewP2PPackets()
		{
			UInt32 packetSize;

			for (int i = 0; i < _listenChannels.Length; i++)
			{
				while (SteamUnityAPI_SteamNetworking_IsP2PPacketAvailable(_networking, out packetSize, _listenChannels[i]))
				{
					IntPtr dataPtr = Marshal.AllocHGlobal((Int32)packetSize);
					UInt64 steamID;

					if (SteamUnityAPI_SteamNetworking_ReadP2PPacket(_networking, dataPtr, packetSize, out steamID, _listenChannels[i]))
					{
						Byte[] data = new Byte[packetSize];
						Marshal.Copy(dataPtr, data, 0, (Int32)packetSize);

						_onP2PPacketReceived(new SteamID(steamID), data, _listenChannels[i]);
					}

					Marshal.FreeHGlobal(dataPtr);
				}
			}
		}

		public void CloseP2PSession(SteamID steamID)
		{
			SteamUnityAPI_SteamNetworking_CloseP2PSession(_networking, steamID.ToUInt64());
		}

		public void CloseP2PSession(SteamID steamID, Int32 channel)
		{
			SteamUnityAPI_SteamNetworking_CloseP2PChannel(_networking, steamID.ToUInt64(), channel);
		}

		public Boolean IsInitialized
		{
			get { return _isInitialized; }
		}
	}
}

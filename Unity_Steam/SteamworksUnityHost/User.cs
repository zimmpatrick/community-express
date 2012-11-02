// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;

namespace CommunityExpressNS
{
	using HSteamUser = Int32;
	using AppId_t = UInt32;
	using SteamAPICall_t = UInt64;

	public enum EUserHasLicenseForAppResult
	{
		k_EUserHasLicenseResultHasLicense = 0,					// User has a license for specified app
		k_EUserHasLicenseResultDoesNotHaveLicense = 1,			// User does not have a license for the specified app
		k_EUserHasLicenseResultNoAuth = 2,						// User has not been authenticated
	}

	delegate void OnUserGetEncryptedAppTicketFromSteam();
	public delegate void OnUserEncryptedAppTicketCreated(Byte[] ticket);

	public class User
	{
		[DllImport("CommunityExpressSW.dll")]
		private static extern IntPtr SteamUnityAPI_SteamUser();
		[DllImport("CommunityExpressSW.dll")]
		private static extern Boolean SteamUnityAPI_SteamUser_BLoggedOn(IntPtr user);
		[DllImport("CommunityExpressSW.dll")]
		private static extern HSteamUser SteamUnityAPI_SteamUser_GetHSteamUser(IntPtr user);
		[DllImport("CommunityExpressSW.dll")]
		private static extern UInt64 SteamUnityAPI_SteamUser_GetSteamID(IntPtr user);
		[DllImport("CommunityExpressSW.dll")]
		private static extern Int32 SteamUnityAPI_SteamUser_InitiateGameConnection(IntPtr user,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] Byte[] authTicket, Int32 authTicketMaxSize, UInt64 serverSteamID,
			UInt32 serverIP, UInt16 serverPort, Boolean isSecure);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamUser_TerminateGameConnection(IntPtr user, UInt32 serverIP, UInt16 serverPort);
		[DllImport("CommunityExpressSW.dll")]
		private static extern EUserHasLicenseForAppResult SteamUnityAPI_SteamUser_UserHasLicenseForApp(IntPtr user, UInt64 steamID, AppId_t appID);
		[DllImport("CommunityExpressSW.dll")]
		private static extern Boolean SteamUnityAPI_SteamUser_BIsBehindNAT(IntPtr user);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamUser_AdvertiseGame(IntPtr user, UInt64 gameServerSteamID, UInt32 serverIP, UInt16 port);
		[DllImport("CommunityExpressSW.dll")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamUser_RequestEncryptedAppTicket(IntPtr user, IntPtr dataToInclude, Int32 dataLength);
		[DllImport("CommunityExpressSW.dll")]
		private static extern Boolean SteamUnityAPI_SteamUser_GetEncryptedAppTicket(IntPtr user,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] Byte[] ticket, int maxTicket, out UInt32 ticketSize);
		[DllImport("CommunityExpressSW.dll")]
		[return: MarshalAs(UnmanagedType.LPStr, SizeParamIndex = 0)]
		private static extern String SteamUnityAPI_GetPersonaNameByID(UInt64 steamID);

		private const Int32 AuthTicketSizeMax = 2048;

		private IntPtr _user;
		private Friends _friends; // for user avatar

		private UInt32 _serverIP = 0;
		private UInt16 _serverPort;

		private OnUserEncryptedAppTicketCreated _onUserEncryptedAppTicketCreated;

		internal User()
		{
			_user = SteamUnityAPI_SteamUser();
			_friends = new Friends();
		}

		~User()
		{
			OnDisconnect();
		}

		public Boolean InitiateClientAuthentication(out Byte[] authTicket, SteamID steamIDServer, IPAddress ipServer, UInt16 serverPort, Boolean isSecure)
		{
			Byte[] serverIPBytes = ipServer.GetAddressBytes();
			_serverIP = (UInt32)serverIPBytes[0] << 24 | (UInt32)serverIPBytes[1] << 16 | (UInt32)serverIPBytes[2] << 8 | (UInt32)serverIPBytes[3];
			_serverPort = serverPort;

			Byte[] internalAuthTicket = new Byte[AuthTicketSizeMax];
			Int32 authTicketSize;

			authTicketSize = SteamUnityAPI_SteamUser_InitiateGameConnection(_user, internalAuthTicket, AuthTicketSizeMax, steamIDServer.ToUInt64(), _serverIP, _serverPort, isSecure);

			if (authTicketSize > 0)
			{
				authTicket = new Byte[authTicketSize];

				for (Int32 i = 0; i < authTicketSize; i++)
				{
					authTicket[i] = internalAuthTicket[i];
				}

				return true;
			}

			// Failed to generate Auth Ticket
			authTicket = null;
			return false;
		}

		public void OnDisconnect()
		{
			if (_serverIP != 0)
			{
				SteamUnityAPI_SteamUser_TerminateGameConnection(_user, _serverIP, _serverPort);
			}
		}

		public EUserHasLicenseForAppResult UserHasLicenseForApp(AppId_t appID)
		{
			return SteamUnityAPI_SteamUser_UserHasLicenseForApp(_user, SteamID.ToUInt64(), appID);
		}

		public EUserHasLicenseForAppResult UserHasLicenseForApp(SteamID steamID, AppId_t appID)
		{
			return SteamUnityAPI_SteamUser_UserHasLicenseForApp(_user, steamID.ToUInt64(), appID);
		}

		public void AdvertiseGame(SteamID gameServerSteamID, IPAddress ip, UInt16 port)
		{
			Byte[] serverIPBytes = ip.GetAddressBytes();
			UInt32 serverIP = (UInt32)serverIPBytes[0] << 24 | (UInt32)serverIPBytes[1] << 16 | (UInt32)serverIPBytes[2] << 8 | (UInt32)serverIPBytes[3];

			SteamUnityAPI_SteamUser_AdvertiseGame(_user, gameServerSteamID.ToUInt64(), serverIP, port);
		}

		public void RequestEncryptedAppTicket(Byte[] dataToInclude, OnUserEncryptedAppTicketCreated onUserEncryptedAppTicketCreated)
		{
			_onUserEncryptedAppTicketCreated = onUserEncryptedAppTicketCreated;

			IntPtr dataPtr = Marshal.AllocHGlobal(dataToInclude.Length);
			Marshal.Copy(dataToInclude, 0, dataPtr, dataToInclude.Length);

			SteamAPICall_t callbackId = SteamUnityAPI_SteamUser_RequestEncryptedAppTicket(_user, dataPtr, dataToInclude.Length);

			CommunityExpress.Instance.AddUserGetEncryptedAppTicketCallback(callbackId, OnGetEncryptedAppTicketFromSteam);

			Marshal.FreeHGlobal(dataPtr);
		}

		private void OnGetEncryptedAppTicketFromSteam()
		{
			Byte[] ticket = null, internalTicket = new Byte[AuthTicketSizeMax];
			UInt32 ticketSize;

			if (SteamUnityAPI_SteamUser_GetEncryptedAppTicket(_user, internalTicket, AuthTicketSizeMax, out ticketSize))
			{
				ticket = new Byte[ticketSize];

				for (Int32 i = 0; i < ticketSize; i++)
				{
					ticket[i] = internalTicket[i];
				}
			}

			_onUserEncryptedAppTicketCreated(ticket);
		}

		public void GetLargeAvatar(OnLargeAvatarReceived largeAvatarReceivedCallback)
		{
			_friends.GetLargeFriendAvatar(SteamID, largeAvatarReceivedCallback);
		}

		public Boolean IsBehindNAT
		{
			get { return SteamUnityAPI_SteamUser_BIsBehindNAT(_user); }
		}

		public Boolean LoggedOn
		{
			get { return SteamUnityAPI_SteamUser_BLoggedOn(_user); }
		}

		public HSteamUser HSteamUser
		{
			get { return SteamUnityAPI_SteamUser_GetHSteamUser(_user); }
		}

		public SteamID SteamID
		{
			get { return new SteamID(SteamUnityAPI_SteamUser_GetSteamID(_user)); }
		}

		public String PersonaName
		{
			get { return SteamUnityAPI_GetPersonaNameByID(SteamID.ToUInt64()); }
		}

		public Image SmallAvatar
		{
			get { return _friends.GetSmallFriendAvatar(SteamID); }
		}

		public Image MediumAvatar
		{
			get { return _friends.GetMediumFriendAvatar(SteamID); }
		}

		public Image LargeAvatar
		{
			get { return _friends.GetLargeFriendAvatar(SteamID, null); }
		}
	}
}
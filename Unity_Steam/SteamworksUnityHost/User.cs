using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;

namespace SteamworksUnityHost
{
	using HSteamUser = Int32;

	public class User
	{
		[DllImport("SteamworksUnity.dll")]
		private static extern IntPtr SteamUnityAPI_SteamUser();
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamUser_BLoggedOn(IntPtr user);
		[DllImport("SteamworksUnity.dll")]
		private static extern HSteamUser SteamUnityAPI_SteamUser_GetHSteamUser(IntPtr user);
		[DllImport("SteamworksUnity.dll")]
		private static extern UInt64 SteamUnityAPI_SteamUser_GetSteamID(IntPtr user);
		[DllImport("SteamworksUnity.dll")]
		private static extern Int32 SteamUnityAPI_SteamUser_InitiateGameConnection(IntPtr user, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] Byte[] authTicket, Int32 authTicketMaxSize, UInt64 serverSteamID, UInt32 serverIP, UInt16 serverPort, Boolean isSecure);
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamUser_TerminateGameConnection(IntPtr user, UInt32 serverIP, UInt16 serverPort);

		private const Int32 AuthTicketSizeMax = 2048;

		private IntPtr _user;

		private UInt32 _serverIP = 0;
		private UInt16 _serverPort;

		internal User()
		{
			_user = SteamUnityAPI_SteamUser();
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
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SteamworksUnityHost
{
	public class User
	{
		[DllImport("SteamworksUnity.dll")]
		private static extern IntPtr SteamUnityAPI_SteamUser();
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_SteamUser_BLoggedOn(IntPtr user);
		[DllImport("SteamworksUnity.dll")]
		private static extern int SteamUnityAPI_SteamUser_GetHSteamUser(IntPtr user);
		[DllImport("SteamworksUnity.dll")]
		private static extern UInt64 SteamUnityAPI_SteamUser_GetSteamID(IntPtr user);

		private IntPtr _user;

		internal User()
		{
			_user = SteamUnityAPI_SteamUser();
		}

		public bool LoggedOn
		{
			get { return SteamUnityAPI_SteamUser_BLoggedOn(_user); }
		}

		public int HSteamUser
		{
			get { return SteamUnityAPI_SteamUser_GetHSteamUser(_user); }
		}

		public SteamID SteamID
		{
			get { return new SteamID(SteamUnityAPI_SteamUser_GetSteamID(_user)); }
		}
	}
}

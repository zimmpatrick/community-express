// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
	using AppId_t = UInt32;

	public class App
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamApps();
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamApps_GetCurrentGameLanguage(IntPtr apps);
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamApps_GetAvailableGameLanguages(IntPtr apps);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.Bool)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsSubscribedApp(IntPtr apps, AppId_t appID);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.Bool)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsDlcInstalled(IntPtr apps, AppId_t appID);
		[DllImport("CommunityExpressSW")]
		private static extern UInt32 SteamUnityAPI_SteamApps_GetEarliestPurchaseUnixTime(IntPtr apps, AppId_t appID);
		[DllImport("CommunityExpressSW")]
		private static extern Int32 SteamUnityAPI_SteamApps_GetDLCCount(IntPtr apps);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.Bool)]
		private static extern Boolean SteamUnityAPI_SteamApps_BGetDLCDataByIndex(IntPtr apps, Int32 index, out AppId_t appID,
            [MarshalAs(UnmanagedType.Bool)] out Boolean available, IntPtr name, Int32 maxNameLength);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamApps_InstallDLC(IntPtr apps, AppId_t appID);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamApps_UninstallDLC(IntPtr apps, AppId_t appID);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.Bool)]
		private static extern Boolean SteamUnityAPI_SteamApps_GetCurrentBetaName(IntPtr apps, IntPtr name, Int32 maxNameLength);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.Bool)]
		private static extern Boolean SteamUnityAPI_SteamApps_MarkContentCorrupt(IntPtr apps,
            [MarshalAs(UnmanagedType.Bool)] Boolean missingFilesOnly);
		[DllImport("CommunityExpressSW")]
		private static extern UInt32 SteamUnityAPI_SteamApps_GetAppInstallDir(IntPtr apps, AppId_t appID, IntPtr directoryDataPtr,
			Int32 maxDirNameLength);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.Bool)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsSubscribed(IntPtr apps);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.Bool)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsSubscribedFromFreeWeekend(IntPtr apps);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.Bool)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsLowViolence(IntPtr apps);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.Bool)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsCybercafe(IntPtr apps);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.Bool)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsVACBanned(IntPtr apps);

		private IntPtr _apps;

		internal App()
		{
			_apps = SteamUnityAPI_SteamApps();
		}

		// Returns one of the following: english, german, french, italian, koreana, spanish, schinese, tchinese,
		// russian, thai, japanese, portuguese, polish, danish, dutch, finnish, norwegian, swedish, hungarian, czech,
		// romanian, turkish, brazilian, Bulgarian
		public String GetCurrentGameLanguage()
		{
			return Marshal.PtrToStringAnsi(SteamUnityAPI_SteamApps_GetCurrentGameLanguage(_apps));
		}

		public String GetAvailableGameLanguages()
		{
			return Marshal.PtrToStringAnsi(SteamUnityAPI_SteamApps_GetAvailableGameLanguages(_apps));
		}

		// Only use this with AppIDs related with your game, such as a demo
		public Boolean IsSubscribedApp(AppId_t appID)
		{
			return SteamUnityAPI_SteamApps_BIsSubscribedApp(_apps, appID);
		}

		// Only use this with AppIDs related with your game, such as a demo
		public Boolean IsDlcInstalled(AppId_t appID)
		{
			return SteamUnityAPI_SteamApps_BIsDlcInstalled(_apps, appID);
		}

		public DateTime GetEarliestPurchaseUnixTime(AppId_t appID)
		{
			DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return epoch.AddSeconds(SteamUnityAPI_SteamApps_GetEarliestPurchaseUnixTime(_apps, appID));
		}

		public Int32 GetDLCCount()
		{
			return SteamUnityAPI_SteamApps_GetDLCCount(_apps);
		}

		public Boolean GetDLCDataByIndex(Int32 index, out AppId_t appID, out Boolean available, out String name, Int32 maxNameLength)
		{
			IntPtr nameDataPtr = Marshal.AllocHGlobal(maxNameLength + 1);
			Boolean result = SteamUnityAPI_SteamApps_BGetDLCDataByIndex(_apps, index, out appID, out available, nameDataPtr, maxNameLength);
			name = Marshal.PtrToStringAnsi(nameDataPtr);
			Marshal.FreeHGlobal(nameDataPtr);
			return result;
		}

		public void InstallDLC(AppId_t appID)
		{
			SteamUnityAPI_SteamApps_InstallDLC(_apps, appID);
		}

		public void UninstallDLC(AppId_t appID)
		{
			SteamUnityAPI_SteamApps_UninstallDLC(_apps, appID);
		}

		public Boolean GetCurrentBetaName(out String name, Int32 maxNameLength)
		{
			IntPtr nameDataPtr = Marshal.AllocHGlobal(maxNameLength + 1);
			Boolean result = SteamUnityAPI_SteamApps_GetCurrentBetaName(_apps, nameDataPtr, maxNameLength);
			name = Marshal.PtrToStringAnsi(nameDataPtr);
			Marshal.FreeHGlobal(nameDataPtr);
			return result;
		}

		public Boolean MarkContentCorrupt(Boolean missingFilesOnly)
		{
			return SteamUnityAPI_SteamApps_MarkContentCorrupt(_apps, missingFilesOnly);
		}

		public UInt32 GetAppInstallDir(AppId_t appID, out String directory, Int32 maxDirNameLength)
		{
			IntPtr directoryDataPtr = Marshal.AllocHGlobal(maxDirNameLength + 1);
			UInt32 result = SteamUnityAPI_SteamApps_GetAppInstallDir(_apps, appID, directoryDataPtr, maxDirNameLength);
			directory = Marshal.PtrToStringAnsi(directoryDataPtr);
			Marshal.FreeHGlobal(directoryDataPtr);
			return result;
		}

		public Boolean IsSubscribed
		{
			get { return SteamUnityAPI_SteamApps_BIsSubscribed(_apps); }
		}

		public Boolean IsSubscribedFromFreeWeekend
		{
			get { return SteamUnityAPI_SteamApps_BIsSubscribedFromFreeWeekend(_apps); }
		}

		public Boolean IsLowViolence
		{
			get { return SteamUnityAPI_SteamApps_BIsLowViolence(_apps); }
		}

		public Boolean IsCybercafe
		{
			get { return SteamUnityAPI_SteamApps_BIsCybercafe(_apps); }
		}

		public Boolean IsVACBanned
		{
			get { return SteamUnityAPI_SteamApps_BIsVACBanned(_apps); }
		}
	}
}

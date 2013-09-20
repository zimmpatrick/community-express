// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
	using AppId_t = UInt32;

    /// <summary>
    /// Information about an app
    /// </summary>
	public class App
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamApps();
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamApps_GetCurrentGameLanguage(IntPtr apps);
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamApps_GetAvailableGameLanguages(IntPtr apps);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsSubscribedApp(IntPtr apps, AppId_t appID);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsDlcInstalled(IntPtr apps, AppId_t appID);
		[DllImport("CommunityExpressSW")]
		private static extern UInt32 SteamUnityAPI_SteamApps_GetEarliestPurchaseUnixTime(IntPtr apps, AppId_t appID);
		[DllImport("CommunityExpressSW")]
		private static extern Int32 SteamUnityAPI_SteamApps_GetDLCCount(IntPtr apps);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
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
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamApps_MarkContentCorrupt(IntPtr apps,
            [MarshalAs(UnmanagedType.Bool)] Boolean missingFilesOnly);
		[DllImport("CommunityExpressSW")]
		private static extern UInt32 SteamUnityAPI_SteamApps_GetAppInstallDir(IntPtr apps, AppId_t appID, IntPtr directoryDataPtr,
			Int32 maxDirNameLength);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsSubscribed(IntPtr apps);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsSubscribedFromFreeWeekend(IntPtr apps);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsLowViolence(IntPtr apps);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsCybercafe(IntPtr apps);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamApps_BIsVACBanned(IntPtr apps);

		private IntPtr _apps;

		internal App()
		{
			_apps = SteamUnityAPI_SteamApps();
		}

        /// <summary>
        /// Gets the current user's language
        /// </summary>
        /// <returns>One of the following: english, german, french, italian, koreana, spanish, schinese, tchinese,
        /// russian, thai, japanese, portuguese, polish, danish, dutch, finnish, norwegian, swedish, hungarian, czech,
        /// romanian, turkish, brazilian, bulgarian</returns>
		public String GetCurrentGameLanguage()
		{
			return Marshal.PtrToStringAnsi(SteamUnityAPI_SteamApps_GetCurrentGameLanguage(_apps));
		}
        /// <summary>
        /// Gets available game languages
        /// </summary>
        /// <returns>One or more of the following: english, german, french, italian, koreana, spanish, schinese, tchinese,
        /// russian, thai, japanese, portuguese, polish, danish, dutch, finnish, norwegian, swedish, hungarian, czech,
        /// romanian, turkish, brazilian, bulgarian</returns>
		public String GetAvailableGameLanguages()
		{
			return Marshal.PtrToStringAnsi(SteamUnityAPI_SteamApps_GetAvailableGameLanguages(_apps));
		}

        /// <summary>
        /// Only use this member if you need to check ownership of another game related to yours, a demo for example
        /// </summary>
        /// <param name="appID">appID to check</param>
        /// <returns>true if the app is subscribed</returns>
		public Boolean IsSubscribedApp(AppId_t appID)
		{
			return SteamUnityAPI_SteamApps_BIsSubscribedApp(_apps, appID);
		}

        /// <summary>
        /// Takes AppID of DLC and checks if the user owns the DLC &amp; if the DLC is installed
        /// </summary>
        /// <param name="appID">appID to check</param>
        /// <returns>true if the DLC is installed</returns>
		public Boolean IsDlcInstalled(AppId_t appID)
		{
			return SteamUnityAPI_SteamApps_BIsDlcInstalled(_apps, appID);
		}


        /// <summary>
        /// Returns the date/time of the purchase of the app
        /// </summary>
        /// <param name="appID">appID to check</param>
        /// <returns>date/time of the purchase</returns>
		public DateTime GetEarliestPurchaseDateTime(AppId_t appID)
		{
			DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return epoch.AddSeconds(SteamUnityAPI_SteamApps_GetEarliestPurchaseUnixTime(_apps, appID));
		}
        /// <summary>
        /// Counts DLC for the app
        /// </summary>
        /// <returns>Amount of DLC</returns>
		public Int32 GetDLCCount()
		{
			return SteamUnityAPI_SteamApps_GetDLCCount(_apps);
		}
        /// <summary>
        /// Indexes the list of DLC for the app
        /// </summary>
        /// <param name="index">Name of the index</param>
        /// <param name="appID">ID of the Steam app</param>
        /// <param name="available">If the DLC is available yet</param>
        /// <param name="name">Name of the DLC</param>
        /// <param name="maxNameLength">Maximum length of the DLC name</param>
        /// <returns>true if the list is created</returns>
		public Boolean GetDLCDataByIndex(Int32 index, out AppId_t appID, out Boolean available, out String name, Int32 maxNameLength)
		{
			IntPtr nameDataPtr = Marshal.AllocHGlobal(maxNameLength + 1);
			Boolean result = SteamUnityAPI_SteamApps_BGetDLCDataByIndex(_apps, index, out appID, out available, nameDataPtr, maxNameLength);
			name = Marshal.PtrToStringAnsi(nameDataPtr);
			Marshal.FreeHGlobal(nameDataPtr);
			return result;
		}
        /// <summary>
        /// Installs the selected DLC for the app
        /// </summary>
        /// <param name="appID">App the DLC should be installed to</param>
		public void InstallDLC(AppId_t appID)
		{
			SteamUnityAPI_SteamApps_InstallDLC(_apps, appID);
		}
        /// <summary>
        /// Uninstalls the selected DLC for the app
        /// </summary>
        /// <param name="appID">App the DLC should be removed from</param>
		public void UninstallDLC(AppId_t appID)
		{
			SteamUnityAPI_SteamApps_UninstallDLC(_apps, appID);
		}
        /// <summary>
        /// Gets the name of the app's beta
        /// </summary>
        /// <param name="name">Name of the beta</param>
        /// <param name="maxNameLength">Maximum length of the beta name</param>
        /// <returns>true if the name is gotten</returns>
		public Boolean GetCurrentBetaName(out String name, Int32 maxNameLength)
		{
			IntPtr nameDataPtr = Marshal.AllocHGlobal(maxNameLength + 1);
			Boolean result = SteamUnityAPI_SteamApps_GetCurrentBetaName(_apps, nameDataPtr, maxNameLength);
			name = Marshal.PtrToStringAnsi(nameDataPtr);
			Marshal.FreeHGlobal(nameDataPtr);
			return result;
		}
        /// <summary>
        /// Marks content that has been corrupted
        /// </summary>
        /// <param name="missingFilesOnly">If only missing files should be marked</param>
        /// <returns>true if content is marked</returns>
		public Boolean MarkContentCorrupt(Boolean missingFilesOnly)
		{
			return SteamUnityAPI_SteamApps_MarkContentCorrupt(_apps, missingFilesOnly);
		}
        /// <summary>
        /// Finds the directory the app will be installed to
        /// </summary>
        /// <param name="appID">ID of the Steam app</param>
        /// <param name="directory">Where the app will be installed</param>
        /// <param name="maxDirNameLength">Maximum length of the directory name</param>
        /// <returns>The directory where the app will be installed</returns>
		public UInt32 GetAppInstallDir(AppId_t appID, out String directory, Int32 maxDirNameLength)
		{
			IntPtr directoryDataPtr = Marshal.AllocHGlobal(maxDirNameLength + 1);
			UInt32 result = SteamUnityAPI_SteamApps_GetAppInstallDir(_apps, appID, directoryDataPtr, maxDirNameLength);
			directory = Marshal.PtrToStringAnsi(directoryDataPtr);
			Marshal.FreeHGlobal(directoryDataPtr);
			return result;
		}
        /// <summary>
        /// If the user owns the app
        /// </summary>
		public Boolean IsSubscribed
		{
			get { return SteamUnityAPI_SteamApps_BIsSubscribed(_apps); }
		}
        /// <summary>
        /// If the user owns the app from a free weekend event
        /// </summary>
		public Boolean IsSubscribedFromFreeWeekend
		{
			get { return SteamUnityAPI_SteamApps_BIsSubscribedFromFreeWeekend(_apps); }
		}
        /// <summary>
        /// If the app has low violence
        /// </summary>
		public Boolean IsLowViolence
		{
			get { return SteamUnityAPI_SteamApps_BIsLowViolence(_apps); }
		}
        /// <summary>
        /// If the app is running in a cyber cafe
        /// </summary>
		public Boolean IsCybercafe
		{
			get { return SteamUnityAPI_SteamApps_BIsCybercafe(_apps); }
		}
        /// <summary>
        /// If the app is banned from Valve Anti-Cheat
        /// </summary>
		public Boolean IsVACBanned
		{
			get { return SteamUnityAPI_SteamApps_BIsVACBanned(_apps); }
		}
	}
}

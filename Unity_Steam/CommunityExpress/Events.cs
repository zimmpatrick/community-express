using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
    using SteamAPICall_t = UInt64;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// The country of the user changed
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    struct IPCountry_t
    {
	    internal const int k_iCallback = Events.k_iSteamUtilsCallbacks + 1;
    };

    /// <summary>
    /// Fired when running on a laptop and less than 10 minutes of battery is left, fires then every minute
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct LowBatteryPower_t
    {
	    internal const int k_iCallback = Events.k_iSteamUtilsCallbacks + 2;

        internal byte m_nMinutesBatteryLeft;
    };


    /// <summary>
    /// Called when a SteamAsyncCall_t has completed (or failed)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    struct SteamAPICallCompleted_t
    {
	    internal const int k_iCallback = Events.k_iSteamUtilsCallbacks + 3;
	
        public SteamAPICall_t m_hAsyncCall;
    };

    /// <summary>
    /// Called when Steam wants to shutdown
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct SteamShutdown_t
    {
	    internal const int k_iCallback = Events.k_iSteamUtilsCallbacks + 4;
    };

    //-----------------------------------------------------------------------------
    // results for CheckFileSignature
    //-----------------------------------------------------------------------------
    enum ECheckFileSignature
    {
	    k_ECheckFileSignatureInvalidSignature = 0,
	    k_ECheckFileSignatureValidSignature = 1,
	    k_ECheckFileSignatureFileNotFound = 2,
	    k_ECheckFileSignatureNoSignaturesFoundForThisApp = 3,
	    k_ECheckFileSignatureNoSignaturesFoundForThisFile = 4,
    };

    /// <summary>
    /// Callback for CheckFileSignature
    /// </summary>
    struct CheckFileSignature_t
    {
	    internal const int k_iCallback = Events.k_iSteamUtilsCallbacks + 5;

        public ECheckFileSignature m_eCheckFileSignature;
    };

    public class Events
    {
        //-----------------------------------------------------------------------------
        // Purpose: Base values for callback identifiers, each callback must
        //			have a unique ID.
        //-----------------------------------------------------------------------------
        internal const int k_iSteamUserCallbacks = 100;
        internal const int k_iSteamGameServerCallbacks = 200;
        internal const int k_iSteamFriendsCallbacks = 300;
        internal const int k_iSteamBillingCallbacks = 400;
        internal const int k_iSteamMatchmakingCallbacks = 500;
        internal const int k_iSteamContentServerCallbacks = 600;
        internal const int k_iSteamUtilsCallbacks = 700;
        internal const int k_iClientFriendsCallbacks = 800;
        internal const int k_iClientUserCallbacks = 900;
        internal const int k_iSteamAppsCallbacks = 1000;
        internal const int k_iSteamUserStatsCallbacks = 1100;
        internal const int k_iSteamNetworkingCallbacks = 1200;
        internal const int k_iClientRemoteStorageCallbacks = 1300;
        internal const int k_iSteamUserItemsCallbacks = 1400;
        internal const int k_iSteamGameServerItemsCallbacks = 1500;
        internal const int k_iClientUtilsCallbacks = 1600;
        internal const int k_iSteamGameCoordinatorCallbacks = 1700;
        internal const int k_iSteamGameServerStatsCallbacks = 1800;
        internal const int k_iSteam2AsyncCallbacks = 1900;
        internal const int k_iSteamGameStatsCallbacks = 2000;
        internal const int k_iClientHTTPCallbacks = 2100;
        internal const int k_iClientScreenshotsCallbacks = 2200;
        internal const int k_iSteamScreenshotsCallbacks = 2300;
        internal const int k_iClientAudioCallbacks = 2400;
        internal const int k_iClientUnifiedMessagesCallbacks = 2500;
        internal const int k_iSteamStreamLauncherCallbacks = 2600;
        internal const int k_iClientControllerCallbacks = 2700;
        internal const int k_iSteamControllerCallbacks = 2800;
        internal const int k_iClientParentalSettingsCallbacks = 2900;
        internal const int k_iClientDeviceAuthCallbacks = 3000;
        internal const int k_iClientNetworkDeviceManagerCallbacks = 3100;
        internal const int k_iClientMusicCallbacks = 3200;
        internal const int k_iClientRemoteClientManagerCallbacks = 3300;
        internal const int k_iClientUGCCallbacks = 3400;
        internal const int k_iSteamStreamClientCallbacks = 3500;
        internal const int k_IClientProductBuilderCallbacks = 3600;
        internal const int k_iClientShortcutsCallbacks = 3700;
        internal const int k_iClientRemoteControlManagerCallbacks = 3800;
        internal const int k_iSteamAppListCallbacks = 3900;
        internal const int k_iSteamMusicCallbacks = 4000;

        public Events()
        {
            CommunityExpress _ce = CommunityExpress.Instance;

            _ce.AddEventHandler(LowBatteryPower_t.k_iCallback, new CommunityExpress.OnEventHandler<LowBatteryPower_t>(Events_LowBatteryPower));
            _ce.AddEventHandler(SteamShutdown_t.k_iCallback, new CommunityExpress.OnEventHandler<SteamShutdown_t>(Events_SteamShutdown));
        }
        /// <summary>
        /// Low battery
        /// </summary>
        /// <param name="minutesLeft">Number of minutes left</param>
        public delegate void LowBatteryHandler(int minutesLeft);
        /// <summary>
        /// Battery charge is low
        /// </summary>
        public event LowBatteryHandler LowBattery;
        private void Events_LowBatteryPower(LowBatteryPower_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (LowBattery != null)
            {
                LowBattery(recv.m_nMinutesBatteryLeft);
            }
        }

        /// <summary>
        /// Steam Shutdown
        /// </summary>
        public delegate void SteamShutdownHandler();
        /// <summary>
        /// Steam wants to shut down
        /// </summary>
        public event SteamShutdownHandler SteamShutdown;
        private void Events_SteamShutdown(SteamShutdown_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (SteamShutdown != null)
            {
                SteamShutdown();
            }
        }
    }
}

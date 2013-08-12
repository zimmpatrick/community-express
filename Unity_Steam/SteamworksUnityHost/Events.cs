using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using CommunityExpressNS.EventsNS;

namespace CommunityExpressNS
{
    using SteamAPICall_t = UInt64;
    using System.Diagnostics;

    
    /// <summary>
    /// The country of the user changed
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    struct IPCountry_t
    {
	    internal const int k_iCallback = Events.k_iSteamUtilsCallbacks + 1;
    };

    /// <summary>
    /// Fired when running on a laptop and less than 10 minutes of battery is left, fires then every minute
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    struct LowBatteryPower_t
    {
	    internal const int k_iCallback = Events.k_iSteamUtilsCallbacks + 2;

        public byte m_nMinutesBatteryLeft;
    };


    /// <summary>
    /// Called when a SteamAsyncCall_t has completed (or failed)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    struct SteamAPICallCompleted_t
    {
	    internal const int k_iCallback = Events.k_iSteamUtilsCallbacks + 3;
	
        public SteamAPICall_t m_hAsyncCall;
    };

    /// <summary>
    /// Called when Steam wants to shutdown
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    struct SteamShutdown_t
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


    namespace EventsNS
    {
        public delegate void UserStatsReceivedHandler(Stats sender, UserStatsReceivedArgs e);
        public delegate void UserStatsStoredHandler(Stats sender, UserStatsStoredArgs e);

        public class UserStatsReceivedArgs : System.EventArgs
        {
            internal UserStatsReceivedArgs(UserStatsReceived_t args)
            {
                GameID = args.m_nGameID;
                Result = args.m_eResult;
                SteamID = new SteamID(args.m_steamIDUser);
            }

            public UInt64 GameID
            {
                get;
                private set;
            }

            public EResult Result
            {
                get;
                private set;
            }

            public SteamID SteamID
            {
                get;
                private set;
            }
        }

        public class UserStatsStoredArgs : System.EventArgs
        {
            internal UserStatsStoredArgs(UserStatsStored_t args)
            {
                GameID = args.m_nGameID;
                Result = args.m_eResult;
            }

            public UInt64 GameID
            {
                get;
                private set;
            }

            public EResult Result
            {
                get;
                private set;
            }
        }
    }
            

    public class Events
    {
        [DllImport("CommunityExpressSW")]
        private static extern Boolean SteamUnityAPI_SteamUtils_IsAPICallCompleted(SteamAPICall_t callHandle, out Byte failed);
        [DllImport("CommunityExpressSW")]
        private static extern Boolean SteamUnityAPI_SteamUtils_GetAPICallResult(SteamAPICall_t callHandle, IntPtr pCallback, int cubCallback, int iCallbackExpected, out Byte failed);


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

        public event UserStatsReceivedHandler UserStatsReceived;
        public event UserStatsStoredHandler UserStatsStored;

        private Dictionary<int, Type> _internalEventFactory = new Dictionary<int, Type>();
        private Dictionary<SteamAPICall_t, IAsynchronousCall> _apiCalls = new Dictionary<SteamAPICall_t, IAsynchronousCall>();

        internal Events()
        {
            _internalEventFactory.Add(UserStatsReceived_t.k_iCallback, typeof(UserStatsReceived_t));
            _internalEventFactory.Add(UserStatsStored_t.k_iCallback, typeof(UserStatsStored_t));
            _internalEventFactory.Add(SteamAPICallCompleted_t.k_iCallback, typeof(SteamAPICallCompleted_t));
        }

        internal AsynchronousCall<T, E> AddAsynchronousCall<T, E>(T sender, SteamAPICall_t asyncCall) where E : class
        {
            AsynchronousCall<T, E> call = new AsynchronousCall<T, E>(sender, asyncCall);

            _apiCalls.Add(asyncCall, call.IAsynchronousCall);
            return call;
        }


        internal void OnEvent(Int32 k_iCallback, IntPtr pvParam, Boolean bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            // internal callbacks


            // user callbacks
            if (k_iCallback == UserStatsReceived_t.k_iCallback)
            {
                UserStatsReceived_t p = (UserStatsReceived_t)Marshal.PtrToStructure(pvParam, typeof(UserStatsReceived_t));
                UserStatsReceivedArgs a = new UserStatsReceivedArgs(p);
                UserStatsReceived(CommunityExpress.Instance.UserStats, a);

                Console.WriteLine(p);
            }
            else if (k_iCallback == UserStatsStored_t.k_iCallback)
            {
                if (UserStatsStored != null)
                {
                    UserStatsStored_t p = (UserStatsStored_t)Marshal.PtrToStructure(pvParam, typeof(UserStatsStored_t));
                    UserStatsStoredArgs a = new UserStatsStoredArgs(p);
                    UserStatsStored(CommunityExpress.Instance.UserStats, a);
                }
            }
            else if (k_iCallback == SteamShutdown_t.k_iCallback)
            {
                Console.WriteLine("shutdown");

                Process.GetCurrentProcess().Kill();
            }
            else if (k_iCallback == SteamAPICallCompleted_t.k_iCallback)
            {
                SteamAPICallCompleted_t p = (SteamAPICallCompleted_t)Marshal.PtrToStructure(pvParam, typeof(SteamAPICallCompleted_t));

                if (_apiCalls.ContainsKey(p.m_hAsyncCall))
                {
                    IAsynchronousCall call = _apiCalls[p.m_hAsyncCall];

                    int sizeOf = Marshal.SizeOf(typeof(LeaderboardFindResult_t));
                    IntPtr unmanagedAddr = Marshal.AllocHGlobal(sizeOf);

                    byte failed;
                    SteamUnityAPI_SteamUtils_GetAPICallResult(p.m_hAsyncCall, unmanagedAddr, sizeOf, LeaderboardFindResult_t.k_iCallback, out failed);
                    LeaderboardFindResult_t findLearderboardResult = (LeaderboardFindResult_t)Marshal.PtrToStructure(unmanagedAddr, typeof(LeaderboardFindResult_t));

                    Marshal.FreeHGlobal(unmanagedAddr);
                    unmanagedAddr = IntPtr.Zero;

                    Leaderboards.LeaderboardRecievedArgs hack = new Leaderboards.LeaderboardRecievedArgs(call.Sender as Leaderboards, findLearderboardResult);

                    call.Complete(hack.Leaderboard);
                }
            }
            else
            {

                Console.WriteLine(k_iCallback);
            }


        }

    }
}

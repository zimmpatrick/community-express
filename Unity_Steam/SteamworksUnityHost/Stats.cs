// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
	using SteamAPICall_t = UInt64;

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	struct UserStatsReceived_t
	{
        internal const int k_iCallback = Events.k_iSteamUserStatsCallbacks + 1;

		public UInt64 m_nGameID;		// Game these stats are for
		public EResult m_eResult;		// Success / error fetching the stats
		public UInt64 m_steamIDUser;	// The user for whom the stats are retrieved for
	}

    /// <summary>
    /// Result of a request to store the user stats for a game
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    struct UserStatsStored_t
    {
        internal const int k_iCallback = Events.k_iSteamUserStatsCallbacks + 2;

        public UInt64 m_nGameID;		// Game these stats are for
        public EResult m_eResult;		// Success / error fetching the stats
    }

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct GSStatsReceived_t
	{
		public EResult m_eResult;		// Success / error fetching the stats
		public UInt64 m_steamIDUser;	// The user for whom the stats are retrieved for
	}

	delegate void OnUserStatsReceivedFromSteam(ref UserStatsReceived_t CallbackData);
	public delegate void OnUserStatsReceived(Stats stats, Achievements achievements);

	public class Stats : ICollection<Stat>
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats();
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamUserStats_RequestCurrentStats(IntPtr stats);
        [DllImport("CommunityExpressSW")]
        private static extern Boolean SteamUnityAPI_SteamUserStats_StoreStats(IntPtr stats);
        [DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamGameServerStats();
		[DllImport("CommunityExpressSW")]
        private static extern SteamAPICall_t SteamUnityAPI_SteamGameServerStats_RequestUserStats(IntPtr gameserverStats, UInt64 steamID);
        [DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamGameServerStats_StoreUserStats(IntPtr gameserverStats, UInt64 steamID);
        [DllImport("CommunityExpressSW")]
        private static extern Boolean SteamUnityAPI_SteamUserStats_ResetAllStats(IntPtr stats, Boolean achievementsToo);

        private CommunityExpress _ce;
		private IntPtr _stats = IntPtr.Zero;
		private IntPtr _gameserverStats = IntPtr.Zero;
		private SteamID _id;
		private List<Stat> _statList = new List<Stat>();
        private List<string> _requestedStats;
        private List<Type> _requestedStatTypes;

        private CommunityExpress.OnEventHandler<UserStatsReceived_t> _statsRecievedEventHandler = null;
        private CommunityExpress.OnEventHandler<UserStatsStored_t> _statsStoredEventHandler = null;

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

        public event UserStatsReceivedHandler UserStatsReceived;
        public event UserStatsStoredHandler UserStatsStored;

		internal Stats(CommunityExpress ce, SteamID steamID, Boolean isGameServer = false)
		{
            _ce = ce;
			_id = steamID;

			if (isGameServer)
			{
				_gameserverStats = SteamUnityAPI_SteamGameServerStats();
			}
			else
			{
                _stats = SteamUnityAPI_SteamUserStats();
			}

            _statsRecievedEventHandler = new CommunityExpress.OnEventHandler<UserStatsReceived_t>(Events_UserStatsReceived);
            _statsStoredEventHandler = new CommunityExpress.OnEventHandler<UserStatsStored_t>(Events_UserStatsStored);
		}

        private void Events_UserStatsReceived(UserStatsReceived_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (_id.ToUInt64() != recv.m_steamIDUser) return;

            _ce.RemoveEventHandler(UserStatsReceived_t.k_iCallback, _statsRecievedEventHandler);

            // Make sure we don't double up the list of Stats
            Clear();

            if (_gameserverStats != IntPtr.Zero)
            {
                for (int i = 0; i < _requestedStats.Count; i++)
                {
                    string s = _requestedStats[i];
                    Type t = _requestedStatTypes[i];

                    Add(new Stat(_gameserverStats, _id, s, t, true));
                }
            }
            else
            {
                for (int i = 0; i < _requestedStats.Count; i++)
                {
                    string s = _requestedStats[i];
                    Type t = _requestedStatTypes[i];

                    Add(new Stat(_stats, _id, s, t, false));
                }
            }

            if (UserStatsReceived != null) UserStatsReceived(this, new UserStatsReceivedArgs(recv));
        }

        private void Events_UserStatsStored(UserStatsStored_t stored, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            _ce.RemoveEventHandler(UserStatsStored_t.k_iCallback, _statsStoredEventHandler);

            if (UserStatsStored != null) UserStatsStored(this, new UserStatsStoredArgs(stored));
        }


        public void RequestCurrentStats(IEnumerable<String> requestedStats)
        {
            List<Type> requestedTypes = new List<Type>();
            foreach(string s in requestedStats)
            {
                requestedTypes.Add(typeof(int));
            }

            RequestCurrentStats(requestedStats, requestedTypes);
        }

        public void RequestCurrentStats(IEnumerable<String> requestedStats,
             IEnumerable<Type> requestedTypes)
		{
            _ce.AddEventHandler(UserStatsReceived_t.k_iCallback, _statsRecievedEventHandler);

			_requestedStats = new List<string>(requestedStats);
            _requestedStatTypes = new List<Type>(requestedTypes);

            if (_requestedStats.Count != _requestedStatTypes.Count)
            {
                throw new ArgumentException("requestedTypes Length doesn't match requestedStats Length", "requestedTypes");
            }

			if (_gameserverStats != IntPtr.Zero)
			{
				SteamUnityAPI_SteamGameServerStats_RequestUserStats(_gameserverStats, _id.ToUInt64());
			}
			else
			{
				SteamUnityAPI_SteamUserStats_RequestCurrentStats(_stats);
			}
		}

        /// <summary>
        /// Store the current data on the server, will get a callback when set
        /// And one callback for every new achievement
        ///
        /// If the callback has a result of k_EResultInvalidParam, one or more stats 
        /// uploaded has been rejected, either because they broke constraints
        /// or were out of date. In this case the server sends back updated values.
        /// The stats should be re-iterated to keep in sync.
        /// </summary>
		public void WriteStats()
        {
            _ce.AddEventHandler(UserStatsStored_t.k_iCallback, _statsStoredEventHandler);

			if (_gameserverStats != IntPtr.Zero)
			{
				SteamUnityAPI_SteamGameServerStats_StoreUserStats(_gameserverStats, _id.ToUInt64());
			}
			else
            {
				SteamUnityAPI_SteamUserStats_StoreStats(_stats);
			}
		}

        /// <summary>
        /// Reset stats 
        /// </summary>
        /// <param name="achievementsToo"></param>
        /// <returns></returns>
        public bool ResetAllStats(bool achievementsToo)
        { 
            return SteamUnityAPI_SteamUserStats_ResetAllStats(_stats, achievementsToo);
        }

		public SteamID SteamID
		{
			get { return _id; }
		}

		public IList<Stat> StatsList
		{
			get { return _statList; }
		}
		
		public int Count
		{
			get { return _statList.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void Add(Stat item)
		{
			_statList.Add(item);
		}

		public void Clear()
		{
			_statList.Clear();
		}

		public bool Contains(Stat item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(Stat[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(Stat item)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<Stat> GetEnumerator()
		{
			return new ListEnumerator<Stat>(_statList);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
	using SteamLeaderboard_t = UInt64;
    using SteamAPICall_t = UInt64;

	/// <summary>
    /// type of data request, when downloading leaderboard entries
	/// </summary>
	public enum ELeaderboardDataRequest
	{
		k_ELeaderboardDataRequestGlobal = 0,
		k_ELeaderboardDataRequestGlobalAroundUser = 1,
		k_ELeaderboardDataRequestFriends = 2,
	};

	/// <summary>
    /// the sort order of a leaderboard
	/// </summary>
	public enum ELeaderboardSortMethod
	{
		k_ELeaderboardSortMethodNone = 0,
		k_ELeaderboardSortMethodAscending = 1,	// top-score is lowest number
		k_ELeaderboardSortMethodDescending = 2,	// top-score is highest number
	};

	/// <summary>
    /// the display type (used by the Steam Community web site) for a leaderboard
	/// </summary>
	public enum ELeaderboardDisplayType
	{
		k_ELeaderboardDisplayTypeNone = 0,
		k_ELeaderboardDisplayTypeNumeric = 1,			// simple numerical score
		k_ELeaderboardDisplayTypeTimeSeconds = 2,		// the score represents a time, in seconds
		k_ELeaderboardDisplayTypeTimeMilliSeconds = 3,	// the score represents a time, in milliseconds
	};
    /// <summary>
    /// The method from uploading scores to a leaderboard
    /// </summary>
	public enum ELeaderboardUploadScoreMethod
	{
		k_ELeaderboardUploadScoreMethodNone = 0,
		k_ELeaderboardUploadScoreMethodKeepBest = 1,	// Leaderboard will keep user's best score
		k_ELeaderboardUploadScoreMethodForceUpdate = 2,	// Leaderboard will always replace score with specified
	};

    /// <summary>
    /// List of leaderboards
    /// </summary>
	public class Leaderboards : ICollection<Leaderboard>
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct LeaderboardFindResult_t
        {
            internal const int k_iCallback = Events.k_iSteamUserStatsCallbacks + 4;

            public SteamLeaderboard_t m_hSteamLeaderboard;	// handle to the leaderboard serarched for, 0 if no leaderboard found
            public Byte m_bLeaderboardFound;				// 0 if no leaderboard found
        };
        /// <summary>
        /// When the leaderboard is retrieved
        /// </summary>
        /// <param name="leaderboard">Leaderboard to retrieve</param>
        public delegate void OnLeaderboardRetrieved(Leaderboard leaderboard);

		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats();
		[DllImport("CommunityExpressSW")]
        private static extern SteamAPICall_t SteamUnityAPI_SteamUserStats_FindLeaderboard(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] String leaderboardName);
		[DllImport("CommunityExpressSW")]
        private static extern SteamAPICall_t SteamUnityAPI_SteamUserStats_FindOrCreateLeaderboard(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] String leaderboardName,
			ELeaderboardSortMethod sortMethod, ELeaderboardDisplayType displayType);
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats_GetLeaderboardName(IntPtr stats, SteamLeaderboard_t leaderboard);
		[DllImport("CommunityExpressSW")]
		private static extern Int32 SteamUnityAPI_SteamUserStats_GetLeaderboardEntryCount(IntPtr stats, SteamLeaderboard_t leaderboard);
		[DllImport("CommunityExpressSW")]
		private static extern ELeaderboardSortMethod SteamUnityAPI_SteamUserStats_GetLeaderboardSortMethod(IntPtr stats, SteamLeaderboard_t leaderboard);
		[DllImport("CommunityExpressSW")]
		private static extern ELeaderboardDisplayType SteamUnityAPI_SteamUserStats_GetLeaderboardDisplayType(IntPtr stats, SteamLeaderboard_t leaderboard);

        private CommunityExpress _ce;
		private IntPtr _stats;
		private List<Leaderboard> _leaderboardList = new List<Leaderboard>();

        private CommunityExpress.OnEventHandler<LeaderboardFindResult_t> _onLeaderboardRetrieved;
        /// <summary>
        /// Leaderboard is retireved
        /// </summary>
        /// <param name="sender">Sender of retrieval request</param>
        /// <param name="leaderboard">Leaderboard</param>
        public delegate void LeaderboardRetrievedHandler(Leaderboards sender, Leaderboard leaderboard);
        /// <summary>
        /// Leaderboard is retrieved
        /// </summary>
        public event LeaderboardRetrievedHandler LeaderboardReceived;

		internal Leaderboards(CommunityExpress ce)
		{
            _ce = ce;
			_stats = SteamUnityAPI_SteamUserStats();

            _onLeaderboardRetrieved = new CommunityExpress.OnEventHandler<LeaderboardFindResult_t>(OnLeaderboardRetrievedCallback);
		}

        /// <summary>
        /// Asks the Steam back-end for a leaderboard by name
        /// </summary>
        /// <param name="leaderboardName">Name of the leaderboard</param>
        /// <param name="useCache">Uses the cached data</param>
        public void FindLeaderboard(String leaderboardName, bool useCache = true)
		{
			Leaderboard leaderboard = null;

			foreach (Leaderboard l in _leaderboardList)
			{
				if (l.LeaderboardName == leaderboardName)
				{
					leaderboard = l;
					break;
				}
			}

            if (leaderboard != null && useCache == true)
			{
                if (LeaderboardReceived != null) LeaderboardReceived(this, leaderboard);
			}
			else
			{
                _ce.AddEventHandler<LeaderboardFindResult_t>(LeaderboardFindResult_t.k_iCallback, _onLeaderboardRetrieved);

                if (SteamUnityAPI_SteamUserStats_FindLeaderboard(_stats, leaderboardName) == 0)
                {
                    _ce.RemoveEventHandler<LeaderboardFindResult_t>(LeaderboardFindResult_t.k_iCallback, _onLeaderboardRetrieved);

                    if (LeaderboardReceived != null) LeaderboardReceived(this, null);
                }
			}
		}

        /// <summary>
        /// Asks the Steam back-end for a leaderboard by name, and will create it if needed
        /// </summary>
        /// <param name="leaderboardName">Name of leaderboard</param>
        /// <param name="sortMethod">Method of sorting leaderboard entries</param>
        /// <param name="displayType">Type of leaderboard display</param>
		public void FindOrCreateLeaderboard(String leaderboardName, ELeaderboardSortMethod sortMethod, ELeaderboardDisplayType displayType)
		{
			Leaderboard leaderboard = null;

			foreach (Leaderboard l in _leaderboardList)
			{
				if (l.LeaderboardName == leaderboardName)
				{
					leaderboard = l;
					break;
				}
			}

			if (leaderboard != null)
            {
                if (LeaderboardReceived != null) LeaderboardReceived(this, leaderboard);
			}
			else
            {
                _ce.AddEventHandler<LeaderboardFindResult_t>(LeaderboardFindResult_t.k_iCallback, _onLeaderboardRetrieved);

				if (SteamUnityAPI_SteamUserStats_FindOrCreateLeaderboard(_stats, leaderboardName, sortMethod, displayType) == 0)
                {
                    _ce.RemoveEventHandler<LeaderboardFindResult_t>(LeaderboardFindResult_t.k_iCallback, _onLeaderboardRetrieved);

                    if (LeaderboardReceived != null) LeaderboardReceived(this, null);
				}
			}
		}

		private void OnLeaderboardRetrievedCallback(LeaderboardFindResult_t findLearderboardResult, Boolean bIOFailure, SteamAPICall_t hSteamAPICall)
		{
            _ce.RemoveEventHandler<LeaderboardFindResult_t>(LeaderboardFindResult_t.k_iCallback, _onLeaderboardRetrieved);
            
            Leaderboard leaderboard = null;

            if (findLearderboardResult.m_bLeaderboardFound != 0)
            {
                String leaderboardName = Marshal.PtrToStringAnsi(SteamUnityAPI_SteamUserStats_GetLeaderboardName(_stats, findLearderboardResult.m_hSteamLeaderboard));
                
                foreach (Leaderboard l in _leaderboardList)
                {
                    if (l.LeaderboardName == leaderboardName)
                    {
                        leaderboard = l;
                        break;
                    }
                }

                if (leaderboard != null)
                {
                    leaderboard.LeaderboardRefreshed(findLearderboardResult);
                }
                else
                {
                    int count = SteamUnityAPI_SteamUserStats_GetLeaderboardEntryCount(_stats, findLearderboardResult.m_hSteamLeaderboard);
                    ELeaderboardSortMethod sort = SteamUnityAPI_SteamUserStats_GetLeaderboardSortMethod(_stats, findLearderboardResult.m_hSteamLeaderboard);
                    ELeaderboardDisplayType dtype = SteamUnityAPI_SteamUserStats_GetLeaderboardDisplayType(_stats, findLearderboardResult.m_hSteamLeaderboard);

                    leaderboard = new Leaderboard(this, findLearderboardResult.m_hSteamLeaderboard,
                                                  leaderboardName,
                                                  count,
                                                  sort,
                                                  dtype);
                    Add(leaderboard);
                }
            }

            if (LeaderboardReceived != null) LeaderboardReceived(this, leaderboard);
		}

		internal IntPtr Stats
		{
			get { return _stats; }
		}
        /// <summary>
        /// Number of leaderboards on the list
        /// </summary>
		public int Count
		{
			get { return _leaderboardList.Count; }
		}
        /// <summary>
        /// If the leaderboard is read-only
        /// </summary>
		public bool IsReadOnly
		{
			get { return true; }
		}
        /// <summary>
        /// Adds a leaderboard to the list
        /// </summary>
        /// <param name="item">Entry to add</param>
		public void Add(Leaderboard item)
		{
			_leaderboardList.Add(item);
		}
        /// <summary>
        /// Clears list
        /// </summary>
		public void Clear()
		{
			_leaderboardList.Clear();
		}
        /// <summary>
        /// Checks if the list contains a specific leaderboard
        /// </summary>
        /// <param name="item">Leaderboard to check for</param>
        /// <returns>true if found</returns>
		public bool Contains(Leaderboard item)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Copies list to index
        /// </summary>
        /// <param name="array">Array of leaderboards</param>
        /// <param name="arrayIndex">Index to copy to</param>
		public void CopyTo(Leaderboard[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Removes leaderboard from list
        /// </summary>
        /// <param name="item">Leaderboard to remove</param>
        /// <returns>true if entry removed</returns>
		public bool Remove(Leaderboard item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Gets enumerator for list
        /// </summary>
        /// <returns>true if enumerator gotten</returns>
		public IEnumerator<Leaderboard> GetEnumerator()
		{
			return new ListEnumerator<Leaderboard>(_leaderboardList);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

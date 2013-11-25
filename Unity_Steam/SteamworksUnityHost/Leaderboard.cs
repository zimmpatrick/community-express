// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
	using SteamLeaderboard_t = UInt64;
	using SteamLeaderboardEntries_t = UInt64;
    using SteamAPICall_t = UInt64;

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LeaderboardEntry_t
	{
		public UInt64 m_steamIDUser;	// user with the entry - use SteamFriends()->GetFriendPersonaName() & SteamFriends()->GetFriendAvatar() to get more info
		public Int32 m_nGlobalRank;		// [1..N], where N is the number of users with an entry in the leaderboard
		public Int32 m_nScore;			// score as set in the leaderboard
		public Int32 m_cDetails;		// number of int32 details available for this entry
		public UInt64 m_hUGC;			// handle for UGC attached to the entry
	};

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct LeaderboardScoresDownloaded_t
	{
        internal const int k_iCallback = Events.k_iSteamUserStatsCallbacks + 5;
        
		public SteamLeaderboard_t m_hSteamLeaderboard;
		public SteamLeaderboardEntries_t m_hSteamLeaderboardEntries;	// the handle to pass into GetDownloadedLeaderboardEntries()
		public Int32 m_cEntryCount; // the number of entries downloaded
	};
    
    /// <summary>
    /// Information about a leaderboard
    /// </summary>
	public class Leaderboard
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats();
		[DllImport("CommunityExpressSW")]
        private static extern SteamAPICall_t SteamUnityAPI_SteamUserStats_FindLeaderboard(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string leaderboardName);
        [DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats_GetLeaderboardName(IntPtr stats, SteamLeaderboard_t leaderboard);
		[DllImport("CommunityExpressSW")]
		private static extern ELeaderboardSortMethod SteamUnityAPI_SteamUserStats_GetLeaderboardSortMethod(IntPtr stats, SteamLeaderboard_t leaderboard);
		[DllImport("CommunityExpressSW")]
		private static extern ELeaderboardDisplayType SteamUnityAPI_SteamUserStats_GetLeaderboardDisplayType(IntPtr stats, SteamLeaderboard_t leaderboard);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_SteamUserStats_UploadLeaderboardScore(IntPtr stats, SteamLeaderboard_t leaderboard, ELeaderboardUploadScoreMethod uploadScoreMethod,
			Int32 score, [MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.I4)] Int32[] scoreDetails, Int32 scoreDetailCount);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_SteamUserStats_RequestLeaderboardEntries(IntPtr stats, SteamLeaderboard_t leaderboard, ELeaderboardDataRequest requestType,
			Int32 startIndex, Int32 endIndex);
		[DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_SteamUserStats_GetDownloadedLeaderboardEntry(IntPtr stats, SteamLeaderboardEntries_t leaderboardEntries, Int32 index,
			ref LeaderboardEntry_t leaderboardEntry, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4)] Int32[] scoreDetails, Int32 maxScoreDetailCount);

        private CommunityExpress _ce;

		private IntPtr _stats;
		private Leaderboards _leaderboards;

		private SteamLeaderboard_t _leaderboard;
		private String _leaderboardName;
		private Int32 _entryCount;
		private ELeaderboardSortMethod _sortMethod;
		private ELeaderboardDisplayType _displayType;

		private Int32 _maxDetails;
        /// <summary>
        /// Retrieve Leaderboard entries
        /// </summary>
        /// <param name="sender">Sender of request</param>
        /// <param name="entries">Entries to recieve</param>
        public delegate void OnLeaderboardEntriesRetrievedHandler(Leaderboard sender, LeaderboardEntries entries);
        /// <summary>
        /// Entries on leaderboard are recieved
        /// </summary>
        public event OnLeaderboardEntriesRetrievedHandler LeaderboardEntriesReceived;

        private CommunityExpress.OnEventHandler<LeaderboardScoresDownloaded_t> LeaderboardEntriesRetrievedCallback;

		internal Leaderboard(Leaderboards leaderboards, SteamLeaderboard_t leaderboard, String leaderboardName, Int32 entryCount, ELeaderboardSortMethod sortMethod, ELeaderboardDisplayType displayType)
		{
			_stats = SteamUnityAPI_SteamUserStats();
			_leaderboards = leaderboards;
			_leaderboard = leaderboard;
			_leaderboardName = leaderboardName;
			_entryCount = entryCount;
			_sortMethod = sortMethod;
			_displayType = displayType;
            _ce = CommunityExpress.Instance;

            LeaderboardEntriesRetrievedCallback = new CommunityExpress.OnEventHandler<LeaderboardScoresDownloaded_t>(OnLeaderboardEntriesRetrievedCallback);
            _ce.AddEventHandler<LeaderboardScoresDownloaded_t>(LeaderboardScoresDownloaded_t.k_iCallback, LeaderboardEntriesRetrievedCallback);
		}
        /// <summary>
        /// Updates scores on leaderboard
        /// </summary>
        /// <param name="uploadScoreMethod">Method of updating</param>
        /// <param name="score">Score to update</param>
        /// <param name="scoreDetails">Details of score</param>
		public void UploadLeaderboardScore(ELeaderboardUploadScoreMethod uploadScoreMethod, Int32 score, List<Int32> scoreDetails)
		{
			if (scoreDetails != null)
			{
				SteamUnityAPI_SteamUserStats_UploadLeaderboardScore(_leaderboards.Stats, _leaderboard, uploadScoreMethod, score, scoreDetails.ToArray(), scoreDetails.Count);
			}
			else
			{
				SteamUnityAPI_SteamUserStats_UploadLeaderboardScore(_leaderboards.Stats, _leaderboard, uploadScoreMethod, score, null, 0);
			}
		}
        /// <summary>
        /// Requests entries on leaderboard
        /// </summary>
        /// <param name="startIndex">Where the index starts</param>
        /// <param name="endIndex">Where the index ends</param>
        /// <param name="maxExpectedDetails">Maximum number of entries</param>
		public void RequestLeaderboardEntries(Int32 startIndex, Int32 endIndex, Int32 maxExpectedDetails)
		{
			_maxDetails = maxExpectedDetails;

            //_ce.AddEventHandler<LeaderboardScoresDownloaded_t>(LeaderboardScoresDownloaded_t.k_iCallback, LeaderboardEntriesRetrievedCallback);

            if (!SteamUnityAPI_SteamUserStats_RequestLeaderboardEntries(_leaderboards.Stats, _leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, startIndex, endIndex))
            {
               // _ce.RemoveEventHandler<LeaderboardScoresDownloaded_t>(Leaderboards.LeaderboardFindResult_t.k_iCallback, LeaderboardEntriesRetrievedCallback);
                if (LeaderboardEntriesReceived != null) LeaderboardEntriesReceived(this, null);
            }
		}
        /// <summary>
        /// Requests the entries near the current user's score
        /// </summary>
        /// <param name="rowsBefore">Rows before user</param>
        /// <param name="rowsAfter">Rows after user</param>
        /// <param name="maxExpectedDetails">Maximum number of entries</param>
		public void RequestLeaderboardEntriesAroundCurrentUser(Int32 rowsBefore, Int32 rowsAfter, Int32 maxExpectedDetails)
		{
            _maxDetails = maxExpectedDetails;

           // _ce.RemoveEventHandler<LeaderboardScoresDownloaded_t>(Leaderboards.LeaderboardFindResult_t.k_iCallback, LeaderboardEntriesRetrievedCallback);

            if (!SteamUnityAPI_SteamUserStats_RequestLeaderboardEntries(_leaderboards.Stats, _leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, -1 * rowsBefore, rowsAfter))
            {
               // _ce.RemoveEventHandler<LeaderboardScoresDownloaded_t>(Leaderboards.LeaderboardFindResult_t.k_iCallback, LeaderboardEntriesRetrievedCallback);
                if (LeaderboardEntriesReceived != null) LeaderboardEntriesReceived(this, null);
            }
		}
        /// <summary>
        /// Requests entries for a friend of the user
        /// </summary>
        /// <param name="maxExpectedDetails">Maximum number of entries</param>
		public void RequestFriendLeaderboardEntries(Int32 maxExpectedDetails)
		{
            _maxDetails = maxExpectedDetails;

           // _ce.AddEventHandler<LeaderboardScoresDownloaded_t>(LeaderboardScoresDownloaded_t.k_iCallback, LeaderboardEntriesRetrievedCallback);

            if (!SteamUnityAPI_SteamUserStats_RequestLeaderboardEntries(_leaderboards.Stats, _leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends, 0, Int32.MaxValue - 1))
            {
                //_ce.RemoveEventHandler<LeaderboardScoresDownloaded_t>(Leaderboards.LeaderboardFindResult_t.k_iCallback, LeaderboardEntriesRetrievedCallback);
                if (LeaderboardEntriesReceived != null) LeaderboardEntriesReceived(this, null);
            }
		}

        private void OnLeaderboardEntriesRetrievedCallback(LeaderboardScoresDownloaded_t callbackData, Boolean bIOFailure, SteamAPICall_t hSteamAPICall)
		{
            //_ce.RemoveEventHandler<LeaderboardScoresDownloaded_t>(Leaderboards.LeaderboardFindResult_t.k_iCallback, LeaderboardEntriesRetrievedCallback);

			int entryCount = callbackData.m_cEntryCount;
			LeaderboardEntry_t leaderboardEntry = new LeaderboardEntry_t();

			LeaderboardEntries leaderboardEntries = new LeaderboardEntries(this);
			Int32[] details = new Int32[_maxDetails];

			for (int index = 0; index < entryCount; index++)
			{
				if (SteamUnityAPI_SteamUserStats_GetDownloadedLeaderboardEntry(_leaderboards.Stats, callbackData.m_hSteamLeaderboardEntries, index, ref leaderboardEntry, details, _maxDetails))
				{
					List<Int32> scoreDetails = null;

					if (details != null)
					{
						scoreDetails = new List<Int32>(details);
					}

					leaderboardEntries.Add(new LeaderboardEntry(leaderboardEntry.m_steamIDUser, leaderboardEntry.m_nGlobalRank, leaderboardEntry.m_nScore, scoreDetails));
				}
			}

            if (LeaderboardEntriesReceived != null) LeaderboardEntriesReceived(this, leaderboardEntries);
		}
        /// <summary>
        /// Refresh the leaderboard
        /// </summary>
		public void Refresh()
		{
            _leaderboards.FindLeaderboard(_leaderboardName, false);
		}

		internal void LeaderboardRefreshed(Leaderboards.LeaderboardFindResult_t findLearderboardResult)
		{
			if (findLearderboardResult.m_bLeaderboardFound != 0)
			{
				_leaderboard = findLearderboardResult.m_hSteamLeaderboard;
				_leaderboardName = Marshal.PtrToStringAnsi(SteamUnityAPI_SteamUserStats_GetLeaderboardName(_stats, findLearderboardResult.m_hSteamLeaderboard));
				_sortMethod = SteamUnityAPI_SteamUserStats_GetLeaderboardSortMethod(_stats, findLearderboardResult.m_hSteamLeaderboard);
				_displayType = SteamUnityAPI_SteamUserStats_GetLeaderboardDisplayType(_stats, findLearderboardResult.m_hSteamLeaderboard);
			}
		}
        /// <summary>
        /// Name of the leaderboard
        /// </summary>
		public String LeaderboardName
		{
			get { return _leaderboardName; }
		}
        /// <summary>
        /// Number of entries on the leaderboard
        /// </summary>
		public Int32 EntryCount
		{
			get { return _entryCount; }
		}
        /// <summary>
        /// Method in which the leaderboard is sorted
        /// </summary>
		public ELeaderboardSortMethod SortMethod
		{
			get { return _sortMethod; }
		}
        /// <summary>
        /// Display type of the leaderboard
        /// </summary>
		public ELeaderboardDisplayType DisplayType
		{
			get { return _displayType; }
		}
	}
}

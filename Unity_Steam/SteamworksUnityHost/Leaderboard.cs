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

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct LeaderboardEntry_t
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
		public SteamLeaderboard_t m_hSteamLeaderboard;
		public SteamLeaderboardEntries_t m_hSteamLeaderboardEntries;	// the handle to pass into GetDownloadedLeaderboardEntries()
		public Int32 m_cEntryCount; // the number of entries downloaded
	};

	delegate void OnLeaderboardEntriesRetrievedFromSteam(ref LeaderboardScoresDownloaded_t callbackData);
	public delegate void OnLeaderboardEntriesRetrieved(LeaderboardEntries leaderboardEntries);

	public class Leaderboard
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats();
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_SteamUserStats_FindLeaderboard(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string leaderboardName,
			IntPtr OnLeaderboardRetrievedCallback);
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
			Int32 startIndex, Int32 endIndex, IntPtr OnLeaderboardEntriesRetrievedCallback);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_SteamUserStats_GetDownloadedLeaderboardEntry(IntPtr stats, SteamLeaderboardEntries_t leaderboardEntries, Int32 index,
			ref LeaderboardEntry_t leaderboardEntry, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4)] Int32[] scoreDetails, Int32 maxScoreDetailCount);

		private IntPtr _stats;
		private Leaderboards _leaderboards;

		private SteamLeaderboard_t _leaderboard;
		private String _leaderboardName;
		private Int32 _entryCount;
		private ELeaderboardSortMethod _sortMethod;
		private ELeaderboardDisplayType _displayType;

		private Int32 _maxDetails;

		private OnLeaderboardRetrievedFromSteam _internalOnLeaderboardRetrieved = null;
		private OnLeaderboardRetrieved _onLeaderboardRetrieved;

		private OnLeaderboardEntriesRetrievedFromSteam _internalOnLeaderboardEntriesRetrieved = null;
		private OnLeaderboardEntriesRetrieved _onLeaderboardEntriesRetrieved;

		internal Leaderboard(Leaderboards leaderboards, SteamLeaderboard_t leaderboard, String leaderboardName, Int32 entryCount, ELeaderboardSortMethod sortMethod, ELeaderboardDisplayType displayType)
		{
			_stats = SteamUnityAPI_SteamUserStats();
			_leaderboards = leaderboards;
			_leaderboard = leaderboard;
			_leaderboardName = leaderboardName;
			_entryCount = entryCount;
			_sortMethod = sortMethod;
			_displayType = displayType;
		}

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

		public void RequestLeaderboardEntries(Int32 startIndex, Int32 endIndex, Int32 maxExpectedDetails, OnLeaderboardEntriesRetrieved onLeaderboardEntriesRetrieved)
		{
			_onLeaderboardEntriesRetrieved = onLeaderboardEntriesRetrieved;
			_maxDetails = maxExpectedDetails;

			if (_internalOnLeaderboardEntriesRetrieved == null)
			{
				_internalOnLeaderboardEntriesRetrieved = new OnLeaderboardEntriesRetrievedFromSteam(OnLeaderboardEntriesRetrievedCallback);
			}

			if (!SteamUnityAPI_SteamUserStats_RequestLeaderboardEntries(_leaderboards.Stats, _leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, startIndex, endIndex, Marshal.GetFunctionPointerForDelegate(_internalOnLeaderboardEntriesRetrieved)))
			{
				_onLeaderboardEntriesRetrieved(null);
			}
		}

		public void RequestLeaderboardEntriesAroundCurrentUser(Int32 rowsBefore, Int32 rowsAfter, Int32 maxExpectedDetails, OnLeaderboardEntriesRetrieved onLeaderboardEntriesRetrieved)
		{
			_onLeaderboardEntriesRetrieved = onLeaderboardEntriesRetrieved;
			_maxDetails = maxExpectedDetails;

			if (_internalOnLeaderboardEntriesRetrieved == null)
			{
				_internalOnLeaderboardEntriesRetrieved = new OnLeaderboardEntriesRetrievedFromSteam(OnLeaderboardEntriesRetrievedCallback);
			}

			if (!SteamUnityAPI_SteamUserStats_RequestLeaderboardEntries(_leaderboards.Stats, _leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, -1 * rowsBefore, rowsAfter, Marshal.GetFunctionPointerForDelegate(_internalOnLeaderboardEntriesRetrieved)))
			{
				_onLeaderboardEntriesRetrieved(null);
			}
		}

		public void RequestFriendLeaderboardEntries(Int32 maxExpectedDetails, OnLeaderboardEntriesRetrieved onLeaderboardEntriesRetrieved)
		{
			_onLeaderboardEntriesRetrieved = onLeaderboardEntriesRetrieved;
			_maxDetails = maxExpectedDetails;

			if (_internalOnLeaderboardEntriesRetrieved == null)
			{
				_internalOnLeaderboardEntriesRetrieved = new OnLeaderboardEntriesRetrievedFromSteam(OnLeaderboardEntriesRetrievedCallback);
			}

			if (!SteamUnityAPI_SteamUserStats_RequestLeaderboardEntries(_leaderboards.Stats, _leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends, 0, Int32.MaxValue - 1, Marshal.GetFunctionPointerForDelegate(_internalOnLeaderboardEntriesRetrieved)))
			{
				_onLeaderboardEntriesRetrieved(null);
			}
		}

		private void OnLeaderboardEntriesRetrievedCallback(ref LeaderboardScoresDownloaded_t callbackData)
		{
			if (callbackData.m_cEntryCount > 0)
			{
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

				_onLeaderboardEntriesRetrieved(leaderboardEntries);
			}
			else
			{
				_onLeaderboardEntriesRetrieved(null);
			}
		}

		public void Refresh(OnLeaderboardRetrieved onLeaderboardRefreshComplete)
		{
			_onLeaderboardRetrieved = onLeaderboardRefreshComplete;

			if (_internalOnLeaderboardRetrieved == null)
			{
				_internalOnLeaderboardRetrieved = new OnLeaderboardRetrievedFromSteam(OnLeaderboardRetrievedCallback);
			}

			if (!SteamUnityAPI_SteamUserStats_FindLeaderboard(_stats, _leaderboardName, Marshal.GetFunctionPointerForDelegate(_internalOnLeaderboardRetrieved)))
			{
				_onLeaderboardRetrieved(this);
			}
		}

		private void OnLeaderboardRetrievedCallback(ref LeaderboardFindResult_t findLearderboardResult)
		{
			if (findLearderboardResult.m_bLeaderboardFound != 0)
			{
				_leaderboard = findLearderboardResult.m_hSteamLeaderboard;
				_leaderboardName = Marshal.PtrToStringAnsi(SteamUnityAPI_SteamUserStats_GetLeaderboardName(_stats, findLearderboardResult.m_hSteamLeaderboard));
				_sortMethod = SteamUnityAPI_SteamUserStats_GetLeaderboardSortMethod(_stats, findLearderboardResult.m_hSteamLeaderboard);
				_displayType = SteamUnityAPI_SteamUserStats_GetLeaderboardDisplayType(_stats, findLearderboardResult.m_hSteamLeaderboard);
			}

			_onLeaderboardRetrieved(this);
		}

		public String LeaderboardName
		{
			get { return _leaderboardName; }
		}

		public Int32 EntryCount
		{
			get { return _entryCount; }
		}

		public ELeaderboardSortMethod SortMethod
		{
			get { return _sortMethod; }
		}

		public ELeaderboardDisplayType DisplayType
		{
			get { return _displayType; }
		}
	}
}

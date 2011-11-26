using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SteamworksUnityHost
{
	using SteamLeaderboard_t = UInt64;
	using SteamLeaderboardEntries_t = UInt64;

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct LeaderboardEntry_t
	{
		UInt64 m_steamIDUser;	// user with the entry - use SteamFriends()->GetFriendPersonaName() & SteamFriends()->GetFriendAvatar() to get more info
		Int32 m_nGlobalRank;	// [1..N], where N is the number of users with an entry in the leaderboard
		Int32 m_nScore;			// score as set in the leaderboard
		Int32 m_cDetails;		// number of int32 details available for this entry
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct LeaderboardScoresDownloaded_t
	{
		SteamLeaderboard_t m_hSteamLeaderboard;
		SteamLeaderboardEntries_t m_hSteamLeaderboardEntries;	// the handle to pass into GetDownloadedLeaderboardEntries()
		Int32 m_cEntryCount; // the number of entries downloaded
	};

	public class Leaderboard
	{
		private Leaderboards _leaderboards;

		private SteamLeaderboard_t _leaderboard;
		private String _leaderboardName;

		public Leaderboard(Leaderboards leaderboards, SteamLeaderboard_t leaderboard, String leaderboardName)
		{
			_leaderboards = leaderboards;
			_leaderboard = leaderboard;
			_leaderboardName = leaderboardName;
		}

		public String LeaderboardName
		{
			get { return _leaderboardName; }
		}
	}
}

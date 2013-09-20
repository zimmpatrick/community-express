// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
    /// <summary>
    /// Information about a leaderboard entry
    /// </summary>
	public class LeaderboardEntry
	{
		private SteamID _id;
		private Int32 _rank;
		private Int32 _score;
		private List<Int32> _scoreDetails;

		internal LeaderboardEntry(UInt64 steamIDUser, Int32 globalRank, Int32 score, List<Int32> scoreDetails)
		{
			_id = new SteamID(steamIDUser);
			_rank = globalRank;
			_score = score;
			_scoreDetails = scoreDetails;
		}
        /// <summary>
        /// ID of the user
        /// </summary>
		public SteamID SteamID
		{
			get { return _id; }
		}
        /// <summary>
        /// Global ranking of the user
        /// </summary>
		public Int32 GlobalRank
		{
			get { return _rank; }
		}
        /// <summary>
        /// Score of the user
        /// </summary>
		public Int32 Score
		{
			get { return _score; }
		}
        /// <summary>
        /// Name of the uesr
        /// </summary>
		public String PersonaName
		{
			get { return CommunityExpress.Instance.Friends.GetFriendPersonaName(_id); }
		}
        /// <summary>
        /// Details of the user's score
        /// </summary>
		public IList<Int32> ScoreDetails
		{
			get { return _scoreDetails; }
		}
	}
}

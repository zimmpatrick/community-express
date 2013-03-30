// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
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

		public SteamID SteamID
		{
			get { return _id; }
		}

		public Int32 GlobalRank
		{
			get { return _rank; }
		}

		public Int32 Score
		{
			get { return _score; }
		}

		public String PersonaName
		{
			get { return CommunityExpress.Instance.Friends.GetFriendPersonaName(_id); }
		}

		public IList<Int32> ScoreDetails
		{
			get { return _scoreDetails; }
		}
	}
}

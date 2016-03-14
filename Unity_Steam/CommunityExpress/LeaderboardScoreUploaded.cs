using System;

namespace CommunityExpressNS
{
	/// <summary>
	/// Information about a score that has just been uploaded
	/// </summary>
	public class LeaderboardScoreUploaded
	{
		private bool _success;
		private Leaderboard _leaderboard;
		private int _score;
		private bool _scoreChanged;
		private int _globalRankNew;
		private int _globalRankPrevious;

		public LeaderboardScoreUploaded(bool success, Leaderboard leaderboard, int score, bool scoreChanged, int globalRankNew, int globalRankPrevious)
		{
			_success = success;
			_leaderboard = leaderboard;
			_score = score;
			_scoreChanged = scoreChanged;
			_globalRankNew = globalRankNew;
			_globalRankPrevious = globalRankPrevious;
		}

		public bool Success
		{
			get { return _success; }
		}

		public Leaderboard Leaderboard
		{
			get { return _leaderboard; }
		}

		public Int32 Score
		{
			get { return _score; }
		}

		public bool ScoreChanged
		{
			get { return _scoreChanged; }
		}

		public Int32 GlobalRankNew
		{
			get { return _globalRankNew; }
		}

		public Int32 GlobalRankPrevious
		{
			get { return _globalRankPrevious; }
		}
	}
}
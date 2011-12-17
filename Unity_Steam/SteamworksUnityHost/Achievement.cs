using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
	public class Achievement
	{
		private Achievements _achievements;
		private String _achievementName;
		private bool _isAchieved;

		public Achievement(Achievements achievements)
		{
			_achievements = achievements;
		}

		public Achievement(Achievements achievements, String achievementName, bool isAchieved)
		{
			_achievements = achievements;

			_achievementName = achievementName;
			_isAchieved = isAchieved;
		}

		public String AchievementName
		{
			get { return _achievementName; }
			private set { _achievementName = value; }
		}

		public bool IsAchieved
		{
			get { return _isAchieved; }
			internal set { _isAchieved = true; }
		}
	}
}
using System;
using System.Collections.Generic;
using System.Text;

namespace SteamworksUnityHost
{
	public class Stat
	{
		public String StatName;
		public Decimal StatValue;
		public Boolean IsIntValue;

		private Stats _stats;

		public Stat(Stats stats)
		{
			_stats = stats;
		}

		public Stat(Stats stats, String statName, Decimal statValue, Boolean isIntValue)
		{
			_stats = stats;

			StatName = statName;
			StatValue = statValue;
			IsIntValue = isIntValue;
		}
	}
}
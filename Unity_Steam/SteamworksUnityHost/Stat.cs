using System;
using System.Collections.Generic;
using System.Text;

namespace SteamworksUnityHost
{
	public class Stat
	{
		public String StatName;
		public object StatValue;

		private Stats _stats;

		public Stat(Stats stats)
		{
			_stats = stats;
		}

		public Stat(Stats stats, String statName, object statValue)
		{
			_stats = stats;

			StatName = statName;
			StatValue = statValue;
		}
	}
}
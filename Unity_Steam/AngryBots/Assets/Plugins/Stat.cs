using System;
using System.Collections.Generic;
using System.Text;

namespace SteamworksUnityHost
{
	public class Stat
	{
		private Stats _stats;
		private String _statName;
		private object _statValue;
		private bool _hasChanged;

		public Stat(Stats stats)
		{
			_stats = stats;
		}

		public Stat(Stats stats, String statName, object statValue)
		{
			_stats = stats;

			_statName = statName;
			_statValue = statValue;
			_hasChanged = false;
		}

		public String StatName
		{
			get { return _statName; }
			private set { _statName = value; }
		}

		public object StatValue
		{
			get { return _statValue; }
			set { _statValue = value; _hasChanged = true; }
		}

		public bool HasChanged
		{
			get { return _hasChanged; }
			internal set { _hasChanged = value; }
		}
	}
}
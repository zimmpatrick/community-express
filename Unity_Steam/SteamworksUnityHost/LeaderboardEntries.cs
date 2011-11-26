using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace SteamworksUnityHost
{
	public class LeaderboardEntries : ICollection<LeaderboardEntry>
	{
		private Leaderboard _leaderboard;

		private List<LeaderboardEntry> _leaderboardEntryList = new List<LeaderboardEntry>();

		private class LeaderboardEntryEnumator : IEnumerator<LeaderboardEntry>
		{
			private int _index;
			private LeaderboardEntries _leaderboardEntries;

			public LeaderboardEntryEnumator(LeaderboardEntries leaderboardEntries)
			{
				_leaderboardEntries = leaderboardEntries;
				_index = -1;
			}

			public LeaderboardEntry Current
			{
				get
				{
					return _leaderboardEntries._leaderboardEntryList[_index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public bool MoveNext()
			{
				_index++;
				return _index < _leaderboardEntries.Count;
			}

			public void Reset()
			{
				_index = -1;
			}

			public void Dispose()
			{
			}
		}

		public LeaderboardEntries(Leaderboard leaderboard)
		{
			_leaderboard = leaderboard;
		}

		public Leaderboard Leaderboard
		{
			get { return _leaderboard; }
		}

		public int Count
		{
			get { return _leaderboardEntryList.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void Add(LeaderboardEntry item)
		{
			_leaderboardEntryList.Add(item);
		}

		public void Clear()
		{
			_leaderboardEntryList.Clear();
		}

		public bool Contains(LeaderboardEntry item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(LeaderboardEntry[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(LeaderboardEntry item)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<LeaderboardEntry> GetEnumerator()
		{
			return new LeaderboardEntryEnumator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

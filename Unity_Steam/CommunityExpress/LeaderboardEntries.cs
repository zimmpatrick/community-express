/*
 * Community Express SDK
 * http://www.communityexpresssdk.com/
 *
 * Copyright (c) 2011-2014, Zimmdot, LLC
 * All rights reserved.
 *
 * Subject to terms and condition provided in LICENSE.txt
 * Dual licensed under a Commercial Development and LGPL licenses.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
    /// <summary>
    /// Information about leaderboard entries
    /// </summary>
	public class LeaderboardEntries : IList<LeaderboardEntry>
	{
		private Leaderboard _leaderboard;

		private List<LeaderboardEntry> _leaderboardEntryList = new List<LeaderboardEntry>();

		private Int32 _minRank = Int32.MaxValue;
		private Int32 _maxRank = Int32.MinValue;
		
		internal LeaderboardEntries(Leaderboard leaderboard)
		{
			_leaderboard = leaderboard;
		}
        /// <summary>
        /// Leaderboard where the entry is
        /// </summary>
		public Leaderboard Leaderboard
		{
			get { return _leaderboard; }
		}
        /// <summary>
        /// Lowest rank on the leaderboard
        /// </summary>
		public Int32 LowestRank
		{
			get { return _minRank; }
		}
        /// <summary>
        /// Highest rank on a leaderboard
        /// </summary>
		public Int32 HighestRank
		{
			get { return _maxRank; }
		}
        /// <summary>
        /// Number of entries on the leaderboard
        /// </summary>
		public int Count
		{
			get { return _leaderboardEntryList.Count; }
		}
        /// <summary>
        /// If the leaderboard is read-only
        /// </summary>
		public bool IsReadOnly
		{
			get { return true; }
		}
        /// <summary>
        /// Add a leaderboard entry
        /// </summary>
        /// <param name="item">Entry to add</param>
		public void Add(LeaderboardEntry item)
		{
			_leaderboardEntryList.Add(item);

			if (item.GlobalRank < _minRank)
			{
				_minRank = item.GlobalRank;
			}

			if (item.GlobalRank > _maxRank)
			{
				_maxRank = item.GlobalRank;
			}
		}
        /// <summary>
        /// Clears the leaderboard
        /// </summary>
		public void Clear()
		{
			_leaderboardEntryList.Clear();
		}
        /// <summary>
        /// Checks if the leaderboard contains a specific entry
        /// </summary>
        /// <param name="item">Entry to check for</param>
        /// <returns>true if entry found</returns>
		public bool Contains(LeaderboardEntry item)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Copies the leaderboard to an index
        /// </summary>
        /// <param name="array">Array of entries</param>
        /// <param name="arrayIndex">Index to copy to</param>
		public void CopyTo(LeaderboardEntry[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Removes an entry from the leaderboard
        /// </summary>
        /// <param name="item">Entry to remove</param>
        /// <returns>true if entry removed</returns>
		public bool Remove(LeaderboardEntry item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Gets enumerator for leaderboard entries
        /// </summary>
        /// <returns>true if enumerator gotten</returns>
		public IEnumerator<LeaderboardEntry> GetEnumerator()
		{
			return new ListEnumerator<LeaderboardEntry>(_leaderboardEntryList);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
        /// <summary>
        /// Gets leaderboard entries
        /// </summary>
        /// <param name="index">Index of entries</param>
        /// <returns>true if gotten</returns>
		public LeaderboardEntry this[int index] 
		{
			get
			{
				return _leaderboardEntryList[index];
			}

			set
			{
				throw new NotSupportedException();
			}
		}
        /// <summary>
        /// Index of entries
        /// </summary>
        /// <param name="item">Entries to index</param>
        /// <returns>true if indexed</returns>
		public int IndexOf(LeaderboardEntry item)
		{
			return _leaderboardEntryList.IndexOf(item);
		}
        /// <summary>
        /// Inserts entries at a point
        /// </summary>
        /// <param name="index">Where to insert the entries</param>
        /// <param name="item">Entries to insert</param>
		public void Insert(int index, LeaderboardEntry item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Removes entries from a point
        /// </summary>
        /// <param name="index">Entries to remove</param>
		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
	}
}

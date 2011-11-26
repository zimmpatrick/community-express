using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace SteamworksUnityHost
{
	using SteamLeaderboard_t = UInt64;

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct LeaderboardFindResult_t
	{
		public SteamLeaderboard_t m_hSteamLeaderboard;	// handle to the leaderboard serarched for, 0 if no leaderboard found
		public Byte m_bLeaderboardFound;				// 0 if no leaderboard found
	};

	internal delegate void OnLeaderboardRetrievedFromSteam(ref LeaderboardFindResult_t findLearderboardResult);
	public delegate void OnLeaderboardRetrieved(Leaderboard leaderboard);

	public class Leaderboards : ICollection<Leaderboard>
	{
		[DllImport("SteamworksUnity.dll")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats();
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_SteamUserStats_FindLeaderboard(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string leaderboardName, IntPtr OnUserStatsReceivedCallback);
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_SteamUserStats_FindOrCreateLeaderboard(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string leaderboardName, ELeaderboardSortMethod sortMethod, ELeaderboardDisplayType displayType, IntPtr OnUserStatsReceivedCallback);
		[DllImport("SteamworksUnity.dll")]
		[return: MarshalAs(UnmanagedType.LPStr, SizeParamIndex = 0)]
		private static extern String SteamUnityAPI_SteamUserStats_GetLeaderboardName(IntPtr stats, SteamLeaderboard_t leaderboard);

		private IntPtr _stats;
		private List<Leaderboard> _leaderboardList = new List<Leaderboard>();

		private OnLeaderboardRetrievedFromSteam _internalOnLeaderboardRetrieved;
		private OnLeaderboardRetrieved _onLeaderboardRetrieved;

		private class LeaderboardEnumator : IEnumerator<Leaderboard>
		{
			private int _index;
			private Leaderboards _leaderboards;

			public LeaderboardEnumator(Leaderboards leaderboards)
			{
				_leaderboards = leaderboards;
				_index = -1;
			}

			public Leaderboard Current
			{
				get
				{
					return _leaderboards._leaderboardList[_index];
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
				return _index < _leaderboards.Count;
			}

			public void Reset()
			{
				_index = -1;
			}

			public void Dispose()
			{
			}
		}

		public Leaderboards()
		{
			_stats = SteamUnityAPI_SteamUserStats();
		}

		public void FindLeaderboard(OnLeaderboardRetrieved onLeaderboardRetrieved, String leaderboardName)
		{
			Leaderboard leaderboard = null;

			foreach (Leaderboard l in _leaderboardList)
			{
				if (l.LeaderboardName == leaderboardName)
				{
					leaderboard = l;
					break;
				}
			}

			if (leaderboard != null)
			{
				onLeaderboardRetrieved(leaderboard);
			}
			else
			{
				_onLeaderboardRetrieved = onLeaderboardRetrieved;
				_internalOnLeaderboardRetrieved = new OnLeaderboardRetrievedFromSteam(OnLeaderboardRetrievedCallback);

				SteamUnityAPI_SteamUserStats_FindLeaderboard(_stats, leaderboardName, Marshal.GetFunctionPointerForDelegate(_internalOnLeaderboardRetrieved));
			}
		}

		public void FindOrCreateLeaderboard(OnLeaderboardRetrieved onLeaderboardRetrieved, String leaderboardName, ELeaderboardSortMethod sortMethod, ELeaderboardDisplayType displayType)
		{
			Leaderboard leaderboard = null;

			foreach (Leaderboard l in _leaderboardList)
			{
				if (l.LeaderboardName == leaderboardName)
				{
					leaderboard = l;
					break;
				}
			}

			if (leaderboard != null)
			{
				onLeaderboardRetrieved(leaderboard);
			}
			else
			{
				_onLeaderboardRetrieved = onLeaderboardRetrieved;
				_internalOnLeaderboardRetrieved = new OnLeaderboardRetrievedFromSteam(OnLeaderboardRetrievedCallback);

				if (!SteamUnityAPI_SteamUserStats_FindOrCreateLeaderboard(_stats, leaderboardName, sortMethod, displayType, Marshal.GetFunctionPointerForDelegate(_internalOnLeaderboardRetrieved)))
				{
					_onLeaderboardRetrieved(null);
				}
			}
		}

		internal void OnLeaderboardRetrievedCallback(ref LeaderboardFindResult_t findLearderboardResult)
		{
			Leaderboard leaderboard = null;

			if (findLearderboardResult.m_bLeaderboardFound != 0)
			{
				leaderboard = new Leaderboard(this, findLearderboardResult.m_hSteamLeaderboard, SteamUnityAPI_SteamUserStats_GetLeaderboardName(_stats, findLearderboardResult.m_hSteamLeaderboard));
				Add(leaderboard);
			}

			_onLeaderboardRetrieved(leaderboard);
		}

		public int Count
		{
			get { return _leaderboardList.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void Add(Leaderboard item)
		{
			_leaderboardList.Add(item);
		}

		public void Clear()
		{
			_leaderboardList.Clear();
		}

		public bool Contains(Leaderboard item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(Leaderboard[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(Leaderboard item)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<Leaderboard> GetEnumerator()
		{
			return new LeaderboardEnumator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace SteamworksUnityHost
{
	using SteamLeaderboard_t = UInt64;

	// type of data request, when downloading leaderboard entries
	public enum ELeaderboardDataRequest
	{
		k_ELeaderboardDataRequestGlobal = 0,
		k_ELeaderboardDataRequestGlobalAroundUser = 1,
		k_ELeaderboardDataRequestFriends = 2,
	};

	// the sort order of a leaderboard
	public enum ELeaderboardSortMethod
	{
		k_ELeaderboardSortMethodNone = 0,
		k_ELeaderboardSortMethodAscending = 1,	// top-score is lowest number
		k_ELeaderboardSortMethodDescending = 2,	// top-score is highest number
	};

	// the display type (used by the Steam Community web site) for a leaderboard
	public enum ELeaderboardDisplayType
	{
		k_ELeaderboardDisplayTypeNone = 0,
		k_ELeaderboardDisplayTypeNumeric = 1,			// simple numerical score
		k_ELeaderboardDisplayTypeTimeSeconds = 2,		// the score represents a time, in seconds
		k_ELeaderboardDisplayTypeTimeMilliSeconds = 3,	// the score represents a time, in milliseconds
	};

	public enum ELeaderboardUploadScoreMethod
	{
		k_ELeaderboardUploadScoreMethodNone = 0,
		k_ELeaderboardUploadScoreMethodKeepBest = 1,	// Leaderboard will keep user's best score
		k_ELeaderboardUploadScoreMethodForceUpdate = 2,	// Leaderboard will always replace score with specified
	};


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
		private static extern bool SteamUnityAPI_SteamUserStats_FindLeaderboard(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string leaderboardName,
			IntPtr OnLeaderboardRetrievedCallback);
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_SteamUserStats_FindOrCreateLeaderboard(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string leaderboardName,
			ELeaderboardSortMethod sortMethod, ELeaderboardDisplayType displayType, IntPtr OnLeaderboardRetrievedCallback);
		[DllImport("SteamworksUnity.dll")]
		[return: MarshalAs(UnmanagedType.LPStr, SizeParamIndex = 0)]
		private static extern String SteamUnityAPI_SteamUserStats_GetLeaderboardName(IntPtr stats, SteamLeaderboard_t leaderboard);
		[DllImport("SteamworksUnity.dll")]
		private static extern ELeaderboardSortMethod SteamUnityAPI_SteamUserStats_GetLeaderboardSortMethod(IntPtr stats, SteamLeaderboard_t leaderboard);
		[DllImport("SteamworksUnity.dll")]
		private static extern ELeaderboardDisplayType SteamUnityAPI_SteamUserStats_GetLeaderboardDisplayType(IntPtr stats, SteamLeaderboard_t leaderboard);

		private IntPtr _stats;
		private List<Leaderboard> _leaderboardList = new List<Leaderboard>();

		private OnLeaderboardRetrievedFromSteam _internalOnLeaderboardRetrieved = null;
		private OnLeaderboardRetrieved _onLeaderboardRetrieved;
		
		internal Leaderboards()
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

				if (_internalOnLeaderboardRetrieved == null)
				{
					_internalOnLeaderboardRetrieved = new OnLeaderboardRetrievedFromSteam(OnLeaderboardRetrievedCallback);
				}

				if (!SteamUnityAPI_SteamUserStats_FindLeaderboard(_stats, leaderboardName, Marshal.GetFunctionPointerForDelegate(_internalOnLeaderboardRetrieved)))
				{
					_onLeaderboardRetrieved(null);
				}
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

				if (_internalOnLeaderboardRetrieved == null)
				{
					_internalOnLeaderboardRetrieved = new OnLeaderboardRetrievedFromSteam(OnLeaderboardRetrievedCallback);
				}

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
				leaderboard = new Leaderboard(this, findLearderboardResult.m_hSteamLeaderboard, SteamUnityAPI_SteamUserStats_GetLeaderboardName(_stats, findLearderboardResult.m_hSteamLeaderboard), SteamUnityAPI_SteamUserStats_GetLeaderboardSortMethod(_stats, findLearderboardResult.m_hSteamLeaderboard), SteamUnityAPI_SteamUserStats_GetLeaderboardDisplayType(_stats, findLearderboardResult.m_hSteamLeaderboard));
				Add(leaderboard);
			}

			_onLeaderboardRetrieved(leaderboard);
		}

		public IntPtr Stats
		{
			get { return _stats; }
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
			return new ListEnumerator<Leaderboard>(_leaderboardList);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

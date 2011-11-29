using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace SteamworksUnityHost
{
	public class Achievements : ICollection<Achievement>
	{
		[DllImport("SteamworksUnity.dll")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats();
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_SteamUserStats_RequestCurrentStats(IntPtr stats, IntPtr OnUserStatsReceivedCallback);
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_SteamUserStats_GetAchievement(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string achievementName,
			out Byte isAchieved);
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_SteamUserStats_GetUserAchievement(IntPtr stats, UInt64 steamID, [MarshalAs(UnmanagedType.LPStr)] string achievementName,
			out Byte isAchieved);
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_SteamUserStats_SetAchievement(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string achievementName);
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_SteamUserStats_StoreStats(IntPtr stats);

		private IntPtr _stats;
		private SteamID _id;
		private List<Achievement> _achievementList = new List<Achievement>();
		private IEnumerable<string> _requestedAchievements;

		private OnUserStatsReceivedFromSteam _internalOnUserStatsReceived = null;
		private OnUserStatsReceived _onUserStatsReceived;

		public Achievements()
		{
			_stats = SteamUnityAPI_SteamUserStats();
		}

		public void RequestCurrentAchievements(OnUserStatsReceived onUserStatsReceived, IEnumerable<string> requestedAchievements)
		{
			_requestedAchievements = requestedAchievements;
			_onUserStatsReceived = onUserStatsReceived;

			if (_internalOnUserStatsReceived == null)
			{
				_internalOnUserStatsReceived = new OnUserStatsReceivedFromSteam(OnUserStatsReceivedCallback);
			}

			SteamUnityAPI_SteamUserStats_RequestCurrentStats(_stats, Marshal.GetFunctionPointerForDelegate(_internalOnUserStatsReceived));
		}

		internal void OnUserStatsReceivedCallback(ref UserStatsReceived_t CallbackData)
		{
			_id = new SteamID(CallbackData.m_steamIDUser);

			InitializeAchievementList(_requestedAchievements);

			_onUserStatsReceived(null, this);
		}

		public void InitializeAchievementList(IEnumerable<string> requestedAchievements)
		{
			Byte achieved;

			// Make sure we don't double up the list of Achievements
			Clear();

			_requestedAchievements = requestedAchievements;

			if (_id != null)
			{
				foreach (string s in _requestedAchievements)
				{
					if (SteamUnityAPI_SteamUserStats_GetUserAchievement(_stats, _id.ToUInt64(), s, out achieved))
					{
						Add(new Achievement(this, s, achieved != 0));
					}
				}
			}
			else
			{
				foreach (string s in _requestedAchievements)
				{
					if (SteamUnityAPI_SteamUserStats_GetAchievement(_stats, s, out achieved))
					{
						Add(new Achievement(this, s, achieved != 0));
					}
				}
			}
		}

		public void UnlockAchievement(string achievementName, bool storeStats = false)
		{
			foreach (Achievement a in _achievementList)
			{
				if (a.AchievementName == achievementName)
				{
					if (!a.IsAchieved)
					{
						SteamUnityAPI_SteamUserStats_SetAchievement(_stats, a.AchievementName);
						a.IsAchieved = true;

						if (storeStats)
						{
							WriteStats();
						}
					}

					break;
				}
			}
		}

		public void WriteStats()
		{
			SteamUnityAPI_SteamUserStats_StoreStats(_stats);
		}

		public SteamID SteamID
		{
			get { return _id; }
		}

		public IList<Achievement> StatsList
		{
			get { return _achievementList; }
		}

		public int Count
		{
			get { return _achievementList.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void Add(Achievement item)
		{
			_achievementList.Add(item);
		}

		public void Clear()
		{
			_achievementList.Clear();
		}

		public bool Contains(Achievement item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(Achievement[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(Achievement item)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<Achievement> GetEnumerator()
		{
			return new ListEnumerator<Achievement>(_achievementList);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
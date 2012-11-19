// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
	using SteamAPICall_t = UInt64;

	public class Achievements : ICollection<Achievement>
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats();
		[DllImport("CommunityExpressSW")]
		private static extern bool SteamUnityAPI_SteamUserStats_RequestCurrentStats(IntPtr stats, IntPtr OnUserStatsReceivedCallback);
		[DllImport("CommunityExpressSW")]
		private static extern bool SteamUnityAPI_SteamUserStats_GetAchievement(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string achievementName,
			out Byte isAchieved);
		[DllImport("CommunityExpressSW")]
		private static extern bool SteamUnityAPI_SteamUserStats_GetUserAchievement(IntPtr stats, UInt64 steamID, [MarshalAs(UnmanagedType.LPStr)] string achievementName,
			out Byte isAchieved);
		[DllImport("CommunityExpressSW")]
		private static extern bool SteamUnityAPI_SteamUserStats_SetAchievement(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string achievementName);
		[DllImport("CommunityExpressSW")]
		private static extern bool SteamUnityAPI_SteamUserStats_StoreStats(IntPtr stats);
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamGameServerStats();
		[DllImport("CommunityExpressSW")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamGameServerStats_RequestUserStats(IntPtr gameserverStats, UInt64 steamID);
		[DllImport("CommunityExpressSW")]
		private static extern bool SteamUnityAPI_SteamGameServerStats_GetUserAchievement(IntPtr gameserverStats, UInt64 steamID,
			[MarshalAs(UnmanagedType.LPStr)] string achievementName, out Byte isAchieved);
		[DllImport("CommunityExpressSW")]
		private static extern bool SteamUnityAPI_SteamGameServerStats_SetUserAchievement(IntPtr stats, UInt64 steamID,
			[MarshalAs(UnmanagedType.LPStr)] string achievementName);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamGameServerStats_StoreUserStats(IntPtr gameserverStats, UInt64 steamID);

		private IntPtr _stats = IntPtr.Zero;
		private IntPtr _gameserverStats = IntPtr.Zero;
		private SteamID _id;
		private List<Achievement> _achievementList = new List<Achievement>();
		private IEnumerable<string> _requestedAchievements;

		private OnUserStatsReceivedFromSteam _internalOnUserStatsReceived = null;
		private OnUserStatsReceived _onUserStatsReceived;

		public Achievements()
		{
		}

		internal void Init(SteamID steamID = null, Boolean isGameServer = false)
		{
			_id = steamID;

			if (isGameServer)
			{
				_gameserverStats = SteamUnityAPI_SteamGameServerStats();
			}
			else
			{
				_stats = SteamUnityAPI_SteamUserStats();
			}
		}

		public void RequestCurrentAchievements(OnUserStatsReceived onUserStatsReceived, IEnumerable<string> requestedAchievements)
		{
			_requestedAchievements = requestedAchievements;
			_onUserStatsReceived = onUserStatsReceived;

			if (_internalOnUserStatsReceived == null)
			{
				_internalOnUserStatsReceived = new OnUserStatsReceivedFromSteam(OnUserStatsReceivedCallback);
			}

			if (_gameserverStats != IntPtr.Zero)
			{
				SteamAPICall_t result = SteamUnityAPI_SteamGameServerStats_RequestUserStats(_gameserverStats, _id.ToUInt64());
				CommunityExpress.Instance.AddGameServerUserStatsReceivedCallback(result, OnUserStatsReceivedCallback);
			}
			else
			{
				SteamUnityAPI_SteamUserStats_RequestCurrentStats(_stats, Marshal.GetFunctionPointerForDelegate(_internalOnUserStatsReceived));
			}
		}

		private void OnUserStatsReceivedCallback(ref UserStatsReceived_t CallbackData)
		{
			_id = new SteamID(CallbackData.m_steamIDUser);

			InitializeAchievementList(_requestedAchievements);

			if (_onUserStatsReceived != null)
			{
				_onUserStatsReceived(null, this);
			}
		}

		public void InitializeAchievementList(IEnumerable<string> requestedAchievements)
		{
			Byte achieved;

			// Make sure we don't double up the list of Achievements
			Clear();

			_requestedAchievements = requestedAchievements;

			if (_gameserverStats != IntPtr.Zero)
			{
				foreach (string s in _requestedAchievements)
				{
					if (SteamUnityAPI_SteamGameServerStats_GetUserAchievement(_gameserverStats, _id.ToUInt64(), s, out achieved))
					{
						Add(new Achievement(this, _stats, s, achieved != 0));
					}
				}
			}
			else if (_id != null)
			{
				foreach (string s in _requestedAchievements)
				{
					if (SteamUnityAPI_SteamUserStats_GetUserAchievement(_stats, _id.ToUInt64(), s, out achieved))
					{
						Add(new Achievement(this, _stats, s, achieved != 0));
					}
				}
			}
			else
			{
				foreach (string s in _requestedAchievements)
				{
					if (SteamUnityAPI_SteamUserStats_GetAchievement(_stats, s, out achieved))
					{
						Add(new Achievement(this, _stats, s, achieved != 0));
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
						if (_gameserverStats != IntPtr.Zero)
						{
							SteamUnityAPI_SteamGameServerStats_SetUserAchievement(_gameserverStats, _id.ToUInt64(), a.AchievementName);
						}
						else
						{
							SteamUnityAPI_SteamUserStats_SetAchievement(_stats, a.AchievementName);
						}

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

		public void UnlockAchievement(Achievement achievement, bool storeStats)
		{
			if (!achievement.IsAchieved)
			{
				if (_gameserverStats != IntPtr.Zero)
				{
					SteamUnityAPI_SteamGameServerStats_SetUserAchievement(_gameserverStats, _id.ToUInt64(), achievement.AchievementName);
				}
				else
				{
					SteamUnityAPI_SteamUserStats_SetAchievement(_stats, achievement.AchievementName);
				}

				achievement.IsAchieved = true;

				if (storeStats)
				{
					WriteStats();
				}
			}
		}

		public void WriteStats()
		{
			if (_gameserverStats != IntPtr.Zero)
			{
				SteamUnityAPI_SteamGameServerStats_StoreUserStats(_gameserverStats, _id.ToUInt64());
			}
			else
			{
				SteamUnityAPI_SteamUserStats_StoreStats(_stats);
			}
		}

		public SteamID SteamID
		{
			get { return _id; }
		}

		public IList<Achievement> AchievementList
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
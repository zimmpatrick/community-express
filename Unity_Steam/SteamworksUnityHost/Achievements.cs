// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
	using SteamAPICall_t = UInt64;

    /// <summary>
    /// List of user's achievements
    /// </summary>
	public class Achievements : ICollection<Achievement>
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats();
		[DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_SteamUserStats_RequestCurrentStats(IntPtr stats);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_SteamUserStats_GetAchievement(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string achievementName,
			out Byte isAchieved);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_SteamUserStats_GetUserAchievement(IntPtr stats, UInt64 steamID, [MarshalAs(UnmanagedType.LPStr)] string achievementName,
			out Byte isAchieved);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_SteamUserStats_SetAchievement(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string achievementName);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SteamUnityAPI_SteamUserStats_IndicateAchievementProgress(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string achievementName, UInt32 nCurProgress, UInt32 nMaxProgress);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_SteamUserStats_StoreStats(IntPtr stats);
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamGameServerStats();
		[DllImport("CommunityExpressSW")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamGameServerStats_RequestUserStats(IntPtr gameserverStats, UInt64 steamID);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_SteamGameServerStats_GetUserAchievement(IntPtr gameserverStats, UInt64 steamID,
			[MarshalAs(UnmanagedType.LPStr)] string achievementName, out Byte isAchieved);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern bool SteamUnityAPI_SteamGameServerStats_SetUserAchievement(IntPtr stats, UInt64 steamID,
			[MarshalAs(UnmanagedType.LPStr)] string achievementName);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SteamUnityAPI_SteamGameServerStats_StoreUserStats(IntPtr gameserverStats, UInt64 steamID);

        private CommunityExpress _ce;
		private IntPtr _stats = IntPtr.Zero;
		private IntPtr _gameserverStats = IntPtr.Zero;
		private SteamID _id;
		private List<Achievement> _achievementList = new List<Achievement>();
		private IEnumerable<string> _requestedAchievements;
        private CommunityExpress.OnEventHandler<UserStatsReceived_t> _statsRecievedEventHandler = null;

        public delegate void UserAchievementsReceivedHandler(Achievements sender, UserAchievementsReceivedArgs e);

        public class UserAchievementsReceivedArgs : System.EventArgs
        {
            internal UserAchievementsReceivedArgs(UserStatsReceived_t args)
            {
                GameID = args.m_nGameID;
                Result = args.m_eResult;
                SteamID = new SteamID(args.m_steamIDUser);
            }

            public UInt64 GameID
            {
                get;
                private set;
            }

            public EResult Result
            {
                get;
                private set;
            }

            public SteamID SteamID
            {
                get;
                private set;
            }
        }

        internal event UserAchievementsReceivedHandler UserAchievementsReceived;

        internal Achievements(CommunityExpress ce, SteamID steamID, Boolean isGameServer = false)
		{
            _ce = ce;
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

		internal void RequestCurrentAchievements(IEnumerable<string> requestedAchievements)
        {
            _statsRecievedEventHandler = new CommunityExpress.OnEventHandler<UserStatsReceived_t>(OnUserStatsReceivedCallback);
            _ce.AddEventHandler(UserStatsReceived_t.k_iCallback, _statsRecievedEventHandler);

			_requestedAchievements = requestedAchievements;

			if (_gameserverStats != IntPtr.Zero)
			{
				SteamUnityAPI_SteamGameServerStats_RequestUserStats(_gameserverStats, _id.ToUInt64());
			}
			else
			{
				SteamUnityAPI_SteamUserStats_RequestCurrentStats(_stats);
			}
		}

		private void OnUserStatsReceivedCallback(UserStatsReceived_t CallbackData, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (_id.ToUInt64() != CallbackData.m_steamIDUser) return;

			InitializeAchievementList(_requestedAchievements);

            _ce.RemoveEventHandler(UserStatsReceived_t.k_iCallback, _statsRecievedEventHandler);
            _statsRecievedEventHandler = null;

            if (UserAchievementsReceived != null) UserAchievementsReceived(this, new UserAchievementsReceivedArgs(CallbackData));
		}
        /// <summary>
        /// Creates a list of achievements
        /// </summary>
        /// <param name="requestedAchievements">Requested achievements to lits</param>
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

        /// <summary>
        /// Unlocks the acheivement for the current user
        /// </summary>
        /// <param name="achievementName">name of acheivement to unlock</param>
        /// <param name="storeStats">true if status should be uploaded now</param>
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

        /// <summary>
        /// Unlocks the acheivement for the current user
        /// </summary>
        /// <param name="achievement">achievement to unlock</param>
        /// <param name="storeStats">true if status should be uploaded now</param>
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

        /// <summary>
        /// Achievement progress - triggers an AchievementProgress callback, that is all.
        /// Calling this w/ N out of N progress will NOT set the achievement, the game must still do that.
        /// </summary>
        /// <param name="achievement">achievement to update</param>
        /// <param name="nCurProgress">current progress</param>
        /// <param name="nMaxProgress">max progress</param>
        /// <param name="storeStats">true if progress should be uploaded now</param>
        public void IndicateAchievementProgress(Achievement achievement, UInt32 nCurProgress, UInt32 nMaxProgress, bool storeStats)
        {
            if (_gameserverStats != IntPtr.Zero)
            {
                // not yet supported
                return;
            }
            else
            {
                SteamUnityAPI_SteamUserStats_IndicateAchievementProgress(_stats, achievement.AchievementName, nCurProgress, nMaxProgress);
            }

            if (storeStats)
            {
                WriteStats();
            }
        }
        /// <summary>
        /// Writes stats about achievements
        /// </summary>
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
        /// <summary>
        /// User's Steam ID
        /// </summary>
		public SteamID SteamID
		{
			get { return _id; }
		}
        
        /// <summary>
        /// List of achievements
        /// </summary>
		public IList<Achievement> AchievementList
		{
			get { return _achievementList; }
		}
        
        /// <summary>
        /// Counts achievements in list
        /// </summary>
		public int Count
		{
			get { return _achievementList.Count; }
		}

        /// <summary>
        /// If list is read-only
        /// </summary>
		public bool IsReadOnly
		{
			get { return true; }
		}
        
        /// <summary>
        /// Adds achievement to list
        /// </summary>
        /// <param name="item">Achievement to add</param>
		public void Add(Achievement item)
		{
			_achievementList.Add(item);
		}
        /// <summary>
        /// Clears list of achievements
        /// </summary>
		public void Clear()
		{
			_achievementList.Clear();
		}

        /// <summary>
        /// Checks if a list contains a certain achievement (NotImplemented)
        /// </summary>
        /// <param name="item">Achievement to check for</param>
        /// <returns>true if achievement is found</returns>
		public bool Contains(Achievement item)
		{
            return _achievementList.Contains(item);
		}

        /// <summary>
        /// Copies list to index (NotImplemented)
        /// </summary>
        /// <param name="array">Array of achievements</param>
        /// <param name="arrayIndex">Index to copy to</param>
		public void CopyTo(Achievement[] array, int arrayIndex)
		{
            _achievementList.CopyTo(array, arrayIndex);
		}

        /// <summary>
        /// Removes achievement from list (NotSupported)
        /// </summary>
        /// <param name="item">Achievement to remove</param>
        /// <returns>true if successfully removed</returns>
		public bool Remove(Achievement item)
		{
			throw new NotSupportedException();
		}

        /// <summary>
        /// Gets enumerator for achievements
        /// </summary>
        /// <returns>true if enumerator gotten</returns>
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
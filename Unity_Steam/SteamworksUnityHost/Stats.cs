using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace SteamworksUnityHost
{
	using SteamAPICall_t = UInt64;

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct UserStatsReceived_t
	{
		public UInt64 m_nGameID;		// Game these stats are for
		public EResult m_eResult;		// Success / error fetching the stats
		public UInt64 m_steamIDUser;	// The user for whom the stats are retrieved for
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct GSStatsReceived_t
	{
		public EResult m_eResult;		// Success / error fetching the stats
		public UInt64 m_steamIDUser;	// The user for whom the stats are retrieved for
	}

	internal delegate void OnUserStatsReceivedFromSteam(ref UserStatsReceived_t CallbackData);
	public delegate void OnUserStatsReceived(Stats stats, Achievements achievements);

	public class Stats : ICollection<Stat>
	{
		[DllImport("SteamworksUnity.dll")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats();
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamUserStats_RequestCurrentStats(IntPtr stats, IntPtr OnUserStatsReceivedCallback);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamUserStats_GetUserStatInt(IntPtr stats, UInt64 steamID, [MarshalAs(UnmanagedType.LPStr)] string statName,
			out Int32 value);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamUserStats_GetUserStatFloat(IntPtr stats, UInt64 steamID, [MarshalAs(UnmanagedType.LPStr)] string statName,
			out float value);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamUserStats_SetStatInt(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string statName, Int32 value);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamUserStats_SetStatFloat(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string statName, float value);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamUserStats_StoreStats(IntPtr stats);
		[DllImport("SteamworksUnity.dll")]
		private static extern IntPtr SteamUnityAPI_SteamGameServerStats();
		[DllImport("SteamworksUnity.dll")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamGameServerStats_RequestUserStats(IntPtr gameserverStats, UInt64 steamID);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamGameServerStats_GetUserStatInt(IntPtr gameserverStats, UInt64 steamID,
			[MarshalAs(UnmanagedType.LPStr)] string statName, out Int32 value);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamGameServerStats_GetUserStatFloat(IntPtr gameserverStats, UInt64 steamID,
			[MarshalAs(UnmanagedType.LPStr)] string statName, out float value);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamGameServerStats_SetUserStatInt(IntPtr gameserverStats, UInt64 steamID,
			[MarshalAs(UnmanagedType.LPStr)] string statName, Int32 value);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamGameServerStats_SetUserStatFloat(IntPtr gameserverStats, UInt64 steamID,
			[MarshalAs(UnmanagedType.LPStr)] string statName, float value);
		[DllImport("SteamworksUnity.dll")]
		private static extern Boolean SteamUnityAPI_SteamGameServerStats_StoreUserStats(IntPtr gameserverStats, UInt64 steamID);

		private IntPtr _stats = IntPtr.Zero;
		private IntPtr _gameserverStats = IntPtr.Zero;
		private SteamID _id;
		private List<Stat> _statList = new List<Stat>();
		private IEnumerable<string> _requestedStats;

		private OnUserStatsReceivedFromSteam _internalOnUserStatsReceived = null;
		private OnUserStatsReceived _onUserStatsReceived;
		
		internal Stats()
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

		public void RequestCurrentStats(OnUserStatsReceived onUserStatsReceived, IEnumerable<String> requestedStats)
		{
			_requestedStats = requestedStats;
			_onUserStatsReceived = onUserStatsReceived;

			if (_internalOnUserStatsReceived == null)
			{
				_internalOnUserStatsReceived = new OnUserStatsReceivedFromSteam(OnUserStatsReceivedCallback);
			}

			if (_gameserverStats != IntPtr.Zero)
			{
				SteamAPICall_t result = SteamUnityAPI_SteamGameServerStats_RequestUserStats(_gameserverStats, _id.ToUInt64());
				SteamUnity.Instance.AddGameServerUserStatsReceivedCallback(result, OnUserStatsReceivedCallback);
			}
			else
			{
				SteamUnityAPI_SteamUserStats_RequestCurrentStats(_stats, Marshal.GetFunctionPointerForDelegate(_internalOnUserStatsReceived));
			}
		}

		internal void OnUserStatsReceivedCallback(ref UserStatsReceived_t CallbackData)
		{
			Int32 intValue;
			float floatValue;

			// Make sure we don't double up the list of Stats
			Clear();

			_id = new SteamID(CallbackData.m_steamIDUser);

			if (_gameserverStats != IntPtr.Zero)
			{
				foreach (string s in _requestedStats)
				{
					if (SteamUnityAPI_SteamGameServerStats_GetUserStatInt(_gameserverStats, _id.ToUInt64(), s, out intValue))
					{
						Add(new Stat(this, s, intValue));
					}
					else if (SteamUnityAPI_SteamGameServerStats_GetUserStatFloat(_gameserverStats, _id.ToUInt64(), s, out floatValue))
					{
						Add(new Stat(this, s, floatValue));
					}
				}
			}
			else
			{
				foreach (string s in _requestedStats)
				{
					if (SteamUnityAPI_SteamUserStats_GetUserStatInt(_stats, _id.ToUInt64(), s, out intValue))
					{
						Add(new Stat(this, s, intValue));
					}
					else if (SteamUnityAPI_SteamUserStats_GetUserStatFloat(_stats, _id.ToUInt64(), s, out floatValue))
					{
						Add(new Stat(this, s, floatValue));
					}
				}
			}

			_onUserStatsReceived(this, null);
		}

		public void WriteStats()
		{
			if (_gameserverStats != IntPtr.Zero)
			{
				foreach (Stat s in _statList)
				{
					if (s.HasChanged)
					{
						if (s.StatValue is int)
						{
							SteamUnityAPI_SteamGameServerStats_SetUserStatInt(_gameserverStats, _id.ToUInt64(), s.StatName, (int)s.StatValue);
						}
						else
						{
							SteamUnityAPI_SteamGameServerStats_SetUserStatFloat(_gameserverStats, _id.ToUInt64(), s.StatName, (float)s.StatValue);
						}

						s.HasChanged = false;
					}
				}

				SteamUnityAPI_SteamGameServerStats_StoreUserStats(_gameserverStats, _id.ToUInt64());
			}
			else
			{
				foreach (Stat s in _statList)
				{
					if (s.HasChanged)
					{
						if (s.StatValue is int)
						{
							SteamUnityAPI_SteamUserStats_SetStatInt(_stats, s.StatName, (int)s.StatValue);
						}
						else
						{
							SteamUnityAPI_SteamUserStats_SetStatFloat(_stats, s.StatName, (float)s.StatValue);
						}

						s.HasChanged = false;
					}
				}

				SteamUnityAPI_SteamUserStats_StoreStats(_stats);
			}
		}

		public SteamID SteamID
		{
			get { return _id; }
		}

		public IList<Stat> StatsList
		{
			get { return _statList; }
		}
		
		public int Count
		{
			get { return _statList.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void Add(Stat item)
		{
			_statList.Add(item);
		}

		public void Clear()
		{
			_statList.Clear();
		}

		public bool Contains(Stat item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(Stat[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(Stat item)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<Stat> GetEnumerator()
		{
			return new ListEnumerator<Stat>(_statList);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
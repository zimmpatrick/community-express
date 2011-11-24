using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace SteamworksUnityHost
{
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct UserStatsReceived_t
	{
		public UInt64 m_nGameID;		// Game these stats are for
		public EResult m_eResult;		// Success / error fetching the stats
		public UInt64 m_steamIDUser;	// The user for whom the stats are retrieved for
	}

	public delegate void OnUserStatsReceivedFromSteam(ref UserStatsReceived_t CallbackData);
	public delegate void OnUserStatsReceived(Stats stats);

	public class Stats : ICollection<Stat>
	{
		[DllImport("SteamworksUnity.dll")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats();
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_SteamUserStats_RequestCurrentStats(IntPtr stats, IntPtr OnUserStatsReceivedCallback);
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_SteamUserStats_GetUserStatInt(IntPtr stats, UInt64 steamID, [MarshalAs(UnmanagedType.LPStr)] string statName, out Int32 intValue);
		[DllImport("SteamworksUnity.dll")]
		private static extern bool SteamUnityAPI_SteamUserStats_GetUserStatFloat(IntPtr stats, UInt64 steamID, [MarshalAs(UnmanagedType.LPStr)] string statName, out float intValue);

		private IntPtr _stats;
		private SteamID _id;
		private List<Stat> _statList = new List<Stat>();

		private OnUserStatsReceivedFromSteam _internalOnUserStatsReceived;
		private OnUserStatsReceived _onUserStatsReceived;

		private class StatEnumator : IEnumerator<Stat>
		{
			private int _index;
			private Stats _stats;

			public StatEnumator(Stats stats)
			{
				_stats = stats;
				_index = -1;
			}

			public Stat Current
			{
				get
				{
					return _stats._statList[_index];
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
				return _index < _stats.Count;
			}

			public void Reset()
			{
				_index = -1;
			}

			public void Dispose()
			{
			}
		}

		public Stats()
		{
			_stats = SteamUnityAPI_SteamUserStats();
		}

		public void RequestCurrentStats(OnUserStatsReceived onUserStatsReceived)
		{
			_onUserStatsReceived = onUserStatsReceived;
			_internalOnUserStatsReceived = new OnUserStatsReceivedFromSteam(OnUserStatsReceivedCallback);

			SteamUnityAPI_SteamUserStats_RequestCurrentStats(_stats, Marshal.GetFunctionPointerForDelegate(_internalOnUserStatsReceived));
		}

		public void OnUserStatsReceivedCallback(ref UserStatsReceived_t CallbackData)
		{
			Int32 intValue;
			float floatValue;

			_id = new SteamID(CallbackData.m_steamIDUser);

			foreach (string s in StatList)
			{
				if (SteamUnityAPI_SteamUserStats_GetUserStatInt(_stats, _id.ToUInt64(), s, out intValue))
				{
					Add(new Stat(this, s, intValue, true));
				}
				else if (SteamUnityAPI_SteamUserStats_GetUserStatFloat(_stats, _id.ToUInt64(), s, out floatValue))
				{
					Add(new Stat(this, s, (decimal)floatValue, false));
				}
			}

			_onUserStatsReceived(this);
		}

		public SteamID SteamID
		{
			get { return _id; }
		}

		public static readonly string[] StatList = new string[]
		{
			"Kills",
			"DamageHealed"
		};

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
			throw new NotSupportedException();
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
			return new StatEnumator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
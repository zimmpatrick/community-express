using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace SteamworksUnityHost
{
	public class Stats : ICollection<Stat>
	{
		[DllImport("SteamworksUnity.dll")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats();

		private IntPtr _stats;

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
					return new Stat(_stats);
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

		public int Count
		{
			get { return 1; /* todo: SteamUnityAPI_SteamUserStats_GetStatCount(_stats);*/ }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void Add(Stat item)
		{
			throw new NotSupportedException();
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
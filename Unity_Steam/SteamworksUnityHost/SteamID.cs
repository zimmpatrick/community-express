using System;
using System.Collections.Generic;
using System.Text;

namespace SteamworksUnityHost
{
	// Currently read-only
	public class SteamID
	{
		internal SteamID(UInt64 id)
		{
			_id = id;
		}

		public UInt32 AccountID
		{
			// account ID is low 32 bits
			get { return (uint)(_id & 0xFFFFFFFF); }
		}

		public UInt32 AccountInstance
		{
			// account instance is next 20 bits
			get { return (uint)((_id >> 32) & 0xFFFFF); }
		}

		public EAccountType AccountType
		{
			// account type is next 4 bits
			get { return (EAccountType)((_id >> 52) & 0xF); }
		}

		public EUniverse Universe
		{
			// universe is the last 8 bits
			get { return (EUniverse)((_id >> 56) & 0xFF); }
		}

		public UInt64 ToUInt64()
		{
			return _id;
		}

		public override string ToString()
		{
			return _id.ToString();
		}

		public override bool Equals(System.Object obj)
		{
			// If parameter cannot be cast to ThreeDPoint return false:
			SteamID p = obj as SteamID;
			if ((object)p == null)
			{
				return false;
			}

			// Return true if the fields match:
			return _id == p.ToUInt64();
		}

		public static bool operator ==(SteamID a, SteamID b)
		{
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}

			if (System.Object.ReferenceEquals(a, null) || System.Object.ReferenceEquals(b, null))
			{
				return false;
			}

			return a.ToUInt64() == b.ToUInt64();
		}

		public static bool operator !=(SteamID a, SteamID b)
		{
			return !(a == b);
		}

		public static bool operator ==(SteamID a, UInt64 b)
		{
			if (System.Object.ReferenceEquals(a, null))
				return b == 0;

			return a.ToUInt64() == b;
		}

		public static bool operator !=(SteamID a, UInt64 b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ (int)_id;
		}

		private UInt64 _id;
	}
}

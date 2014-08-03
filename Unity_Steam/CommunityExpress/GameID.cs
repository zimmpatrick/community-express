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

namespace CommunityExpressNS
{
    /// <summary>
    /// Game ID Type
    /// </summary>
    public enum EGameIDType
    {
        /// <summary>
        /// App Type
        /// </summary>
        k_EGameIDTypeApp = 0,
        
        /// <summary>
        /// Mode Type
        /// </summary>
        k_EGameIDTypeGameMod = 1,
        
        /// <summary>
        /// Shortcut Type
        /// </summary>
        k_EGameIDTypeShortcut = 2,

        /// <summary>
        /// P2P Type
        /// </summary>
        k_EGameIDTypeP2P = 3,
    };

	/// <summary>
    /// A GameID encapsulates an appID / modID pair
	/// </summary>
	public class GameID
	{
        public GameID(UInt64 id)
		{
			_id = id;
		}

        /// <summary>
        /// App ID
        /// </summary>
		public UInt32 AppID
		{
			// AppID is low 24 bits
			get { return (uint)(_id & 0xFFFFFF); }
		}

        /// <summary>
        /// Type
        /// </summary>
        public EGameIDType Type
        {
            // account instance is next 8 bits
            get { return (EGameIDType)((_id >> 24) & 0xFF); }
        }

        /// <summary>
        /// Mod ID
        /// </summary>
		public UInt32 ModID
		{
			// account instance is next 32 bits
			get { return (uint)((_id >> 32) & 0xFFFFFFFF); }
		}

        /// <summary>
        /// Converts Game ID to its 64-bit representation
        /// </summary>
        /// <returns>64-bit representation of a Steam ID</returns>
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
            GameID p = obj as GameID;
			if ((object)p == null)
			{
				return false;
			}

			// Return true if the fields match:
			return _id == p.ToUInt64();
		}

        /// <summary>
        /// Is GameID valid
        /// </summary>
        /// <returns>returns true if valid</returns>
	    bool IsValid
	    {
            get
            {
                // each type has it's own invalid fixed point:
                switch (Type)
                {
                    case EGameIDType.k_EGameIDTypeApp:
                        return AppID != CommunityExpress.k_uAppIdInvalid;

                    case EGameIDType.k_EGameIDTypeGameMod:
                        return AppID != CommunityExpress.k_uAppIdInvalid && (ModID & 0x80000000) != 0;

                    case EGameIDType.k_EGameIDTypeShortcut:
                        return (ModID & 0x80000000) != 0;

                    case EGameIDType.k_EGameIDTypeP2P:
                        return AppID == CommunityExpress.k_uAppIdInvalid && (ModID & 0x80000000) != 0;

                    default:
                        return false;
                }
            }
	    }

        public static bool operator ==(GameID a, GameID b)
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

        public static bool operator !=(GameID a, GameID b)
		{
			return !(a == b);
		}

        public static bool operator ==(GameID a, UInt64 b)
		{
			if (System.Object.ReferenceEquals(a, null))
				return b == 0;

			return a.ToUInt64() == b;
		}

        public static bool operator !=(GameID a, UInt64 b)
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

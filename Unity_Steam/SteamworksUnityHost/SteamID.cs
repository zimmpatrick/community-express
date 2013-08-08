// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
	/// <summary>
    /// A SteamID is a unique identifier for an account, and used to differentiate users in all parts of the Steamworks API
	/// </summary>
	public class SteamID
	{
		public SteamID(UInt64 id)
		{
			_id = id;
		}

        /// <summary>
        /// Account ID
        /// </summary>
		public UInt32 AccountID
		{
			// account ID is low 32 bits
			get { return (uint)(_id & 0xFFFFFFFF); }
		}

        /// <summary>
        /// Instance
        /// </summary>
		public UInt32 AccountInstance
		{
			// account instance is next 20 bits
			get { return (uint)((_id >> 32) & 0xFFFFF); }
		}

        /// <summary>
        /// Type of account
        /// </summary>
		public EAccountType AccountType
		{
			// account type is next 4 bits
			get { return (EAccountType)((_id >> 52) & 0xF); }
		}

        /// <summary>
        /// Universe this account belongs to
        /// </summary>
		public EUniverse Universe
		{
			// universe is the last 8 bits
			get { return (EUniverse)((_id >> 56) & 0xFF); }
		}

        /// <summary>
        /// Is this an anonymous game server login that will be filled in?
        /// </summary>
	    public bool BlankAnonAccount
	    {
		    get { return (AccountID == 0 && AnonAccount && AccountInstance == 0); }
	    }

        /// <summary>
	    /// Is this a game server account id?  (Either persistent or anonymous)
        /// </summary>
	    public bool  GameServerAccount
	    {
		    get { return AccountType == EAccountType.EAccountTypeGameServer || AccountType == EAccountType.EAccountTypeAnonGameServer; }
	    }

        /// <summary>
        /// Is this a persistent (not anonymous) game server account id?
        /// </summary>
	    public bool PersistentGameServerAccount
	    {
		    get { return AccountType == EAccountType.EAccountTypeGameServer; }
	    }
        
        /// <summary>
        /// Is this an anonymous game server account id?
        /// </summary>
	    public bool AnonGameServerAccount
	    {
		    get { return AccountType == EAccountType.EAccountTypeAnonGameServer; }
	    }
        
        /// <summary>
        /// Is this a content server account id?
        /// </summary>
	    public bool ContentServerAccount
	    {
		    get {  return AccountType == EAccountType.EAccountTypeContentServer; }
	    }
        
        /// <summary>
        /// Is this a clan account id?
        /// </summary>
	    public bool ClanAccount
	    {
		    get { return AccountType == EAccountType.EAccountTypeClan; }
	    }
        
        /// <summary>
        /// Is this a chat account id?
        /// </summary>
	    public bool ChatAccount
	    {
		    get { return AccountType == EAccountType.EAccountTypeChat; }
	    }
        
        /// <summary>
        /// Is this an individual user account id?
        /// </summary>
	    public bool IndividualAccount
	    {
		    get { return AccountType == EAccountType.EAccountTypeIndividual || AccountType == EAccountType.EAccountTypeConsoleUser; }
	    }
        
        /// <summary>
        /// Is this an anonymous account?
        /// </summary>
	    public bool AnonAccount
	    {
		    get { return AccountType == EAccountType.EAccountTypeAnonUser || AccountType == EAccountType.EAccountTypeAnonGameServer; }
	    }
        
        /// <summary>
        /// Is this an anonymous user account? ( used to create an account or reset a password )
        /// </summary>
	    public bool AnonUserAccount
	    {
		    get { return AccountType == EAccountType.EAccountTypeAnonUser; }
	    }
        
        /// <summary>
        /// Is this a faked up Steam ID for a PSN friend account?
        /// </summary>
	    public bool ConsoleUserAccount
	    {
		    get { return AccountType == EAccountType.EAccountTypeConsoleUser; }
	    }

        /// <summary>
        /// Converts steam ID to its 64-bit representation
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

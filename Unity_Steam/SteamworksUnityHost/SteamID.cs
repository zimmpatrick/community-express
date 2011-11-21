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

        private UInt64 _id;
    }
}

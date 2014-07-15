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
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
    /// <summary>
    /// Class for accessing stats, achievements, and leaderboard information
    /// </summary>
	public class Stat
	{
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern Boolean SteamUnityAPI_SteamUserStats_GetUserStatInt(IntPtr stats, UInt64 steamID, [MarshalAs(UnmanagedType.LPStr)] string statName,
            out Int32 value);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern Boolean SteamUnityAPI_SteamUserStats_GetUserStatFloat(IntPtr stats, UInt64 steamID, [MarshalAs(UnmanagedType.LPStr)] string statName,
            out float value);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern Boolean SteamUnityAPI_SteamUserStats_SetStatInt(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string statName, Int32 value);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern Boolean SteamUnityAPI_SteamUserStats_SetStatFloat(IntPtr stats, [MarshalAs(UnmanagedType.LPStr)] string statName, float value);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern Boolean SteamUnityAPI_SteamGameServerStats_GetUserStatInt(IntPtr gameserverStats, UInt64 steamID,
            [MarshalAs(UnmanagedType.LPStr)] string statName, out Int32 value);
        [DllImport("CommunityExpressSW")]
        private static extern Boolean SteamUnityAPI_SteamGameServerStats_GetUserStatFloat(IntPtr gameserverStats, UInt64 steamID,
            [MarshalAs(UnmanagedType.LPStr)] string statName, out float value);
        [DllImport("CommunityExpressSW")]
        private static extern Boolean SteamUnityAPI_SteamGameServerStats_SetUserStatInt(IntPtr gameserverStats, UInt64 steamID,
            [MarshalAs(UnmanagedType.LPStr)] string statName, Int32 value);
        [DllImport("CommunityExpressSW")]
        private static extern Boolean SteamUnityAPI_SteamGameServerStats_SetUserStatFloat(IntPtr gameserverStats, UInt64 steamID,
            [MarshalAs(UnmanagedType.LPStr)] string statName, float value);

		private IntPtr _stats;
		private String _statName;
		private object _statValue;
        private SteamID _id;
		private bool _isServer;


        internal Stat(IntPtr stats, SteamID steamID, String statName, Type t, bool isServer)
        {
			_stats = stats;
            _id = steamID;
            _isServer = isServer;
			_statName = statName;

            Int32 intValue;
            float floatValue;
            if (isServer)
            {
                if (t == typeof(int) && SteamUnityAPI_SteamGameServerStats_GetUserStatInt(_stats, _id.ToUInt64(), _statName, out intValue))
                {
                    _statValue = intValue;
                }
                else if (t == typeof(float) && SteamUnityAPI_SteamGameServerStats_GetUserStatFloat(_stats, _id.ToUInt64(), _statName, out floatValue))
                {
                    _statValue = floatValue;
                }
                else
                {
                    _statValue = null;
                }
            }
            else
            {

                if (t == typeof(int) && SteamUnityAPI_SteamUserStats_GetUserStatInt(_stats, _id.ToUInt64(), _statName, out intValue))
                {
                    _statValue = intValue;
                }
                else if (t == typeof(float) && SteamUnityAPI_SteamUserStats_GetUserStatFloat(_stats, _id.ToUInt64(), _statName, out floatValue))
                {
                    _statValue = floatValue;
                }
                else
                {
                    _statValue = null;
                }
            }
		}
        /// <summary>
        /// Name of the stat
        /// </summary>
		public String StatName
		{
			get { return _statName; }
			private set { _statName = value; }
		}
        /// <summary>
        /// Value of the stat
        /// </summary>
		public object StatValue
		{
			get { return _statValue; }
			set 
            {     
                _statValue = value;
                SaveChanges();
            }
		}

        private void SaveChanges()
        {
            if (_isServer)
            {
                if (StatValue is int)
                {
                    SteamUnityAPI_SteamGameServerStats_SetUserStatInt(_stats, _id.ToUInt64(), StatName, (int)StatValue);
                }
                else
                {
                    SteamUnityAPI_SteamGameServerStats_SetUserStatFloat(_stats, _id.ToUInt64(), StatName, (float)StatValue);
                }
            }
            else
            {

                if (StatValue is int)
                {
                    SteamUnityAPI_SteamUserStats_SetStatInt(_stats, StatName, (int)StatValue);
                }
                else
                {
                    SteamUnityAPI_SteamUserStats_SetStatFloat(_stats, StatName, (float)StatValue);
                }
            }
        }

	}
}
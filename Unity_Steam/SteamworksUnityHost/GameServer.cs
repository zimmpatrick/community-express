using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SteamworksUnityHost
{
    public class GameServer
    {
        [DllImport("SteamworksUnity.dll")]
        private static extern IntPtr SteamUnityAPI_SteamGameServer();
        [DllImport("SteamworksUnity.dll")]
        private static extern UInt64 SteamUnityAPI_SteamGameServer_GetSteamID(IntPtr user);

        private IntPtr _gameserver;

        internal GameServer()
        {
            _gameserver = SteamUnityAPI_SteamGameServer();
        }

        public SteamID SteamID
        {
            get { return new SteamID(SteamUnityAPI_SteamGameServer_GetSteamID(_gameserver)); }
        }
    }
}

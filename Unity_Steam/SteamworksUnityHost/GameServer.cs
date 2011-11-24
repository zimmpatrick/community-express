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
		[DllImport("SteamworksUnity.dll")]
		private static extern void SteamUnityAPI_SteamGameServer_Shutdown();
		
		private IntPtr _gameserver;

        internal GameServer()
        {
            _gameserver = SteamUnityAPI_SteamGameServer();
        }

		~GameServer()
		{
			Shutdown();
		}

        public SteamID SteamID
        {
            get { return new SteamID(SteamUnityAPI_SteamGameServer_GetSteamID(_gameserver)); }
        }

		public void Shutdown()
		{
			SteamUnityAPI_SteamGameServer_Shutdown();
		}
    }
}

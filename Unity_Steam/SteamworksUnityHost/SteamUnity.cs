using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SteamworksUnityHost
{
    public sealed class SteamUnity
    {
        [DllImport("SteamworksUnity.dll")]
        private static extern bool SteamUnityAPI_RestartAppIfNecessary(uint unOwnAppID);
        [DllImport("SteamworksUnity.dll")]
        private static extern bool SteamUnityAPI_Init();

        private static readonly SteamUnity _instance = new SteamUnity();
        private SteamUnity() { }

        public const uint k_uAppIdInvalid = 0x0;

        public static SteamUnity Instance
        {
            get
            {
                return _instance;
            }
        }

        public bool RestartAppIfNecessary(uint unOwnAppID)
        {
            return SteamUnityAPI_RestartAppIfNecessary(unOwnAppID);
        }

        public bool Initialize()
        {
            return SteamUnityAPI_Init();
        }

        public bool InitalizeGameServer()
        {
            return true;
        }

        public RemoteStorage RemoteStorage
        {
            get
            {
                return new RemoteStorage();
            }
        }

        public User User
        {
            get
            {
                return new User();
            }
        }

        public GameServer GameServer
        {
            get
            {
                return new GameServer();
            }
        }
    }
}

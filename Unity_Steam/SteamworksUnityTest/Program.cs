using System;
using System.Collections.Generic;
using System.Text;
using SteamworksUnityHost;

namespace SteamworksUnityTest
{
    class Program
    {
        static int Main(string[] args)
        {
            SteamUnity s = SteamUnity.Instance;

#if false
            if (s.RestartAppIfNecessary(SteamUnity.k_uAppIdInvalid))
            {
                // if Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the 
                // local Steam client and also launches this game again.

                // Once you get a public Steam AppID assigned for this game, you need to replace k_uAppIdInvalid with it and
                // removed steam_appid.txt from the game depot.
                return 1;
            }
#endif

	        // Initialize SteamAPI, if this fails we bail out since we depend on Steam for lots of stuff.
	        // You don't necessarily have to though if you write your code to check whether all the Steam
	        // interfaces are NULL before using them and provide alternate paths when they are unavailable.
	        //
	        // This will also load the in-game steam overlay dll into your process.  That dll is normally
	        // injected by steam when it launches games, but by calling this you cause it to always load,
	        // even when not launched via steam.
            if (!s.Initialize())
	        {
		        System.Diagnostics.Debug.WriteLine( "SteamAPI_Init() failed" );
		        throw new Exception("Steam must be running to play this game (SteamAPI_Init() failed).");
	        }

            RemoteStorage r = s.RemoteStorage;
            Console.WriteLine(r.FileCount);

            if (r.FileCount > 0)
            {
                int fileSize;
                string name = r.GetFileNameAndSize(0, out fileSize);
                string msg = r.FileRead(name, fileSize);
                Console.WriteLine(msg);
            }

            return 0;
        }
    }
}

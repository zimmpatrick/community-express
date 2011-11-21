using System;
using System.Collections.Generic;
using System.Text;

namespace SteamworksUnityHost
{
    public enum EUniverse
    {
        EUniverseInvalid = 0,
        EUniversePublic = 1,
        EUniverseBeta = 2,
        EUniverseInternal = 3,
        EUniverseDev = 4,
        EUniverseRC = 5
    };

    public enum EAccountType
    {
        EAccountTypeInvalid = 0,
        EAccountTypeIndividual = 1,		// single user account
        EAccountTypeMultiseat = 2,		// multiseat (e.g. cybercafe) account
        EAccountTypeGameServer = 3,		// game server account
        EAccountTypeAnonGameServer = 4,	// anonymous game server account
        EAccountTypePending = 5,		// pending
        EAccountTypeContentServer = 6,	// content server
        EAccountTypeClan = 7,
        EAccountTypeChat = 8,
        EAccountTypeAnonUser = 10,
    };
}
